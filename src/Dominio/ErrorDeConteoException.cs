namespace ContadorLoc.Dominio;

/// <summary>Error esperado: el archivo o el código no se pueden analizar.</summary>
public class ErrorDeConteoException : Exception
{
    public ErrorDeConteoException(string mensaje) : base(mensaje)
    {
    }
}
