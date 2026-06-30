# SINTRA-RD — Fase 2: Código Tributario Dominicano

Documento de dominio: ITBIS, ISR, ISC, Impuesto sobre Activos e IPI.
Cada seccion corresponde a un sub-bloque de la Fase 2 (2A a 2D).

---

# 2A — ITBIS

SINTRA-RD

Fase 2A — Análisis del ITBIS

Impuesto sobre Transferencias de Bienes Industrializados y Servicios

Documentación de dominio tributario para modelado del módulo de liquidación

Base legal: Código Tributario (Ley 11-92), Título III, Arts. 335–360

Reglamento de aplicación: Decreto 293-11

Guillermo Estefán Puello · StarBound SRL · Metodología DEVGEP+

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Este documento está estructurado para dos audiencias. La Sección 1 (Resumen Ejecutivo) está dirigida a la gerencia y al equipo de negocio: explica qué es el ITBIS, por qué es relevante para el sistema y qué implica su modelación, sin requerir conocimientos técnicos. Las secciones 2 a 6 contienen el detalle de dominio tributario que el equipo técnico necesitará para diseñar el módulo correspondiente. Un lector de negocio puede detenerse al final de la Sección 1; un lector técnico debe continuar.


## 1. Resumen Ejecutivo (para gerencia)

El ITBIS es el impuesto al consumo más importante de la República Dominicana, equivalente al IVA de otros países. Es un impuesto indirecto que grava la transferencia e importación de bienes industrializados y la prestación de servicios. Para el sistema SINTRA-RD, el ITBIS representa uno de los módulos de mayor volumen transaccional, ya que afecta a prácticamente toda la actividad económica formal del país y se declara de forma mensual.

| Puntos clave para la toma de decisiones Alcance: afecta a casi todos los contribuyentes con actividad comercial o de servicios. Frecuencia: declaración y pago mensual (antes del día 20 de cada mes), lo que genera picos de carga predecibles y recurrentes. Complejidad de reglas: no es una tasa única; hay tasa general (18%), tasa reducida (16%), tasa cero (exportaciones) y exenciones. El sistema debe modelar esto como reglas configurables, no como un valor fijo. Mecánica de crédito: el impuesto se calcula como débito menos crédito fiscal, lo que exige que el sistema cruce las ventas y las compras de cada contribuyente. Retenciones: existen múltiples regímenes de retención (30% y 100%) según el tipo de operación, que el sistema debe aplicar automáticamente. |
| --- |

La conclusión de negocio es que el ITBIS no puede modelarse como un cálculo simple. Su correcta implementación requiere un motor de reglas que distinga tipos de bienes y servicios, aplique la tasa correcta, gestione el crédito fiscal y ejecute las retenciones aplicables. Este es precisamente el tipo de lógica que justifica la separación entre la plataforma técnica y el motor de reglas tributarias planteada en la Fase 1.


## 2. Marco Legal y Naturaleza del Impuesto

El ITBIS se establece en el Título III del Código Tributario (Ley 11-92), que comprende desde el artículo 335 hasta el artículo 360, organizado en seis capítulos (Anchi Advisors, 2026). Su reglamento de aplicación vigente es el Decreto 293-11, donde se detallan los procedimientos de cálculo, los bienes y servicios exentos y los mecanismos de declaración (Alegra, 2025).

Es un impuesto general al consumo de tipo valor agregado que grava tres hechos generadores: la transferencia de bienes industrializados, la importación de bienes industrializados y la prestación o locación de servicios (DGII, 2024). Son contribuyentes las personas físicas (profesionales liberales y negocios de único dueño) y las personas jurídicas, nacionales o extranjeras, que realicen operaciones gravadas, así como las empresas obligadas a actuar como agentes de retención.


## 3. Tasas y Base Imponible


### 3.1. Estructura de tasas

A diferencia de un impuesto de tasa única, el ITBIS opera con una estructura de tasas diferenciadas que el sistema debe modelar explícitamente:

| Tasa | Valor | Aplicación |
| --- | --- | --- |
| General | 18 % | Mayoría de bienes manufacturados y servicios gravados |
| Reducida | 16 % | Productos específicos: yogurt, mantequilla, café, cacao, chocolate, aceites, azúcar |
| Tasa cero | 0 % | Bienes exportados (con derecho a reembolso del crédito fiscal) |
| Exento | — | Bienes y servicios esenciales (no genera obligación) |

Nota. La tasa general del 18% rige desde 2016 conforme al Art. 23 de la Ley 253-12 (DGII, 2024). Es importante distinguir entre operación exenta (no se cobra impuesto) y tasa cero (se aplica 0% pero se conserva el derecho a crédito fiscal), pues tienen consecuencias contables distintas.


### 3.2. Base imponible

La base imponible es el monto sobre el cual se aplica la tasa, y varía según el tipo de operación, conforme al artículo 339 de la Ley 11-92 y el artículo 9 del Decreto 293-11 (Alegra, 2025):

Transferencia de bienes: precio neto de venta más elementos accesorios obligatorios (transporte, embalaje, intereses), menos descuentos y bonificaciones.

Importaciones: valor CIF más aranceles y demás tributos a la importación.

Servicios: valor total del servicio, excluyendo la propina obligatoria.

La fórmula para desglosar el impuesto de un precio que ya lo incluye es: base imponible = precio final ÷ (1 + tasa). Por ejemplo, un precio de RD$1,180 con ITBIS del 18% tiene una base imponible de RD$1,000 y un ITBIS de RD$180 (Alegra, 2025). Esta fórmula es esencial para el módulo de facturación.


## 4. Exenciones y Operaciones No Sujetas

