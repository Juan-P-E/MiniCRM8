# MiniCRM (C# / .NET 8)

Aplicación de consola desarrollada en C# para la gestión básica de clientes, conversaciones y mensajes, utilizando SQLite como base de datos.

El proyecto está orientado a simular funcionalidades simples de un CRM, trabajando con persistencia de datos y relaciones entre entidades.

## Funcionalidades

### Clientes
- Alta de clientes con validaciones
- Email con formato válido
- Email único (comparación case-insensitive)
- Teléfono opcional con normalización y validación
- Modificación campo por campo
- Eliminación con confirmación
- Búsqueda por ID
- Búsqueda por nombre
- Listado en formato tabla

### Conversaciones
- Creación manual de conversaciones desde menú
- Asociación de cada conversación a un cliente
- Listado de conversaciones disponibles

### Mensajes
- Registro de mensajes dentro de una conversación
- Visualización de historial por conversación
- Modo chat desde consola
- Persistencia en SQLite

## Tecnologías

- C#
- .NET 8
- SQLite
- LINQ
- Regex
- Git y GitHub

## Estructura del proyecto

Cliente → Conversación → Mensaje

Cada entidad se relaciona mediante IDs, simulando una estructura básica de un sistema CRM.

## Ejecución

Clonar el repositorio:

```bash
git clone https://github.com/Juan-P-E/MiniCRM8.git