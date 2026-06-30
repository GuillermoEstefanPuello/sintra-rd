SINTRA-RD

Fase 3 — Arquitectura Lógica (Completa)

Catálogo de Módulos, Diseño de Sistema y Principios DEVGEP+

Documento técnico de arquitectura (gerencia + ingeniería)

Versión unificada: incorpora la Ley de Conway y los principios de IA

Guillermo Estefán Puello · StarBound SRL

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Este documento inicia la fase técnica del proyecto y unifica en una sola pieza la arquitectura lógica y los principios de diseño aplicados. La Sección 1 (Resumen Ejecutivo) y la Sección 2 (Principios de Diseño) son legibles por gerencia. Las secciones 3 a 9 contienen el detalle de ingeniería. Se trata de una propuesta de arquitectura lógica: describe cómo debería estructurarse el sistema, no un sistema ya construido.


## 1. Resumen Ejecutivo (para gerencia)

Las fases anteriores documentaron el problema, el modelo de referencia (Estonia), el estado actual (DGII) y las reglas de cada impuesto. Esta fase traduce todo ello en una arquitectura: un mapa de los componentes (módulos) que conforman el sistema, qué hace cada uno y cómo se relacionan. El diseño propone diecisiete módulos de alto nivel, organizados en cuatro planos funcionales.

| La decisión arquitectónica central El hallazgo más importante de la Fase 2 fue que todos los impuestos comparten la misma estructura (sujeto, base, tasa, exenciones, plazos, mora) y solo difieren en sus reglas. Por ello, el sistema NO se construye como un módulo por impuesto, sino como un único Motor de Liquidación Genérico (M3) que ejecuta las reglas que le entrega un Motor de Reglas configurable (M16). En términos de negocio: cuando cambie una ley tributaria, no se reescribe el sistema — se actualiza una configuración. Esto reduce drásticamente el costo y el riesgo de mantener el sistema vigente frente a reformas fiscales. |
| --- |

Los cuatro planos del sistema son: el plano de servicio (la cara al contribuyente), el plano transaccional (donde se registran y liquidan las operaciones), el plano de cumplimiento (fiscalización, cobranza y control) y el plano de integración (la plataforma transversal que conecta todo y se comunica con otras instituciones del Estado).

| Nota importante sobre el alcance de los 17 módulos Los diecisiete módulos representan la arquitectura lógica de alto nivel del sistema: los grandes bloques funcionales. No constituyen el inventario final de construcción. Cada módulo se descompone, en el diseño detallado, en sub-módulos, servicios y componentes (por ejemplo, el Motor de Liquidación incluye internamente el cálculo de base, exenciones, retenciones y mora). Un sistema tributario nacional, al construirse, comprende decenas de servicios y cientos de componentes. Los 17 son la vista de arquitectura, expandible en fases posteriores. |
| --- |


## 2. Principios de Diseño (Marco DEVGEP+ por Nivel)

El diseño se rige por el marco DEVGEP+ de principios por nivel de obligatoriedad. Un sistema tributario nacional es, por definición, un sistema crítico, transversal, legal y multi-institucional; por ello activa de forma obligatoria los principios de Nivel 1 y Nivel 2, y —por usar IA en su proceso de negocio— también el sub-bloque de IA del Nivel 3. Documentar esto explícitamente es lo que distingue una arquitectura profesional de un simple listado de funcionalidades.

Nivel 1 — Innegociables (siempre)

Aplican sin excepción a cada módulo: operaciones CRUD completas, transaccionalidad ACID (especialmente en pagos y liquidaciones), una única fuente de verdad por dato (Single Source of Truth, p. ej. el contribuyente vive solo en M1), idempotencia (un pago reintentado no se cobra dos veces), nunca confiar en el input del usuario (validación estricta), fail fast, separación de responsabilidades y diseño sin estado (statelessness) en los servicios.

Nivel 2 — Obligatorios por ser sistema crítico/legal/estatal