El régimen de exenciones es uno de los componentes más extensos del ITBIS y, para el sistema, el de mayor complejidad de configuración, pues exige clasificar cada bien o servicio. Las exenciones de bienes se establecen en el artículo 343 y las de servicios en el artículo 344 de la Ley 11-92 (Pellerano & Herrera, 2025).

Principales bienes exentos (Art. 343)

Alimentos básicos (carnes frescas, pescados de consumo popular, leche, miel, huevos, legumbres, hortalizas, tubérculos, frutas sin procesar, cereales, harinas, pan, pastas); productos agropecuarios (semillas, abonos, insecticidas, insumos pecuarios); bienes de primera necesidad (combustibles, medicamentos, sillas de ruedas); material educativo (libros, revistas, material preuniversitario); y materias primas como cacao en grano y café sin tostar.

Principales servicios exentos (Art. 344)

Servicios de educación en todos los niveles; servicios de salud (incluidos servicios odontológicos y gimnasios, considerados salud preventiva); servicios financieros y de seguros; transporte terrestre de pasajeros y carga; servicios públicos básicos (energía eléctrica, agua potable, recogida de basura); alquiler de viviendas familiares; servicios funerarios; salones de belleza y peluquerías; y espectáculos artísticos (DGII, 2024; Pellerano & Herrera, 2025).

Operaciones no sujetas

Distintas de las exenciones, no generan el hecho imponible: transferencias de bienes inmuebles, transferencia de dinero, títulos, valores y acciones, y el arrendamiento de derechos o bienes intangibles (DGII, 2024).

Un tratamiento especial reciben las Zonas Francas (Ley 8-90): los bienes transferidos desde el territorio nacional a una Zona Franca Industrial se tratan como exportados, y los transferidos desde la Zona Franca al territorio se tratan como importados (OEA/MESICIC, Título III). El sistema debe modelar este comportamiento de frontera fiscal interna.


## 5. Determinación del Impuesto y Retenciones


### 5.1. Mecánica de débito y crédito fiscal

El ITBIS a pagar se determina restando del impuesto cobrado en las ventas (débito fiscal o ITBIS por pagar) el impuesto pagado en las compras (crédito fiscal o ITBIS adelantado), conforme al artículo 346 de la Ley 11-92 (Anchi Advisors, 2026). El sistema debe, por tanto, mantener para cada contribuyente el registro cruzado de su ITBIS en ventas y su ITBIS en compras del período.

Para que el crédito fiscal sea deducible deben cumplirse condiciones que el sistema debe validar: que quien deduce sea contribuyente del ITBIS, que el ITBIS esté separado del precio en el comprobante, y que ese ITBIS no se haya considerado como costo o gasto para fines del Impuesto sobre la Renta (DGII, 2024).


### 5.2. Regímenes de retención

El sistema debe aplicar automáticamente distintos porcentajes de retención según el tipo de operación y de sujetos involucrados. Los principales regímenes son:

| Operación | Retención |
| --- | --- |
| Persona jurídica paga servicios profesionales liberales a persona física | 30 % del ITBIS |
| Persona jurídica paga alquiler de bienes muebles | 30 % del ITBIS |
| Persona jurídica paga servicios gravados a persona física (Reg. 293-11) | 100 % del ITBIS |
| Sociedades pagan publicidad/servicios a entidades sin fines de lucro | 100 % del ITBIS |
| Líneas aéreas y hoteles pagan comisiones a agencias de viajes | 100 % del ITBIS |
| Administradoras de tarjetas / entidades de pago (ITBIS desglosado) | 30 % del ITBIS |

Fuente: Pellerano & Herrera (2025); DGII (2024), con base en la Norma General 02-05 y modificaciones, y el Reglamento 293-11.


## 6. Plazos, Pago y Sanciones

La declaración y el pago del ITBIS deben realizarse dentro de los primeros 20 días del mes siguiente al período declarado; por ejemplo, el período de enero debe presentarse y pagarse antes del 20 de febrero. En las importaciones, el ITBIS se paga junto con los aranceles aduaneros (DGII, 2024).

El régimen de mora que el sistema debe calcular automáticamente es el siguiente: un recargo del 10% sobre el valor del impuesto por el primer mes o fracción de retraso, un 4% progresivo por cada mes o fracción subsiguiente, y adicionalmente un 1.10% acumulativo de interés indemnizatorio por cada mes o fracción (DGII, 2024). Esta lógica de cálculo de mora es un requisito funcional preciso para el módulo.


## 7. Implicaciones para el Modelado del Sistema (para el equipo técnico)

De este análisis de dominio se derivan los siguientes requisitos que regirán el diseño del módulo de ITBIS en las fases de arquitectura:

Catálogo de clasificación de bienes y servicios: el sistema necesita una entidad maestra que asocie cada bien o servicio a su tratamiento (gravado 18%, reducido 16%, tasa cero, exento), configurable y versionada por base legal.

Motor de cálculo de base imponible: con lógica diferenciada por tipo de operación (bien, servicio, importación), incluyendo accesorios y descuentos.

Libro de ventas y compras por contribuyente: para el cruce débito-crédito, alimentado idealmente por los e-CF de la Fase 1, lo que conecta este módulo con el de facturación electrónica.

Motor de retenciones: que evalúe el tipo de pagador, el tipo de receptor y el tipo de servicio para aplicar el porcentaje correcto (30% o 100%).

Calculadora de mora e intereses: parametrizable, dada la estructura escalonada de recargos.

Tratamiento de frontera fiscal de Zonas Francas: lógica de exportación/importación ficticia para operaciones con el régimen de Zona Franca.

La conexión más relevante es que el libro de ventas del ITBIS se alimenta naturalmente de los e-CF documentados en la Fase 1. Esto confirma que el e-CF no es un módulo aislado, sino la fuente de datos primaria del cálculo del ITBIS, reforzando su carácter de componente central del sistema.


