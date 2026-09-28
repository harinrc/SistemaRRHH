using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SistemaRRHH.Controllers
{
    // Solo permitimos el ingreso a usuarios autenticados cuyo Rol sea 'Admin'
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly SistemaRrhhContext _context;

        public AdminController(SistemaRrhhContext context)
        {
            _context = context;
        }

        // Pantalla Principal del Admin: Muestra las solicitudes de vacaciones pendientes
        public async Task<IActionResult> Vacaciones()
        {
            var pendientes = await _context.SolicitudesVacaciones.ToListAsync();
            return View(pendientes);
        }

        // Acción para Aprobar la solicitud usando el Stored Procedure de SQL
        [HttpPost]
        public async Task<IActionResult> Aprobar(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_AprobarVacaciones @p0", id);
            TempData["Mensaje"] = "La solicitud ha sido aprobada con éxito y los días fueron descontados.";
            return RedirectToAction(nameof(Vacaciones));
        }

        // Pantalla: Lista de Empleados y Formulario de Registro
        [HttpGet]
        public async Task<IActionResult> Empleados()
        {
            ViewBag.Departamentos = await _context.Departamentos.ToListAsync();
            var listaEmpleados = await _context.Empleados.ToListAsync();
            return View(listaEmpleados);
        }

        // Acción: Procesa el formulario y guarda el nuevo empleado en SQL Server
        [HttpPost]
        public async Task<IActionResult> RegistrarEmpleado(string nombre, string apellido, string email, string contrasena, int departamentoId, string rol)
        {
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
            {
                TempData["Error"] = "Por favor, completa todos los campos obligatorios.";
                return RedirectToAction(nameof(Empleados));
            }

            var nuevoEmpleado = new Empleados
            {
                Nombre = nombre,
                Apellido = apellido,
                Email = email,
                ContrasenaHash = contrasena,
                FechaIngreso = System.DateTime.Today,
                DiasVacacionesDisponibles = 15,
                DepartamentoId = departamentoId,
                Rol = rol
            };

            _context.Empleados.Add(nuevoEmpleado);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"¡Empleado {nombre} {apellido} registrado con éxito en el sistema!";
            return RedirectToAction(nameof(Empleados));
        }

        //  Carga la pantalla de Monitoreo Global de Asistencias
        [HttpGet]
        public async Task<IActionResult> HistorialAsistencias()
        {
            var asistenciasGlobales = await _context.Asistencias
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

            ViewBag.Empleados = await _context.Empleados.ToDictionaryAsync(e => e.Id, e => $"{e.Nombre} {e.Apellido}");

            return View(asistenciasGlobales);
        }
    }
}