Aplican por la naturaleza nacional del sistema: soft delete en lugar de borrado físico (nada se elimina de verdad en un sistema fiscal); pista de auditoría inmutable (quién hizo qué y cuándo, sin posibilidad de alteración); principio de mínimo privilegio y defensa en profundidad (seguridad por capas); bajo acoplamiento y alta cohesión entre módulos; patrones Circuit Breaker y Bulkhead para aislar fallos; eliminación de puntos únicos de falla (SPOF), crítico para el validador de e-CF; estándares de interoperabilidad; estrategia de recuperación ante desastres (objetivos RTO/RPO definidos); los tres pilares de la observabilidad (logs, métricas y trazas); y la conformidad con los doce factores (Twelve-Factor App).

Nivel 3 — Importantes (se optimizan después)

Se atienden tras la base: DRY, KISS, YAGNI, manejo del problema N+1, estrategia de caché e invalidación, decisión consciente entre consistencia eventual y fuerte por caso de uso, degradación elegante (graceful degradation) y el principio de Postel. La Ley de Conway y los principios de IA, por su relevancia particular en este sistema, se desarrollan con mayor detalle en las secciones 8 y 9.

| Por qué este marco importa en la presentación Declarar explícitamente qué niveles activa un sistema tributario demuestra criterio de arquitecto: no se trata de saber muchos patrones, sino de saber cuáles son obligatorios para este dominio concreto y por qué. Un sistema fiscal sin auditoría inmutable, sin eliminación de SPOF y sin plan de recuperación ante desastres no es viable, independientemente de lo bien que calcule los impuestos. |
| --- |


## 3. Catálogo de Módulos

A continuación se describe cada módulo por su responsabilidad única (Single Responsibility), sus entidades núcleo y sus dependencias. La numeración es de referencia, no de orden de implementación.


### 3.1. Plano Transaccional (OLTP)

M1 · Registro Único de Contribuyentes   [OLTP]

Responsabilidad: Mantiene la identidad fiscal de todo contribuyente (persona física, jurídica, fideicomiso). Es la única fuente de verdad de quién es quién. Para personas físicas, el RNC coincide con la cédula.

Entidades núcleo: Contribuyente, TipoContribuyente, DomicilioFiscal, RepresentanteLegal, EstadoRNC.

Depende de / conecta con: Junta Central Electoral (cédula) y Migración (residencia fiscal) vía M13. Alimenta a casi todos los demás módulos.

M2 · Obligaciones y Calendario Fiscal   [OLTP]

Responsabilidad: Determina qué obligaciones tiene cada contribuyente y cuándo vencen, soportando calendarios fiscales múltiples (cierres en marzo, junio, septiembre, diciembre) y frecuencias heterogéneas (mensual, semanal).

Entidades núcleo: Obligacion, CalendarioFiscal, PeriodoFiscal, Vencimiento.

Depende de / conecta con: M1 (perfil del contribuyente), M16 (reglas de qué obligación aplica a quién).

M3 · Motor de Liquidación Genérico   [OLTP — núcleo]

Responsabilidad: Calcula el impuesto debido para cualquier figura tributaria (ITBIS, ISR, ISC, IPI, Activos) ejecutando las reglas que le entrega M16. Es el corazón del sistema y el principal activo de la arquitectura.

Entidades núcleo: Liquidacion, ResultadoCalculo, BaseImponible, DetalleLiquidacion.

Depende de / conecta con: M16 (reglas), M2 (período), M5/M4 (datos de entrada), M6 (genera el cobro).

M4 · Declaraciones Electrónicas   [OLTP]

Responsabilidad: Recibe, valida y almacena las declaraciones juradas (IR-1, IR-2, ISC-02, IST-01, DSS, IPI). Exige declaración aun cuando el impuesto sea cero.

Entidades núcleo: Declaracion, Formulario, AnexoDeclaracion, EstadoDeclaracion.

Depende de / conecta con: M3 (liquida lo declarado), M5 (precarga datos desde e-CF), M16 (validaciones de formulario).

M5 · e-CF / Facturación Electrónica   [OLTP — núcleo crítico]