## Referencias

Alegra. (2025). ¿Cuál es la base imponible del ITBIS? https://blog.alegra.com/republica-dominicana/cual-es-la-base-imponible-del-itbis/

Anchi Advisors. (2026). Impuesto a la Transferencia de Bienes Industrializados y Servicios (ITBIS). https://www.anchiadvisors.com/

Dirección General de Impuestos Internos. (2024). ITBIS. https://dgii.gov.do/cicloContribuyente/obligacionesTributarias/principalesImpuestos/Paginas/Itbis.aspx

Organización de los Estados Americanos, MESICIC. (s.f.). Código Tributario, Título III, Del Impuesto sobre Transferencias de Bienes Industrializados y Servicios. http://www.oas.org/

Pellerano & Herrera. (2025). El ITBIS en la República Dominicana: guía completa, y Exenciones y reducciones de ITBIS. https://phlaw.com/


---

# 2B — ISR

SINTRA-RD

Fase 2B — Análisis del ISR

Impuesto Sobre la Renta

Documentación de dominio tributario para modelado del módulo de renta

Base legal: Código Tributario (Ley 11-92), Título II, Arts. 267–329

Modificado por la Ley 253-12 · Tasas vigentes 2026

Guillermo Estefán Puello · StarBound SRL · Metodología DEVGEP+

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Como en los documentos previos de esta fase, el contenido está organizado para dos audiencias. La Sección 1 (Resumen Ejecutivo) está dirigida a gerencia y negocio. Las secciones 2 a 7 contienen el detalle de dominio tributario para el equipo técnico. Un lector de negocio puede detenerse al final de la Sección 1.


## 1. Resumen Ejecutivo (para gerencia)

El Impuesto Sobre la Renta (ISR) grava los ingresos, utilidades y beneficios obtenidos por personas físicas, jurídicas y sucesiones indivisas. Es un impuesto directo de periodicidad anual y constituye, junto al ITBIS, una de las dos columnas de la recaudación dominicana. A diferencia del ITBIS (mensual y de tasa relativamente uniforme), el ISR es más complejo de modelar porque trata de forma distinta a cada tipo de contribuyente y a cada tipo de renta.

| Puntos clave para la toma de decisiones Dos lógicas distintas en un mismo impuesto: las personas físicas pagan con una escala progresiva (a más ingreso, mayor tasa marginal); las empresas pagan una tasa fija del 27%. El sistema debe modelar ambas. Principio territorial: se gravan las rentas de fuente dominicana; las de fuente extranjera solo tributan para residentes y en casos específicos (inversiones y ganancias financieras). Anticipos: las empresas no pagan solo al final del año; adelantan el impuesto en cuotas durante el ejercicio. El sistema debe calcular y dar seguimiento a estos anticipos. Retenciones de dos naturalezas: algunas son definitivas (cierran la obligación) y otras computables (se acreditan a la declaración anual). Confundirlas causa errores detectables en auditoría. Impuesto mínimo: las empresas pagan el mayor entre su ISR y el 1% de sus activos, lo que vincula este módulo con el de Impuesto sobre Activos (Fase 2D). |
| --- |

La conclusión de negocio es que el ISR es el módulo de mayor complejidad de reglas de toda la Fase 2. Requiere un motor capaz de aplicar escalas progresivas, gestionar anticipos, distinguir tipos de retención y calcular un impuesto mínimo alternativo. Su correcta implementación es donde el conocimiento contable del equipo aporta el mayor valor diferencial.


## 2. Marco Legal, Sujetos y Principio Territorial

El ISR se regula en el Título II del Código Tributario (Ley 11-92), modificado de forma relevante por la Ley 253-12 de 2012 (Siempre al Día, 2026). Conforme al artículo 267 y siguientes, toda persona natural o jurídica residente o domiciliada en el país, y las sucesiones indivisas de causantes domiciliados, pagan el impuesto sobre sus rentas de fuente dominicana, y sobre las de fuente extranjera únicamente cuando provengan de inversiones y ganancias financieras (DGII, 2024; OEA/DGII, Título II).

El régimen es territorial: todos los ingresos de fuente dominicana tributan localmente, sin importar la nacionalidad de quien los genere; los ingresos generados en el extranjero, en cambio, no están sujetos a impuesto dominicano salvo la excepción señalada (Guzmán Ariza, 2025). Los contribuyentes se clasifican en tres categorías que el sistema debe distinguir: personas físicas (asalariados, independientes, negocios de único dueño), personas jurídicas (sociedades comerciales) y no residentes.


## 3. ISR de Personas Físicas: Escala Progresiva

Las personas físicas residentes pagan según una escala progresiva por tramos, establecida en el artículo 296 de la Ley 11-92 y ajustada anualmente por la inflación publicada por el Banco Central (DGII, 2024). Las cifras vigentes para 2025-2026 son las siguientes:

Es esencial comprender, para el modelado, que la progresividad opera por tramos y no de forma global: cada porción del ingreso paga la tasa de su tramo, de modo que superar un umbral no penaliza la totalidad del ingreso. El monto de RD$416,220.00 anuales (unos RD$34,685 mensuales) se denomina exención contributiva y representa el ingreso máximo libre de ISR; este umbral no ha variado desde 2018 (Alegra, 2026; CECAFI, 2025).

Exenciones y deducciones relevantes para personas físicas

Están exentas de declarar las personas cuyos ingresos provengan enteramente de su trabajo asalariado y aquellas con ingresos inferiores al umbral exento (Guzmán Ariza, 2025). El sistema debe contemplar además: la exención de la Regalía Pascual (doble sueldo) hasta la duodécima parte de los salarios del año; la deducción de gastos educativos (Ley 179-09) sujeta a topes; y la no sujeción de los aportes a la seguridad social (CECAFI, 2025).


