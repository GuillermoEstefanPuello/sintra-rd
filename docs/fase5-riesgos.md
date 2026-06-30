SINTRA-RD

Fase 5 — Riesgos y Supuestos

Análisis de viabilidad y gestión de incertidumbre

Documento de gestión de proyecto (gerencia + ingeniería)

Lo que puede fallar, lo que asumimos y de qué dependemos

Guillermo Estefán Puello · StarBound SRL

Santo Domingo, República Dominicana · Junio 2026


## Cómo leer este documento

Todo el documento es de lectura accesible para gerencia y técnicos por igual, pues trata de la viabilidad del proyecto más que de su construcción. Documenta tres cosas que un proyecto serio no puede omitir: los supuestos sobre los que se sostiene, los riesgos que enfrenta, y cómo se mitigarían. Su existencia es, en sí misma, una señal de madurez del proyecto.


## 1. Resumen Ejecutivo

Ningún proyecto de escala nacional está libre de incertidumbre. Lo que distingue una propuesta madura de una ingenua no es prometer que nada saldrá mal, sino identificar con honestidad qué puede fallar y tener un plan para ello. Este documento hace exactamente eso: declara los supuestos que el proyecto da por ciertos, cataloga diez riesgos principales, los ordena por probabilidad e impacto, y propone una estrategia de mitigación para cada uno.

| La señal más importante de este documento Un evaluador técnico experimentado busca este capítulo de inmediato. Una propuesta sin análisis de riesgos sugiere que quien la hizo no ha construido sistemas reales. Presentar esta sección demuestra experiencia: se entiende que el éxito de un sistema tributario nacional depende tanto de la arquitectura como de factores externos —adopción, integraciones, presupuesto y voluntad política— que deben gestionarse activamente. |
| --- |


## 2. Supuestos del Proyecto

El diseño propuesto se sostiene sobre los siguientes supuestos. Si alguno no se cumple, el proyecto debe replantearse en la dimensión correspondiente. Declararlos explícitamente permite verificarlos antes de invertir.

Voluntad institucional e interoperabilidad: se asume que las instituciones externas (Catastro, Aduanas, Junta Central Electoral, Banco Central, INDOTEL, banca) están dispuestas y son capaces de exponer sus datos vía servicios, conforme al principio de interoperabilidad.

Estabilidad gestionable del marco legal: se asume que los cambios en la legislación tributaria, aunque frecuentes, son absorbibles mediante el Motor de Reglas (M16) sin reescribir el sistema; cambios estructurales profundos quedarían fuera de ese supuesto.

Continuidad presupuestaria y política: se asume financiamiento sostenido a lo largo del ciclo de vida del proyecto, que excede un período de gobierno.

Infraestructura de conectividad: se asume que la conectividad nacional y el acceso digital de los contribuyentes son suficientes para una operación mayoritariamente digital, con canales alternativos para la brecha existente.

Vigencia del calendario del e-CF: se asume que la obligatoriedad de la facturación electrónica (Ley 32-23) se mantiene como impulsor de la adopción.

Disponibilidad de datos migrables: se asume que los datos del sistema actual de la DGII pueden extraerse y migrarse con una calidad razonable.


## 3. Catálogo de Riesgos

Se identifican diez riesgos principales, agrupados por naturaleza. Cada uno se nombra con un código (R1 a R10) que se usa en la matriz y en el plan de mitigación.


### 3.1. Riesgos técnicos y de datos

R1 — Caída del validador e-CF (SPOF): al ser el punto por el que pasa toda la facturación, su indisponibilidad detiene la actividad económica formal. Impacto crítico.

R4 — Calidad de los datos migrados: datos sucios o inconsistentes del sistema legacy contaminan liquidaciones y análisis de riesgo (principio Garbage In, Garbage Out).

R7 — Ataque de seguridad o fuga de datos: el sistema custodia datos sensibles de millones de contribuyentes; una brecha tendría impacto crítico legal y reputacional.

R8 — Falsos positivos de la IA de fiscalización: un modelo mal calibrado puede señalar contribuyentes cumplidores, generando carga indebida y desconfianza.


### 3.2. Riesgos de adopción y dependencia externa

R2 — Baja adopción del e-CF: dado que cerca del 70% de las empresas aún no facturaba electrónicamente a mediados de 2024, existe riesgo de incumplimiento masivo del calendario y saturación de soporte.

R3 — Integraciones externas no disponibles o poco confiables: el sistema depende de datos de Catastro, Aduanas y otras instituciones; si no se integran a tiempo o entregan datos de baja calidad, módulos completos se ven afectados (IPI, importaciones).