Responsabilidad: Valida en tiempo real (modelo Clearance) los comprobantes fiscales electrónicos antes de su entrega: estructura XML/UBL, firma digital y asignación de e-NCF y TrackID. Es un componente de misión crítica con requisitos de alta disponibilidad.

Entidades núcleo: eCF, TrackID, e-NCF, FirmaDigital, TipoComprobante (31, 32, etc.).

Depende de / conecta con: M14 (firma/certificados), M1 (emisor/receptor), INDOTEL vía M13. Alimenta a M3 (ITBIS) y M4.

M6 · Recaudación y Pagos   [OLTP]

Responsabilidad: Gestiona el cobro: cuotas, conciliación bancaria, asignación de pagos a obligaciones. Debe ser idempotente para no duplicar cobros ante reintentos.

Entidades núcleo: Pago, Cuota, ConciliacionBancaria, MedioPago.

Depende de / conecta con: Banca y Tesorería vía M13; M7b (acredita a la cuenta del contribuyente).

M7 · Motor de Retenciones   [OLTP]

Responsabilidad: Aplica automáticamente las retenciones según el tipo de pagador, receptor y operación (30%/100% de ITBIS; definitivas/computables de ISR).

Entidades núcleo: Retencion, AgenteRetencion, TipoRetencion, NaturalezaRetencion.

Depende de / conecta con: M16 (reglas de retención), M3 (las retenciones afectan la liquidación), M7b.

M7b · Cuenta Corriente Tributaria   [OLTP]

Responsabilidad: Mantiene el estado de cuenta de cada contribuyente: débitos (impuestos liquidados), créditos (pagos, retenciones computables, saldos a favor) y saldo neto.

Entidades núcleo: CuentaCorriente, MovimientoCuenta, SaldoAFavor, CompensacionCredito.

Depende de / conecta con: M3, M6, M7; es consultada por M9 (cobranza) y M11 (portal).


### 3.2. Plano de Cumplimiento y Control

M8 · Fiscalización, Riesgo y Auditoría   [Cumplimiento]

Responsabilidad: Motor analítico (OLAP) que perfila contribuyentes, detecta patrones de fraude (p. ej. missing trader del IVA, cruces de e-CF) y prioriza casos de auditoría. Inspirado en el modelo HMRC Connect. Usa IA (ver Sección 9).

Entidades núcleo: PerfilRiesgo, CasoFiscalizacion, AlertaFraude, IndicadorRiesgo.

Depende de / conecta con: Consume datos de M3, M5, M7 (data warehouse); no escribe en el plano transaccional.

M9 · Cobranza y Gestión de Mora   [Cumplimiento]

Responsabilidad: Gestiona la deuda vencida: calcula recargos y mora (servicio compartido parametrizable por impuesto), administra acuerdos de pago y ejecuta el proceso sancionador.

Entidades núcleo: Deuda, CalculoMora, AcuerdoPago, Sancion.

Depende de / conecta con: M7b (saldos vencidos), M16 (régimen de mora por impuesto), M6 (pagos de acuerdos).

M10 · Trazabilidad Fiscal   [Cumplimiento]

Responsabilidad: Gestiona el control físico de mercancías gravadas con ISC (alcohol, tabaco): licencias oficiales, marcación fiscal (estampillas con códigos únicos) y verificación.

Entidades núcleo: LicenciaOficial, MarcacionFiscal, Estampilla, VerificacionCampo.

Depende de / conecta con: M1 (titular de licencia), Aduanas vía M13, M3 (ISC).


### 3.3. Plano de Servicio y Experiencia

M11 · Portal del Contribuyente / Autoservicio   [Servicio]

Responsabilidad: Cara digital al contribuyente. Implementa el principio 'pregúntame solo una vez' y las declaraciones pre-llenadas. Reemplaza la actual Oficina Virtual legacy.

Entidades núcleo: Sesion, SolicitudServicio, BorradorDeclaracion, Notificacion (vista).

Depende de / conecta con: Casi todos los módulos vía M13; M14 (autenticación).

