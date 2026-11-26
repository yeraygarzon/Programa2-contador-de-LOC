# Estándar de conteo de LOC (borrador)

> **Si tu instructor te dio un estándar, usa ese y cambia este archivo.** Este es un borrador para que el programa tenga reglas claras desde el inicio. Una vez confirmado, no se cambia durante el desarrollo.

## Qué es una LOC
Una **línea física de código**: cada línea del archivo con al menos un carácter que no sea espacio ni tabulación y que **no** sea solo comentario.

## Reglas
| # | Elemento | ¿Cuenta? |
|---|---|---|
| E1 | Línea en blanco | No |
| E2 | Línea con solo comentario `//` o `///` | No |
| E3 | Línea dentro de un bloque `/* ... */` (incluidas la de apertura y la de cierre si solo tienen comentario) | No |
| E4 | Línea con código y un comentario al final | **Sí**, 1 LOC |
| E5 | Línea con solo `{` o `}` | **Sí** |
| E6 | `using`, `namespace`, atributos `[...]` | **Sí** |
| E7 | Sentencia partida en varias líneas | **Sí**, cada línea física |
| E8 | `//` o `/*` dentro de un texto entre comillas | No es comentario: la línea cuenta |
| E9 | Líneas dentro de un texto literal de varias líneas (`@"..."`) | **Sí** |
| E10 | Directivas `#region`, `#if`, etc. | **Sí** |

## Clases
- La LOC de una **clase** (también `struct`, `record`, `interface`, `enum`) es el número de líneas contadas desde la línea de su declaración hasta su `}` de cierre, **incluidas** esas dos líneas.
- Si hay clases anidadas, se reportan aparte como `Externa.Interna`, y sus líneas **también** se incluyen en la clase externa.

## Métodos
- Un **método** incluye métodos, constructores, destructores y operadores, desde la línea de su firma hasta su `}` de cierre (o hasta el `;` si usa `=>`).
- Los comentarios y atributos que están **encima** de la firma no forman parte del método.
- Funciones locales y lambdas cuentan dentro del método que las contiene.
- Las propiedades y los campos **no** son métodos: cuentan en la clase pero no aparecen en la tabla de métodos.

## Tamaño total
LOC total = todas las líneas contadas del archivo (incluye `using` y `namespace`, que no pertenecen a ninguna clase). Por eso el total puede ser mayor que la suma de las clases.

## Límites declarados
- No se compila ni se interpreta el código: solo se analiza el texto.
- Se asume un solo tipo de comentario por línea y llaves balanceadas; si no lo están, se informa un error.
