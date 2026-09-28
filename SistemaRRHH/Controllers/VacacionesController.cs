using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models;
using System;
using System.Threading.Tasks;


namespace SistemaRRHH.Controllers
{
    [Authorize]
    public class VacacionesController : Controller
    {
        private readonly SistemaRrhhContext _context;

        // Conectamos el controlador a tu base de datos SQL Server
        public VacacionesController(SistemaRrhhContext context)
        {
            _context = context;
        }

        // Pantalla Principal: Muestra los días disponibles de Ana Gómez
        public async Task<IActionResult> Index()
        {
            // Extrae de forma segura el ID del empleado que inició sesión
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int empleadoId = int.Parse(userIdClaim.Value);

            var empleado = await _context.Empleados.FindAsync(empleadoId);

            ViewBag.DiasDisponibles = empleado?.DiasVacacionesDisponibles ?? 0;
            ViewBag.NombreEmpleado = empleado != null ? $"{empleado.Nombre} {empleado.Apellido}" : "Empleado";

            return View();
        }

        // Procesa el formulario al enviar la solicitud (NUEVO FLUJO APROBACIÓN)
        [HttpPost]
        public async Task<IActionResult> SolicitarVacaciones(DateTime fechaInicio, DateTime fechaFin, string comentarios)
        {
            // Extrae de forma segura el ID del empleado que inició sesión
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int empleadoId = int.Parse(userIdClaim.Value);

            // Validamos que las fechas tengan coherencia básica
            if (fechaInicio > fechaFin || fechaInicio < DateTime.Today)
            {
                TempData["Error"] = "Las fechas seleccionadas no son válidas.";
                return RedirectToAction(nameof(Index));
            }

            // Calculamos la cantidad de días que el empleado está pidiendo
            int diasSolicitados = (fechaFin - fechaInicio).Days + 1;
            var empleado = await _context.Empleados.FindAsync(empleadoId);

            if (empleado != null)
            {
                // Validamos si tiene suficientes días acumulados para hacer la solicitud
                if (empleado.DiasVacacionesDisponibles >= diasSolicitados)
                {
                    // NUEVO: En lugar de restar, creamos la solicitud en estado 'Pendiente'
                    var nuevaSolicitud = new SolicitudesVacaciones
                    {
                        EmpleadoId = empleadoId,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        DiasSolicitados = diasSolicitados,
                        Estado = "Pendiente", // Se guarda esperando al Admin
                        FechaSolicitud = DateTime.Now,
                        Comentarios = comentarios
                    };

                    // Insertamos el registro en la tabla SolicitudesVacaciones
                    _context.SolicitudesVacaciones.Add(nuevaSolicitud);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = $"¡Solicitud enviada con éxito por {diasSolicitados} días! Queda en espera de aprobación por el Administrador.";
                }
                else
                {
                    TempData["Error"] = $"No tienes suficientes días disponibles. Solicitas {diasSolicitados} días y te quedan {empleado.DiasVacacionesDisponibles}.";
                }
            }
            else
            {
                TempData["Error"] = "Error al identificar el usuario en el sistema.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}