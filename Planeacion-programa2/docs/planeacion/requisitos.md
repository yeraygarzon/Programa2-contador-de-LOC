# Requisitos — Programa 2: Contador de LOC

## 1. Preguntas del enunciado (con propuesta de respuesta)

| # | Pregunta | Propuesta | Confirmada |
|---|---|---|---|
| P1 | ¿Hay un estándar de conteo definido por el instructor? | Se usa `estandar-conteo.md` hasta que diga otra cosa | ☐ |
| P2 | ¿Se analiza un archivo o una carpeta? | Un solo archivo `.cs` por ejecución | ☐ |
| P3 | ¿Se permite usar Roslyn u otras librerías de análisis de C#? | No. El conteo se hace leyendo el texto línea a línea | ☐ |
| P4 | ¿Cómo se muestra el resultado? | Por consola, en tablas de texto. Sin archivo de salida | ☐ |
| P5 | ¿Se cuentan solo clases y métodos o también propiedades? | Solo clases y métodos (las propiedades suman en la clase) | ☐ |
| P6 | ¿Hay un límite de tamaño de archivo? | Hasta 10 000 líneas | ☐ |

## 2. Requisitos funcionales

| ID | Requisito | Criterio de aceptación |
|---|---|---|
| RF-01 | Recibir por argumento la ruta de un archivo `.cs` (o pedirla si falta) | Con `datos/Ejemplo.cs` el programa corre sin pedir más datos |
| RF-02 | Contar las LOC con el estándar definido | `datos/Ejemplo.cs` da 30 LOC en total |
| RF-03 | Mostrar la LOC de cada clase | Calculadora = 19, Mensajes = 7 |
| RF-04 | Mostrar la LOC de cada método, indicando su clase | Ctor = 4, Sumar = 5, Doble = 1, Url = 4, Saludar = 4 |
| RF-05 | Mostrar el tamaño total del programa y los totales de clases y métodos | Aparece `Total LOC = 30`, 2 clases y 5 métodos |
| RF-06 | Excluir blancos y comentarios según el estándar | El comentario de bloque y las líneas `///` de `Ejemplo.cs` no suman |
| RF-07 | Informar errores: archivo inexistente, vacío, extensión distinta de `.cs`, llaves desbalanceadas | Cada caso muestra `Error:` con su causa y devuelve código 1 |

## 3. Requisitos no funcionales

| ID | Requisito | Criterio de aceptación |
|---|---|---|
| RNF-01 | Rendimiento | Un archivo de 10 000 líneas se procesa en menos de 1 s |
| RNF-02 | Mismo resultado en cada ejecución | Dos ejecuciones sobre el mismo archivo dan salida idéntica |
| RNF-03 | El análisis no depende de la consola ni de archivos (capa Dominio) | Se puede probar con una lista de líneas en memoria |
| RNF-04 | Salida con cultura invariante | No cambia con la cultura del sistema |
| RNF-05 | Cumple el estándar de codificación | 0 errores y 0 advertencias al compilar |

## 4. Resultado esperado para `datos/Ejemplo.cs`
(Verificado con un script aparte siguiendo el estándar.)

| Elemento | LOC |
|---|---|
| Total del programa | 30 |
| Clase `Calculadora` | 19 |
| Clase `Mensajes` | 7 |
| `Calculadora.Calculadora` (constructor) | 4 |
| `Calculadora.Sumar` | 5 |
| `Calculadora.Doble` | 1 |
| `Calculadora.Url` | 4 |
| `Mensajes.Saludar` | 4 |

Nota: el texto `"http://..."` en `Url()` comprueba la regla E8.
