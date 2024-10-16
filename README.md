Personal Finance Tracker API
Esta API, creada en C# y hospedada en Azure, es una solución para el seguimiento de finanzas personales. Permite a los usuarios gestionar sus transacciones, categorías y metas financieras.

Características
Gestión de Transacciones: Los usuarios pueden crear, leer, actualizar y eliminar transacciones financieras. Cada transacción contiene:

Cantidad
Tipo de transacción (Ingreso/Gasto)
Categoría
Fecha
Descripción
Categorías Personalizadas: Los usuarios pueden crear sus propias categorías para organizar mejor sus transacciones según sus necesidades específicas.

Metas Financieras: Los usuarios pueden establecer metas financieras para ayudarlos a planificar su presupuesto. Cada meta incluye:

Categoría
Cantidad objetivo
Periodo de tiempo (Diario/Semanal/Mensual/Anual)
Fecha de inicio y fin

Tecnologías Utilizadas
Lenguaje: C#
Framework: ASP.NET Core
Base de Datos: PostgreSQL
Hospedaje: Azure
Documentación
La API está documentada en Postman y puede ser utilizada fácilmente para realizar pruebas y ver el comportamiento de cada endpoint.
https://web.postman.co/workspace/bf560dd2-cae4-46f0-b6ae-80d7bff4cefa/overview

Instalación y Uso
Clona el repositorio.
Configura las cadenas de conexión en el archivo appsettings.json.
Restaura los paquetes de NuGet y compila la solución.
Ejecuta la API localmente o despliega en Azure siguiendo las configuraciones recomendadas.
