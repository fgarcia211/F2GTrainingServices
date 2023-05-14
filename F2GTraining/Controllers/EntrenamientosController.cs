using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Mvc;
using F2GTraining.Services;

namespace F2GTraining.Controllers
{
    public class EntrenamientosController : Controller
    {
        private ServiceAPIF2GTraining service;

        public EntrenamientosController(ServiceAPIF2GTraining service)
        {
            this.service = service;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ListaSesiones(int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            Equipo equipo = await this.service.GetEquipo(token,idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                ViewData["IDEQUIPO"] = idequipo;
                ViewData["NOMBREEQUIPO"] = equipo.Nombre;
                return View(await this.service.GetEntrenamientosEquipo(token,idequipo));
            }

        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> ListaSesiones(int idequipo, string nombre)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            Equipo equipo = await this.service.GetEquipo(token,idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                await this.service.InsertEntrenamiento(token,idequipo, nombre);
                return RedirectToAction("ListaSesiones", new { idequipo = idequipo });
            }

        }

        [AuthorizeUsers]
        public async Task<IActionResult> EliminaEntrenamiento(int identrenamiento, int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            Equipo equipo = await this.service.GetEquipo(token,idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                await this.service.DeleteEntrenamiento(token,identrenamiento);
                return RedirectToAction("ListaSesiones", new { idequipo = idequipo });
            }

        }

        [AuthorizeUsers]
        public async Task<IActionResult> VistaEntrenamiento(int identrenamiento, int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            Equipo equipo = await this.service.GetEquipo(token,idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

            Entrenamiento entrena = await this.service.GetEntrenamiento(token,identrenamiento);
            List<Jugador> jugadoresequipo;

            if (entrena.Activo == false && entrena.FechaFin == null)
            {
                jugadoresequipo = await this.service.GetJugadoresEquipo(token,idequipo);
            }
            else if (entrena.Activo == false && entrena.FechaFin != null)
            {
                jugadoresequipo = await this.service.GetJugadoresXSesion(token,identrenamiento);
                List<JugadorEntrenamiento> notas = await this.service.GetNotasSesion(token,identrenamiento);
                ViewData["NOTAS"] = notas;
            }
            else
            {
                jugadoresequipo = await this.service.GetJugadoresXSesion(token,identrenamiento);
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
            string token = HttpContext.Session.GetString("TOKEN");

            Equipo equipo = await this.service.GetEquipo(token,idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

            Entrenamiento entrena = await this.service.GetEntrenamiento(token,identrenamiento);
            List<Jugador> jugadoresequipo;

            if (entrena.Activo == false)
            {
                //AQUI VA EL CODIGO PARA AÑADIRLO A LA AUXILIAR DE ENTRENAMIENTOJUGADOR
                await this.service.AniadirJugadoresSesion(token,identrenamiento,seleccionados);
                jugadoresequipo = await this.service.GetJugadoresXSesion(token,identrenamiento);
            }
            else 
            {
                jugadoresequipo = await this.service.GetJugadoresXSesion(token,identrenamiento);
                //AQUI HAY QUE HACER PROCEDURE PARA ASIGNAR NOTAS A CADA JUGADOR APUNTADO
                await this.service.AniadirPuntuacionesEntrenamiento(token, identrenamiento, seleccionados, valoraciones);

                List<JugadorEntrenamiento> notas = await this.service.GetNotasSesion(token,identrenamiento);
                ViewData["NOTAS"] = notas;
            }

            ViewData["JUGADORES"] = jugadoresequipo;
            ViewData["IDEQUIPO"] = idequipo;
            ViewData["IDENTRENAMIENTO"] = identrenamiento;

            return RedirectToAction("VistaEntrenamiento", new { idequipo = idequipo, identrenamiento = identrenamiento });

        }
    }
}
