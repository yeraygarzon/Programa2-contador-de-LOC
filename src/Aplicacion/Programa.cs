using ContadorLoc.Dominio;
using ContadorLoc.Infraestructura;
using ContadorLoc.Presentacion;

namespace ContadorLoc.Aplicacion;

/// <summary>Orquesta el flujo: pedir ruta, leer, contar y mostrar. Único lugar que captura errores.</summary>
public static class Programa
{
    public static int Main(string[] args)
    {
        try
        {
            string ruta = ConsolaUI.ObtenerRuta(args);
            string[] lineas = LectorArchivoFuente.Leer(ruta);
            ResultadoConteo resultado = ContadorDeLoc.Contar(lineas);
            ConsolaUI.MostrarResultado(resultado);
            return 0;
        }
        catch (Exception ex) when (ex is ErrorDeConteoException || ex is IOException || ex is UnauthorizedAccessException)
        {
            ConsolaUI.MostrarError(ex.Message);
            return 1;
        }
    }
}
