SINTRA-RD

Sistema Integral de Transformación Tributaria de la República Dominicana

Documento de Investigación y Ingeniería de Requisitos — Fase I

Arquitectura de referencia inspirada en el modelo e-Estonia (e-MTA / X-Road),

regida por el Código Tributario dominicano (Ley 11-92) y la Ley 32-23 de Facturación Electrónica

Guillermo Estefán Puello

StarBound SRL · Metodología DEVGEP+

Santo Domingo, República Dominicana · Junio 2026


## Resumen

El presente documento constituye la Fase I de investigación e ingeniería de requisitos del proyecto SINTRA-RD, una propuesta de arquitectura para la transformación digital integral del sistema tributario de la República Dominicana. La investigación adopta el modelo de gobierno digital de Estonia (e-MTA y la capa de interoperabilidad X-Road) como referencia arquitectónica probada, manteniendo el principio rector de que la lógica de negocio del sistema debe estar gobernada en su totalidad por el marco legal dominicano vigente, en particular el Código Tributario (Ley 11-92) y la Ley General de Facturación Electrónica (Ley 32-23). Se documenta el estado actual de la Dirección General de Impuestos Internos (DGII), su plataforma de Oficina Virtual, y el Comprobante Fiscal Electrónico (e-CF) como componente crítico del ecosistema. Asimismo, se dimensiona la carga potencial del sistema a partir de datos demográficos oficiales del Censo 2022 y cifras de recaudación, con el fin de fundamentar las decisiones de escalabilidad arquitectónica. El documento sigue la metodología DEVGEP+ de desarrollo de productos digitales.

Palabras clave: administración tributaria digital, e-CF, modelo Clearance, X-Road, arquitectura API-first, DGII, escalabilidad.


## 1. Introducción y Planteamiento del Problema

La administración tributaria constituye uno de los pilares operativos del Estado moderno. Su eficiencia determina, en buena medida, la capacidad recaudatoria de un país y, por extensión, su capacidad de financiar el gasto público. En la República Dominicana, la Dirección General de Impuestos Internos (DGII) es la institución responsable de la administración y recaudación de los principales impuestos internos (DGII, 2024).

El problema central que motiva este proyecto es doble. En primer lugar, los sistemas de administración tributaria tienden a desarrollarse de forma fragmentada: cada figura impositiva, cada proceso y cada canal de atención evolucionan como módulos aislados, sin una capa de interoperabilidad común. En segundo lugar, los componentes tecnológicos heredados (legacy) limitan la capacidad del Estado de ofrecer servicios modernos y de exponer sus datos como interfaces de programación (API) reutilizables por terceros.

La oportunidad consiste en diseñar una arquitectura que separe de forma estricta dos planos: el plano técnico de integración (cómo se comunican los sistemas), que puede inspirarse en modelos internacionales probados, y el plano normativo de negocio (qué calcula y exige el sistema), que debe permanecer gobernado por la legislación tributaria local. Esta separación es la decisión arquitectónica fundamental del proyecto.


### 1.1. Objetivos

Objetivo general

Diseñar una arquitectura de referencia para la transformación digital integral del sistema tributario dominicano, modelando cada línea impositiva como un módulo gobernado por su ley correspondiente, e integrada mediante una capa central de servicios (API-first).

Objetivos específicos

Documentar el marco institucional y tecnológico actual de la DGII y su plataforma de servicios.

Analizar el Comprobante Fiscal Electrónico (e-CF) bajo la Ley 32-23 como componente crítico del ecosistema.

Dimensionar la carga potencial del sistema a partir de datos demográficos y de recaudación oficiales, para fundamentar la escalabilidad.

Establecer la separación arquitectónica entre la capa de integración (referencia: e-Estonia) y el motor de reglas tributarias (gobernado por la ley dominicana).


## 2. Marco de Referencia Internacional

La selección de un modelo de referencia se fundamentó en un análisis comparativo de la simplicidad y madurez digital de distintos sistemas tributarios. Estonia es reconocida internacionalmente como el sistema fiscal más simple y competitivo de la OCDE, caracterizado por tipos impositivos planos y un enfoque neutral (Tax Foundation, según Mapfre, 2023).