## 4. ISR de Personas Jurídicas: Tasa Fija e Impuesto Mínimo

Las personas jurídicas domiciliadas pagan una tasa única del 27% sobre su renta neta imponible, conforme al artículo 297 modificado por la Ley 253-12, vigente desde el ejercicio 2015 (Siempre al Día, 2026). A diferencia de otros países, la tasa es la misma para todos los tipos de sociedad (Guzmán Ariza, 2025).

El año fiscal por defecto va del 1 de enero al 31 de diciembre (Art. 300), pero la sociedad puede optar por cierres alternativos al 31 de marzo, 30 de junio o 30 de septiembre, y una vez elegida la fecha solo puede modificarla con autorización de la DGII (Siempre al Día, 2026). El sistema debe, por tanto, soportar múltiples calendarios fiscales simultáneos, no un único cierre nacional.

Impuesto mínimo y compensación de pérdidas

Las personas jurídicas pagan el mayor valor entre su ISR liquidado y el 1% del total de sus activos imponibles; el Impuesto sobre Activos opera así como un pago mínimo del ISR (CECAFI, 2025). Esto enlaza directamente este módulo con la Fase 2D. Adicionalmente, si una sociedad registra pérdidas, puede compensarlas en los cinco ejercicios siguientes, con límites en los años cuarto y quinto (Ministerio de Hacienda, 2019). El sistema debe arrastrar y aplicar estas pérdidas acumuladas.


## 5. Retenciones del ISR: Definitivas vs. Computables

Esta es la distinción más crítica del ISR para el modelado, y la fuente más común de error en la práctica. Las retenciones se dividen en dos naturalezas que el sistema debe tratar de forma diferente (Alegra, 2026):

Retenciones definitivas: cierran la obligación fiscal; el monto retenido no se declara ni se compensa después. Ejemplos: dividendos (10%), intereses (10%), pagos al exterior (27%).

Retenciones computables: se acreditan contra la declaración anual del contribuyente como un adelanto. Ejemplos: el 5% en honorarios a empresas locales y las retenciones mensuales de asalariados.

Los principales regímenes de retención que el sistema debe aplicar automáticamente son:

| Concepto | Tasa | Naturaleza |
| --- | --- | --- |
| Dividendos a residentes o del exterior (Art. 308) | 10 % | Definitiva |
| Intereses | 10 % | Definitiva |
| Pagos al exterior a no residentes | 27 % | Definitiva |
| Pagos al exterior — residentes de Canadá | 18 % | Definitiva |
| Préstamos del exterior / pagos sucursal a casa matriz | 10 % | Definitiva |
| Alquileres pagados a personas físicas | 10 % | Computable |
| Honorarios y comisiones a empresas locales | 5 % | Computable |
| Retención mensual de asalariados (escala progresiva) | Escala | Computable |

Fuente: Guzmán Ariza (2025); Alegra (2026); DGII (2024). Base legal principal: Arts. 287, 289–295, 296, 297, 305, 305-A, 308, 309 de la Ley 11-92.


## 6. Ganancias de Capital y Anticipos

La ganancia de capital se determina deduciendo del valor de enajenación el costo de adquisición o producción ajustado por inflación; en bienes depreciables se considera el valor residual (Guzmán Ariza, 2025). La tasa es la misma del ISR: 27% para personas jurídicas y la escala progresiva para personas físicas. El sistema debe calcular este ajuste por inflación, lo que requiere mantener series históricas del índice de precios.

El ISR no se paga únicamente en la liquidación anual: se adelanta mediante anticipos durante el ejercicio, que luego se acreditan a la declaración final (CECAFI, 2025). El módulo debe, por tanto, gestionar un ciclo de vida del impuesto que abarca anticipos periódicos, retenciones computables acumuladas y liquidación final, calculando el saldo a pagar o a favor.


## 7. Plazos, Formularios y Mora

Los formularios y plazos que el sistema debe gestionar son: el IR-1 para personas físicas, que vence el 31 de marzo del año siguiente; y el IR-2 para personas jurídicas, cuyo vencimiento depende de la fecha de cierre fiscal (el cierre al 31 de diciembre vence el 30 de abril) (DGII, 2024; Siempre al Día, 2026).

El régimen de mora es análogo al del ITBIS y el sistema debe calcularlo automáticamente: 10% de recargo el primer mes o fracción, 4% progresivo por cada mes o fracción subsiguiente, más un interés indemnizatorio del 1.10% acumulativo por mes o fracción sobre el monto dejado de pagar (DGII, 2024; Siempre al Día, 2026). La consistencia de esta lógica de mora entre impuestos sugiere, para la arquitectura, un servicio de cálculo de mora compartido y reutilizable.


## 8. Implicaciones para el Modelado del Sistema (para el equipo técnico)

Del análisis del ISR se derivan los siguientes requisitos para el diseño del módulo:

Motor de escala progresiva: cálculo por tramos para personas físicas, con la escala parametrizada y versionada por año (ajuste anual por inflación).

Soporte de calendarios fiscales múltiples: el sistema no puede asumir un cierre único; debe manejar cierres por contribuyente (marzo, junio, septiembre, diciembre).

Motor de impuesto mínimo: comparar ISR liquidado contra el 1% de activos y aplicar el mayor, lo que exige integración con el módulo de Impuesto sobre Activos (Fase 2D).

Distinción definitiva/computable en retenciones: cada retención debe etiquetarse por naturaleza, pues determina si cierra la obligación o se acredita a la declaración anual.

