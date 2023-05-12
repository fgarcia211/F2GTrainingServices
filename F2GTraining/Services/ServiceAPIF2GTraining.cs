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
        private string UrlApiF2G;

        public ServiceAPIF2GTraining(IConfiguration configuration)
        {
            this.UrlApiF2G = configuration.GetValue<string>("ServicesAzure:APIF2G");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
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
        public async Task InsertEquipo(string containerName, string blobName, Stream stream)
        {
            //CODIGO PARA EL BLOB STORAGE
            //FIN CODIGO BLOB STORAGE

            //SOLICITUD DEL REQUEST
        }
        #endregion

        #region METODOSJUGADORES
        #endregion

        #region METODOSENTRENAMIENTOS
        #endregion
    }
}
