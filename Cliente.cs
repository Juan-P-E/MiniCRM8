using System;

namespace MiniCRM
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Telefono { get; set; }

        public Cliente() { }

        public Cliente(int id, string nombre, string email, string? telefono)
        {
            Id = id;
            Nombre = nombre;
            Email = email;
            Telefono = telefono;
        }
    }
}