Arrastre de pérdidas: acumulación y aplicación de pérdidas compensables hasta cinco ejercicios, con los límites de los años cuarto y quinto.

Cálculo de ajuste por inflación: para ganancias de capital y para la propia escala, requiriendo series históricas del índice de precios del Banco Central.

Ciclo de anticipos y liquidación: gestión del ciclo completo (anticipos, retenciones computables, liquidación final, saldo a favor o a pagar).

La conexión cruzada más relevante de este módulo es doble: por un lado, las retenciones computables y los gastos deducibles se sustentan en los e-CF de la Fase 1 (los gastos solo se deducen con e-CF válidos); por otro, el impuesto mínimo lo enlaza con el Impuesto sobre Activos de la Fase 2D. El ISR es, así, el módulo más interconectado del sistema.


## Referencias

Alegra. (2026). Impuesto sobre la Renta (ISR) en República Dominicana 2026. https://blog.alegra.com/republica-dominicana/impuesto-sobre-la-renta-isr/

Centro de Capacitación Financiera CECAFI. (2025). Impuesto sobre la renta y sus retenciones en la RD. https://cecafi1.blogspot.com/

Dirección General de Impuestos Internos. (2024). Impuesto sobre la Renta. https://dgii.gov.do/cicloContribuyente/obligacionesTributarias/principalesImpuestos/Paginas/impuestoSobreRenta.aspx

Guzmán Ariza. (2025). Derecho fiscal. https://drlawyer.com/espanol/derecho-fiscal/

Ministerio de Hacienda. (2019). Estimación del gasto tributario. https://www.hacienda.gob.do/

Organización de los Estados Americanos / DGII. (s.f.). Código Tributario, Título II, Impuesto sobre la Renta. https://dgii.gov.do/legislacion/

Siempre al Día. (2026). Claves del Impuesto sobre la Renta para personas jurídicas. https://siemprealdia.co/republica-dominicana/impuestos/


---

# 2C — ISC

SINTRA-RD

Fase 2C — Análisis del ISC

Impuesto Selectivo al Consumo

Documentación de dominio tributario para modelado del módulo selectivo

Base legal: Código Tributario (Ley 11-92), Título IV, Arts. 361–400

Reglamento de aplicación: Decreto 01-18

Guillermo Estefán Puello · StarBound SRL · Metodología DEVGEP+

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Como en los documentos previos de esta fase, el contenido está organizado para dos audiencias. La Sección 1 (Resumen Ejecutivo) está dirigida a gerencia y negocio. Las secciones 2 a 7 contienen el detalle de dominio tributario para el equipo técnico. Un lector de negocio puede detenerse al final de la Sección 1.


## 1. Resumen Ejecutivo (para gerencia)

El Impuesto Selectivo al Consumo (ISC) grava bienes y servicios específicos que el Estado busca desincentivar (como alcohol y tabaco), considera suntuarios, o sobre los que ejerce un control particular (telecomunicaciones, seguros, operaciones financieras, hidrocarburos). Aunque menos universal que el ITBIS, en ciertos productos su carga puede superar a la del propio ITBIS, lo que lo convierte en un módulo de alto impacto recaudatorio y de alta complejidad de cálculo.

| Puntos clave para la toma de decisiones Cálculo combinado, único en el sistema: a diferencia de los demás impuestos, el ISC de alcohol y tabaco suma dos componentes — una tasa porcentual (ad-valorem) sobre el precio sugerido más un monto fijo por unidad (específico). El sistema debe modelar ambos a la vez. Tablas que cambian cada trimestre: los montos específicos se ajustan trimestralmente por inflación, lo que obliga a un sistema de tablas vigentes por período, no a valores fijos en código. Calendarios de pago heterogéneos: alcohol y tabaco se pagan mensual; hidrocarburos, semanal (jueves); operaciones financieras, semanal (viernes). El sistema debe soportar múltiples frecuencias. Control físico (trazabilidad fiscal): alcohol y tabaco requieren marcación fiscal (estampillas/sellos) y licencias oficiales. El sistema enlaza con un componente de trazabilidad de productos físicos. Formularios múltiples: cada categoría tiene su propio formulario (ISC-02 alcohol/tabaco, IST-01 telecomunicaciones, DSS seguros). |
| --- |

La conclusión de negocio es que el ISC es el impuesto de mayor heterogeneidad operativa del sistema: distintas bases de cálculo, distintas frecuencias de pago, distintos formularios y un componente de control físico de mercancías. Modelarlo bien exige un motor altamente configurable por categoría de producto.


## 2. Marco Legal, Hecho Generador y Sujetos

El ISC se regula en el Título IV del Código Tributario (Ley 11-92), artículos 361 a 400, y su reglamento de aplicación es el Decreto 01-18 (Siempre al Día, 2024). El hecho generador, conforme al artículo 362, es la transferencia a título oneroso o gratuito de ciertos bienes a nivel de fabricante o productor, su importación, y la prestación de los servicios gravados; se consideran también el retiro de bienes y los faltantes de inventario, con ciertas excepciones (Siempre al Día, 2024).

Son sujetos obligados los fabricantes o productores de los bienes gravados, los importadores de dichos bienes y los prestadores o locadores de los servicios gravados (DGII, 2024). A diferencia del ITBIS, que grava en cada etapa de la cadena, el ISC de bienes grava principalmente a nivel de fabricante o importador, no en cada reventa.


## 3. Bienes y Servicios Gravados

El ISC alcanza un conjunto definido de bienes y servicios que el sistema debe tener catalogados (DGII, 2024; CECAFI, 2025):

Productos del alcohol: cervezas, alcohol etílico y bebidas espirituosas.