M12 · Servicio al Cliente y Mesa de Ayuda   [Servicio]

Responsabilidad: Gestión de casos, tickets y asistencia al contribuyente, presencial y digital. Provee trazabilidad de la atención. Puede incorporar asistentes conversacionales (ver Sección 9).

Entidades núcleo: Ticket, Caso, Interaccion, BaseConocimiento.

Depende de / conecta con: M1 (contribuyente), M11 (canal), todos como fuentes de consulta.


### 3.4. Plano de Integración y Plataforma (transversal)

M13 · Capa de Interoperabilidad   [Plataforma]

Responsabilidad: Bus central de servicios (inspirado en X-Road) que estandariza y asegura el intercambio de datos entre módulos y con instituciones externas. Punto único de gobierno de las integraciones.

Entidades núcleo: ServicioRegistrado, ContratoInterfaz, LogIntercambio.

Depende de / conecta con: Conecta con Catastro, Aduanas, banca, Migración, JCE, Banco Central, INDOTEL.

M14 · Identidad, Seguridad y Firma Digital   [Plataforma]

Responsabilidad: Autenticación, autorización (RBAC con mínimo privilegio), gestión de certificados y validación de firma digital de los e-CF y declaraciones.

Entidades núcleo: Usuario, Rol, Permiso, Certificado, TokenSesion.

Depende de / conecta con: INDOTEL (certificados) vía M13; protege a todos los módulos.

M15 · Observabilidad y Auditoría Inmutable   [Plataforma]

Responsabilidad: Implementa los tres pilares (logs, métricas, trazas) y la pista de auditoría inmutable de todo evento fiscal relevante. Requisito Nivel 2.

Entidades núcleo: EventoAuditoria, Log, Metrica, Traza.

Depende de / conecta con: Recibe eventos de todos los módulos; no es modificable por ellos.

M16 · Motor de Reglas Tributarias   [Plataforma — núcleo]

Responsabilidad: Repositorio configurable y versionado de toda la lógica tributaria: tasas, bases, exenciones, retenciones, umbrales y montos específicos, vigentes por fecha. Es lo que convierte 'la ley en configuración'.

Entidades núcleo: ReglaTributaria, Tasa, Exencion, VigenciaTemporal, MontoEspecifico.

Depende de / conecta con: Es consumido por M3, M4, M7, M9; alimentado por las resoluciones de la DGII.

M17 · Notificaciones y Mensajería   [Plataforma]

Responsabilidad: Canal centralizado de comunicaciones al contribuyente (vencimientos, requerimientos, resoluciones), reutilizando el patrón de notificación genérica.

Entidades núcleo: Notificacion, Plantilla, CanalEnvio, AcuseRecibo.

Depende de / conecta con: Disparado por M2 (vencimientos), M8 (requerimientos), M9 (cobranza).


## 4. Integraciones Externas

El sistema no opera aislado: depende de datos de otras instituciones del Estado, intercambiados a través del bus de interoperabilidad (M13) bajo el principio 'pregúntame solo una vez'. La siguiente figura resume las integraciones críticas identificadas en las fases previas.

Cada integración corresponde a una dependencia real documentada: Catastro provee el valor del inmueble para el IPI (Fase 2D); Aduanas liquida ITBIS e ISC en importaciones (Fases 2A y 2C); la banca canaliza la recaudación; Migración y la Junta Central Electoral validan identidad y residencia fiscal (Fase 2B); el Banco Central provee los índices de inflación para los ajustes anuales; e INDOTEL acredita los certificados de firma digital del e-CF (Fase 1).


## 5. El Motor de Liquidación Genérico en Detalle

Por ser el activo central de la arquitectura, se detalla el funcionamiento del Motor de Liquidación (M3). Su principio es simple y potente: un solo motor procesa todos los impuestos, variando únicamente las reglas que carga del Motor de Reglas (M16).

