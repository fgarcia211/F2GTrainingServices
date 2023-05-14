using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Mvc;
using F2GTraining.Services;

namespace F2GTraining.Controllers
{
    public class EquiposController : Controller
    {
        private ServiceAPIF2GTraining service;

        public EquiposController(ServiceAPIF2GTraining service)
        {
            this.service = service;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> MenuEquipo()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");
            List<Equipo> equipos = await this.service.GetEquiposUser(token);

            if (equipos.Count == 0)
            {
                return View();
            }
            else
            {
                //CHANGE
                ViewData["JUGADORESUSUARIO"] = await this.service.GetJugadoresUsuario(token);
                return View(equipos);
            }

        }

        [AuthorizeUsers]
        public async Task<IActionResult> _PartialVistaEquipo(int idequipo)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            return PartialView(await this.service.GetEquipo(token, idequipo));
        }

        [AuthorizeUsers]
        public IActionResult CrearEquipo()
        {
            return View();
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> CrearEquipo(string nombre, IFormFile imagen)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            string extension = System.IO.Path.GetExtension(imagen.FileName);

            if (extension == ".png")
            {
                using (Stream stream = imagen.OpenReadStream())
                {
                    int rand = new Random().Next(0, 10000);
                    string nombrearchivo = nombre.Replace(" ","") + rand.ToString() + ".png";
                    await this.service.InsertEquipo("containerequipos", nombrearchivo, stream, nombre, token);
                }

                return RedirectToAction("MenuEquipo","Equipos");
            }
            else
            {
                ViewData["ERROR"] = "ERROR: La imagen debe ser en formato PNG";
                return View();
            }
        
        }

    }
}
