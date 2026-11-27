using ContadorLoc.Dominio;

namespace ContadorLoc.Presentacion;

/// <summary>Entrada y salida por consola. No contiene lógica de conteo.</summary>
public static class ConsolaUI
{
    public static string ObtenerRuta(string[] args)
    {
        if (args.Length > 0)
        {
            return args[0];
        }
        Console.Write("Ruta del archivo .cs: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public static void MostrarResultado(ResultadoConteo resultado)
    {
        Console.WriteLine("Clases");
        Console.WriteLine(FormattableString.Invariant($"{"Clase",-30} {"LOC",5} {"Métodos",9}"));
        foreach (ResultadoClase c in resultado.Clases)
        {
            Console.WriteLine(FormattableString.Invariant($"{c.Nombre,-30} {c.Loc,5} {c.CantidadMetodos,9}"));
        }

        Console.WriteLine();
        Console.WriteLine("Métodos");
        Console.WriteLine(FormattableString.Invariant($"{"Clase.Método",-30} {"LOC",5}"));
        foreach (ResultadoMetodo m in resultado.Metodos)
        {
            Console.WriteLine(FormattableString.Invariant($"{m.Clase + "." + m.Nombre,-30} {m.Loc,5}"));
        }

        Console.WriteLine();
        Console.WriteLine(FormattableString.Invariant($"Clases = {resultado.Clases.Count}"));
        Console.WriteLine(FormattableString.Invariant($"Métodos = {resultado.Metodos.Count}"));
        Console.WriteLine(FormattableString.Invariant($"Total LOC = {resultado.TotalLoc}"));
    }

    public static void MostrarError(string mensaje)
    {
        Console.Error.WriteLine($"Error: {mensaje}");
    }
}
