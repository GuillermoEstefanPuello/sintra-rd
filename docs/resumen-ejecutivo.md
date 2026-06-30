# SINTRA-RD — Resumen Ejecutivo del Proyecto

Este documento resume el proyecto completo en lenguaje directo. Es el punto de entrada
para entender, en pocos minutos, que es SINTRA-RD, por que se diseno asi, y que se
construye en este repositorio. Para el detalle de cada parte, ver los demas archivos
de esta carpeta `docs/`.

## Que es SINTRA-RD

SINTRA-RD (Sistema Integral de Transformacion Tributaria de la Republica Dominicana)
es una propuesta de arquitectura para modernizar el sistema tributario dominicano.
No es un sistema ya construido por el Estado: es un diseno de referencia, con una
porcion construida como demo tecnico (este repositorio), que demuestra la viabilidad
del enfoque.

## El problema

El sistema tributario actual esta fragmentado: cada impuesto, proceso y canal de
atencion evoluciona de forma aislada, sin una capa comun de interoperabilidad. La
plataforma actual de la DGII (Oficina Virtual) corre sobre tecnologia legacy
(ASP.NET Web Forms) y no expone APIs publicas robustas, una carencia que la propia
comunidad de desarrolladores ha senalado. Ademas, hacia 2026 el 100% de los
contribuyentes debe facturar electronicamente, pero a mediados de 2024 cerca del
70% de las empresas aun no lo hacia.

## El enfoque: dos capas que no se mezclan

La decision central del diseno es separar dos planos que normalmente se confunden:

- **El COMO (capa tecnica):** arquitectura de integracion inspirada en el modelo de
  Estonia (e-MTA / X-Road), probado y de codigo abierto. Bus de interoperabilidad,
  principio "preguntame solo una vez", declaraciones pre-llenadas.
- **El QUE (capa legal):** el Codigo Tributario dominicano (Ley 11-92) y la Ley 32-23
  de Facturacion Electronica. Las tasas, exenciones, retenciones y plazos de cada
  impuesto. Esta capa manda siempre; la tecnica nunca la sustituye.

La arquitectura de Estonia da el esqueleto tecnico; la ley dominicana es el cerebro
y la sangre del sistema.

## La decision arquitectonica central: la ley es configuracion, no codigo

El hallazgo clave del analisis del Codigo Tributario (ver `fase2-codigo-tributario.md`)
es que todos los impuestos dominicanos comparten la misma estructura logica: sujeto,
base imponible, tasa, exenciones, retenciones, plazos y mora. Solo cambian los valores
de esas reglas.

Por eso el sistema NO se construye como un modulo de codigo distinto por cada impuesto.
Se construye como:

- Un **Motor de Liquidacion Generico** (un unico componente que sabe calcular
  cualquier impuesto).
- Un **Motor de Reglas Tributarias** (un repositorio configurable, versionado por
  fecha, donde viven las tasas, exenciones y demas reglas de cada impuesto).

El Motor de Liquidacion ejecuta las reglas que le entrega el Motor de Reglas. Cuando
una ley cambia, se actualiza una configuracion con su fecha de vigencia: el motor no
se reescribe, no se recompila, no se redespliega. Las declaraciones de periodos
pasados se siguen calculando con las reglas que estaban vigentes entonces.

Este es el concepto que el demo tecnico de este repositorio busca demostrar en
funcionamiento: cambiar una regla en configuracion y ver el calculo cambiar, sin
tocar una linea de codigo del motor.

## Arquitectura logica: 17 modulos en 4 planos

El sistema completo (documentado en `fase3-arquitectura.md`) se organiza en diecisiete
modulos de alto nivel, agrupados en cuatro planos:

- **Plano de Servicio:** Portal del Contribuyente, Mesa de Ayuda.
- **Plano Transaccional:** Registro de Contribuyentes, Calendario Fiscal, Motor de
  Liquidacion, Declaraciones, e-CF, Recaudacion, Retenciones, Cuenta Corriente.
