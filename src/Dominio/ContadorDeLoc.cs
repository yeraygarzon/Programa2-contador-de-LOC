namespace ContadorLoc.Dominio;

/// <summary>Paso 3: junta la clasificación y la estructura y mide cada clase y método con un acumulado de LOC.</summary>
public static class ContadorDeLoc
{
    public static ResultadoConteo Contar(IReadOnlyList<string> lineas)
    {
        List<LineaClasificada> clasificadas = ClasificadorDeLineas.Clasificar(lineas);
        int[] acumulado = CalcularAcumulado(clasificadas);
        List<BloqueCerrado> bloques = DetectorDeEstructura.Detectar(clasificadas);

        List<ResultadoMetodo> metodos = ConstruirMetodos(bloques, acumulado);
        List<ResultadoClase> clases = ConstruirClases(bloques, acumulado, metodos);
        return new ResultadoConteo(acumulado[^1], clases, metodos);
    }

    private static int[] CalcularAcumulado(IReadOnlyList<LineaClasificada> lineas)
    {
        var acumulado = new int[lineas.Count + 1];
        for (int i = 1; i <= lineas.Count; i++)
        {
            acumulado[i] = acumulado[i - 1] + (lineas[i - 1].Cuenta ? 1 : 0);
        }
        return acumulado;
    }

    private static int Medir(int[] acumulado, BloqueCerrado bloque)
    {
        return acumulado[bloque.Fin] - acumulado[bloque.Inicio - 1];
    }

    private static List<ResultadoMetodo> ConstruirMetodos(List<BloqueCerrado> bloques, int[] acumulado)
    {
        return bloques
            .Where(b => b.Tipo == TipoBloque.Metodo)
            .OrderBy(b => b.Inicio)
            .Select(b => new ResultadoMetodo(b.Clase, b.Nombre, Medir(acumulado, b)))
            .ToList();
    }

    private static List<ResultadoClase> ConstruirClases(
        List<BloqueCerrado> bloques, int[] acumulado, List<ResultadoMetodo> metodos)
    {
        return bloques
            .Where(b => b.Tipo == TipoBloque.Clase)
            .OrderBy(b => b.Inicio)
            .Select(b => new ResultadoClase(
                b.Nombre,
                Medir(acumulado, b),
                metodos.Count(m => m.Clase == b.Nombre)))
            .ToList();
    }
}
