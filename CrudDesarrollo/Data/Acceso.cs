// Data/MySqlDataAccess.cs
//using MySql.Data.MySqlClient;
using CrudDesarrollo.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace CrudDesarrollo.Data
{
    public class Acceso 
    {
        private string connectionString = "Server=localhost;Port=3306;Database=MVCCRUD;User=root;Password=;";

        //public async Task<IEnumerable<Usuario>> GetUsuariosAsync()

        public async Task<List<Usuarios>> GetUsuariosAsync(int? id)
        {
            var usuarios = new List<Usuarios>();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("select id,nombre,fecha,clave from usuarios", connection);

                    //command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var leer = await command.ExecuteReaderAsync())
                    {
                        while (await leer.ReadAsync())
                        {
                            var usuario = new Usuarios
                            {
                                Id = leer.GetInt32("Id"),
                                Nombre = leer.GetString("Nombre"),
                                Fecha = leer.GetDateTime("Fecha"),
                                Clave = leer.GetString("Clave")
                            };

                            usuarios.Add(usuario);

                            //Manejo robusto de la fecha
                            if (!leer.IsDBNull(leer.GetOrdinal("fecha")))
                            {
                                // Intentamos leer la fecha, si el tipo de datos es adecuado
                                try
                                {
                                    usuario.Fecha = leer.GetDateTime("fecha");
                                }
                                catch (InvalidCastException ex)
                                {
                                    Console.WriteLine($"Error al convertir la fecha: {ex.Message}");
                                    usuario.Fecha = default;  // Asignamos el valor predeterminado en caso de error
                                }
                            }
                            else
                            {
                                usuario.Fecha = default;  // Asignar valor predeterminado si es nulo
                            }
                        }
                    }
                }
                //return usuarios;
            }
            catch(MySqlException ex)
            {
                Console.WriteLine($"Error de MySQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return usuarios;

        }
        public async Task<Usuarios> GetUsuarioAsync(int id)
        {
            var usuario = new Usuarios();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("SELECT id, nombre, fecha, clave FROM usuarios WHERE id = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    using (var leer = await command.ExecuteReaderAsync())
                    {
                        if (await leer.ReadAsync())  // Solo buscamos un usuario, por lo tanto, un único `ReadAsync`
                        {
                            usuario.Id = leer.GetInt32("Id");
                            usuario.Nombre = leer.GetString("Nombre");
                            usuario.Fecha = leer.GetDateTime("Fecha");
                            usuario.Clave = leer.GetString("Clave");

                            // Manejo robusto de la fecha
                            if (!leer.IsDBNull(leer.GetOrdinal("fecha")))
                            {
                                try
                                {
                                    usuario.Fecha = leer.GetDateTime("fecha");
                                }
                                catch (InvalidCastException ex)
                                {
                                    Console.WriteLine($"Error al convertir la fecha: {ex.Message}");
                                    usuario.Fecha = default;
                                }
                            }
                            else
                            {
                                usuario.Fecha = default;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error de MySQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return usuario;  // Devuelve un único usuario
        }

        public async Task<bool> CrearUser(Usuarios nuevoUsuario)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand command;

                    // Verificamos si el usuario tiene un Id existente (para determinar si es una actualización)
                    string query = "INSERT INTO Usuarios (Nombre, Clave, Fecha) VALUES (@Nombre, @Clave, @Fecha)";
                    
                    command = new MySqlCommand(query, connection);

                    // Asignamos los parámetros a la consulta
                    command.Parameters.AddWithValue("@Nombre", nuevoUsuario.Nombre);
                    command.Parameters.AddWithValue("@Clave", nuevoUsuario.Clave);
                    command.Parameters.AddWithValue("@Fecha", nuevoUsuario.Fecha);
                    if (nuevoUsuario.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", nuevoUsuario.Id);
                    }

                    int fila = await command.ExecuteNonQueryAsync();
                    return fila > 0; // Retorna true si hubo una fila afectada
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hubo un error al Crear user: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> Guardar(Usuarios nuevoUsuario)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand command;

                    // Verificamos si el usuario tiene un Id existente (para determinar si es una actualización)
                    string query = "INSERT INTO Usuarios (Nombre, Clave, Fecha) VALUES (@Nombre, @Clave, @Fecha)";

                    //if (nuevoUsuario.Id == 0) // Insertar
                    //{
                    //    query 
                    //}
                    //else // Actualizar
                    //{
                    //    query = "UPDATE Usuarios SET Nombre = @Nombre, Clave = @Clave, Fecha = @Fecha WHERE Id = @Id";
                    //}

                    command = new MySqlCommand(query, connection);

                    // Asignamos los parámetros a la consulta
                    command.Parameters.AddWithValue("@Nombre", nuevoUsuario.Nombre);
                    command.Parameters.AddWithValue("@Clave", nuevoUsuario.Clave);
                    command.Parameters.AddWithValue("@Fecha", nuevoUsuario.Fecha);
                    if (nuevoUsuario.Id != 0)
                    {
                        command.Parameters.AddWithValue("@Id", nuevoUsuario.Id);
                    }

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0; // Retorna true si hubo una fila afectada
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hubo un error al guardar: {ex.Message}");
                return false;
            }
        }

        public async Task<Usuarios> Detalle(int Id)
        {
            Usuarios usuario = null; // Inicializa el objeto para evitar un retorno null si no se encuentra el usuario.
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("SELECT id, nombre, fecha, clave FROM usuarios WHERE id = @id", connection);
                    command.Parameters.AddWithValue("@id", Id);

                    using (var leer = await command.ExecuteReaderAsync())
                    {
                        if (await leer.ReadAsync())
                        {
                            usuario = new Usuarios
                            {
                                Id = leer.GetInt32("Id"),
                                Nombre = leer.GetString("Nombre"),
                                Fecha = leer.IsDBNull(leer.GetOrdinal("Fecha")) ? (DateTime?)null : leer.GetDateTime("Fecha"),
                                Clave = leer.GetString("Clave")
                            };
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error de MySQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return usuario;
        }

        public bool Editar(Usuarios nuevoUsuario)
        {
            bool r;

            try
            {

                // Usamos MySqlConnector para abrir la conexión y realizar la inserción
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // Consulta SQL para insertar un nuevo usuario
                    string query = "INSERT INTO Usuarios (id, Nombre, Fecha, Clave) VALUES (@Nombre, @Fecha, @Clave )";

                    var command = new MySqlCommand(query, connection);
                    // Asignamos los valores de los parámetros a la consulta
                    //command.Parameters.AddWithValue("@Id", nuevoUsuario.Id);
                    command.Parameters.AddWithValue("@Nombre", nuevoUsuario.Nombre);
                    command.Parameters.AddWithValue("@Fecha", nuevoUsuario.Fecha);
                    command.Parameters.AddWithValue("@Clave", nuevoUsuario.Clave);
                    //command.CommandType = CommandType.StoredProcedure; no se ejecuta ningun procedimiento
                    // Ejecutamos la consulta de inserción
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                r = true;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("", "Hubo un Editar al crear el usuario: " + ex.Message);
                r = false;
            }
            return r;
        }
        public async Task<bool> Eliminar(int? Id)
        {
            bool r = false;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Usuarios WHERE Id = @Id";

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", Id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    r = rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hubo un error al eliminar el usuario: {ex.Message}");
            }
            return r;
        }
        public async Task<bool> Actualizar(Usuarios usuario)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("UPDATE usuarios SET nombre = @nombre, fecha = @fecha, clave = @clave WHERE id = @id", connection);

                    command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@fecha", usuario.Fecha);
                    command.Parameters.AddWithValue("@clave", usuario.Clave);
                    command.Parameters.AddWithValue("@id", usuario.Id);

                    var cambio = await command.ExecuteNonQueryAsync();
                    return cambio > 0;
                }
            }
            catch (Exception ex)    
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
//if (!leer.IsDBNull(leer.GetOrdinal("fecha")))
//{
//    // Intentamos leer la fecha, si el tipo de datos es adecuado
//    try
//    {
//        usuario.Fecha = leer.GetDateTime("fecha");
//    }
//    catch (InvalidCastException ex)
//    {
//        Console.WriteLine($"Error al convertir la fecha: {ex.Message}");
//        usuario.Fecha = default;  // Asignamos el valor predeterminado en caso de error
//    }
//}
//else
//{
//    usuario.Fecha = default;  // Asignar valor predeterminado si es nulo
//}