El flujo es: recibe una entrada (una declaración de M4 o un e-CF de M5); identifica el impuesto y el período; carga del M16 las reglas vigentes a esa fecha (tasas, base, exenciones, retenciones, mora); ejecuta el cálculo; y entrega el resultado a Recaudación (M6). Como las reglas están versionadas por fecha, el sistema puede recalcular correctamente un período pasado con las reglas que estaban vigentes entonces — un requisito imprescindible para auditorías y rectificativas.

| Ventaja arquitectónica decisiva Si en 2027 una reforma cambia la tasa del ITBIS o agrega una exención, el cambio se hace en el Motor de Reglas (M16) como un nuevo registro con su fecha de vigencia. El Motor de Liquidación (M3) no se toca, no se recompila, no se redespliega. Las declaraciones de períodos anteriores siguen calculándose con las reglas viejas; las nuevas, con las nuevas. Esto es lo que hace al sistema sostenible en el tiempo frente a un entorno legal que cambia constantemente. |
| --- |


## 6. Requisitos No Funcionales (Nivel 2)

Derivados del marco DEVGEP+ Nivel 2 y de la escala nacional documentada en la Fase 1, el sistema debe satisfacer:

Alta disponibilidad del e-CF (M5): al ser el validador del que depende toda la facturación del país, requiere redundancia, eliminación de SPOF y un modo de contingencia que permita operar ante caídas, sin perder integridad.

Auditoría inmutable (M15): todo evento fiscal (liquidación, pago, modificación) debe quedar registrado de forma inalterable, con trazabilidad de actor y momento.

Recuperación ante desastres: objetivos explícitos de RTO (tiempo de recuperación) y RPO (pérdida de datos tolerable), acordes a un sistema que canaliza la recaudación nacional.

Escalabilidad para picos: capacidad de absorber los picos de carga concentrados en fechas límite (día 20 mensual, cierres fiscales), documentados en la Fase 1.

Seguridad por capas (M14): mínimo privilegio, defensa en profundidad y gestión rigurosa de certificados de firma digital.

Interoperabilidad estándar (M13): contratos de interfaz versionados y estables con cada institución externa.


## 7. Vista de Conjunto y Transición a Principios Avanzados

Las secciones anteriores definieron la estructura del sistema (módulos, planos, integraciones y su núcleo de liquidación) y los requisitos no funcionales que debe cumplir. Las dos secciones siguientes completan el marco DEVGEP+ aplicando dos principios de especial relevancia para este sistema: la Ley de Conway, que vincula la arquitectura con la organización de los equipos, y los principios de Inteligencia Artificial, que gobiernan el uso de IA en el módulo de fiscalización y riesgo.


## 8. Ley de Conway: Arquitectura y Organización de Equipos

La Ley de Conway enuncia que la arquitectura de un sistema tiende a reflejar la estructura de comunicación de la organización que lo construye. Su implicación de diseño es estratégica: la división del sistema en módulos debería alinearse con la forma en que se organizarán los equipos de desarrollo y, eventualmente, las áreas de la administración tributaria que los operarán.

Para SINTRA-RD esto significa que los cuatro planos y sus módulos no son solo una división técnica, sino una propuesta de cómo estructurar los equipos: un equipo por dominio cohesionado (por ejemplo, un equipo para el plano de cumplimiento, otro para el transaccional), con interfaces claras entre ellos que reflejen los contratos entre módulos. Ignorar este principio produce sistemas cuya arquitectura lucha contra la organización que los mantiene. Para la conducción del proyecto, la conformación de los equipos debe diseñarse en espejo con esta arquitectura modular.


## 9. Principios de IA Aplicados al Diseño

El sistema usa Inteligencia Artificial dentro de su proceso de negocio —principalmente en el módulo M8 (Fiscalización, Riesgo y Auditoría) y, potencialmente, en asistentes conversacionales del M12 (Mesa de Ayuda)—, lo que activa de forma obligatoria los seis principios de IA del marco DEVGEP+. La siguiente tabla mapea cada principio a su aplicación concreta.