Más relevante que la simplicidad de su código es la madurez de su infraestructura digital. El sistema estonio e-Tax permite que un contribuyente complete su declaración en menos de tres minutos mediante formularios pre-llenados y firma digital (e-Estonia, 2016). Esta capacidad se sustenta en X-Road, una capa de intercambio de datos de código abierto, gestionada de forma descentralizada, que proporciona una manera estandarizada y segura de producir y consumir servicios entre sistemas de información (Digital Government Network, 2025).

El principio de diseño estonio “pregúntame solo una vez” (once-only) establece que el Estado no debe solicitar al ciudadano un dato que ya posee en algún registro (e-Estonia, 2016). En 2024, el sistema gestionó más de 2.7 mil millones de consultas de datos (Frost & Sullivan Institute, 2026). Este modelo constituye la referencia arquitectónica del presente proyecto, con la salvedad expresa de que la lógica tributaria de SINTRA-RD se rige por la legislación dominicana y no por la estonia.


## 3. Estado Actual: La DGII y su Plataforma


### 3.1. Marco institucional

La DGII es la institución encargada de la administración y recaudación de los principales impuestos internos y tasas de la República Dominicana (DGII, 2024). Es una entidad descentralizada y autónoma adscrita al Ministerio de Hacienda y Economía, órgano responsable de la política fiscal del Estado. Su configuración actual se formalizó mediante la Ley 166-97, que fusionó las antiguas direcciones de Impuesto sobre la Renta y de Rentas Internas, y la Ley 227-06, que le otorgó personalidad jurídica y autonomía funcional, presupuestaria, administrativa, técnica y patrimonial.

Entre sus atribuciones se encuentran la aplicación de las normas tributarias, la inscripción y actualización de contribuyentes en el Registro Nacional de Contribuyentes (RNC), la fiscalización del cumplimiento y la gestión de declaraciones. Estas atribuciones constituyen los macro-procesos que el sistema propuesto deberá soportar.


### 3.2. La Oficina Virtual y sus limitaciones

El principal canal digital actual es la Oficina Virtual (OFV), donde el contribuyente realiza solicitudes, declaraciones y consultas. El acceso se efectúa mediante RNC o cédula, clave y un elemento de seguridad (token digital). Un análisis técnico de la plataforma revela dos limitaciones estructurales relevantes para la justificación del proyecto.

La primera es de naturaleza tecnológica: el portal opera sobre tecnología ASP.NET Web Forms (evidenciado en sus rutas de acceso del tipo login.aspx), un marco de desarrollo de generación anterior que dificulta la evolución hacia arquitecturas modernas orientadas a servicios.

La segunda, y más significativa, es la ausencia de una capa de servicios pública y robusta. La propia comunidad de desarrolladores ha solicitado de forma reiterada un servicio web (API) para validar el RNC desde sistemas de planificación de recursos empresariales (ERP), recibiendo como respuesta oficial que la única vía disponible es la consulta en línea o la descarga de un listado (Comunidad de Ayuda DGII, 2024). Esta carencia valida la pertinencia de una propuesta API-first como la que plantea SINTRA-RD.


## 4. El Comprobante Fiscal Electrónico (e-CF): Componente Crítico

El Comprobante Fiscal Electrónico (e-CF) constituye, a juicio de esta investigación, la pieza más crítica y compleja del ecosistema tributario digital dominicano. Su correcta modelación determina la viabilidad de todo el sistema.


### 4.1. Marco legal y naturaleza

La Ley 32-23, promulgada el 16 de mayo de 2023, crea el Sistema Fiscal de Facturación Electrónica bajo administración de la DGII (SJ & Asociados, 2026). El e-CF es el equivalente digital de la antigua factura con Número de Comprobante Fiscal (NCF), con una diferencia fundamental: se genera en formato XML, se firma con un certificado digital y se transmite a la DGII en tiempo real. El formato debe ajustarse al estándar XML (UBL) definido por la DGII (The Factory HKA, 2026).


