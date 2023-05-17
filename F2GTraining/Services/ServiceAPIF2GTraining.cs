using F2GTraining.Models;
using ModelsF2GTraining;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace F2GTraining.Services
{
    public class ServiceAPIF2GTraining
    {
        private MediaTypeWithQualityHeaderValue Header;
        private ServiceStorageBlobs serviceazure;
        private string UrlApiF2G;

        public ServiceAPIF2GTraining(IConfiguration configuration, ServiceStorageBlobs serviceazure)
        {
            this.UrlApiF2G = configuration.GetValue<string>("ServicesAzure:APIF2G");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.serviceazure = serviceazure;
        }

        #region METODOSGENERICOS
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<HttpStatusCode> InsertApiAsync<T>(string request, T objeto)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                return response.StatusCode;
            }
        }

        private async Task<HttpStatusCode> InsertApiAsync<T>(string request, T objeto, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                return response.StatusCode;
            }
        }

        private async Task<HttpStatusCode> DeleteApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);
                return response.StatusCode;
            }
        }

        private async Task<HttpStatusCode> PutApiAsync<T>(string request, T objeto, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
                return response.StatusCode;
            }
        }

        #endregion

        #region METODOSUSUARIOS
        public async Task InsertUsuario(Usuario user)
        {
            string request = "/api/Usuarios";
            HttpStatusCode response = await this.InsertApiAsync<Usuario>(request,user);
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Usuarios/Login";
                client.BaseAddress = new Uri(this.UrlApiF2G);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    username = username,
                    password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token = jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<Usuario> GetUsuarioAsync(string token)
        {
            string request = "/api/Usuarios/GetUsuarioLogueado";
            return await this.CallApiAsync<Usuario>(request,token);
        }
        public async Task<bool> CompruebaTelefono(int telefono)
        {
            string request = "/api/Usuarios/TelefonoRegistrado/" + telefono;
            return await this.CallApiAsync<bool>(request);
        }

        public async Task<bool> CompruebaNombre(string nombre)
        {
            string request = "/api/Usuarios/NombreRegistrado/" + nombre;
            return await this.CallApiAsync<bool>(request);
        }

        public async Task<bool> CompruebaCorreo(string correo)
        {
            string request = "/api/Usuarios/CorreoRegistrado/" + correo;
            return await this.CallApiAsync<bool>(request);
        }
        #endregion

        #region METODOSEQUIPOS
        public async Task InsertEquipo(string containerName, string blobName, Stream stream, string nombre, string token)
        {
            await this.serviceazure.UploadBlobAsync(containerName, blobName, stream);
            string imagen =  await this.serviceazure.GetBlobUriAsync(containerName, blobName);

            string request = "/api/Equipos";

            EquipoModel model = new EquipoModel
            {
                nombre = nombre,
                imagen = imagen
            };

            HttpStatusCode response = await this.InsertApiAsync<EquipoModel>(request,model,token);
        }

        public async Task<List<Equipo>> GetEquiposUser(string token)
        {
            string request = "/api/Equipos/GetEquiposUser";
            return await this.CallApiAsync<List<Equipo>>(request, token);
        }

        public async Task<Equipo> GetEquipo(string token, int idequipo)
        {
            string request = "/api/Equipos/GetEquipo/" + idequipo;
            return await this.CallApiAsync<Equipo>(request, token);
        }
        #endregion

        #region METODOSJUGADORES
        public async Task InsertJugador(string token, Jugador jugador)
        {
            string request = "/api/Jugadores";
            HttpStatusCode response = await this.InsertApiAsync<Jugador>(request, jugador, token);
        }

        public async Task<List<Posicion>> GetPosiciones()
        {
            string request = "/api/Jugadores/GetPosiciones";
            return await this.CallApiAsync<List<Posicion>>(request);
        }

        public async Task<Jugador> GetJugadorID(string token, int idjugador)
        {
            string request = "/api/Jugadores/GetJugadorID/" + idjugador;
            return await this.CallApiAsync<Jugador>(request, token);
        }

        public async Task<EstadisticaJugador> GetEstadisticasJugador(string token, int idjugador)
        {
            string request = "/api/Jugadores/GetEstadisticasJugador/" + idjugador;
            return await this.CallApiAsync<EstadisticaJugador>(request, token);
        }

        public async Task<List<Jugador>> GetJugadoresEquipo(string token, int idequipo)
        {
            string request = "/api/Jugadores/GetJugadoresEquipo/" + idequipo;
            return await this.CallApiAsync<List<Jugador>>(request, token);
        }

        public async Task<List<Jugador>> GetJugadoresUsuario(string token)
        {
            string request = "/api/Jugadores/JugadoresXUsuario";
            return await this.CallApiAsync<List<Jugador>>(request, token);
        }

        public async Task DeleteJugador(string token, int idjugador)
        {
            string request = "/api/Jugadores/DeleteJugador/" + idjugador;
            HttpStatusCode response = await this.DeleteApiAsync<Jugador>(request, token);
        }
        #endregion

        #region METODOSENTRENAMIENTOS
        public async Task InsertEntrenamiento(string token, int idequipo, string nombre)
        {
            string request = "/api/Entrenamientos/" + idequipo + "/" + nombre;
            HttpStatusCode response = await this.InsertApiAsync<Entrenamiento>(request, null, token);
        }

        public async Task AniadirJugadoresSesion(string token, int identrena, List<int> idsjugador)
        {
            string request = "/api/Entrenamientos/AniadirJugadoresSesion/" + identrena + "?";
            foreach (int id in idsjugador)
            {
                request += "idsjugador="+id+"&";
            }
            ;
            HttpStatusCode response = await this.InsertApiAsync<JugadorEntrenamiento>(request.Trim('&'), null,token);
        }

        public async Task AniadirPuntuacionesEntrenamiento(string token, int identrena, List<int> idsjugador, List<int> valoraciones)
        {
            string request = "/api/Entrenamientos/AniadirPuntuacionesEntrenamiento/" + identrena + "?";
            foreach (int id in idsjugador)
            {
                request += "idsjugador=" + id + "&";
            }
            foreach (int val in valoraciones)
            {
                request += "valoraciones=" + val + "&";
            }
            HttpStatusCode response = await this.PutApiAsync<JugadorEntrenamiento>(request.Trim('&'), null, token);
        }

        public async Task<List<Entrenamiento>> GetEntrenamientosEquipo(string token, int idequipo)
        {
            string request = "/api/Entrenamientos/GetEntrenamientosEquipo/" + idequipo;
            List<Entrenamiento> entrenamientos = await this.CallApiAsync<List<Entrenamiento>>(request, token);
            entrenamientos.Reverse();

            return entrenamientos;
        }

        public async Task<Entrenamiento> GetEntrenamiento(string token, int identrena)
        {
            string request = "/api/Entrenamientos/GetEntrenamiento/" + identrena;
            return await this.CallApiAsync<Entrenamiento>(request, token);
        }
        
        public async Task<List<Jugador>> GetJugadoresXSesion(string token, int identrena)
        {
            string request = "/api/Entrenamientos/JugadoresXSesion/" + identrena;
            return await this.CallApiAsync<List<Jugador>>(request, token);
        }

        public async Task<List<JugadorEntrenamiento>> GetNotasSesion(string token, int identrena)
        {
            string request = "/api/Entrenamientos/GetNotasSesion/" + identrena;
            return await this.CallApiAsync<List<JugadorEntrenamiento>>(request, token);
        }

        public async Task DeleteEntrenamiento(string token, int identrena)
        {
            string request = "/api/Entrenamientos/" + identrena;
            HttpStatusCode response = await this.DeleteApiAsync<Entrenamiento>(request, token);
        }
        #endregion
    }
}
