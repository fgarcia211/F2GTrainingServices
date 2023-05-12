using ModelsF2GTraining;

namespace F2GTraining.Repositories
{
    public interface IRepositoryF2GTraining
    {
        #region METODOSUSUARIOS
        Task InsertUsuario(string nombre, string correo, string contrasenia, int telefono);

        Usuario GetUsuarioNamePass(string nombre, string contrasenia);

        bool CheckTelefonoRegistro(int telefono);

        bool CheckUsuarioRegistro(string nombre);

        bool CheckCorreoRegistro(string correo);

        #endregion

        #region METODOSEQUIPOS
        Task InsertEquipo(int iduser, string nombre, string imagen);

        List<Equipo> GetEquiposUser(int iduser);

        Equipo GetEquipo(int idequipo);

        #endregion

        #region METODOSJUGADORES
        Task InsertJugador(int idequipo, int idposicion, string nombre, int dorsal, int edad, int peso, int altura);

        List<Posicion> GetPosiciones();

        Jugador GetJugadorID(int id);

        EstadisticaJugador GetEstadisticasJugador(int id);

        List<Jugador> GetJugadoresEquipo(int idequipo);

        Task DeleteJugador(int idjugador);

        List<Jugador> JugadoresXUsuario(int idusuario);

        List<Jugador> JugadoresXSesion(int identrenamiento);

        Task AniadirPuntuacionesEntrenamiento(List<int> idsjugador, List<int> valoraciones, int identrenamiento);

        Task AniadirJugadoresSesion(List<int> idsjugador, int identrenamiento);

        List<JugadorEntrenamiento> GetNotasSesion(int identrenamiento);

        #endregion

        #region METODOSENTRENAMIENTOS
        Task InsertEntrenamiento(int idequipo, string nombre);

        List<Entrenamiento> GetEntrenamientosEquipo(int idequipo);

        Entrenamiento GetEntrenamiento(int identrena);

        Task EmpezarEntrenamiento(int identrenamiento);

        Task FinalizarEntrenamiento(int identrenamiento);

        Task BorrarEntrenamiento(int identrenamiento);
        #endregion
    }
}
