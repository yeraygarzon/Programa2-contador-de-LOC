# Traza de escritorio — Programa 2

**Hazla tú primero a mano**, con `datos/Ejemplo.cs`, siguiendo el pseudocódigo: primero marca qué líneas cuentan, luego recorre las llaves con una pila. Después compara con esta tabla. Cada diferencia es un defecto de diseño: regístralo en el momento.

## Clasificación y eventos de estructura
| Línea | Texto | ¿Cuenta? | LOC acum. | Evento de estructura |
|---|---|---|---|---|
| 1 | `using System;` | Sí | 1 |  |
| 2 | *(blanco)* | No | 1 | |
| 3 | `// Ejemplo para probar el contador` | No | 1 |  |
| 4 | `namespace Demo` | Sí | 2 |  |
| 5 | `{` | Sí | 3 | { abre namespace (inicio 4) |
| 6 | `/// <summary>Clase de ejemplo.</summary>` | No | 3 |  |
| 7 | `public class Calculadora` | Sí | 4 |  |
| 8 | `{` | Sí | 5 | { abre tipo Calculadora (inicio 7) |
| 9 | `private int total;` | Sí | 6 |  |
| 10 | *(blanco)* | No | 6 | |
| 11 | `public int Total { get; set; }` | Sí | 7 | { abre bloque otro (inicio 11); } lo cierra (propiedad, no se registra) |
| 12 | *(blanco)* | No | 7 | |
| 13 | `public Calculadora()` | Sí | 8 |  |
| 14 | `{` | Sí | 9 | { abre metodo Calculadora (inicio 13) |
| 15 | `total = 0; // inicio` | Sí | 10 |  |
| 16 | `}` | Sí | 11 | } cierra metodo Calculadora |
| 17 | *(blanco)* | No | 11 | |
| 18 | `/* Suma un valor` | No | 11 |  |
| 19 | `y devuelve el total */` | No | 11 |  |
| 20 | `public int Sumar(int x)` | Sí | 12 |  |
| 21 | `{` | Sí | 13 | { abre metodo Sumar (inicio 20) |
| 22 | `total += x;` | Sí | 14 |  |
| 23 | `return total;` | Sí | 15 |  |
| 24 | `}` | Sí | 16 | } cierra metodo Sumar |
| 25 | *(blanco)* | No | 16 | |
| 26 | `public int Doble(int x) => x * 2;` | Sí | 17 | ; cierra metodo Doble (=>) |
| 27 | *(blanco)* | No | 17 | |
| 28 | `public string Url()` | Sí | 18 |  |
| 29 | `{` | Sí | 19 | { abre metodo Url (inicio 28) |
| 30 | `return "http://ejemplo.com"; // la // dentro del texto no es comentario` | Sí | 20 |  |
| 31 | `}` | Sí | 21 | } cierra metodo Url |
| 32 | `}` | Sí | 22 | } cierra clase Calculadora |
| 33 | *(blanco)* | No | 22 | |
| 34 | `public class Mensajes` | Sí | 23 |  |
| 35 | `{` | Sí | 24 | { abre tipo Mensajes (inicio 34) |
| 36 | `public void Saludar()` | Sí | 25 |  |
| 37 | `{` | Sí | 26 | { abre metodo Saludar (inicio 36) |
| 38 | `Console.WriteLine("Hola");` | Sí | 27 |  |
| 39 | `}` | Sí | 28 | } cierra metodo Saludar |
| 40 | `}` | Sí | 29 | } cierra clase Mensajes |
| 41 | `}` | Sí | 30 | } cierra namespace (no se registra) |

## Medición (paso 3)
LOC de un tramo = acumulado[fin] − acumulado[inicio − 1]

| Elemento | Inicio | Fin | Cálculo | LOC |
|---|---|---|---|---|
| Constructor `Calculadora` | 13 | 16 | 11 − 7 | 4 |
| `Sumar` | 20 | 24 | 16 − 11 | 5 |
| `Doble` | 26 | 26 | 17 − 16 | 1 |
| `Url` | 28 | 31 | 21 − 17 | 4 |
| Clase `Calculadora` | 7 | 32 | 22 − 3 | 19 |
| `Saludar` | 36 | 39 | 28 − 24 | 4 |
| Clase `Mensajes` | 34 | 40 | 29 − 22 | 7 |
| **Total del programa** | 1 | 41 | acumulado[41] | **30** |

Coincide con `requisitos.md`: 30 total; clases 19 y 7; métodos 4, 5, 1, 4 y 4.

## Qué comprobar en la traza
- El `//` dentro de `"http://ejemplo.com"` (línea 30) no corta la línea: el texto se vacía en la versión limpia.
- Las líneas 18 y 19 (comentario de bloque) no cuentan y no ensucian el encabezado de `Sumar`.
- `Total { get; set; }` abre y cierra un bloque `otro` y no aparece como método.
- `Doble` se cierra en el `;`, no en una llave.

## Casos pequeños para trazar también
| Entrada | Resultado esperado |
|---|---|
| Archivo con solo `}` | Error: llave `}` sin abrir (línea 1) |
| `class A {` y `void M() {` sin cerrar | Error: llaves sin cerrar |
| `/* sin cerrar` | Error: comentario de bloque sin cerrar |
