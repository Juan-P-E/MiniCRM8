MiniCRM (C# / .NET 8)

Un gestor de clientes sencillo desarrollado en C# como práctica personal para aplicar conceptos de programación y buenas prácticas en el manejo de datos.
El objetivo es simular funciones básicas de un CRM: alta, edición, baja, búsqueda y listado de clientes.

✨ Funcionalidades

Agregar clientes con validación de:

Email en formato correcto.

Email único (comparación case-insensitive).

Teléfono opcional con normalización y validación (8–15 dígitos, admite “+”).

Modificar clientes:

Edición campo por campo.

Enter → mantiene el valor actual.

Validaciones aplicadas al nuevo valor.

Eliminar clientes con confirmación antes de borrar.

Buscar clientes:

Por ID exacto.

Por nombre (búsqueda contains, case-insensitive).

Listar clientes en tabla alineada (ID, Nombre, Email, Teléfono).

Persistencia en JSON:

Los cambios se guardan automáticamente en clientes.json.

Al iniciar el programa, se cargan los datos guardados.

🛠️ Tecnologías usadas

C# / .NET 8

LINQ

System.Text.Json (serialización y persistencia)

Regex (validación de emails)

Git & GitHub (control de versiones)

📂 Ejecución

Clonar el repositorio:

git clone https://github.com/TU-USUARIO/MiniCRM.git


Entrar al directorio:

cd MiniCRM


Ejecutar con .NET:

dotnet run


El archivo clientes.json se genera automáticamente en la carpeta bin/Debug/net8.0/.

📸 Captura de ejemplo
MINICRM - GESTIÓN DE CLIENTES
------------------------------
1) Agregar cliente
2) Modificar cliente
3) Eliminar cliente
4) Listar clientes
5) Buscar por ID
6) Buscar por Nombre
0) Salir


Listado con datos cargados:

ID  NOMBRE       EMAIL                TELÉFONO
--  ----------   ------------------   --------------
1   Juan Perez   juan@example.com     +5493329551497
2   Maria Lopez  maria@test.com       

🎯 Aprendizaje aplicado

Este proyecto me sirvió para practicar:

Manejo de listas y colecciones en C#.

Validaciones de entrada y normalización de datos.

Serialización/deserialización en JSON.

Interacción con usuario en consola.

Organización de código en múltiples clases.

🚀 Próximos pasos

Separar validaciones en una clase independiente (Validaciones.cs).

Agregar exportación a CSV.

Preparar versión con interfaz gráfica (WinForms o MAUI) como evolución del proyecto.

👉 Repo creado como práctica de nivel trainee con orientación a CRM Dynamics 365 / Power Platform.
Este MiniCRM es la base para seguir aprendiendo y mostrar mi progreso en proyectos reales.