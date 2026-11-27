using System.Text;

namespace ContadorLoc.Dominio;

/// <summary>Paso 2: recorre las líneas limpias con una pila de bloques y anota dónde empieza y termina cada clase y método.</summary>
public sealed class DetectorDeEstructura
{
    private readonly List<Bloque> pila = new();
    private readonly List<BloqueCerrado> cerrados = new();
    private readonly StringBuilder encabezado = new();
    private int lineaEncabezado;
    private int profundidadAtributo;

    public static List<BloqueCerrado> Detectar(IReadOnlyList<LineaClasificada> lineas)
    {
        return new DetectorDeEstructura().Procesar(lineas);
    }

    private List<BloqueCerrado> Procesar(IReadOnlyList<LineaClasificada> lineas)
    {
        for (int numeroLinea = 1; numeroLinea <= lineas.Count; numeroLinea++)
        {
            string limpia = lineas[numeroLinea - 1].Limpia;
            if (limpia.TrimStart().StartsWith('#'))
            {
                continue; // E10: cuenta como LOC pero no es estructura
            }

            foreach (char c in limpia)
            {
                ProcesarCaracter(c, numeroLinea);
            }
            if (encabezado.Length > 0)
            {
                encabezado.Append(' ');
            }
        }

        if (pila.Count > 0)
        {
            throw new ErrorDeConteoException("llaves sin cerrar");
        }
        return cerrados;
    }

    private void ProcesarCaracter(char c, int numeroLinea)
    {
        if (c == '}')
        {
            CerrarBloque(numeroLinea);
            return;
        }
        if (!EstaEnNivelDeMiembros())
        {
            if (c == '{')
            {
                pila.Add(new Bloque(TipoBloque.Otro, "", numeroLinea));
            }
            return; // dentro de un método: lambdas y funciones locales quedan dentro de él
        }
        ProcesarEnNivelDeMiembros(c, numeroLinea);
    }

    private bool EstaEnNivelDeMiembros()
    {
        return pila.Count == 0 || pila[^1].Tipo is TipoBloque.Namespace or TipoBloque.Clase;
    }

    private void ProcesarEnNivelDeMiembros(char c, int numeroLinea)
    {
        if (profundidadAtributo > 0)
        {
            ActualizarAtributo(c);
        }
        else if (c == '[' && encabezado.Length == 0)
        {
            profundidadAtributo = 1; // atributo: no forma parte del método
        }
        else if (c == '{')
        {
            AbrirBloque(numeroLinea);
        }
        else if (c == ';')
        {
            TerminarSentencia(numeroLinea);
        }
        else
        {
            AcumularEncabezado(c, numeroLinea);
        }
    }

    private void ActualizarAtributo(char c)
    {
        if (c == '[')
        {
            profundidadAtributo++;
        }
        else if (c == ']')
        {
            profundidadAtributo--;
        }
    }

    private void CerrarBloque(int numeroLinea)
    {
        if (pila.Count == 0)
        {
            throw new ErrorDeConteoException($"llave }} sin abrir (línea {numeroLinea})");
        }

        Bloque bloque = pila[^1];
        if (bloque.Tipo == TipoBloque.Clase)
        {
            string ruta = RutaDeTipos();
            cerrados.Add(new BloqueCerrado(TipoBloque.Clase, ruta, ruta, bloque.LineaInicio, numeroLinea));
        }
        else if (bloque.Tipo == TipoBloque.Metodo)
        {
            cerrados.Add(new BloqueCerrado(
                TipoBloque.Metodo, bloque.Nombre, RutaDeTipos(), bloque.LineaInicio, numeroLinea));
        }
        pila.RemoveAt(pila.Count - 1);
        LimpiarEncabezado();
    }

    private void AbrirBloque(int numeroLinea)
    {
        (TipoBloque tipo, string nombre) = ClasificadorDeEncabezados.Clasificar(encabezado.ToString());
        int inicio = lineaEncabezado > 0 ? lineaEncabezado : numeroLinea;
        pila.Add(new Bloque(tipo, nombre, inicio));
        LimpiarEncabezado();
    }

    private void TerminarSentencia(int numeroLinea)
    {
        string texto = encabezado.ToString();
        if (ClasificadorDeEncabezados.EsMetodo(texto) && texto.Contains("=>"))
        {
            cerrados.Add(new BloqueCerrado(
                TipoBloque.Metodo,
                ClasificadorDeEncabezados.NombreMetodo(texto),
                RutaDeTipos(),
                lineaEncabezado,
                numeroLinea));
        }
        LimpiarEncabezado();
    }

    private void AcumularEncabezado(char c, int numeroLinea)
    {
        if (encabezado.Length == 0)
        {
            if (char.IsWhiteSpace(c))
            {
                return;
            }
            lineaEncabezado = numeroLinea;
        }
        encabezado.Append(c);
    }

    private void LimpiarEncabezado()
    {
        encabezado.Clear();
        lineaEncabezado = 0;
    }

    /// <summary>Nombres de los bloques de tipo en la pila, unidos con "." (Externa.Interna).</summary>
    private string RutaDeTipos()
    {
        return string.Join(".", pila.Where(b => b.Tipo == TipoBloque.Clase).Select(b => b.Nombre));
    }
}
