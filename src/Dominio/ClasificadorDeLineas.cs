using System.Text;

namespace ContadorLoc.Dominio;

/// <summary>Paso 1: decide qué líneas cuentan como LOC y limpia comentarios y textos.</summary>
public static class ClasificadorDeLineas
{
    private sealed class EstadoLectura
    {
        public bool EnComentarioBloque { get; set; }
        public bool EnTextoLiteral { get; set; }
    }

    /// <summary>Línea que se está leyendo, con la posición actual y lo acumulado hasta ahora.</summary>
    private sealed class LecturaLinea
    {
        public string Linea { get; }
        public int Posicion { get; set; }
        public bool HayCodigo { get; set; }
        public StringBuilder Limpia { get; } = new();

        public LecturaLinea(string linea, bool hayCodigoInicial)
        {
            Linea = linea;
            HayCodigo = hayCodigoInicial;
        }

        public bool Quedan => Posicion < Linea.Length;
        public char Actual => Linea[Posicion];
        public char Siguiente => Posicion + 1 < Linea.Length ? Linea[Posicion + 1] : '\0';
    }

    public static List<LineaClasificada> Clasificar(IReadOnlyList<string> lineas)
    {
        var estado = new EstadoLectura();
        var resultado = new List<LineaClasificada>();
        foreach (string linea in lineas)
        {
            resultado.Add(ClasificarLinea(linea, estado));
        }

        if (estado.EnComentarioBloque)
        {
            throw new ErrorDeConteoException("comentario de bloque sin cerrar");
        }
        if (estado.EnTextoLiteral)
        {
            throw new ErrorDeConteoException("texto literal sin cerrar");
        }
        return resultado;
    }

    private static LineaClasificada ClasificarLinea(string linea, EstadoLectura estado)
    {
        // E9: toda línea que empieza dentro de un @"..." cuenta como código
        var lectura = new LecturaLinea(linea, estado.EnTextoLiteral);
        while (lectura.Quedan)
        {
            if (!AvanzarUnPaso(lectura, estado))
            {
                break; // comentario // : el resto de la línea se ignora
            }
        }
        return new LineaClasificada(lectura.HayCodigo, lectura.Limpia.ToString());
    }

    /// <summary>Procesa un carácter (o un par). Devuelve false si empieza un comentario de línea.</summary>
    private static bool AvanzarUnPaso(LecturaLinea lectura, EstadoLectura estado)
    {
        char c = lectura.Actual;
        char sig = lectura.Siguiente;

        if (estado.EnComentarioBloque)
        {
            lectura.Posicion = AvanzarComentarioBloque(c, sig, lectura.Posicion, estado);
            return true;
        }
        if (estado.EnTextoLiteral)
        {
            lectura.HayCodigo = true;
            lectura.Posicion = AvanzarTextoLiteral(c, sig, lectura.Posicion, estado);
            return true;
        }
        if (c == '/' && sig == '/')
        {
            return false;
        }
        AvanzarCodigo(lectura, estado, c, sig);
        return true;
    }

    private static void AvanzarCodigo(LecturaLinea lectura, EstadoLectura estado, char c, char sig)
    {
        if (c == '/' && sig == '*')
        {
            estado.EnComentarioBloque = true;
            lectura.Posicion += 2;
        }
        else if (c == '@' && sig == '"')
        {
            estado.EnTextoLiteral = true;
            lectura.HayCodigo = true;
            lectura.Limpia.Append("\"\"");
            lectura.Posicion += 2;
        }
        else if (c == '"' || c == '\'')
        {
            lectura.HayCodigo = true;
            lectura.Limpia.Append(c).Append(c);
            lectura.Posicion = SaltarTexto(lectura.Linea, lectura.Posicion);
        }
        else
        {
            if (!char.IsWhiteSpace(c))
            {
                lectura.HayCodigo = true;
            }
            lectura.Limpia.Append(c);
            lectura.Posicion++;
        }
    }

    private static int AvanzarComentarioBloque(char c, char sig, int posicion, EstadoLectura estado)
    {
        if (c == '*' && sig == '/')
        {
            estado.EnComentarioBloque = false;
            return posicion + 2;
        }
        return posicion + 1;
    }

    private static int AvanzarTextoLiteral(char c, char sig, int posicion, EstadoLectura estado)
    {
        if (c == '"' && sig == '"')
        {
            return posicion + 2; // comilla escapada dentro de @"..."
        }
        if (c == '"')
        {
            estado.EnTextoLiteral = false;
        }
        return posicion + 1;
    }

    /// <summary>Salta un texto o carácter normal. Devuelve la posición siguiente a la comilla de cierre.</summary>
    private static int SaltarTexto(string linea, int posicion)
    {
        char comilla = linea[posicion];
        posicion++;
        while (posicion < linea.Length && linea[posicion] != comilla)
        {
            posicion += linea[posicion] == '\\' ? 2 : 1;
        }
        return posicion + 1;
    }
}
