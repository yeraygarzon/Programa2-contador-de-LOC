# Trazabilidad diseño → código

| Pseudocódigo | Archivo / método |
|---|---|
| Paso 1 `ClasificarLineas` | `src/Dominio/ClasificadorDeLineas.cs` → `Clasificar`, `ClasificarLinea`, `AvanzarUnPaso`, `AvanzarCodigo`, `SaltarTexto` |
| Paso 2 `DetectarEstructura` | `src/Dominio/DetectorDeEstructura.cs` → `Detectar`, `ProcesarCaracter`, `CerrarBloque`, `AbrirBloque`, `TerminarSentencia` |
| `ClasificarEncabezado`, `EsMetodo`, `NombreMetodo` | `src/Dominio/ClasificadorDeEncabezados.cs` |
| Paso 3 `Contar` | `src/Dominio/ContadorDeLoc.cs` → `Contar`, `CalcularAcumulado`, `Medir` |
| `LeerArchivoFuente` | `src/Infraestructura/LectorArchivoFuente.cs` → `Leer` |
| Interfaz | `src/Presentacion/ConsolaUI.cs` |
| `Principal` | `src/Aplicacion/Programa.cs` → `Main` |
| Estructuras de datos | `src/Dominio/Modelos.cs` |

El pseudocódigo hacía todo el paso 1 y el paso 2 en un solo bloque; en el código se partió en métodos pequeños para cumplir el estándar de 30 líneas. La lógica es la misma.

## Orden de escritura (de adentro hacia afuera)
1. Dominio: `Modelos` → `ErrorDeConteoException` → `ClasificadorDeLineas` → `ClasificadorDeEncabezados` → `DetectorDeEstructura` → `ContadorDeLoc`
2. Infraestructura: `LectorArchivoFuente`
3. Presentación: `ConsolaUI`
4. Aplicación: `Programa`

## Regla de la fase
**No compiles ni ejecutes todavía.** Los errores de compilación se registran en la fase siguiente. Si descubres un fallo del diseño mientras escribes, regístralo como defecto inyectado en Diseño y eliminado en Código.
