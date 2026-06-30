# SINTRA-RD — Bitacora del Proyecto

DEVGEP+ | Guillermo Estefan Puello | StarBound SRL

Este archivo registra el estado actual del proyecto, las decisiones tomadas y
lo que viene a continuacion. Se actualiza al completar cada hito, no antes ni despues.

---

## Estado actual

**Bloque completado:** Bloque 1 — Cimientos

**Ultimo commit:** Paso 3 — Program.cs limpio y CONTEXT.md creado

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

## Que viene (Bloque 2)

Construccion del nucleo del demo tecnico:

1. Entidades del dominio fiscal (M3 + M16): `ReglaTributaria`, `Liquidacion`,
   `BaseImponible`, `Tasa`, `Exencion`, con `decimal` para todos los montos.
2. Motor de Reglas (M16): repositorio de reglas ITBIS en JSON, versionadas
   por fecha de vigencia.
3. Motor de Liquidacion (M3): servicio que carga las reglas vigentes y calcula
   el ITBIS.
4. Endpoint de la API que expone el calculo.
5. Demostracion: cambiar una regla en configuracion y ver el resultado cambiar
   sin tocar codigo del motor.

---

## Problemas conocidos / pendientes

- Ninguno en este bloque.

---

*Ultima actualizacion: Bloque 1, Paso 3 completado.*
