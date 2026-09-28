using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models;
using System;
using System.Threading.Tasks;

namespace SistemaRRHH.Controllers
{
    [Authorize]
    public class AsistenciaController : Controller
    {
        private readonly SistemaRrhhContext _context;

        public AsistenciaController(SistemaRrhhContext context)
        {
            _context = context;
        }

        // Pantalla principal del Reloj Checador
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int empleadoId = int.Parse(userIdClaim.Value);
            DateTime hoy = DateTime.Today;

            // Buscamos si el empleado ya registró marca el día de hoy
            var asistenciaHoy = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.EmpleadoId == empleadoId && a.Fecha == hoy);

            ViewBag.AsistenciaHoy = asistenciaHoy;
            return View();
        }

        // Acción para registrar la hora de entrada
        [HttpPost]
        public async Task<IActionResult> MarcarEntrada()
        {
            // NUEVO: Obtenemos el ID dinámico del usuario en sesión
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int empleadoId = int.Parse(userIdClaim.Value);

            DateTime hoy = DateTime.Today;

            var existe = await _context.Asistencias.AnyAsync(a => a.EmpleadoId == empleadoId && a.Fecha == hoy);
            if (!existe)
            {
                var nuevaAsistencia = new Asistencias
                {
                    EmpleadoId = empleadoId,
                    Fecha = hoy,
                    HoraEntrada = DateTime.Now
                };

                _context.Asistencias.Add(nuevaAsistencia);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Acción para registrar la hora de salida
        [HttpPost]
        public async Task<IActionResult> MarcarSalida()
        {
            // NUEVO: Obtenemos el ID dinámico del usuario en sesión
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            int empleadoId = int.Parse(userIdClaim.Value);

            DateTime hoy = DateTime.Today;

            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.EmpleadoId == empleadoId && a.Fecha == hoy && a.HoraSalida == null);

            if (asistencia != null)
            {
                asistencia.HoraSalida = DateTime.Now;
                _context.Asistencias.Update(asistencia);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
