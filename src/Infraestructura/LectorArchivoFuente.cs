using ContadorLoc.Dominio;

namespace ContadorLoc.Infraestructura;

/// <summary>Lee un archivo .cs y valida que se pueda analizar.</summary>
public static class LectorArchivoFuente
{
    private const int MaximoLineas = 10_000;

    public static string[] Leer(string ruta)
    {
        if (!string.Equals(Path.GetExtension(ruta), ".cs", StringComparison.OrdinalIgnoreCase))
        {
            throw new ErrorDeConteoException("el archivo debe ser .cs");
        }
        if (!File.Exists(ruta))
        {
            throw new ErrorDeConteoException($"archivo no encontrado: {ruta}");
        }

        string[] lineas = File.ReadAllLines(ruta);
        if (lineas.All(string.IsNullOrWhiteSpace))
        {
            throw new ErrorDeConteoException("el archivo no contiene código");
        }
        if (lineas.Length > MaximoLineas)
        {
            throw new ErrorDeConteoException($"el archivo supera {MaximoLineas} líneas");
        }
        return lineas;
    }
}
