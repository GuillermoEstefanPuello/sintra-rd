# SINTRA-RD — Fase 4: Diagramacion (Modelo de Datos, Flujos y Descomposicion)

Nota: los diagramas visuales (ERD, flujo ITBIS, secuencia e-CF, descomposicion M3/M5)
estan en los documentos .docx originales. Este archivo conserva el texto explicativo
y las tablas de cada sub-componente, que es lo que el agente necesita para programar.

---

# 4A — Modelo de Datos y Flujos

SINTRA-RD

Fase 4A — Modelo de Datos y Flujos

Modelo de entidades núcleo · Flujo extremo a extremo · Secuencia del e-CF

Documento técnico de diagramación (gerencia + ingeniería)

Primer bloque de la Fase 4 — Diagramación completa

Guillermo Estefán Puello · StarBound SRL

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Este documento es el primer bloque de la Fase 4 (diagramación). La Sección 1 (Resumen Ejecutivo) es legible por gerencia. Las secciones 2 a 4 contienen los diagramas técnicos: el modelo de entidades, el flujo de proceso y el diagrama de secuencia. Cada diagrama se acompaña de una explicación en lenguaje accesible.


## 1. Resumen Ejecutivo (para gerencia)

Tras definir los módulos del sistema (Fase 3), esta fase los pone en movimiento mediante tres tipos de diagramas. El primero (modelo de entidades) muestra qué datos guarda el sistema y cómo se relacionan. El segundo (flujo de proceso) muestra el recorrido completo de una operación real —el ciclo mensual del ITBIS— desde que se emite una factura hasta que el impuesto queda saldado. El tercero (diagrama de secuencia) detalla, paso a paso, la operación más crítica del sistema: la validación de un comprobante electrónico en tiempo real.

| Qué demuestran estos diagramas Que el sistema no es una colección de módulos sueltos, sino un proceso coherente: un dato que nace en un comprobante electrónico (e-CF) viaja por el cálculo del impuesto, el pago y la cuenta del contribuyente, dejando en cada paso un rastro de auditoría inmutable. Esta trazabilidad de punta a punta es lo que da confianza a una administración tributaria. |
| --- |


## 2. Modelo de Entidades Núcleo

El modelo de entidades representa los datos centrales del sistema y sus relaciones. No incluye todas las entidades (serían cientos), sino el núcleo que sostiene el ciclo tributario principal. Cada caja es una entidad, con su clave primaria (PK) y sus claves foráneas (FK) que la vinculan a otras.

La entidad central es el Contribuyente (M1), única fuente de verdad de la identidad fiscal, de la que dependen sus obligaciones, declaraciones y comprobantes. Obsérvese cómo la Regla Tributaria (M16) gobierna la Liquidación (M3) sin formar parte de ella: las reglas son configuración externa, materializando en el modelo de datos la decisión arquitectónica central del proyecto. Nótese también que toda Liquidación y todo Pago generan un Evento de Auditoría inmutable (M15), representado con líneas punteadas.


## 3. Flujo Extremo a Extremo: el Ciclo del ITBIS

Para mostrar el sistema en funcionamiento, se traza el recorrido completo de la obligación más frecuente: la declaración mensual del ITBIS. Este flujo cruza casi todos los módulos del plano transaccional y ejemplifica cómo el e-CF alimenta el cálculo del impuesto.

El ciclo ilustra la mecánica de débito y crédito documentada en la Fase 2A: los e-CF de ventas registran el débito fiscal (ITBIS cobrado) y los de compras el crédito fiscal (ITBIS pagado); al vencimiento, el Motor de Liquidación calcula la diferencia menos las retenciones, aplicando las reglas vigentes; el resultado se cobra y se refleja en la cuenta corriente del contribuyente. Cada paso queda registrado en la auditoría inmutable.


## 4. Diagrama de Secuencia: Validación del e-CF (Clearance)

La operación más crítica del sistema —y su mayor reto de ingeniería— es la validación del comprobante fiscal electrónico bajo el modelo Clearance, donde la DGII debe autorizar el comprobante antes de que se entregue al receptor. El siguiente diagrama detalla la secuencia exacta de mensajes entre los componentes.

