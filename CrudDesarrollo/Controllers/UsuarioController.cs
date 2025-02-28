// Controllers/UsuariosController.cs
using CrudDesarrollo.Data;
using CrudDesarrollo.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace CrudDesarrollo.Controllers
{
    public class UsuariosController : Controller
    {
        // Se declara un campo privado para la clase MySqlDataAccess, que manejará las operaciones de acceso a la base de datos.
        private readonly Acceso _context;
        // Asigna el acceso a la base de datos al campo _context
        public UsuariosController(Acceso context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string buscar)
        {
            // Llama al método GetUsuariosAsync() para obtener la lista de usuarios desde la base de datos.
            var usuarios = await _context.GetUsuariosAsync(null);
            // Verifica si la lista de usuarios es null o vacía. Si es así, inicializa una lista vacía.
            if (usuarios == null)
            {
                usuarios = new List<Usuarios>();
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                usuarios = usuarios.Where(u => u.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(usuarios);
        }
        [HttpGet]
        public IActionResult CrearUser()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CrearUser(Usuarios nuevoUsuario)
        {
            try
            {
                //var res = _context.Guardar(nuevoUsuario);
                bool creado = await _context.CrearUser(nuevoUsuario);
                if (creado)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Guardar error", "No se ejecuto Guardar");
                }
                return View(nuevoUsuario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult Guardar()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Guardar(Usuarios nuevoUsuario)
        {
            try
            {
                //var res = _context.Guardar(nuevoUsuario);
                if (ModelState.IsValid)
                {
                    bool guardado = await _context.Guardar(nuevoUsuario);
                    if (guardado)
                    {
                        return RedirectToAction(nameof(Guardar), new { id = nuevoUsuario.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("null", "No se ejecuto Guardar");
                    }
                }
                return View();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Detalle (int id)
        {
            Usuarios nuevoUsuario = new Usuarios();
            var usuarios = await _context.GetUsuarioAsync(id);
            try
            {
                if (usuarios == null)
                {
                    return NotFound();
                }
                //return View();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View("Error");
            }
            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var usuario = await _context.Detalle(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  // Para protección contra ataques CSRF
        public async Task<IActionResult> Editar(int id, Usuarios usuario)
        {
            try
            {
                if (id != usuario.Id)
                {
                    return BadRequest(); // Si el id no coincide, retorna error
                }

                if (ModelState.IsValid)
                {
                    // Llamamos al método de la capa de acceso para actualizar el usuario
                    bool actualizado = await _context.Actualizar(usuario);

                    if (actualizado)
                    {
                        // Si la actualización fue exitosa, redirigimos a la vista de detalles del usuario
                        return RedirectToAction(nameof(Detalle), new { id = usuario.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hubo un error al actualizar el usuario.");
                    }
                }
                return View(usuario); // Si el modelo no es válido, vuelve a mostrar el formulario
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            try
            {
                bool eliminar = await _context.Eliminar(id);
                if(eliminar)
                {
                    Console.WriteLine("Se ha eliminado correctamente a: " + id);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View("Error");
            }

            return View(Index);
        }

    }
}
