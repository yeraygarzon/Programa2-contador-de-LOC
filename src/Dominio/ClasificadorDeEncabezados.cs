using System.Text.RegularExpressions;

namespace ContadorLoc.Dominio;

/// <summary>Decide qué tipo de bloque abre una llave según el texto que la precede.</summary>
public static class ClasificadorDeEncabezados
{
    private static readonly string[] PalabrasDeTipo = { "class", "struct", "interface", "enum", "record" };

    public static (TipoBloque Tipo, string Nombre) Clasificar(string encabezado)
    {
        string prefijo = QuitarGenericos(PrefijoHastaParentesis(encabezado));
        List<string> palabras = ExtraerPalabras(prefijo);

        if (palabras.Contains("namespace"))
        {
            return (TipoBloque.Namespace, "");
        }

        string? nombreDeTipo = BuscarNombreDeTipo(palabras);
        if (nombreDeTipo != null)
        {
            return (TipoBloque.Clase, nombreDeTipo);
        }
        if (EsMetodo(encabezado))
        {
            return (TipoBloque.Metodo, NombreMetodo(prefijo, palabras));
        }
        return (TipoBloque.Otro, "");
    }

    /// <summary>La primera "(" aparece antes que cualquier "=".</summary>
    public static bool EsMetodo(string encabezado)
    {
        int parentesis = encabezado.IndexOf('(');
        int igual = encabezado.IndexOf('=');
        return parentesis >= 0 && (igual < 0 || parentesis < igual);
    }

    public static string NombreMetodo(string encabezado)
    {
        string prefijo = QuitarGenericos(PrefijoHastaParentesis(encabezado));
        return NombreMetodo(prefijo, ExtraerPalabras(prefijo));
    }

    private static string NombreMetodo(string prefijo, List<string> palabras)
    {
        if (palabras.Contains("operator"))
        {
            int posicion = prefijo.LastIndexOf("operator", StringComparison.Ordinal);
            return "operator " + prefijo.Substring(posicion + "operator".Length).Trim();
        }
        return palabras.Count > 0 ? palabras[^1] : "?";
    }

    /// <summary>Si hay class, struct, interface, enum o record, devuelve la palabra que sigue. Si no, null.</summary>
    private static string? BuscarNombreDeTipo(List<string> palabras)
    {
        int indice = palabras.FindIndex(p => PalabrasDeTipo.Contains(p));
        if (indice < 0)
        {
            return null;
        }
        while (indice + 1 < palabras.Count && PalabrasDeTipo.Contains(palabras[indice + 1]))
        {
            indice++;
        }
        return indice + 1 < palabras.Count ? palabras[indice + 1] : null;
    }

    private static List<string> ExtraerPalabras(string texto)
    {
        return Regex.Matches(texto, @"[A-Za-z_]\w*").Select(m => m.Value).ToList();
    }

    private static string PrefijoHastaParentesis(string encabezado)
    {
        int parentesis = encabezado.IndexOf('(');
        return parentesis < 0 ? encabezado : encabezado.Substring(0, parentesis);
    }

    private static string QuitarGenericos(string texto)
    {
        string anterior;
        do
        {
            anterior = texto;
            texto = Regex.Replace(texto, "<[^<>]*>", "");
        }
        while (texto != anterior);
        return texto;
    }
}