Productos del tabaco: cigarrillos y otros productos que contengan tabaco.

Servicios de telecomunicaciones: internet, teléfono, cable y similares.

Seguros en general.

Emisión de cheques y transferencias electrónicas (operaciones financieras).

Hidrocarburos (con régimen especial, regido también por las leyes 112-00, 557-05 y 495-06).


## 4. Estructura de Tasas: el Cálculo Combinado

La característica técnica que distingue al ISC de todos los demás impuestos del sistema es su estructura de cálculo dual para alcohol y tabaco, que combina un componente porcentual (ad-valorem) con un componente fijo (específico), conforme al artículo 375 de la Ley 11-92.


### 4.1. Tasas por categoría

| Categoría | Ad-valorem | Componente específico |
| --- | --- | --- |
| Alcohol, bebidas alcohólicas y cervezas | 10 % sobre PVP | Monto fijo por litro de alcohol absoluto |
| Productos del tabaco | 20 % sobre PVP | Monto fijo por cajetilla (según unidades) |
| Telecomunicaciones | 10 % | No aplica (solo ad-valorem) |
| Seguros en general | 16 % | No aplica |
| Cheques y transferencias | 0.0015 (1.5‰) | Sobre el valor de cada operación |

Fuente: DGII (2024); El Nuevo Diario (2026); CECAFI (2025). PVP = Precio Sugerido de Venta al Público. Base legal: Art. 375 de la Ley 11-92.


### 4.2. Ajuste trimestral de montos específicos

Los montos específicos de alcohol y tabaco no son fijos: se ajustan trimestralmente según el índice de inflación del Banco Central, conforme a los parágrafos III y IX del artículo 375 (Siempre al Día, 2024). La DGII publica estos montos mediante resoluciones periódicas (por ejemplo, la Resolución DDG-AR1-2026-00001). El sistema debe, por tanto, mantener un repositorio de montos vigentes por trimestre y aplicar el correspondiente a la fecha de la operación, nunca un valor codificado de forma estática.


## 5. Trazabilidad Fiscal y Licencias (control físico)

El ISC de alcohol y tabaco introduce un elemento ausente en los demás impuestos: el control físico de la mercancía. Para producir o importar estos bienes se requiere una Licencia Oficial otorgada por la DGII (CECAFI, 2025). Adicionalmente, la Norma General 07-2021 establece el Sistema de Control y Trazabilidad Fiscal, que exige aplicar marcación fiscal (sellos seguros con códigos digitales únicos) a los productos terminados de alcohol y tabaco, salvo los destinados a exportación o a regímenes especiales exentos (DGII, 2021).

Para el sistema, esto significa que el módulo de ISC no es puramente declarativo: debe integrarse con un componente de trazabilidad que gestione licencias, estampillas y verificación física, conectando el mundo documental con el mundo físico de la mercancía. Este es un requisito de arquitectura singular que ningún otro impuesto impone.


## 6. Declaración, Formularios y Plazos

La heterogeneidad del ISC se refleja en sus formularios y calendarios. El sistema debe gestionar los siguientes formularios diferenciados (Siempre al Día, 2024):

ISC-02: declaración de alcohol y tabaco.

IST-01: declaración del impuesto selectivo a las telecomunicaciones.

DSS: declaración de los servicios de seguros.

En cuanto a plazos, el sistema debe soportar frecuencias distintas según la categoría (DGII, 2024): alcohol, tabaco, telecomunicaciones y seguros se declaran y pagan a más tardar el día 20 de cada mes; los hidrocarburos, los jueves de cada semana; y las operaciones financieras, los viernes de cada semana. Una particularidad de cumplimiento es que la declaración jurada debe presentarse aun cuando no exista impuesto a pagar (Siempre al Día, 2024).


## 7. Implicaciones para el Modelado del Sistema (para el equipo técnico)

Del análisis del ISC se derivan los siguientes requisitos para el diseño del módulo:

Motor de cálculo dual: capaz de sumar componente ad-valorem (sobre PVP) y componente específico (fijo por unidad física: litro de alcohol absoluto o cajetilla).

Repositorio de montos específicos por trimestre: tabla vigente por período, alimentada por las resoluciones de la DGII, con resolución automática del monto aplicable a la fecha de operación.

Motor de frecuencias de pago heterogéneas: soporte de calendarios mensual, semanal-jueves (hidrocarburos) y semanal-viernes (operaciones financieras).

Catálogo de categorías gravadas: con su base de cálculo, tasa y formulario asociado (ISC-02, IST-01, DSS).

Integración con trazabilidad fiscal: gestión de licencias oficiales, marcación fiscal (estampillas) y verificación física para alcohol y tabaco.

Regla de declaración obligatoria en cero: el sistema debe exigir la presentación aun sin impuesto a pagar.

La conexión cruzada más relevante de este módulo es con el componente físico-aduanero: en importaciones, el ISC se liquida junto con los impuestos aduaneros, lo que vincula este módulo con la Dirección General de Aduanas (DGA). Además, el sistema de trazabilidad fiscal anticipa una necesidad de integración con dispositivos y verificación en campo, acercando el sistema tributario al control logístico de mercancías.


## Referencias

Centro de Capacitación Financiera CECAFI. (2025). Impuesto Selectivo al Consumo (ISC) en la República Dominicana. https://cecafi1.blogspot.com/

Dirección General de Impuestos Internos. (2021). Norma General 07-2021 sobre control y trazabilidad fiscal. https://dgii.gov.do/legislacion/

Dirección General de Impuestos Internos. (2024). Impuesto Selectivo al Consumo. https://dgii.gov.do/cicloContribuyente/obligacionesTributarias/principalesImpuestos/Paginas/impuestoSelectivoConsumo.aspx