La secuencia evidencia por qué este componente exige alta disponibilidad: cada comprobante del país pasa por esta validación en tiempo real antes de tener validez fiscal. El emisor envía el XML; el módulo e-CF (M5) valida la firma con M14 y la estructura con las reglas de M16; solo si ambas pasan, asigna el e-NCF y el TrackID que certifican la autorización; recién entonces el comprobante puede entregarse. Si cualquier validación falla, el comprobante se rechaza y no adquiere validez. Esta es la razón por la que el M5 se diseñó, en la Fase 3, como componente de misión crítica con eliminación de puntos únicos de falla.


## 5. Cierre del Bloque 4A y Próximo Paso

Este bloque ha traducido la arquitectura de la Fase 3 en diagramas operativos: el modelo de datos núcleo, el flujo extremo a extremo del ITBIS y la secuencia crítica del e-CF. Juntos demuestran que el diseño no solo enumera módulos, sino que define cómo fluye la información y cómo se ejecutan las operaciones reales.

El siguiente bloque (Fase 4B) abordará la descomposición interna de los módulos clave —el Motor de Liquidación (M3) y el e-CF (M5)— en sus sub-componentes, mostrando el segundo nivel de detalle de la arquitectura. Con ello se cerrará la fase de diagramación y el proyecto quedará listo para la fase de riesgos y supuestos y la consolidación de la propuesta de presentación.


---

# 4B — Descomposicion de Modulos Clave (M3 y M5)

SINTRA-RD

Fase 4B — Descomposición de Módulos Clave

Motor de Liquidación (M3) y e-CF (M5) — segundo nivel de detalle

Documento técnico de diagramación (gerencia + ingeniería)

Segundo bloque de la Fase 4 — cierre de la diagramación

Guillermo Estefán Puello · StarBound SRL

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Este documento cierra la Fase 4 (diagramación). Abre los dos módulos más importantes del sistema y muestra su interior. La Sección 1 (Resumen Ejecutivo) es legible por gerencia; las secciones 2 y 3 contienen el detalle técnico de cada módulo, con su diagrama de descomposición y la tabla de sub-componentes.


## 1. Resumen Ejecutivo (para gerencia)

En la Fase 3 se definieron los módulos del sistema como 'cajas' de alto nivel. Este documento abre las dos cajas más críticas —el Motor de Liquidación (M3), que calcula todos los impuestos, y el e-CF (M5), que valida los comprobantes en tiempo real— y muestra las piezas internas de cada una. Esto demuestra que el diseño no se queda en generalidades: puede descender al nivel de detalle necesario para que un equipo de desarrollo lo construya.

| Por qué se eligieron estos dos módulos El Motor de Liquidación (M3) es el cerebro del sistema: si funciona mal, los impuestos se calculan mal. El e-CF (M5) es el componente de mayor criticidad operativa: si se cae, el país no puede facturar. Descomponer precisamente estos dos demuestra que se identificó dónde está el riesgo y el valor, y que se diseñó con el detalle que ese riesgo exige. |
| --- |


## 2. Descomposición del Motor de Liquidación (M3)

El Motor de Liquidación se descompone en una cadena de ocho sub-componentes, cada uno con una responsabilidad única (cumpliendo el principio Single Responsibility del Nivel 1). El procesamiento fluye de forma secuencial, pero cada paso es independiente y verificable.

La tabla siguiente detalla la función de cada sub-componente:

