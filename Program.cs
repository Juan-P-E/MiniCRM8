using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MiniCRM
{
    class Program
    {
        // ====== Estado ======
        static List<Cliente> clientes = new();
        static readonly string rutaArchivo = Path.Combine(AppContext.BaseDirectory, "clientes.json");

        // ====== Main ======
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CargarClientes();

            while (true)
            {
                MostrarMenu();
                Console.Write("> Opción: ");
                var opcion = (Console.ReadLine() ?? "").Trim();

                switch (opcion)
                {
                    case "1": AgregarCliente(); break;
                    case "2": ModificarCliente(); break;
                    case "3": EliminarCliente(); break;
                    case "4": ListarClientes(); break;
                    case "5": BuscarPorId(); break;
                    case "6": BuscarPorNombre(); break;
                    case "0":
                        Console.WriteLine("Saliendo... ¡Hasta luego!");
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Presione ENTER para continuar...");
                Console.ReadLine();
            }
        }

        // ====== Menú ======
        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("MINICRM - GESTIÓN DE CLIENTES");
            Console.WriteLine(new string('-', 30));
            Console.WriteLine("1) Agregar cliente");
            Console.WriteLine("2) Modificar cliente");
            Console.WriteLine("3) Eliminar cliente");
            Console.WriteLine("4) Listar clientes");
            Console.WriteLine("5) Buscar por ID");
            Console.WriteLine("6) Buscar por Nombre");
            Console.WriteLine("0) Salir");
            Console.WriteLine();
        }

        // ====== Persistencia ======
        static void GuardarClientes()
        {
            try
            {
                var opciones = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(clientes, opciones);
                File.WriteAllText(rutaArchivo, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar: {ex.Message}");
            }
        }

        static void CargarClientes()
        {
            try
            {
                if (File.Exists(rutaArchivo))
                {
                    string json = File.ReadAllText(rutaArchivo);
                    var lista = JsonSerializer.Deserialize<List<Cliente>>(json);
                    clientes = lista ?? new List<Cliente>();
                }
                else
                {
                    clientes = new List<Cliente>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                clientes = new List<Cliente>();
            }
        }

        // ====== Altas / Bajas / Modificaciones ======
        static void AgregarCliente()
        {
            Console.WriteLine("ALTA DE CLIENTE");
            Console.WriteLine(new string('-', 18));

            string nombre = PedirNoVacio("Nombre: ");

            // Email con normalización + validación + unicidad
            string email;
            while (true)
            {
                email = NormalizarEmail(PedirNoVacio("Email: "));
                if (!EmailValido(email))
                {
                    Console.WriteLine("✗ Email inválido. Ejemplo válido: juan@dominio.com");
                    continue;
                }
                if (ExisteEmail(email))
                {
                    Console.WriteLine("✗ Ya existe un cliente con ese email (comparación sin mayúsculas/minúsculas).");
                    continue;
                }
                break;
            }

            // Teléfono opcional
            Console.Write("Teléfono (opcional, enter para omitir): ");
            string? telefonoInput = Console.ReadLine();
            string? telefono = string.IsNullOrWhiteSpace(telefonoInput) ? null : NormalizarTelefono(telefonoInput!);

            if (telefono != null && !TelefonoValido(telefono))
            {
                Console.WriteLine("✗ Teléfono inválido. Acepte entre 8 y 15 dígitos (se permite '+').");
                return;
            }

            int nuevoId = clientes.Count == 0 ? 1 : clientes.Max(c => c.Id) + 1;
            var cli = new Cliente(nuevoId, nombre.Trim(), email, telefono);
            clientes.Add(cli);
            GuardarClientes();

            Console.WriteLine("✓ Cliente agregado con éxito.");
        }

        static void ModificarCliente()
        {
            Console.WriteLine("MODIFICAR CLIENTE");
            Console.WriteLine(new string('-', 19));

            if (clientes.Count == 0)
            {
                Console.WriteLine("No hay clientes cargados.");
                return;
            }

            int id = PedirEntero("ID del cliente a modificar: ");
            var cli = clientes.FirstOrDefault(c => c.Id == id);
            if (cli == null)
            {
                Console.WriteLine("✗ No se encontró el cliente.");
                return;
            }

            bool huboCambios = false;

            Console.Write($"Nombre [{cli.Nombre}] (Enter para mantener): ");
            var nuevoNombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoNombre) && nuevoNombre!.Trim() != cli.Nombre)
            {
                cli.Nombre = nuevoNombre.Trim();
                huboCambios = true;
            }

            Console.Write($"Email [{cli.Email}] (Enter para mantener): ");
            var nuevoEmailInp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoEmailInp))
            {
                var nuevoEmail = NormalizarEmail(nuevoEmailInp!);
                if (!EmailValido(nuevoEmail))
                {
                    Console.WriteLine("✗ Email inválido. No se actualiza.");
                }
                else if (ExisteEmail(nuevoEmail, excluirId: cli.Id))
                {
                    Console.WriteLine("✗ Ya existe otro cliente con ese email.");
                }
                else if (nuevoEmail != cli.Email)
                {
                    cli.Email = nuevoEmail;
                    huboCambios = true;
                }
            }

            Console.Write($"Teléfono [{cli.Telefono ?? "vacío"}] (Enter para mantener, '-' para borrar): ");
            var nuevoTelInp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoTelInp))
            {
                if (nuevoTelInp!.Trim() == "-")
                {
                    if (cli.Telefono != null) { cli.Telefono = null; huboCambios = true; }
                }
                else
                {
                    var telNorm = NormalizarTelefono(nuevoTelInp);
                    if (!TelefonoValido(telNorm))
                    {
                        Console.WriteLine("✗ Teléfono inválido. No se actualiza.");
                    }
                    else if (cli.Telefono != telNorm)
                    {
                        cli.Telefono = telNorm;
                        huboCambios = true;
                    }
                }
            }

            if (huboCambios)
            {
                GuardarClientes();
                Console.WriteLine("✓ Cliente modificado con éxito.");
            }
            else
            {
                Console.WriteLine("No se realizaron cambios.");
            }
        }

        static void EliminarCliente()
        {
            Console.WriteLine("ELIMINAR CLIENTE");
            Console.WriteLine(new string('-', 17));

            if (clientes.Count == 0)
            {
                Console.WriteLine("No hay clientes cargados.");
                return;
            }

            int id = PedirEntero("ID del cliente a eliminar: ");
            var cli = clientes.FirstOrDefault(c => c.Id == id);
            if (cli == null)
            {
                Console.WriteLine("✗ No se encontró el cliente.");
                return;
            }

            Console.Write($"¿Confirmar eliminación de '{cli.Nombre}' (s/n)? ");
            var conf = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (conf == "s" || conf == "si" || conf == "sí")
            {
                clientes.Remove(cli);
                GuardarClientes();
                Console.WriteLine("✓ Cliente eliminado.");
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
        }

        // ====== Listado y Búsquedas ======
        static void ListarClientes(IEnumerable<Cliente>? fuente = null)
        {
            var data = (fuente ?? clientes).OrderBy(c => c.Id).ToList();

            Console.WriteLine("LISTA DE CLIENTES");
            Console.WriteLine(new string('-', 18));

            if (data.Count == 0)
            {
                Console.WriteLine("No hay clientes para mostrar.");
                return;
            }

            // Calcular anchos para columnas
            int wId = Math.Max(2, data.Max(c => c.Id.ToString().Length));
            int wNombre = Math.Max(6, data.Max(c => (c.Nombre ?? "").Length));
            int wEmail = Math.Max(5, data.Max(c => (c.Email ?? "").Length));
            int wTel = Math.Max(8, data.Max(c => (c.Telefono ?? "").Length));

            // Encabezados
            Console.WriteLine(
                $"{Pad("ID", wId)}  {Pad("NOMBRE", wNombre)}  {Pad("EMAIL", wEmail)}  {Pad("TELÉFONO", wTel)}");
            Console.WriteLine(new string('-', wId + wNombre + wEmail + wTel + 6));

            // Filas
            foreach (var c in data)
            {
                Console.WriteLine(
                    $"{Pad(c.Id.ToString(), wId)}  {Pad(c.Nombre, wNombre)}  {Pad(c.Email, wEmail)}  {Pad(c.Telefono ?? "", wTel)}");
            }
        }

        static void BuscarPorId()
        {
            Console.WriteLine("BUSCAR POR ID");
            Console.WriteLine(new string('-', 12));
            if (clientes.Count == 0)
            {
                Console.WriteLine("No hay clientes cargados.");
                return;
            }

            int id = PedirEntero("ID: ");
            var c = clientes.FirstOrDefault(x => x.Id == id);
            if (c == null)
            {
                Console.WriteLine("No se encontró el cliente.");
                return;
            }
            ListarClientes(new[] { c });
        }

        static void BuscarPorNombre()
        {
            Console.WriteLine("BUSCAR POR NOMBRE");
            Console.WriteLine(new string('-', 16));
            if (clientes.Count == 0)
            {
                Console.WriteLine("No hay clientes cargados.");
                return;
            }

            Console.Write("Nombre contiene: ");
            var q = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(q))
            {
                Console.WriteLine("Búsqueda vacía.");
                return;
            }

            var res = clientes
                .Where(c => (c.Nombre ?? "").Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (res.Count == 0)
            {
                Console.WriteLine("No hay coincidencias.");
                return;
            }
            ListarClientes(res);
        }

        // ====== Helpers UI ======
        static string Pad(string? s, int width)
        {
            s ??= "";
            if (s.Length >= width) return s;
            return s + new string(' ', width - s.Length);
        }

        static string PedirNoVacio(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s))
                    return s.Trim();
                Console.WriteLine("Este campo no puede estar vacío.");
            }
        }

        static int PedirEntero(string etiqueta)
        {
            while (true)
            {
                Console.Write(etiqueta);
                var s = (Console.ReadLine() ?? "").Trim();
                if (int.TryParse(s, out int n) && n > 0) return n;
                Console.WriteLine("Ingrese un número entero válido (> 0).");
            }
        }

        // ====== Validaciones ======
        static string NormalizarEmail(string email)
            => (email ?? "").Trim().ToLowerInvariant();

        static bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Regex simple: algo@algo.algo (mínimo)
            var rx = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return rx.IsMatch(email);
        }

        static bool ExisteEmail(string email, int? excluirId = null)
        {
            return clientes.Any(c =>
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                (!excluirId.HasValue || c.Id != excluirId.Value));
        }

        static string NormalizarTelefono(string telRaw)
        {
            telRaw = (telRaw ?? "").Trim();
            // Conservar '+' inicial, resto dígitos
            string soloDigitos = new string(telRaw.Where(char.IsDigit).ToArray());
            if (telRaw.StartsWith("+"))
                return "+" + soloDigitos;
            return soloDigitos;
        }

        static bool TelefonoValido(string tel)
        {
            if (string.IsNullOrEmpty(tel)) return false;
            // Acepta + y 8–15 dígitos
            string digits = tel.StartsWith("+") ? tel.Substring(1) : tel;
            return digits.All(char.IsDigit) && digits.Length >= 8 && digits.Length <= 15;
        }
    }
}
