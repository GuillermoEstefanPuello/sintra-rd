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

## Roadmap del proyecto

Definido con manin el 2026-07-01 a partir del inventario de los 17 modulos de fase3-arquitectura.md.

| Bloque | Modulo / Tema | Notas |
|---|---|---|
| Bloque 4 | Seguridad | JWT, rate limiting, CORS, cabeceras HTTP |
| Bloque 5 | M11 — Portal del Contribuyente | Frontend/UI |
| Bloque 6 | M7b — Cuenta Corriente Tributaria | |
| Bloque 7 | M4 — Declaraciones Electronicas | |
| Bloque 8 | M17 — Notificaciones | |
| Bloque 9+ | Evaluar caso por caso | M1, M2, M6, M7, M9, M14, M15 |

**Fuera de alcance por ahora:** M5 (e-CF / Facturacion Electronica), M8 (Fiscalizacion con IA),
M13 (Interoperabilidad X-Road).
Motivo: complejidad Muy Alta desproporcionada para un demo de portafolio.
Se reevaluan si el proyecto avanza a produccion real.

---

## Que se construyo (Bloque 4 — en curso)

### Hito 4.1 — Capa 2: proteccion de la API (completado)

- `Middleware/CabecerasSeguridad.cs`: 7 cabeceras HTTP en todas las respuestas
  (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy,
  Content-Security-Policy, Permissions-Policy, Cache-Control)
- `appsettings.json`: origenes CORS en configuracion (localhost:5173 para M11)
- `ServiceCollectionExtensions.cs`: metodos AgregarCors y AgregarRateLimiting
  - GlobalLimiter: 100 solicitudes por minuto por IP (todas las rutas)
  - Politica "liquidaciones": 20 solicitudes por minuto por IP (POST /liquidaciones)
- `Program.cs`: pipeline actualizado en orden correcto —
  HSTS -> HTTPS -> cabeceras -> CORS -> rate limiting -> autorizacion -> controladores
- `LiquidacionController.cs`: [EnableRateLimiting("liquidaciones")] sobre el POST
- Verificado: build limpio (0 errores, 0 advertencias) + 7/7 tests sin regresiones

### Pendiente (proxima sesion)

- Hito 4.2: FluentValidation sobre SolicitudLiquidacionDto
- Hito 4.3: JWT en cookies HttpOnly + endpoint /auth/login
- Hito 4.4: Lista negra de tokens (logout, IMemoryCache)
- Hito 4.5: Logging estructurado de eventos de seguridad

---

## Problemas conocidos / pendientes

- JsonStringEnumConverter resuelto: ASP.NET Core no deserializa enums como strings
  por defecto; se configuro en Program.cs.

---

*Ultima actualizacion: Hito 4.1 completado. Build limpio + 7/7 tests sin regresiones. Hito 4.2 pendiente.*
