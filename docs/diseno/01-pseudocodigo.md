# Pseudocódigo — Programa 2: Contador de LOC

Sigue `docs/planeacion/estandar-conteo.md` (reglas E1 a E10). El análisis trabaja solo con texto, sin compilar el código.

## Idea general: 3 pasos sobre la lista de líneas

1. **Clasificar** cada línea: ¿cuenta como LOC? Además se obtiene una versión *limpia* de la línea (sin comentarios y con el contenido de los textos vaciado), para que `//`, llaves o paréntesis dentro de comentarios o textos no confundan al paso 2.
2. **Detectar la estructura**: recorrer los caracteres de las líneas limpias con una pila de bloques abiertos, y anotar dónde empieza y termina cada clase y cada método.
3. **Medir**: con un acumulado de LOC por línea, la LOC de cualquier tramo es una resta.

## Estructuras de datos

```
LineaClasificada
    Cuenta : booleano          // ¿es una LOC según el estándar?
    Limpia : texto             // línea sin comentarios y con textos vaciados

Bloque                          // elemento de la pila
    Tipo : "namespace" | "tipo" | "metodo" | "otro"
    Nombre : texto
    LineaInicio : entero

ResultadoClase   (Nombre, Loc, CantidadMetodos)
ResultadoMetodo  (Clase, Nombre, Loc)
ResultadoConteo  (TotalLoc, Clases, Metodos)
```

## Paso 1 — Clasificar líneas

```
ClasificarLineas(lineas):
    enComentarioBloque = falso
    enTextoLiteral = falso               // @"..." que puede seguir en la línea siguiente
    resultado = lista vacía

    PARA CADA linea EN lineas
        limpia = ""
        hayCodigo = enTextoLiteral        // E9: toda línea dentro de un @"..." cuenta
        i = 0
        MIENTRAS i < longitud(linea)
            c = linea[i];  sig = linea[i+1] (o "" si no hay)

            SI enComentarioBloque ENTONCES
                SI c=="*" Y sig=="/" ENTONCES enComentarioBloque = falso; i = i + 2
                SINO i = i + 1
                CONTINUAR
            SI enTextoLiteral ENTONCES
                hayCodigo = verdadero
                SI c=="\"" Y sig=="\"" ENTONCES i = i + 2          // comilla escapada
                SINO SI c=="\"" ENTONCES enTextoLiteral = falso; i = i + 1
                SINO i = i + 1
                CONTINUAR

            SI c=="/" Y sig=="/" ENTONCES SALIR del MIENTRAS      // resto = comentario (E2, E4)
            SI c=="/" Y sig=="*" ENTONCES enComentarioBloque = verdadero; i = i + 2; CONTINUAR
            SI c=="@" Y sig=="\"" ENTONCES
                enTextoLiteral = verdadero; hayCodigo = verdadero
                limpia = limpia + "\"\""; i = i + 2; CONTINUAR
            SI c es "\"" o "'" ENTONCES                            // texto o carácter normal (E8)
                saltar hasta la comilla de cierre, tratando "\" + carácter como una unidad
                hayCodigo = verdadero
                limpia = limpia + c + c; CONTINUAR
            SI c no es espacio ni tabulación ENTONCES hayCodigo = verdadero
            limpia = limpia + c
            i = i + 1

        agregar LineaClasificada(Cuenta = hayCodigo, Limpia = limpia) a resultado

    SI enComentarioBloque ENTONCES error "comentario de bloque sin cerrar"
    SI enTextoLiteral ENTONCES error "texto literal sin cerrar"
    RETORNAR resultado
```

## Paso 2 — Detectar clases y métodos

