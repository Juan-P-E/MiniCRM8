MiniCRM (C# / .NET 8)

Aplicación de consola desarrollada en C# para la gestión básica de clientes, conversaciones y mensajes, utilizando SQLite como base de datos.

El proyecto simula funcionalidades simples de un CRM, trabajando con relaciones entre entidades y persistencia de datos.

Funcionalidades
Clientes

Alta de clientes con validaciones:

Email con formato válido

Email único (case-insensitive)

Teléfono opcional con normalización (8–15 dígitos, admite "+")

Modificación campo por campo (Enter mantiene valor)

Eliminación con confirmación

Búsqueda por ID

Búsqueda por nombre (contains, sin distinción de mayúsculas)

Listado en formato tabla

Conversaciones

Creación de conversaciones asociadas a un cliente

Relación Cliente → Conversación

Mensajes

Registro de mensajes dentro de una conversación

Relación Conversación → Mensajes

Persistencia en SQLite

Visualización de historial por conversación

Tecnologías

C# / .NET 8

SQLite

LINQ

System.Text.Json (uso previo para persistencia)

Regex (validaciones)

Git & GitHub

Estructura

Cliente → Conversación → Mensajes

Cada entidad está vinculada mediante IDs.

Ejecución

Clonar el repositorio:

git clone https://github.com/Juan-P-E/MiniCRM8.git

Entrar al directorio:

cd MiniCRM8

Ejecutar:

dotnet run

La base de datos clientes.db se crea automáticamente.

Ejemplo de uso
MINICRM - GESTIÓN DE CLIENTES

Agregar cliente

Modificar cliente

Eliminar cliente

Listar clientes

Buscar por ID

Buscar por Nombre

Agregar mensaje a conversación

Ver mensajes de una conversación

Salir

Próximos pasos

Selección de conversación sin ingreso manual de ID

Mejora del flujo de mensajes tipo chat

Separación de validaciones en una clase independiente

Versión con interfaz gráfica

Notas

Proyecto en evolución, enfocado en lógica de negocio y manejo de datos en aplicaciones tipo CRM.