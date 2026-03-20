using MiniCRM;
using System;
using System.Data.SQLite;

// Update 2026 - connection handling improvements
public static class ConexionSQL
{
    private const string ConnStr = "Data Source=clientes.db;Version=3;";

    // CREA las tablas si no existen
    public static void ProbarConexion()
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        using var cmd = new SQLiteCommand(
            "CREATE TABLE IF NOT EXISTS Clientes (" +
            "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Nombre TEXT NOT NULL, " +
            "Email TEXT NOT NULL UNIQUE, " +
            "Telefono TEXT)", conn);
        cmd.ExecuteNonQuery();

        Console.WriteLine("✅ Base SQLite lista (clientes.db creada o verificada).");

        using (var cmdConversaciones = new SQLiteCommand(@"
CREATE TABLE IF NOT EXISTS Conversaciones (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER,
    Fecha TEXT
);", conn))
        {
            cmdConversaciones.ExecuteNonQuery();
        }

        using (var cmdMensajes = new SQLiteCommand(@"
CREATE TABLE IF NOT EXISTS Mensajes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ConversacionId INTEGER,
    Texto TEXT,
    Fecha TEXT
);", conn))
        {
            cmdMensajes.ExecuteNonQuery();
        }
    }

    public static void ListarConversaciones()
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "SELECT Id, ClienteId, Fecha FROM Conversaciones ORDER BY Id";
        using var cmd = new SQLiteCommand(sql, conn);
        using var rd = cmd.ExecuteReader();

        Console.WriteLine("\nID | CLIENTE ID | FECHA");
        Console.WriteLine("-------------------------------------------");

        while (rd.Read())
        {
            var id = rd.GetInt32(0);
            var clienteId = rd.GetInt32(1);
            var fecha = rd.GetString(2);

            Console.WriteLine($"{id,-3}| {clienteId,-10} | {fecha}");
        }

        Console.WriteLine();
    }
    public static void GuardarClienteEnSQL(Cliente c)
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "INSERT OR IGNORE INTO Clientes (Nombre, Email, Telefono) VALUES (@nombre, @mail, @tel)";
        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nombre", c.Nombre.Trim());
        cmd.Parameters.AddWithValue("@mail", c.Email.Trim().ToLower());
        cmd.Parameters.AddWithValue("@tel", c.Telefono?.Trim() ?? "");

        int filas = cmd.ExecuteNonQuery();
        if (filas == 0)
            Console.WriteLine("ℹ️ Ya existía un cliente con ese email. No se insertó.");
        else
            Console.WriteLine("✅ Cliente guardado en SQLite.");
    }

    public static void CrearConversacion(int clienteId)
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        string sql = "INSERT INTO Conversaciones (ClienteId, Fecha) VALUES (@clienteId, @fecha)";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@clienteId", clienteId);
        cmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        cmd.ExecuteNonQuery();

        Console.WriteLine("✔ Conversación creada correctamente.");
    }

    public static List<Mensaje> ObtenerMensajesPorConversacion(int conversacionId)
    {
        List<Mensaje> mensajes = new List<Mensaje>();

        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "SELECT Id, ConversacionId, Texto, Fecha FROM Mensajes WHERE ConversacionId = @conversacionId ORDER BY Id";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@conversacionId", conversacionId);

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            Mensaje mensaje = new Mensaje
            {
                Id = rd.GetInt32(0),
                ConversacionId = rd.GetInt32(1),
                Texto = rd.GetString(2),
                Fecha = rd.GetString(3)
            };

            mensajes.Add(mensaje);
        }

        return mensajes;
    }

    public static void GuardarMensajeEnSQL(Mensaje mensaje)
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "INSERT INTO Mensajes (ConversacionId, Texto, Fecha) VALUES (@conversacionId, @texto, @fecha)";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@conversacionId", mensaje.ConversacionId);
        cmd.Parameters.AddWithValue("@texto", mensaje.Texto);
        cmd.Parameters.AddWithValue("@fecha", mensaje.Fecha);

        cmd.ExecuteNonQuery();

        Console.WriteLine("✅ Mensaje guardado correctamente.");
    }
    // LISTA los clientes
    public static void ListarClientesSQL()
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "SELECT Id, Nombre, Email, Telefono FROM Clientes ORDER BY Id";
        using var cmd = new SQLiteCommand(sql, conn);
        using var rd = cmd.ExecuteReader();

        Console.WriteLine("\nID | NOMBRE               | EMAIL                     | TELÉFONO");
        Console.WriteLine("---------------------------------------------------------------------");

        while (rd.Read())
        {
            var id = rd.GetInt32(0);
            var nom = rd.GetString(1);
            var mail = rd.GetString(2);
            var tel = rd.IsDBNull(3) ? "" : rd.GetString(3);

            Console.WriteLine($"{id,-3}| {nom,-20} | {mail,-25} | {tel}");
        }

        Console.WriteLine();
    }
}