El Nuevo Diario. (2026). El impuesto selectivo al consumo (ISC): cervezas, tabaco y telecomunicaciones. https://elnuevodiario.com.do/

Siempre al Día. (2024). Características del impuesto selectivo al consumo. https://siemprealdia.co/republica-dominicana/impuestos/


---

# 2D — Impuesto sobre Activos e IPI

SINTRA-RD

Fase 2D — Impuestos al Patrimonio

Impuesto sobre Activos e Impuesto al Patrimonio Inmobiliario (IPI)

Cierre de la Fase 2 — Documentación del Código Tributario

Documentación de dominio tributario para modelado del módulo de patrimonio

Base legal: Código Tributario (Ley 11-92), Título V (Activos) · Ley 18-88 (IPI)

Modificadas por las leyes 253-12 y 288-04

Guillermo Estefán Puello · StarBound SRL · Metodología DEVGEP+

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Este documento cierra la Fase 2 (documentación del Código Tributario). Como los anteriores, se organiza para dos audiencias: la Sección 1 (Resumen Ejecutivo) para gerencia, y las secciones 2 a 6 con el detalle de dominio para el equipo técnico. La Sección 7 ofrece, adicionalmente, una síntesis integradora de toda la Fase 2.


## 1. Resumen Ejecutivo (para gerencia)

Este bloque cubre los impuestos al patrimonio, que gravan la riqueza acumulada (activos, inmuebles) en lugar del flujo (ingresos o consumo). Existen dos figuras complementarias y mutuamente excluyentes según el tipo de contribuyente: el Impuesto sobre Activos para personas jurídicas y el Impuesto al Patrimonio Inmobiliario (IPI) para personas físicas y fideicomisos.

| Puntos clave para la toma de decisiones Regla de exclusión mutua: una empresa paga Impuesto sobre Activos y no paga IPI; una persona física paga IPI y no paga Activos. El sistema debe enrutar al contribuyente al impuesto correcto según su tipo. El Activo es un piso del ISR: las empresas pagan el mayor entre su ISR y el 1% de sus activos. Esto cierra el circuito abierto en la Fase 2B (ISR) y evita que una empresa con utilidades bajas no tribute nada. El IPI tiene umbral exento ajustado por inflación: solo se paga sobre el valor que excede un umbral (RD$10,695,494 en 2026), actualizado anualmente. El sistema necesita el umbral vigente por año. Dependencia de una valoración externa: el IPI depende del valor catastral fijado por la Dirección General de Catastro Nacional, lo que obliga a integrar el sistema con esa institución. Función de bloqueo: la certificación de estar al día en IPI es requisito para transferir un inmueble, lo que convierte a este módulo en un control de habilitación de transacciones inmobiliarias. |
| --- |

La conclusión de negocio es que estos impuestos, aunque de cálculo más simple que el ISR, introducen dos requisitos arquitectónicos importantes: la integración con un registro externo de valoración (Catastro) y una función de bloqueo/habilitación que condiciona operaciones de terceros (la transferencia de inmuebles).


## 2. Impuesto sobre Activos (Personas Jurídicas)

El Impuesto sobre Activos grava el valor total de los activos de las personas jurídicas o de las personas físicas con negocios de único dueño, incluidos los inmuebles del balance, no ajustados por inflación y luego de aplicar deducciones por depreciación, amortización y reservas para cuentas incobrables (DGII, 2024). Su base legal está en el Título V de la Ley 11-92.

La tasa es del 1% del monto total de los activos imponibles. Su función esencial, ya señalada en la Fase 2B, es operar como un impuesto mínimo: la persona jurídica paga el mayor valor entre su ISR liquidado y este 1% de activos (CECAFI, 2025). Están exentas de Activos las personas jurídicas que, por el Código Tributario, leyes especiales o contratos aprobados por el Congreso, estén totalmente exentas del ISR (DGII, 2024). La declaración se presenta en la misma fecha que la del ISR, y el impuesto resultante se paga en dos cuotas iguales.


## 3. Impuesto al Patrimonio Inmobiliario — IPI (Personas Físicas)

El IPI es un impuesto anual sobre el patrimonio inmobiliario de las personas físicas y los fideicomisos, con base legal en la Ley 18-88, modificada por las leyes 288-04 y 253-12 (DGII, 2024; ABA, 2026). La distinción fundamental que el sistema debe modelar es: las personas jurídicas no pagan IPI (pagan Activos), mientras que las personas físicas y los fideicomisos sí (Alegra, 2026).


### 3.1. Base imponible, umbral y tasa

La tasa es del 1%, pero aplicada de forma distinta según el sujeto. Para personas físicas, el 1% se aplica únicamente sobre el valor que excede un umbral exento; para fideicomisos, el 1% se aplica sobre el valor total, sin umbral (Puente Azul, 2024). El umbral exento se ajusta anualmente por inflación: para el ejercicio 2026 es de RD$10,695,494.00, fijado por la Resolución DDG-AR1-2026-00001 (Siempre al Día, 2026).

La fórmula que el sistema debe implementar para personas físicas es: base imponible = valor total de inmuebles − monto exento; impuesto = base imponible × 1% (Alegra, 2026). El valor de los inmuebles es determinado por la Dirección General de Catastro Nacional, lo que constituye una dependencia de datos externa al sistema tributario (Ley 18-88, Art. 1).


### 3.2. Bienes gravados y exenciones

