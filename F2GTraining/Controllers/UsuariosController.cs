using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using F2GTraining.Services;

namespace F2GTraining.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceAPIF2GTraining service;

        public UsuariosController(ServiceAPIF2GTraining service)
        {
            this.service = service;
        }


        public IActionResult InicioSesion()
        {
            if (HttpContext.User.FindFirst("IDUSUARIO") != null)
            {
                return RedirectToAction("MenuEquipo","Equipos");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InicioSesion(string usuario, string contrasenha)
        {
            string token = await this.service.GetTokenAsync(usuario, contrasenha);

            if (token != null)
            {
                Usuario user = await this.service.GetUsuarioAsync(token);

                ClaimsIdentity identity = new ClaimsIdentity
                (CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                Claim claimName = new Claim(ClaimTypes.Name, user.Nombre);
                identity.AddClaim(claimName);

                Claim claimId = new Claim("IDUSUARIO", user.IdUsuario.ToString());
                identity.AddClaim(claimId);

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddHours(12)
                    });

                HttpContext.Session.SetString("TOKEN", token);
                return RedirectToAction("MenuEquipo", "Equipos");

            }
            else
            {
                ViewData["ERROR"] = "ERROR: Credenciales de acceso erróneas";
                return View();
            }

        }

        public IActionResult RegistroUsuario()
        {
            if (HttpContext.User.FindFirst("IDUSUARIO") != null)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistroUsuario(string usuario, string contrasenha, string correo, int telefono)
        {
            if (HttpContext.User.FindFirst("IDUSUARIO") != null)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

            if (usuario.Count() > 16)
            {
                ViewData["ERROR"] = "ERROR: EL NOMBRE DE USUARIO NO PUEDE TENER MAS DE 16 CARACTERES";

            }else if (await this.service.CompruebaNombre(usuario))
            {
                ViewData["ERROR"] = "ERROR: EL NOMBRE DE USUARIO INTRODUCIDO YA EXISTE";
            }
            else if (await this.service.CompruebaCorreo(correo))
            {
                ViewData["ERROR"] = "ERROR: EL CORREO ELECTRONICO INTRODUCIDO YA EXISTE";
            }
            else if (await this.service.CompruebaTelefono(telefono))
            {
                ViewData["ERROR"] = "ERROR: EL TELEFONO INTRODUCIDO YA EXISTE";
            }
            else if (contrasenha.Count() <= 8)
            {
                ViewData["ERROR"] = "ERROR: LA CONTRASEÑA DEBE TENER UN MINIMO DE 8 CARACTERES";
            }

            if (ViewData["ERROR"] != null)
            {
                return View();
            }
            else
            {
                await this.service.InsertUsuario(new Usuario
                {
                    IdUsuario = 0,
                    Nombre = usuario,
                    Contrasenia = contrasenha,
                    Correo = correo,
                    Telefono = telefono,
                    Token = "SIN TOKEN"
                });
                TempData["MENSAJE"] = "Usuario registrado correctamente";
                return RedirectToAction("InicioSesion");
            }
        }

        [AuthorizeUsers]
        public async Task<IActionResult> CerrarSesion()
        {
            HttpContext.Session.Remove("TOKEN");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("InicioSesion");
        }

    }
}
