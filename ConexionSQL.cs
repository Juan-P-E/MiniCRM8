using MiniCRM;
using System;
using System.Data.SQLite;
// Update 2026 - connection handling improvements
public static class ConexionSQL
{
    private const string ConnStr = "Data Source=clientes.db;Version=3;";

    // 👉 CREA la tabla si no existe
    public static void ProbarConexion()
    {
        using var conn = new SQLiteConnection(ConnStr);
        conn.Open(); // Si no existe, crea el archivo clientes.db

        using var cmd = new SQLiteCommand(
            "CREATE TABLE IF NOT EXISTS Clientes (" +
            "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Nombre TEXT NOT NULL, " +
            "Email TEXT NOT NULL UNIQUE, " +   // 👈 Email único
            "Telefono TEXT)", conn);
        cmd.ExecuteNonQuery();

        Console.WriteLine("✅ Base SQLite lista (clientes.db creada o verificada).");
    }

    // 👉 GUARDA un cliente (sin duplicar)
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


    // 👉 LISTA los clientes
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