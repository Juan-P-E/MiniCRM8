using MiniCRM;
using System;
using System.Data.SQLite;
using System.Collections.Generic;

public static class ConexionSQL
{
    private const string ConnStr = "Data Source=clientes.db;Version=3;";

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

        using var cmdConversaciones = new SQLiteCommand(@"
CREATE TABLE IF NOT EXISTS Conversaciones (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER,
    Fecha TEXT
);", conn);
        cmdConversaciones.ExecuteNonQuery();

        using var cmdMensajes = new SQLiteCommand(@"
CREATE TABLE IF NOT EXISTS Mensajes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ConversacionId INTEGER,
    Texto TEXT,
    Fecha TEXT
);", conn);
        cmdMensajes.ExecuteNonQuery();

        Console.WriteLine("Base SQLite lista.");
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

        cmd.ExecuteNonQuery();
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
    }

    public static bool ExisteClientePorId(int clienteId)
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "SELECT COUNT(1) FROM Clientes WHERE Id = @id";
        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", clienteId);

        long cantidad = (long)cmd.ExecuteScalar();
        return cantidad > 0;
    }

    public static List<Mensaje> ObtenerMensajesPorConversacion(int conversacionId)
    {
        List<Mensaje> mensajes = new List<Mensaje>();

        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = @"
SELECT Id, ConversacionId, Texto, Fecha
FROM Mensajes
WHERE ConversacionId = @id
ORDER BY Id";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", conversacionId);

        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            mensajes.Add(new Mensaje
            {
                Id = rd.GetInt32(0),
                ConversacionId = rd.GetInt32(1),
                Texto = rd.GetString(2),
                Fecha = rd.GetString(3)
            });
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
    }

    public static void ListarClientesSQL()
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = "SELECT Id, Nombre, Email, Telefono FROM Clientes ORDER BY Id";
        using var cmd = new SQLiteCommand(sql, conn);
        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            Console.WriteLine($"{rd.GetInt32(0)} - {rd.GetString(1)}");
        }
    }

    public static List<(int Id, string NombreCliente, string Fecha)> ObtenerConversaciones()
    {
        List<(int Id, string NombreCliente, string Fecha)> conversaciones = new();

        using var conn = new SQLiteConnection(ConnStr);
        conn.Open();

        const string sql = @"
SELECT c.Id, cl.Nombre, c.Fecha
FROM Conversaciones c
JOIN Clientes cl ON c.ClienteId = cl.Id
ORDER BY c.Id";

        using var cmd = new SQLiteCommand(sql, conn);
        using var rd = cmd.ExecuteReader();

        while (rd.Read())
        {
            conversaciones.Add((
                rd.GetInt32(0),
                rd.GetString(1),
                rd.GetString(2)
            ));
        }

        return conversaciones;
    }

}