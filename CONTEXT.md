# SINTRA-RD — Bitacora del Proyecto

DEVGEP+ | Guillermo Estefan Puello | StarBound SRL

Este archivo registra el estado actual del proyecto, las decisiones tomadas y
lo que viene a continuacion. Se actualiza al completar cada hito, no antes ni despues.

---

## Estado actual

**Bloque completado:** Bloque 3 — Pruebas unitarias del Motor de Liquidacion

**Ultimo commit:** 8 pruebas unitarias del Motor de Liquidacion, todas correctas

---

## Que se construyo (Bloque 2)

- 8 entidades del dominio fiscal: 3 enums (TipoImpuesto, TipoTasa, TipoOperacion)
  y 5 clases (Tasa, Exencion, ReglaTributaria, DetalleLiquidacion, Liquidacion)
- Regla tributaria ITBIS en JSON (`itbis-2016-v1.json`): 4 tasas y 14 exenciones
  segun Arts. 343-344 Ley 11-92, vigente desde 2016-01-01
- IReglaTributariaRepository + ReglaTributariaRepository: lee y filtra JSONs por vigencia
- IMotorLiquidacion + MotorLiquidacion: calcula ITBIS ejecutando reglas de M16
- SolicitudLiquidacionDto: contrato de entrada del motor
- POST api/v1/liquidaciones: endpoint HTTP versionado que expone el motor
- JsonStringEnumConverter configurado: la API acepta strings en lugar de enteros para enums
- Demostracion central validada: cambiar 0.18 a 0.20 en el JSON cambia el resultado
  de 180.00 a 200.00 sin tocar una linea del motor ni reiniciar la API

---

## Que se construyo (Bloque 1)

- Solucion `SintraRd.sln` con proyecto `SintraRd.Api` en **.NET 9**
- Estructura de capas en `SintraRd.Api/`:
  - `Controllers/` — capa HTTP
  - `Services/` + `Services/Interfaces/` — logica de negocio
  - `Repositories/` + `Repositories/Interfaces/` — acceso a datos
  - `Models/Entidades/` — entidades del dominio fiscal
  - `Models/Dtos/` — contratos de entrada/salida de la API
  - `MotorReglas/Reglas/` — configuracion tributaria (M16), no codigo
  - `Middleware/` — middleware personalizado
  - `Infrastructure/` — registro de servicios y extensiones de startup
- `.gitignore` blindado (bin/, obj/, secretos, claves, IDE, .claude/)
- Repositorio Git inicializado con dos commits limpios
- `Program.cs` documentado en espanol con pipeline base de ASP.NET Core

---

## Decisiones registradas

| Decision | Motivo |
|---|---|
| .NET 9 en lugar de .NET 8 | Solo habia SDK 9 instalado en la maquina. Aprobado por manin. |
| `--use-controllers` en lugar de Minimal API | La arquitectura de 17 modulos requiere estructura, no conveniencia. |
| `appsettings.json` versionado, `appsettings.Local.json` gitignoreado | Los secretos van en local o variables de entorno, nunca en el repo. |
| `.claude/` excluido del repo | Es configuracion local del agente, no codigo del proyecto. |

---

## Que se construyo (Bloque 3)

- Proyecto `SintraRd.Tests` con xUnit (net9.0), agregado a la solucion con
  referencia a `SintraRd.Api`
- `SintraRd.Tests/Fakes/ReglaTributariaRepositoryFake.cs`: implementacion falsa
  de `IReglaTributariaRepository` que recibe `ReglaTributaria?` en el constructor;
  permite controlar el repositorio en pruebas sin acceso al sistema de archivos
- `SintraRd.Tests/MotorLiquidacion/MotorLiquidacionTests.cs`: 7 pruebas unitarias
  del Motor de Liquidacion (M3), todas verificadas en local fuera de Claude Code:
  - General 18% sin credito: 1000 -> MontoImpuesto 180, TotalAPagar 180
  - General 18% con credito 50: MontoImpuesto 180, TotalAPagar 130
  - Reducida 16%: 500 -> MontoImpuesto 80
  - Exento: MontoImpuesto 0, TotalAPagar 0, TipoTasaAplicada == Exento
  - TasaCero: MontoImpuesto 0, TipoTasaAplicada == TasaCero (distinto de Exento)
  - Sin regla vigente (fake con null): lanza InvalidOperationException con "vigente"
  - Credito 200 > impuesto 180: TotalAPagar == -20 (saldo a favor)

---

## Que viene (Bloque 4)

Por definir con manin. Opciones naturales:
- Capa de seguridad (JWT, rate limiting, cabeceras HTTP)
- Interfaz web simple para ingresar operaciones y ver el calculo en tiempo real

---

## Problemas conocidos / pendientes

- JsonStringEnumConverter resuelto: ASP.NET Core no deserializa enums como strings
  por defecto; se configuro en Program.cs.

---

*Ultima actualizacion: Bloque 3 completado. 8 de 8 pruebas unitarias correctas, verificadas en local.*
