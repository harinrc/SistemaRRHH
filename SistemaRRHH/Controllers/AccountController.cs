using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaRRHH.Controllers
{
    public class AccountController : Controller
    {
        private readonly SistemaRrhhContext _context;

        public AccountController(SistemaRrhhContext context)
        {
            _context = context;
        }

        // Muestra la pantalla de Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Procesa los datos que el usuario escribe
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Buscamos si el correo y contraseña coinciden en SQL Server
            var usuario = await _context.Empleados
                .FirstOrDefaultAsync(e => e.Email == email && e.ContrasenaHash == password);

            if (usuario != null)
            {
                // Si existe, creamos su credencial digital de acceso
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), 
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol ?? "Empleado")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Redirección inteligente según el rol del usuario
                if (usuario.Rol == "Admin")
                {
                    // Si es Administrador, lo mandamos al Panel de Control principal
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Si es Empleado común, también lo mandamos al Panel de Control principal
                    return RedirectToAction("Index", "Home");
                }

            }

            // Si los datos son incorrectos, mandamos un aviso a la pantalla
            ViewBag.Error = "El correo electrónico o la contraseña son incorrectos.";
            return View();
        }

        // Acción para Cerrar Sesión
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
