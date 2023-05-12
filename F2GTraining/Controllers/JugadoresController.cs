using F2GTraining.Extensions;
using F2GTraining.Filters;
using ModelsF2GTraining;
using F2GTraining.Repositories;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace F2GTraining.Controllers
{
    public class JugadoresController : Controller
    {
        private IRepositoryF2GTraining repo;

        public JugadoresController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        [AuthorizeUsers]
        public IActionResult CrearJugador(int idequipo)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());

            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo == null || equipo.IdUsuario != idusuario)
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            else
            {
                ViewData["IDEQUIPO"] = equipo.IdEquipo;
                ViewData["NOMBRE"] = equipo.Nombre;
                return View(this.repo.GetPosiciones());
            }

        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> CrearJugador(int idequipo, int idposicion, string nombre, int dorsal, int edad, int peso, int altura)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());

            Equipo equipo = this.repo.GetEquipo(idequipo);

            if (equipo != null || equipo.IdUsuario == idusuario)
            {
                await this.repo.InsertJugador(idequipo, idposicion, nombre, dorsal, edad, peso, altura);
            }

            return RedirectToAction("MenuEquipo", "Equipos");

        }

        [AuthorizeUsers]
        public async Task<IActionResult> DeleteJugador(int idjugador)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            List<Jugador> jugadoresUser = this.repo.JugadoresXUsuario(idusuario);

            if (jugadoresUser.Contains(this.repo.GetJugadorID(idjugador)))
            {
                await this.repo.DeleteJugador(idjugador);
            }

            return RedirectToAction("MenuEquipo", "Equipos");

        }

        [AuthorizeUsers]
        public IActionResult GraficaJugador(int idjugador)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());
            List<Jugador> jugadoresUser = this.repo.JugadoresXUsuario(idusuario);
            Jugador jugMostrar = this.repo.GetJugadorID(idjugador);

            if (jugadoresUser.Contains(jugMostrar))
            {
                EstadisticaJugador stats = this.repo.GetEstadisticasJugador(idjugador);
                ViewData["ESTADISTICAS"] = stats;
                return View(jugMostrar);
                /*return new ViewAsPdf
                {
                    Model = jugMostrar
                };*/
            }
            else
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }

        }

        [AuthorizeUsers]
        public IActionResult _PartialJugadoresEquipo(int idequipo)
        {
            return PartialView(this.repo.GetJugadoresEquipo(idequipo));
        }

        public IActionResult GraficaComoPDF(int idjugador)
        {
            int idusuario = int.Parse(HttpContext.User.FindFirst("IDUSUARIO").Value.ToString());

            List<Jugador> jugadoresUser = this.repo.JugadoresXUsuario(idusuario);
            Jugador jug = this.repo.GetJugadorID(idjugador);

            if (jugadoresUser.Contains(jug))
            {
                Equipo equipo = this.repo.GetEquipo(jug.IdEquipo);
                EstadisticaJugador stats = this.repo.GetEstadisticasJugador(idjugador);
                PDFJugador modeloPDF = new PDFJugador
                {
                    Jugador = jug,
                    Estadisticas = stats,
                    EquipoJugador = equipo
                };

                return new ViewAsPdf(modeloPDF);
            }
            else
            {
                return RedirectToAction("MenuEquipo", "Equipos");
            }
            
        }
    }
}