Están gravados las viviendas, los solares urbanos y los inmuebles destinados a actividades comerciales, industriales y profesionales (DGII, 2024). Quedan exentos, entre otros: los terrenos rurales y las mejoras de uso agropecuario; la vivienda de personas mayores de 65 años cuando constituya su único patrimonio inmobiliario; los pensionistas y rentistas de fuente extranjera en un 50% (Ley 171-07); y los inmuebles acogidos a leyes especiales como la 158-01 de Fomento Turístico (CONFOTUR) (ABA, 2026; Ministerio de Hacienda, 2023). El sistema debe modelar estas exenciones como reglas evaluables sobre el perfil del contribuyente y del inmueble.


### 3.3. Declaración, pago y función de bloqueo

La declaración jurada del IPI se presenta en los primeros 60 días del año, y el pago se realiza en dos cuotas semestrales: la primera antes del 11 de marzo y la segunda antes del 11 de septiembre (DGII, 2024). Un aspecto de especial relevancia para la arquitectura es que la certificación de estar al día en el IPI (o de que el inmueble está exento) es requisito indispensable para transferir la propiedad de un inmueble (Puente Azul, 2024). Esto convierte al módulo en un punto de control que habilita o bloquea operaciones en el registro inmobiliario.


## 4. Otros Tributos del Sistema

Además de los cuatro impuestos principales documentados en la Fase 2 (ITBIS, ISR, ISC y patrimonio), el sistema deberá contemplar otros tributos menores que comparten la misma infraestructura de declaración y pago, entre ellos el impuesto sobre ganancias de capital (ya tratado en la Fase 2B como parte del ISR), el impuesto a la primera placa de vehículos, y diversas tasas administrativas. Estos se modelarán como variantes del mismo motor de liquidación, sin requerir arquitectura propia.

Conviene también señalar la existencia de regímenes simplificados, como el Régimen Simplificado de Tributación (RST), que ofrecen modalidades alternativas de cumplimiento para pequeños contribuyentes. El sistema debe contemplar estos regímenes como rutas de cálculo alternativas, seleccionables según la clasificación del contribuyente.


## 5. Plazos y Mora

Para el Impuesto sobre Activos aplica el régimen general de mora del Código Tributario (10% el primer mes, 4% progresivo, 1.10% de interés), idéntico al del ITBIS e ISR. El IPI, por regirse por la Ley 18-88, tiene su propio régimen de recargo por pago tardío. El sistema debe, por tanto, asociar a cada impuesto su régimen sancionador correspondiente, confirmando la conveniencia (ya señalada en la Fase 2B) de un servicio de mora parametrizable por tipo de impuesto.


## 6. Implicaciones para el Modelado del Sistema (para el equipo técnico)

Del análisis de los impuestos al patrimonio se derivan los siguientes requisitos:

Enrutamiento por tipo de contribuyente: el sistema debe dirigir automáticamente a la persona jurídica al Impuesto sobre Activos y a la persona física o fideicomiso al IPI, garantizando la exclusión mutua.

Motor de impuesto mínimo (ISR vs. Activos): comparación del 1% de activos contra el ISR liquidado, cerrando la integración con el módulo de la Fase 2B.

Gestión de umbral exento por año: repositorio del umbral del IPI vigente por ejercicio, ajustado por inflación vía resoluciones de la DGII.

Integración con Catastro Nacional: consumo del valor catastral de los inmuebles como dato externo, lo que exige una interfaz con la Dirección General de Catastro.

Motor de exenciones sobre perfil: evaluación de exenciones según atributos del contribuyente (edad, condición de pensionista) y del inmueble (rural, turístico CONFOTUR).

Función de bloqueo/habilitación: emisión de certificaciones de estar al día que condicionan la transferencia de inmuebles, integrando el módulo con el registro de la propiedad.


## 7. Síntesis Integradora de la Fase 2

Con el cierre de este bloque se completa la documentación de las principales figuras del Código Tributario dominicano. La Fase 2 ha revelado un patrón arquitectónico consistente que guiará el diseño: todos los impuestos comparten una estructura común (sujeto, base imponible, tasa, exenciones, plazos, mora) pero difieren en sus reglas específicas. Esto confirma, de forma empírica y a partir de la ley real, la decisión arquitectónica central tomada en la Fase 1: el sistema debe construirse como un motor de liquidación genérico configurado por un repositorio de reglas tributarias, y no como módulos independientes con lógica duplicada.

La Fase 2 también ha evidenciado una densa red de interdependencias: el e-CF (Fase 1) alimenta el ITBIS (2A) y los gastos deducibles del ISR (2B); el ISR (2B) se conecta con el Impuesto sobre Activos (2D) mediante el impuesto mínimo; el ISC (2C) se vincula con Aduanas y la trazabilidad física; y el IPI (2D) depende de Catastro y condiciona el registro inmobiliario. El sistema tributario no es un conjunto de silos, sino un organismo de componentes interdependientes. Esta comprensión es el principal insumo para la siguiente fase de arquitectura lógica.


## Referencias

ABA. (2026). Cómo pagar el IPI de la DGII en República Dominicana. https://aba.org.do/

Alegra. (2026). Guía para calcular y pagar el IPI. https://blog.alegra.com/republica-dominicana/ipi/

Centro de Capacitación Financiera CECAFI. (2025). Impuesto sobre la renta y sus retenciones en la RD. https://cecafi1.blogspot.com/

Dirección General de Impuestos Internos. (2024). Impuesto al Patrimonio Inmobiliario (IPI) e Impuesto sobre Activos. https://dgii.gov.do/

Ministerio de Hacienda y Economía. (2023). Impuesto al patrimonio inmobiliario (IPI). https://www.hacienda.gob.do/

Puente Azul. (2024). ¿Qué es el Impuesto al Patrimonio Inmobiliario (IPI)? https://www.puenteazul.net/que-es-el-ipi

Siempre al Día. (2026). Exención del IPI y vivienda de bajo costo en 2026. https://siemprealdia.co/