### 4.2. El modelo Clearance: validación previa

El e-CF opera bajo un modelo denominado Clearance, en el cual el comprobante debe ser enviado a la DGII para su validación antes de ser entregado al receptor (Alegra, 2026). Una vez superadas las validaciones de estructura, firma y asignación del número de comprobante electrónico (e-NCF), la DGII asigna un identificador de rastreo (TrackID) que certifica la transmisión exitosa (EDICOM, 2025). La Figura 4 ilustra este flujo.

Este modelo tiene una implicación arquitectónica de primer orden: el validador de e-CF se convierte en un componente de misión crítica con requisitos de alta disponibilidad y baja latencia. Si el validador central no está disponible, ningún contribuyente del país puede emitir facturas válidas. Esta es una decisión de diseño de escala nacional que exige tolerancia a fallos, redundancia y mecanismos de contingencia.


### 4.3. Tipos, conservación y sanciones

El sistema debe manejar múltiples tipos de e-CF, entre ellos el de Crédito Fiscal (Tipo 31) para operaciones que generan crédito fiscal y el de Consumo (Tipo 32) para ventas al consumidor final, además de comprobantes gubernamentales, de exportación y notas de crédito y débito (Alanube, 2025). Tanto emisores como receptores deben conservar los e-CF durante un período mínimo de diez años, garantizando integridad, seguridad y accesibilidad para auditorías (EDICOM, 2025).

El régimen sancionador es severo: multas de cinco a cincuenta salarios mínimos por incumplimiento, invalidez fiscal de las facturas en papel emitidas fuera de plazo, cierre temporal del establecimiento y sanciones penales de hasta cinco años para la emisión de comprobantes falsos (SJ & Asociados, 2026). El sistema, por tanto, no solo facilita el cumplimiento sino que materializa la consecuencia legal del incumplimiento.


## 5. Dimensionamiento para la Escalabilidad

El diseño de una plataforma de alcance nacional exige fundamentar sus decisiones de escalabilidad en datos reales de la población a servir. Esta sección establece el orden de magnitud de la carga que el sistema deberá soportar.


### 5.1. Base poblacional

Según el X Censo Nacional de Población y Vivienda 2022, la República Dominicana cuenta con 10,773,983 habitantes, de los cuales el 50.5 % son mujeres y el 49.5 % hombres (Oficina Nacional de Estadística [ONE], 2024). Esta cifra representa la base máxima teórica de usuarios potenciales del sistema, considerando que para las personas físicas el RNC coincide con el número de cédula. La Figura 1 muestra la evolución intercensal de la población.

La tasa de crecimiento poblacional medio anual del período 2010-2022 fue del 1.11 %, evidenciando una desaceleración respecto a períodos anteriores (ONE, 2024). Esto implica que el dimensionamiento puede asumir un crecimiento moderado y predecible de la base de usuarios, favoreciendo una planificación de capacidad estable.


### 5.2. Embudo de actores del ecosistema

No toda la población es contribuyente activo. La carga real del sistema se distribuye en un embudo de actores, desde la población total hasta el universo de emisores electrónicos obligados. La Figura 2 representa este dimensionamiento de orden de magnitud, que orienta la planificación de concurrencia y capacidad.

Resulta determinante para la arquitectura el hecho de que, conforme al calendario de la Ley 32-23, hacia 2026 la totalidad de los contribuyentes debe facturar electrónicamente, cuando a mediados de 2024 cerca del 70 % de las empresas aún no había adoptado la facturación electrónica (Programas Contabilidad, 2026). Esto anticipa una curva de adopción pronunciada y, por tanto, picos de carga concentrados en las fechas límite, que el sistema debe absorber sin degradación.


### 5.3. Magnitud del flujo financiero

La escala financiera del sistema refuerza la criticidad de su disponibilidad. La DGII reportó recaudaciones de RD$79,480.7 millones en octubre de 2025 y de RD$78,703.8 millones en mayo de 2026, superando de forma sostenida las metas presupuestarias (DGII, 2026). La Figura 3 ilustra esta magnitud mensual.