R6 — Brecha de conectividad del contribuyente: segmentos de la población con acceso digital limitado pueden quedar excluidos sin canales alternativos.


### 3.3. Riesgos legales, organizacionales y de gobernanza

R5 — Cambios en la ley tributaria: reformas frecuentes; mitigado en gran medida por el diseño (motor de reglas), pero un cambio estructural profundo podría exceder esa capacidad.

R9 — Discontinuidad de presupuesto o voluntad política: un proyecto que abarca varios años es vulnerable a cambios de prioridad institucional. Impacto crítico sobre la continuidad.

R10 — Concentración de conocimiento y rotación: si el conocimiento del sistema se concentra en pocas personas, su salida pone en riesgo el mantenimiento (relacionado con la Ley de Conway).


## 4. Matriz de Riesgos

La siguiente matriz ubica cada riesgo según su probabilidad y su impacto, permitiendo priorizar la atención. Los riesgos en la zona superior derecha (alta probabilidad, alto impacto) exigen mitigación prioritaria.

La matriz revela que los riesgos más críticos a gestionar de forma prioritaria son la baja adopción del e-CF (R2) y la disponibilidad de las integraciones externas (R3), ambos de alta probabilidad y alto impacto, seguidos por la continuidad presupuestaria (R9) y la seguridad (R7), de impacto crítico aunque menor probabilidad.


## 5. Estrategias de Mitigación

Para cada riesgo se define una estrategia de mitigación concreta. Varias se apoyan directamente en decisiones de arquitectura ya tomadas en fases anteriores, lo que demuestra que el diseño anticipó estos riesgos.

| Riesgo | Estrategia de mitigación | Apoyo en el diseño |
| --- | --- | --- |
| R1 | Redundancia activa, modo de contingencia offline y eliminación de SPOF en el validador. | Ya previsto: M5 diseñado como componente de alta disponibilidad (Fase 3, Nivel 2). |
| R2 | Facturador gratuito, onboarding gradual por segmento, campañas y soporte reforzado en fechas límite. | Plano de servicio (M11, M12) y las tres vías de emisión documentadas en la Fase 1. |
| R3 | Acuerdos de interoperabilidad tempranos, contratos de interfaz versionados y degradación elegante si una fuente falla. | Bus de interoperabilidad (M13) con contratos estables (Fase 3). |
| R4 | Proceso de saneamiento y validación de datos previo a la migración; reglas de calidad de datos. | Principio GIGO y validación estricta (Nivel 1). |
| R5 | Mantener toda regla tributaria como configuración versionada por fecha. | Motor de Reglas (M16): la ley es configuración, no código. |
| R6 | Canales alternativos (presencial, asistido) y diseño accesible; no forzar el 100% digital de golpe. | Mesa de Ayuda (M12) y portal con foco en accesibilidad. |
| R7 | Defensa en profundidad, mínimo privilegio, cifrado, auditoría inmutable y pruebas de seguridad periódicas. | M14 y M15, principios de Nivel 2. |
| R8 | Human-in-the-loop obligatorio, calibración continua y monitoreo de tasa de falsos positivos. | La IA es asesor, no juez (Adenda de IA, Fase 3). |
| R9 | Entrega por fases con valor incremental demostrable, para sostener el respaldo en cada etapa. | Enfoque modular permite priorizar módulos de alto impacto temprano. |
| R10 | Documentación viva (estilo DEVGEP+), ADRs y equipos por dominio que difundan el conocimiento. | Ley de Conway aplicada y metodología de documentación. |


## 6. Conclusión de la Fase

El análisis confirma que SINTRA-RD es un proyecto viable pero exigente, cuyo éxito depende tanto de la solidez de su arquitectura como de la gestión activa de factores externos. La mayoría de los riesgos técnicos ya están mitigados por decisiones de diseño tomadas en fases anteriores —lo que evidencia que la arquitectura fue concebida con conciencia del riesgo—, mientras que los riesgos de adopción, integración y gobernanza requieren gestión institucional sostenida más allá de lo puramente técnico.

Con esta fase, el proyecto cuenta con un cuerpo de análisis completo: investigación (Fase 1), dominio tributario (Fase 2), arquitectura lógica (Fase 3), diagramación (Fase 4) y gestión de riesgos (Fase 5). El paso final será consolidar todo ello en una propuesta de presentación ejecutiva, orientada a comunicar el valor del proyecto y la capacidad de quien lo lidera.