| Sub-componente | Función |
| --- | --- |
| 3.1 Receptor de entrada | Recibe la solicitud de liquidación, ya provenga de una declaración (M4) o de un e-CF (M5). Normaliza el formato de entrada. |
| 3.2 Identificador | Determina de qué impuesto y período se trata, para saber qué conjunto de reglas solicitar. |
| 3.3 Cargador de reglas | Solicita al Motor de Reglas (M16) las reglas vigentes a la fecha de la operación. Es el punto donde 'la ley entra' al cálculo. |
| 3.4 Calculador de base | Determina la base imponible según el tipo de operación (bien, servicio, importación, patrimonio). |
| 3.5 Aplicador de exenciones | Evalúa y aplica las exenciones que correspondan al contribuyente y a la operación. |
| 3.6 Calculador de retenciones | Aplica las retenciones pertinentes (coordinando con M7), distinguiendo su naturaleza. |
| 3.7 Calculador de mora | Si la liquidación es extemporánea, calcula recargos e intereses con la fórmula del impuesto correspondiente. |
| 3.8 Generador de liquidación | Consolida el resultado final, lo entrega a Recaudación (M6) y emite el evento a la Auditoría Inmutable (M15). |

| Ventaja de esta descomposición Como cada sub-componente tiene una sola responsabilidad, el sistema es testeable pieza por pieza y extensible: si en el futuro se agrega un nuevo impuesto con una particularidad de cálculo, se ajusta el sub-componente afectado sin reescribir todo el motor. Esto es separación de responsabilidades (Nivel 1) aplicada al detalle. |
| --- |


## 3. Descomposición del e-CF (M5)

El módulo e-CF se descompone en ocho sub-componentes que materializan el modelo Clearance documentado en la Fase 1. A diferencia del M3, su flujo incluye puntos de validación que pueden rechazar el comprobante, y se apoya en módulos externos para la firma y las reglas.

La tabla siguiente detalla cada sub-componente:

| Sub-componente | Función |
| --- | --- |
| 5.1 Receptor de e-CF | Expone la API que recibe el comprobante en formato XML/UBL desde el emisor (ERP propio, PSFE o facturador gratuito). |
| 5.2 Validador de estructura | Verifica que el XML cumpla el esquema UBL definido por la DGII. Si falla, rechaza. |
| 5.3 Validador de firma | Verifica la firma digital con el módulo de certificados (M14). Si la firma es inválida, rechaza. |
| 5.4 Validador de reglas | Comprueba reglas de negocio (tipo de comprobante, montos, RNC válidos) consultando M16. Si falla, rechaza. |
| 5.5 Asignador de TrackID | Si todas las validaciones pasan, asigna el e-NCF y el TrackID que otorgan validez fiscal al comprobante. |
| 5.6 Generador de representación | Produce la representación impresa fiel del e-CF para receptores no electrónicos. |
| 5.7 Archivo y conservación | Almacena el e-CF garantizando su integridad y disponibilidad por el mínimo legal de diez años. |
| 5.8 Notificador de estado | Informa al emisor el resultado (aceptado con TrackID o rechazado con motivo) y deja el comprobante disponible para entrega al receptor. |

| El punto crítico de disponibilidad Los sub-componentes 5.2, 5.3 y 5.4 son puertas de validación: cualquiera puede rechazar el comprobante. El 5.5 (asignación de TrackID) es el instante exacto en que un comprobante adquiere validez fiscal. Por eso toda la cadena debe estar disponible en tiempo real para que el país facture; un fallo aquí detiene la actividad económica formal. Esta es la justificación concreta de los requisitos de redundancia y eliminación de SPOF definidos en la Fase 3. |
| --- |


## 4. Cierre de la Fase 4

Con la descomposición de los dos módulos clave se cierra la fase de diagramación. El proyecto cuenta ahora con: el modelo de entidades núcleo, el flujo extremo a extremo del ITBIS, el diagrama de secuencia del e-CF (Bloque 4A) y la descomposición interna del Motor de Liquidación y del e-CF (este Bloque 4B). En conjunto, el diseño desciende coherentemente desde la visión de planos (Fase 3) hasta el detalle de sub-componentes, demostrando capacidad de diseño en múltiples niveles de abstracción.

El siguiente paso, conforme al plan, es la Fase de Riesgos y Supuestos, que documentará qué puede fallar, qué se asume y de qué terceros depende el proyecto, antes de consolidar la propuesta final de presentación. Esa fase es la que dota al proyecto de la madurez necesaria para ser evaluado con seriedad.

