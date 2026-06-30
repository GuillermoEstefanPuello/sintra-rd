# SINTRA-RD — Reglas de Trabajo (DEVGEP+ para Claude Code)

Este archivo define COMO trabajamos en este proyecto. Leelo completo al inicio de
cada sesion. Las reglas aqui descritas son obligatorias, no sugerencias.

Autor del proyecto: Guillermo Estefan Puello (te llamaras a el "manin" en el tono
de las respuestas). Metodologia: DEVGEP+. StarBound SRL.

---

## 0. EL TRUCO — Consultar la documentacion antes de construir

Antes de construir, modificar o explicar cualquier modulo de este sistema, **lee
primero el archivo correspondiente en la carpeta `docs/`**. Esa carpeta contiene
la investigacion completa del proyecto: el dominio tributario, la arquitectura,
los riesgos y el resumen ejecutivo.

Mapa de la documentacion:

- `docs/resumen-ejecutivo.md` — vision general del proyecto completo. Leelo primero
  si es la primera vez que tocas este repositorio en la sesion.
- `docs/fase1-investigacion.md` — el problema, el modelo de referencia (Estonia),
  el estado actual de la DGII, el e-CF.
- `docs/fase2-codigo-tributario.md` — las reglas legales de ITBIS, ISR, ISC,
  Activos e IPI. Consultar SIEMPRE antes de programar cualquier calculo fiscal.
  Las tasas, exenciones y formulas de este archivo son la fuente de verdad legal.
- `docs/fase3-arquitectura.md` — los 17 modulos del sistema, en que plano vive
  cada uno, sus entidades nucleo y sus dependencias. Consultar antes de crear
  cualquier modulo nuevo para respetar su responsabilidad y sus limites.
- `docs/fase4-diagramacion.md` — el detalle interno del Motor de Liquidacion (M3)
  y del e-CF (M5), con sus sub-componentes. Consultar antes de programar el motor.
- `docs/fase5-riesgos.md` — riesgos identificados y su mitigacion.

Nunca inventes una regla tributaria ni una tasa que no este documentada en
`docs/fase2-codigo-tributario.md`. Si hace falta un dato que no esta documentado,
preguntale a manin antes de asumir un valor.

---

## 1. El proyecto en una frase

SINTRA-RD es un demo tecnico que construye el nucleo de un sistema tributario
nacional: un Motor de Liquidacion Generico que calcula impuestos ejecutando
reglas que vienen de un Motor de Reglas configurable. La decision central es que
**la ley es configuracion, no codigo**. Stack: C# / .NET 8.

---

## 2. Reglas de Hierro (de DEVGEP+, aplican siempre, sin excepcion)

1. **Set-Location al inicio de cada script** — todo script PowerShell empieza
   confirmando la ubicacion antes de hacer nada.
2. **Boletin de progreso al final de cada hito** — que se hizo, que falta.
3. **Commit y push despues de cada hito** — siempre preguntar primero si manin ya
   probo en local antes de subir.
4. **Un paso a la vez, sin saltos** — nunca combinar varias tareas en un mismo
   movimiento sin confirmar cada una.
5. **Validacion real antes de continuar** — no asumir que algo funciono, verificarlo.
6. **Todo codigo comentado en espanol.**
7. **Firma en cada archivo nuevo:** `// DEVGEP+ | Guillermo Estefan Puello`
8. **Codigo SIN emojis.** Ni en comentarios, ni en strings, ni en logs, ni en
   ningun archivo del proyecto. Esto es una regla dura, sin excepciones.
9. **El codigo debe verse humano** — legible para un tecnico que lo revise despues,
   no codigo generado de forma robotica o sobre-abstraida sin necesidad.
10. **Bitacora viva del proyecto** — mantener `CONTEXT.md` actualizado.
11. **Explicacion tecnica corta y precisa en cada paso** — breve pero correcta.
12. **Terminos tecnicos explicados entre parentesis**, corto y preciso, la primera
    vez que aparecen.
13. **Boletin + pronostico de tiempo al final de cada hito** (cuando se estima
    terminar el siguiente).
14. **Especificar siempre en que terminal ejecutar cada comando** (ver seccion 5,
    Terminales).
15. **Cada 5 hitos, revisar y actualizar la documentacion global del proyecto.**
16. **`CONTEXT.md` se actualiza al completar cada hito**, no antes, no despues.
17. **Tono coloquial y directo, llamar al usuario "manin".**
18. **Nunca dos scripts a la vez** — uno, ver su salida, recien el siguiente.
19. **Si algo falla, diagnosticar antes de correr el siguiente comando.** Nunca
    seguir adelante sobre un error sin explicar.
20. **Cada sesion nueva: levantar el entorno completo antes de trabajar** (abrir
    las terminales que se vayan a usar y el proyecto en el editor).
21. **Antes de cualquier deploy, verificar versiones de dependencias criticas.**
22. **Cada decision de arquitectura importante se registra en `docs/decisions/`**
    como un archivo markdown (formato ADR: contexto, opciones, decision,
    consecuencias).
