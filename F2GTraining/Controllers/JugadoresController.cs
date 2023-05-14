using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using F2GTraining.Services;

namespace F2GTraining.Controllers
{
    public class JugadoresController : Controller
    {
        private ServiceAPIF2GTraining service;

        public JugadoresController(ServiceAPIF2GTraining service)
        {
            this.service = service;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> CrearJugador(int idequipo)
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
                ViewData["IDEQUIPO"] = equipo.IdEquipo;
                ViewData["NOMBRE"] = equipo.Nombre;
                return View(await this.service.GetPosiciones());
            }

        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> CrearJugador(int idequipo, int idposicion, string nombre, int dorsal, int edad, int peso, int altura)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            string token = HttpContext.Session.GetString("TOKEN");

            Equipo equipo = await this.service.GetEquipo(token,idequipo);

            if (equipo != null || equipo.IdUsuario == idusuario)
            {
                Jugador jug = new Jugador
                {
                    IdEquipo = idequipo,
                    IdPosicion = idposicion,
                    Nombre = nombre,
                    Dorsal = dorsal,
                    Edad = edad,
                    Peso = peso,
                    Altura = altura
                };

                await this.service.InsertJugador(token, jug);
            }

            return RedirectToAction("MenuEquipo", "Equipos");

        }

        [AuthorizeUsers]
        public async Task<IActionResult> DeleteJugador(int idjugador)
        {
            string token = HttpContext.Session.GetString("TOKEN");

            List<Jugador> jugadoresUser = await this.service.GetJugadoresUsuario(token);
            Jugador jugBorrar = await this.service.GetJugadorID(token, idjugador);

            foreach (Jugador jugador in jugadoresUser)
            {
                if (jugador.IdJugador == jugBorrar.IdJugador)
                {
                    await this.service.DeleteJugador(token, idjugador);
                }
            }

            return RedirectToAction("MenuEquipo", "Equipos");

        }

        [AuthorizeUsers]
        public async Task<IActionResult> GraficaJugador(int idjugador)
        {
            string token = HttpContext.Session.GetString("TOKEN");

            List<Jugador> jugadoresUser = await this.service.GetJugadoresUsuario(token);
            Jugador jugMostrar = await this.service.GetJugadorID(token,idjugador);

            foreach (Jugador jugador in jugadoresUser)
            {
                if (jugador.IdJugador == jugMostrar.IdJugador)
                {
                    EstadisticaJugador stats = await this.service.GetEstadisticasJugador(token, idjugador);
                    ViewData["ESTADISTICAS"] = stats;
                    return View(jugMostrar);
                }
            }
            
            return RedirectToAction("MenuEquipo", "Equipos");

        }

        [AuthorizeUsers]
        public async Task<IActionResult> _PartialJugadoresEquipo(int idequipo)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            return PartialView(await this.service.GetJugadoresEquipo(token,idequipo));
        }

        public async Task<IActionResult> GraficaComoPDF(int idjugador)
        {
            string token = HttpContext.Session.GetString("TOKEN");

            List<Jugador> jugadoresUser = await this.service.GetJugadoresUsuario(token);
            Jugador jugGrafica = await this.service.GetJugadorID(token,idjugador);

            foreach (Jugador jugador in jugadoresUser)
            {
                if (jugador.IdJugador == jugGrafica.IdJugador)
                {
                    Equipo equipo = await this.service.GetEquipo(token, jugGrafica.IdEquipo);
                    EstadisticaJugador stats = await this.service.GetEstadisticasJugador(token, idjugador);
                    PDFJugador modeloPDF = new PDFJugador
                    {
                        Jugador = jugGrafica,
                        Estadisticas = stats,
                        EquipoJugador = equipo
                    };

                    return new ViewAsPdf(modeloPDF);
                }
            }

            return RedirectToAction("MenuEquipo", "Equipos");        
        }
    }
}
