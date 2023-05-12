using F2GTraining.Extensions;
using F2GTraining.Filters;
using F2GTraining.Helpers;
using ModelsF2GTraining;
using F2GTraining.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace F2GTraining.Controllers
{
    public class EquiposController : Controller
    {
        private IRepositoryF2GTraining repo;
        private HelperSubirFicheros helperArchivos;

        public EquiposController(IRepositoryF2GTraining repo, HelperSubirFicheros helperPath)
        {
            this.repo = repo;
            this.helperArchivos = helperPath;
        }

        [AuthorizeUsers]
        public IActionResult MenuEquipo()
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());

            List<Equipo> equipos = this.repo.GetEquiposUser(idusuario);

            if (equipos.Count == 0)
            {
                return View();
            }
            else
            {
                ViewData["JUGADORESUSUARIO"] = this.repo.JugadoresXUsuario(idusuario);
                return View(equipos);
            }

        }

        [AuthorizeUsers]
        public IActionResult _PartialVistaEquipo(int idequipo)
        {
            return PartialView(this.repo.GetEquipo(idequipo));
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

            string extension = System.IO.Path.GetExtension(imagen.FileName);

            if (extension == ".png")
            {
                string path = await this.helperArchivos.UploadFileAsync(imagen, nombre.ToLower());
                await this.repo.InsertEquipo(idusuario, nombre, path);
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