```
DetectarEstructura(clasificadas):
    pila = pila vacía de Bloque
    encabezado = ""            // texto acumulado desde el último  ;  {  }  a nivel de tipo
    lineaEncabezado = nulo     // línea donde empezó el encabezado (sin contar atributos)
    profAtributo = 0           // > 0 mientras se salta un atributo [ ... ]
    bloquesCerrados = lista vacía  // (tipo, nombre, claseDueña, inicio, fin)

    PARA numeroLinea = 1 HASTA cantidad de líneas
        SI la línea limpia, sin espacios, empieza con "#" ENTONCES CONTINUAR   // E10: cuenta pero no es estructura

        PARA CADA c EN linea.Limpia
            nivelDeMiembros = pila vacía O tope.Tipo es "namespace" o "tipo"

            SI c == "}" ENTONCES
                SI pila vacía ENTONCES error "llave } sin abrir (línea N)"
                cerrado = sacar tope de la pila
                SI cerrado.Tipo es "tipo" o "metodo" ENTONCES
                    guardar (cerrado.Tipo, cerrado.Nombre, RutaDeTipos(), cerrado.LineaInicio, numeroLinea)
                encabezado = ""; lineaEncabezado = nulo
                CONTINUAR

            SI NO nivelDeMiembros ENTONCES          // dentro de un método o de un bloque "otro"
                SI c == "{" ENTONCES apilar Bloque("otro", "", numeroLinea)
                CONTINUAR                            // lambdas y funciones locales quedan dentro del método

            SI profAtributo > 0 ENTONCES
                SI c=="[" ENTONCES profAtributo++  SINO SI c=="]" ENTONCES profAtributo--
                CONTINUAR
            SI c=="[" Y encabezado vacío ENTONCES profAtributo = 1; CONTINUAR   // atributo: no forma parte del método

            SI c == "{" ENTONCES
                (tipo, nombre) = ClasificarEncabezado(encabezado)
                apilar Bloque(tipo, nombre, lineaEncabezado, o numeroLinea si es nulo)
                encabezado = ""; lineaEncabezado = nulo
            SINO SI c == ";" ENTONCES
                SI EsMetodo(encabezado) Y encabezado contiene "=>" ENTONCES   // método con cuerpo de expresión
                    guardar ("metodo", NombreMetodo(encabezado), RutaDeTipos(), lineaEncabezado, numeroLinea)
                encabezado = ""; lineaEncabezado = nulo
            SINO
                SI encabezado vacío Y c es espacio ENTONCES CONTINUAR
                SI encabezado vacío ENTONCES lineaEncabezado = numeroLinea
                encabezado = encabezado + c
        al terminar la línea: SI encabezado no vacío ENTONCES encabezado = encabezado + " "

    SI pila no vacía ENTONCES error "llaves sin cerrar"
    RETORNAR bloquesCerrados
```

```
EsMetodo(encabezado):                  // la primera "(" aparece antes que cualquier "="
    p = posición de la primera "(" ;  e = posición del primer "="
    RETORNAR p existe Y (e no existe O p < e)

ClasificarEncabezado(encabezado):
    prefijo = texto antes de la primera "(", sin los <genéricos>
    palabras = identificadores del prefijo
    SI "namespace" está en palabras ENTONCES RETORNAR ("namespace", "")
    SI alguna palabra es class | struct | interface | enum | record ENTONCES
        RETORNAR ("tipo", palabra que sigue a la última palabra clave consecutiva)
    SI EsMetodo(encabezado) ENTONCES RETORNAR ("metodo", NombreMetodo(encabezado))
    RETORNAR ("otro", "")              // propiedades, indexadores, inicializadores, etc.

NombreMetodo(encabezado):              // último identificador antes de "(" ; "operator +" si es un operador
RutaDeTipos():                         // nombres de los bloques "tipo" de la pila unidos con "." (Externa.Interna)
```

## Paso 3 — Medir

```
Contar(lineas):
    clasificadas = ClasificarLineas(lineas)
    acumulado[0] = 0
    PARA i = 1 HASTA cantidad de líneas
        acumulado[i] = acumulado[i-1] + (1 si clasificadas[i].Cuenta, 0 si no)

    bloques = DetectarEstructura(clasificadas)
    PARA CADA b EN bloques
        loc = acumulado[b.fin] - acumulado[b.inicio - 1]
        SI b es "tipo" ENTONCES agregar ResultadoClase(b.ruta, loc) a clases
        SI b es "metodo" ENTONCES agregar ResultadoMetodo(b.clasePadre, b.nombre, loc) a métodos
    ordenar clases y métodos por línea de inicio
    CantidadMetodos de cada clase = cantidad de métodos cuya clase dueña es esa clase
    RETORNAR ResultadoConteo(TotalLoc = acumulado[última], clases, métodos)
```

## Infraestructura y aplicación

```
LeerArchivoFuente(ruta):
    SI la extensión no es ".cs" ENTONCES error "el archivo debe ser .cs"
    SI no existe el archivo ENTONCES error "archivo no encontrado: <ruta>"
    lineas = todas las líneas del archivo
    SI lineas está vacía O todas son blancas ENTONCES error "el archivo no contiene código"
    SI cantidad de líneas > 10 000 ENTONCES error "el archivo supera 10 000 líneas"
    RETORNAR lineas

Principal(argumentos):
    ruta = argumentos[0], o pedirla por consola si falta
    INTENTAR
        lineas = LeerArchivoFuente(ruta)
        resultado = Contar(lineas)
        mostrar tabla de clases, tabla de métodos y totales
        terminar con código 0
    SI ocurre error ENTONCES mostrar "Error: <mensaje>"; terminar con código 1
```

## Decisiones de diseño (confírmalas contra tu estándar)

- Una declaración **sin cuerpo** (método de interfaz o `abstract`, ej. `void F();`) **no** se reporta como método. El estándar actual no lo dice; si el tuyo sí, cambia `;` en el paso 2.
- Propiedades y campos suman a la clase, pero no aparecen en la tabla de métodos.
- Las líneas que empiezan con `#` (como `#region`) cuentan como LOC (E10) pero no participan en la detección de estructura.
- La LOC de un método **no** incluye los atributos ni los comentarios que están encima de su firma.

## Límites conocidos
No se soportan: textos crudos (`"""`), `@$"..."` partidos en varias líneas, ni sentencias de nivel superior (C# 9+). Se informan como limitación, no como error.