Un sistema que canaliza un flujo financiero de esta magnitud no admite indisponibilidad prolongada ni pérdida de integridad de datos. Esto confirma los requisitos no funcionales de alta disponibilidad, trazabilidad completa y auditabilidad que gobernarán el diseño arquitectónico en las fases siguientes.


## 6. Decisiones Arquitectónicas Preliminares (DEVGEP+)

De la investigación realizada emergen, conforme a la metodología DEVGEP+ de registro de decisiones de arquitectura (ADR), las siguientes definiciones preliminares que regirán las fases posteriores del proyecto:

Separación de planos. El sistema separará la capa de integración técnica (referencia e-Estonia / X-Road) del motor de reglas tributarias (gobernado por las leyes 11-92 y 32-23). La ley es configuración, no código rígido.

Arquitectura API-first. Toda funcionalidad se expondrá como servicio reutilizable, atacando directamente la carencia de APIs identificada en el sistema actual.

El e-CF como nodo crítico. El validador de comprobantes se diseñará como componente de alta disponibilidad y baja latencia, con redundancia y contingencia, dada su condición de punto único de dependencia nacional.

Modularidad por línea impositiva. Cada figura tributaria (ITBIS, ISR, ISC, entre otras) se modelará como un módulo independiente con su lógica gobernada por su base legal específica, a documentar en la fase del Código Tributario.

Escalabilidad fundamentada. El dimensionamiento parte de datos oficiales (Censo 2022, recaudación DGII) y contempla picos de carga asociados al calendario de obligatoriedad del e-CF.


## 7. Alcance de la Siguiente Fase

La presente Fase I establece el marco de referencia, el estado actual y el dimensionamiento del sistema. La siguiente fase abordará la documentación exhaustiva del Código Tributario dominicano (Ley 11-92 y sus modificaciones), desglosando cada figura impositiva (ITBIS, ISR, ISC y demás) en su lógica de cálculo, sujetos, exenciones y plazos, con el fin de convertir cada línea tributaria en un módulo lógico del sistema. Sobre esa base se construirá, en fases posteriores, la arquitectura lógica completa y la diagramación de extremo a extremo del proyecto SINTRA-RD.


## Referencias

Alanube. (2025). El e-CF en República Dominicana: guía técnica Ley 32-23. https://blog.alanube.co/rd/e-cf-en-republica-dominicana/

Alegra. (2026). Cambios en la facturación electrónica con la Ley 32-23. https://blog.alegra.com/republica-dominicana/cambios-facturacion-electronica/

Comunidad de Ayuda DGII. (2024). Web Services/EndPoint (API) para consultar RNC/Cédula. https://ayuda.dgii.gov.do/

Digital Government Network. (2025). E-Estonia X-Road: Middleware platform for a digital nation. https://digitalgov.network/estonia-xroad-platform/

Dirección General de Impuestos Internos. (2024). Acerca de la DGII. https://dgii.gov.do/

Dirección General de Impuestos Internos. (2026). Informes de recaudación mensual. https://dgii.gov.do/

e-Estonia. (2016). How Estonia is digitalizing an entire country. Harvard Business School. https://d3.harvard.edu/

EDICOM. (2025). Sistema de factura electrónica en República Dominicana. https://edicomgroup.com/es/factura-electronica/republica-dominicana

Frost & Sullivan Institute. (2026). Why Estonia is Europe’s digital powerhouse. https://frostandsullivaninstitute.org/

Mapfre. (2023). ¿Qué países tienen el mejor sistema tributario? https://planesdefuturo.mapfre.es/

Oficina Nacional de Estadística. (2024). Informe general del X Censo Nacional de Población y Vivienda 2022. https://www.one.gob.do/

Programas Contabilidad. (2026). Facturación electrónica en República Dominicana (Ley 32-23). https://programascontabilidad.com/

SJ & Asociados. (2026). Ley 32-23 de facturación electrónica en RD: qué significa para tu empresa. https://www.sj.com.do/

The Factory HKA Dominicana. (2026). Ley 32-23 y la obligatoriedad de factura electrónica. https://thefactoryhka.com.do/