23. **Nunca usar `dotnet tool run` de un paquete no instalado en produccion** —
    usar siempre el binario/paquete ya restaurado localmente.
24. **Si se migra de una herramienta o enfoque a otro, documentar por que se
    abandono el anterior.**
25. **Todo bug resuelto se agrega a `docs/problemas-conocidos.md`.**
26. **Dos lineas explicando que se hace y por que, en cada cambio propuesto.**
27. **Orden al crear archivos: primero crear el archivo, luego abrirlo, luego
    escribir el contenido.**
28. **Recordatorios importantes solo al final de cada hito, no a mitad de tarea.**
29. **Formato de edicion: "Busca esto -> Reemplaza por esto."** Claro, reversible,
    rastreable. Nunca reescribir un archivo entero si un cambio puntual basta.
30. **Confirmar antes de proceder.** Ningun cambio se aplica sin que manin lo
    confirme explicitamente, salvo que el modo de permisos ya lo autorice.
31. **Nunca usar `&&` en PowerShell** — separar comandos con `;` o en lineas
    distintas.
32. **Usar `-LiteralPath`** para cualquier ruta que contenga corchetes u otros
    caracteres especiales.
33. **Rutas absolutas siempre** en cualquier comando o referencia a archivo.

---

## 3. Traduccion de reglas especificas de stack (Node/ShepherdBI -> C#/.NET)

Estas reglas existian en otro proyecto de manin (ShepherdBI, stack Node) y se
traducen asi para SINTRA-RD:

- **Version de .NET fija:** no actualizar la version de .NET ni los paquetes
  NuGet criticos del proyecto sin aprobacion explicita de manin.
- **Comandos `dotnet` desde el proyecto correcto:** nunca correr comandos `dotnet`
  de forma ambigua desde la raiz de la solucion si el comando aplica a un
  proyecto especifico (API o Web). Especificar siempre el `.csproj` o moverse a
  su carpeta primero.
- **El frontend siempre llama a la API por su ruta versionada** (por ejemplo
  `/api/v1/...`), nunca a una URL hardcodeada de otro ambiente.
- **Auditoria de dependencias periodica:** usar `dotnet list package --vulnerable`
  como equivalente a `pnpm audit`, antes de cada entrega importante.
- **Cultura/encoding invariante:** fijar `CultureInfo.InvariantCulture` en
  cualquier parseo o formato de numeros decimales, para evitar errores de
  separador decimal entre configuraciones regionales distintas.

---

## 4. Reglas nuevas, especificas de este proyecto (C# + dominio fiscal)

A. **Para dinero, SIEMPRE usar `decimal`. Nunca `double` ni `float`.** Esto es
   critico: un sistema que calcula impuestos no puede tener errores de redondeo
   de punto flotante. Cualquier monto, tasa o base imponible es `decimal`.

B. **Convencion de nombres C#:** `PascalCase` para todo miembro publico (clases,
   metodos, propiedades). `camelCase` para variables locales y parametros.
   Interfaces siempre empiezan con `I` (ejemplo: `IMotorLiquidacion`).

C. **La ley es configuracion, no codigo.** Ninguna tasa, exencion, base imponible
   o regla de retencion se escribe como valor fijo (hardcoded) dentro del Motor
   de Liquidacion. Toda regla tributaria vive en el Motor de Reglas, versionada
   por fecha de vigencia. Si una tarea pide "agregar una tasa", la respuesta
   correcta es agregar un registro de configuracion, no modificar el motor.

D. **Seguridad desde el primer commit.** Antes de cualquier primer commit del
   repositorio: `.gitignore` debe excluir archivos de configuracion con secretos,
   carpetas `bin/` y `obj/`, y cualquier archivo de entorno local. Ningun secreto,
   cadena de conexion o clave va jamas en el codigo fuente.

---

## 5. Terminales (igual que ShepherdBI, adaptado a .NET)

Este proyecto usa el mismo esquema de 4 terminales de ShepherdBI, pero se abren de
forma progresiva: solo cuando el componente correspondiente ya existe y hay algo
que correr en ella. No abrir una terminal antes de que haga falta.

- **Terminal 1 — Trabajo:** la terminal principal. Aqui corre Claude Code, los
  comandos de Git, y cualquier comando puntual de `dotnet` que no quede corriendo
  de forma indefinida (build, restore, migraciones). Activa desde el inicio del
  proyecto.
- **Terminal 2 — API:** corre `dotnet run` del proyecto de backend, dejando los
  logs visibles en vivo. Se abre quando el proyecto de API ya existe.
- **Terminal 3 — Web:** corre el servidor de desarrollo del frontend. Se abre
  cuando el proyecto web ya existe.
- **Docker:** para los contenedores de base de datos y/o Redis, si el bloque de
  trabajo actual los requiere. No se levanta antes de que haya algo que
  persistir.

Cada instruccion de Claude Code que implique ejecutar algo debe indicar
explicitamente en cual de estas terminales corre.

---

## 6. Arquitectura de Seguridad — 3 Capas (equivalente .NET de ShepherdBI)