- **Plano de Cumplimiento:** Fiscalizacion y Riesgo, Cobranza, Trazabilidad Fiscal.
- **Plano de Integracion:** Bus de Interoperabilidad, Identidad y Firma Digital,
  Observabilidad, Motor de Reglas, Notificaciones.

Los 17 son modulos de alto nivel; cada uno se descompone en sub-componentes en el
diseno detallado (ver el detalle de M3 y M5 en `fase4-diagramacion.md`).

Este demo tecnico construye, en su primera fase, el nucleo: el Motor de Liquidacion
(M3) y el Motor de Reglas (M16), aplicados al ITBIS como primer impuesto.

## El componente critico: el e-CF

El Comprobante Fiscal Electronico (e-CF) opera bajo el modelo Clearance: la DGII debe
autorizar el comprobante en tiempo real, antes de que se entregue al receptor. Toda
la facturacion del pais depende de este validador, por lo que se disena como
componente de alta disponibilidad, sin puntos unicos de falla.

## Principios de ingenieria aplicados (DEVGEP+ por nivel)

El diseno declara explicitamente que principios son obligatorios para un sistema de
esta naturaleza (critico, legal, transversal):

- **Nivel 1 (siempre):** CRUD, ACID, Single Source of Truth, idempotencia, nunca
  confiar en el input, fail fast, separacion de responsabilidades, statelessness.
- **Nivel 2 (por ser sistema critico/legal/estatal):** soft delete, auditoria
  inmutable, minimo privilegio, defensa en profundidad, eliminacion de SPOF,
  recuperacion ante desastres, observabilidad (logs, metricas, trazas).
- **Nivel 3:** incluye la Ley de Conway (la arquitectura debe reflejar la
  organizacion de los equipos) y los principios de IA, por usar inteligencia
  artificial en el modulo de fiscalizacion.

## Inteligencia artificial: asesor, no juez

El modulo de Fiscalizacion y Riesgo usa IA para detectar patrones de fraude y
priorizar casos de auditoria, pero bajo una regla innegociable: **human-in-the-loop**.
Ninguna sancion se ejecuta de forma automatica; toda accion con efecto sobre un
contribuyente requiere la decision de un funcionario, registrada en auditoria
inmutable. Esto protege simultaneamente la eficiencia y la legitimidad legal del
sistema.

## Riesgos principales y como se mitigan

El analisis de riesgos (ver `fase5-riesgos.md`) identifica diez riesgos. Los de mayor
prioridad son la baja adopcion del e-CF, la disponibilidad de integraciones externas
(Catastro, Aduanas, banca), la continuidad presupuestaria, y la seguridad de los
datos. La mayoria de los riesgos tecnicos ya estan mitigados por decisiones de diseno
tomadas en el propio proyecto (por ejemplo, el motor de reglas mitiga el riesgo de
cambios legales).

## Que construye este repositorio (el demo tecnico)

Este repositorio NO construye el sistema tributario nacional completo. Construye una
porcion representativa, con el rigor de diseno del sistema completo, para demostrar
el concepto central:

1. Motor de Reglas Tributarias (configuracion del ITBIS: tasas, exenciones).
2. Motor de Liquidacion Generico (calcula el ITBIS a partir de las reglas).
3. Una API que expone el motor (arquitectura API-first).
4. Una interfaz web simple donde se ingresa una operacion y se ve el ITBIS calculado
   en tiempo real.
5. Una demostracion en vivo: cambiar una regla en configuracion (por ejemplo, la
   tasa del ITBIS) y ver el resultado cambiar sin tocar codigo del motor.

Las bases (estructura de proyecto, capas, seguridad) se construyen pensando en que
el sistema pueda crecer hacia los 17 modulos completos, aunque en esta fase solo se
implemente el nucleo.

## Autor y metodologia

Guillermo Estefan Puello, StarBound SRL. Proyecto desarrollado bajo la metodologia
propia DEVGEP+. Las reglas de como se construye este codigo estan en `CLAUDE.md`,
en la raiz del repositorio.
