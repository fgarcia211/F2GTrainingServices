using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using F2GTraining.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace F2GTraining.Controllers
{
    public class EntrenamientosController : Controller
    {
        private IRepositoryF2GTraining repo;

        public EntrenamientosController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        [AuthorizeUsers]
        public IActionResult ListaSesiones(int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                ViewData["IDEQUIPO"] = idequipo;
                ViewData["NOMBREEQUIPO"] = equipo.Nombre;
                return View(this.repo.GetEntrenamientosEquipo(idequipo));
            }

        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> ListaSesiones(int idequipo, string nombre)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                await this.repo.InsertEntrenamiento(idequipo, nombre);
                return RedirectToAction("ListaSesiones", new { idequipo = idequipo });
            }

        }

        [AuthorizeUsers]
        public async Task<IActionResult> EliminaEntrenamiento(int identrenamiento, int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                await this.repo.BorrarEntrenamiento(identrenamiento);
                return RedirectToAction("ListaSesiones", new { idequipo = idequipo });
            }

        }

        [AuthorizeUsers]
        public IActionResult VistaEntrenamiento(int identrenamiento, int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString()); 
            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

            Entrenamiento entrena = this.repo.GetEntrenamiento(identrenamiento);
            List<Jugador> jugadoresequipo;

            if (entrena.Activo == false && entrena.FechaFin == null)
            {
                jugadoresequipo = this.repo.GetJugadoresEquipo(idequipo);
            }
            else if (entrena.Activo == false && entrena.FechaFin != null)
            {
                jugadoresequipo = this.repo.JugadoresXSesion(identrenamiento);
                List<JugadorEntrenamiento> notas = this.repo.GetNotasSesion(identrenamiento);
                ViewData["NOTAS"] = notas;
            }
            else
            {
                jugadoresequipo = this.repo.JugadoresXSesion(identrenamiento);
            }
            
            ViewData["JUGADORES"] = jugadoresequipo;
            ViewData["IDEQUIPO"] = idequipo;
            ViewData["IDENTRENAMIENTO"] = identrenamiento;

            return View(entrena);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> VistaEntrenamiento(int identrenamiento, int idequipo, List<int> seleccionados, List<int> valoraciones)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString()); 
            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

            Entrenamiento entrena = this.repo.GetEntrenamiento(identrenamiento);
            List<Jugador> jugadoresequipo;

            if (entrena.Activo == false)
            {
                //AQUI VA EL CODIGO PARA AÑADIRLO A LA AUXILIAR DE ENTRENAMIENTOJUGADOR
                await this.repo.AniadirJugadoresSesion(seleccionados, identrenamiento);
                //AQUI VA EL CODIGO PARA QUE LA SESION SE INICIE
                await this.repo.EmpezarEntrenamiento(identrenamiento);

                jugadoresequipo = this.repo.JugadoresXSesion(identrenamiento);
            }
            else 
            {
                jugadoresequipo = this.repo.JugadoresXSesion(identrenamiento);
                //AQUI HAY QUE HACER PROCEDURE PARA ASIGNAR NOTAS A CADA JUGADOR APUNTADO
                await this.repo.AniadirPuntuacionesEntrenamiento(seleccionados, valoraciones, identrenamiento);
                //AQUI VA EL CODIGO PARA QUE LA SESION SE ACABE
                await this.repo.FinalizarEntrenamiento(identrenamiento);

                List<JugadorEntrenamiento> notas = this.repo.GetNotasSesion(identrenamiento);
                ViewData["NOTAS"] = notas;
            }

            ViewData["JUGADORES"] = jugadoresequipo;
            ViewData["IDEQUIPO"] = idequipo;
            ViewData["IDENTRENAMIENTO"] = identrenamiento;

            return RedirectToAction("VistaEntrenamiento", new { idequipo = idequipo, identrenamiento = identrenamiento });

        }
    }
}