Este proyecto reutiliza la arquitectura de seguridad de 3 capas que manin ya
valido en produccion con ShepherdBI (NestJS), traducida a su equivalente en
ASP.NET Core. Se implementa en un hito dedicado, no de forma parcial dispersa.

### Capa 1 — Identidad y Autenticacion
- JWT en cookies `HttpOnly`, `Secure`, `SameSite=Strict` (nunca en localStorage).
- Lista negra de tokens invalidados en Redis al hacer logout, con TTL igual al
  tiempo de vida restante del token.
- Validacion de configuracion al arrancar la aplicacion (Options pattern con
  validacion), para que la app falle de forma temprana y visible si falta una
  variable critica, en vez de arrancar de forma insegura en silencio.

### Capa 2 — Proteccion de la API
- Middleware de cabeceras de seguridad HTTP (equivalente a Helmet): politica de
  contenido, anti-clickjacking, HSTS, y demas cabeceras de seguridad estandar.
- Rate limiting nativo de .NET 8, granular por endpoint: limites mas estrictos
  para login y endpoints de escritura que para lectura general.
- Politica CORS explicita con lista blanca de origenes permitidos, nunca un
  comodin abierto.
- Validacion estricta de entrada con FluentValidation o Data Annotations en
  todos los modelos de entrada: rechazar campos no esperados, nunca confiar en
  el input del usuario (regla de Nivel 1).

### Capa 3 — Observabilidad y Resiliencia
- Logging estructurado de eventos de seguridad: intentos de login, accesos
  denegados, origenes CORS bloqueados.
- Auditoria de dependencias antes de cada entrega importante.
- Pista de auditoria inmutable para cualquier evento fiscal (liquidacion, pago).

---

## 7. Principios DEVGEP+ por Nivel de Obligatoriedad

### Nivel 1 — Innegociables, siempre
CRUD completo. ACID. Una unica fuente de verdad por dato (Single Source of
Truth). Idempotencia. Nunca confiar en el input del usuario. Fail fast.
Separacion de responsabilidades. Single Responsibility. Statelessness.

### Nivel 2 — Obligatorios por ser sistema critico, transversal o legal
Soft delete en vez de borrado fisico. Pista de auditoria inmutable. Principio de
minimo privilegio. Defensa en profundidad. Bajo acoplamiento, alta cohesion.
Circuit Breaker. Bulkhead Pattern. Eliminacion de puntos unicos de falla (SPOF),
especialmente critico en el validador de e-CF. Estandares de interoperabilidad.
Estrategia de recuperacion ante desastres (RTO/RPO). Los tres pilares de la
observabilidad (logs, metricas, trazas). Doce Factores (Twelve-Factor App).

### Nivel 3 — Importantes, se corrigen despues sin catastrofe
DRY. KISS. YAGNI. Problema N+1 de consultas. Estrategias de cache e
invalidacion. Consistencia eventual vs consistencia fuerte, segun el caso de
uso. Graceful degradation. Principio de Postel. Ley de Conway (la arquitectura
de modulos debe reflejar como se organizan los equipos que la mantienen).

### Nivel 3 — Principios de IA (aplican porque el sistema usa IA en su proceso)
Garbage In, Garbage Out amplificado: la calidad del dato de entrada determina la
calidad de cualquier deteccion de fraude o analisis. Human-in-the-loop: ninguna
sancion o accion sobre un contribuyente se ejecuta de forma automatica por un
modelo, siempre requiere decision humana registrada. No-determinismo de los LLM:
ningun calculo fiscal se delega a un modelo de lenguaje, los calculos viven en
el Motor de Liquidacion, deterministas. Grounding: cualquier salida de un
componente de IA debe estar anclada en datos reales del sistema, nunca inventada.
Inyeccion de prompts: blindar cualquier componente conversacional contra
manipulacion via input del usuario. Conciencia de costo: usar IA solo donde
aporta valor real, no por defecto.

---

## 8. Flujo de trabajo: local primero, produccion despues

Todo cambio se prueba en local. Solo se promueve a produccion cuando el cambio
ya convencio a manin en local. Nunca se hace deploy directo de algo no probado.

---

## 9. Checklist de seguridad por modulo nuevo

Antes de dar por cerrado cualquier modulo nuevo, verificar:

- [ ] Los endpoints que requieren autenticacion tienen su guard/atributo de
      autorizacion aplicado.
- [ ] Los modelos de entrada tienen validacion explicita de tipo y formato.
- [ ] No hay secretos, cadenas de conexion ni claves escritas en el codigo.
- [ ] Los montos y calculos fiscales usan `decimal`, nunca `double` o `float`.
- [ ] Si el modulo agrega una regla tributaria, esa regla vive en configuracion
      (Motor de Reglas), no hardcodeada.
- [ ] Existe al menos un registro de auditoria para las operaciones que
      modifican datos fiscales.

---

Fin de las reglas. Si alguna instruccion de manin entra en conflicto con una
regla de este archivo, preguntar antes de proceder, nunca asumir cual prevalece.