| Principio de IA | Aplicación concreta en SINTRA-RD |
| --- | --- |
| Garbage In, Garbage Out (amplificado) | La calidad de la detección de fraude depende por completo de la calidad de los datos del data warehouse (M8). Datos sucios o incompletos de e-CF (M5) o declaraciones (M4) producen falsos positivos que se amplifican a escala nacional. Se exige validación y saneamiento de datos antes de alimentar cualquier modelo. |
| Human-in-the-loop | Regla innegociable: la IA de M8 prioriza, perfila y sugiere casos, pero NUNCA ejecuta una sanción, embargo o requerimiento de forma automática. Toda acción con efecto sobre el contribuyente requiere la decisión de un funcionario, que queda registrada en la auditoría inmutable (M15). |
| No-determinismo de los LLM | Ningún cálculo fiscal (liquidación, mora, retención) se delega a un modelo de lenguaje: esos cálculos son deterministas y viven en el Motor de Liquidación (M3) y el Motor de Reglas (M16). La IA se restringe a clasificación, priorización y asistencia, nunca al cómputo del impuesto debido. |
| Grounding | Cualquier salida de un componente de IA (perfil de riesgo, respuesta de un asistente) debe estar anclada en datos reales y verificables del sistema, con referencia a la fuente. Un asistente conversacional no inventa normativa: la recupera del Motor de Reglas (M16) y la cita. |
| Inyección de prompts (Prompt Injection) | Si el M12 incorpora asistentes conversacionales públicos, deben blindarse contra inyección de prompts: separación estricta entre instrucciones del sistema y entrada del usuario, y validación de que el contribuyente no pueda manipular el asistente para obtener datos de terceros. Conecta con el principio Nivel 1 de nunca confiar en el input. |
| Conciencia de costo (Cost Awareness) | La inferencia de modelos a escala de millones de contribuyentes tiene un costo operativo real. El diseño reserva la IA para donde aporta valor (fraude de alto impacto, asistencia masiva) y usa reglas deterministas y caché donde sean suficientes, evitando el sobreuso costoso de modelos. |


### 9.1. El componente de IA es asesor, no juez

La síntesis de estos principios produce una decisión de arquitectura clara y defendible: en SINTRA-RD, la inteligencia artificial ocupa el rol de asesor analítico, nunca el de autoridad decisoria. El módulo M8 produce señales (perfiles de riesgo, alertas, priorizaciones) que son insumos para la decisión humana, no sustitutos de ella. Esta frontera —entre lo que la IA sugiere y lo que un funcionario decide— se materializa mediante la auditoría inmutable (M15), que registra tanto la sugerencia del modelo como la decisión del funcionario.

Esta postura no es solo técnicamente correcta; es legalmente necesaria. Una administración tributaria que sancione con base en una decisión algorítmica no revisada se expone a impugnaciones por falta de motivación y debido proceso. El diseño, por tanto, protege simultáneamente la eficiencia (la IA enfoca los recursos de fiscalización donde más rinden) y la legitimidad (la decisión final es humana, motivada y auditable).


## 10. Cierre de la Fase 3 y Próximo Paso

Esta fase ha definido la arquitectura lógica del sistema: diecisiete módulos de alto nivel en cuatro planos, regidos por el marco DEVGEP+ por nivel aplicado en su totalidad (Nivel 1, Nivel 2 y Nivel 3, incluyendo la Ley de Conway y los principios de IA), con un motor de liquidación genérico como núcleo y un bus de interoperabilidad como columna vertebral. La arquitectura materializa, en componentes concretos, las decisiones tomadas desde la Fase 1 y validadas con la ley en la Fase 2.

La siguiente fase (Fase 4 — Diagramación completa) profundizará en los diagramas de detalle: el modelo de entidades núcleo, los flujos de proceso de negocio extremo a extremo (por ejemplo, el ciclo completo de una declaración de ITBIS desde el e-CF hasta el pago) y los diagramas de secuencia de las operaciones críticas. Allí podrá iniciarse también la descomposición interna de los módulos clave (M3 y M5) en sus sub-componentes.

