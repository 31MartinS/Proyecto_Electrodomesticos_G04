using ClienteWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClienteWeb.Controllers
{
    public class HomeController : Controller
    {
        private const string USUARIO_VALIDO = "MONSTER";
        private const string PASSWORD_VALIDA = "monster9";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (model.Usuario == USUARIO_VALIDO && model.Password == PASSWORD_VALIDA)
            {
                // Login exitoso: podrías setear sesión si quieres; para el examen basta con ir al menú
                return RedirectToAction("Menu");
            }

            model.LoginFallido = true;
            model.MensajeError = "Usuario o contraseña incorrectos.";
            model.Password = string.Empty;

            return View(model);
        }

        public IActionResult Menu()
        {
            return View();
        }
    }
}
