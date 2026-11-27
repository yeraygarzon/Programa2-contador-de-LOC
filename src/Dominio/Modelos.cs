namespace ContadorLoc.Dominio;

/// <summary>Clase de bloque que se puede abrir con una llave.</summary>
public enum TipoBloque
{
    Namespace,
    Clase,   // class, struct, interface, enum, record
    Metodo,  // métodos, constructores y operadores
    Otro     // propiedades, inicializadores, sentencias dentro de métodos
}

/// <summary>Resultado de clasificar una línea: si cuenta como LOC y su versión sin comentarios ni textos.</summary>
public readonly record struct LineaClasificada(bool Cuenta, string Limpia);

/// <summary>Bloque abierto que está en la pila del detector.</summary>
public sealed record Bloque(TipoBloque Tipo, string Nombre, int LineaInicio);

/// <summary>Bloque ya cerrado, con el tramo de líneas que ocupa.</summary>
public sealed record BloqueCerrado(TipoBloque Tipo, string Nombre, string Clase, int Inicio, int Fin);

public sealed record ResultadoClase(string Nombre, int Loc, int CantidadMetodos);

public sealed record ResultadoMetodo(string Clase, string Nombre, int Loc);

public sealed record ResultadoConteo(
    int TotalLoc,
    IReadOnlyList<ResultadoClase> Clases,
    IReadOnlyList<ResultadoMetodo> Metodos);
