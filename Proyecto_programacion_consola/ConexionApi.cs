using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Proyecto_programacion_consola.Models;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;

namespace Proyecto_Programacion
{
    public class TokenMaster
    {
        public static string Token { get; set; }

        public static string Error { get; set; }

    }

    public class ConexionApi
    {
        //TOKEN
        public async Task<TokenMaster> Token(DtoUsuario usuario)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var datos_usuario = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

            string Path = client.BaseAddress + "api/Autentificacion/Validar";
            HttpResponseMessage response = await client.PostAsync(Path, datos_usuario);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var token = jObject["access_token"].ToString();
                TokenMaster tokenMaster = new TokenMaster();
                TokenMaster.Token = token;
                return tokenMaster;
            }
            else
            {
                TokenMaster tokenMaster = new TokenMaster();
                TokenMaster.Error = "Usuario o contraseña incorrectos";
                return tokenMaster;
            }
        }
        //DUEÑOS
        public async Task<List<DtoDueñosPorID>> ObtenerDueñosxID(int idDueño)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);


            List<DtoDueñosPorID> lstDueño = null;
            string Path = client.BaseAddress + "api/Dueño/MostrarDueñoxID/" + idDueño;

            HttpResponseMessage response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var jArray = jObject["dueños"].ToObject<List<DtoDueñosPorID>>();
                lstDueño = jArray;
            }
            return lstDueño;
        }

        public async Task<List<DtoMostrarDueños>> ObtenerDueños()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            List<DtoMostrarDueños> lstDueño = null;
            string Path = client.BaseAddress + "api/Dueño/MostrarDueño";

            HttpResponseMessage response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var jArray = jObject["dueños"].ToObject<List<DtoMostrarDueños>>();
                lstDueño = jArray;
            }
            return lstDueño;
        }

        public async Task<DtoAgregarDueño> InsertarDueño(DtoAgregarDueño dueño_agregar)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var datos_dueño = new StringContent(JsonConvert.SerializeObject(dueño_agregar), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            string Path = client.BaseAddress + "api/Dueño/InsertarDueño";
            HttpResponseMessage response = await client.PostAsync(Path, datos_dueño);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<DtoAgregarDueño>(jsonString);
                return resultado;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var errors = jObject["errors"].First().ToString();
                return new DtoAgregarDueño
                {
                    Error = errors
                };
            }
        }

        public async Task<DtoActualizarDueño> ActualizarDueño(int idDueño,DtoActualizarDueño dueño_actualizar)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            var datos_dueño_actualizar = new StringContent(JsonConvert.SerializeObject(dueño_actualizar), Encoding.UTF8, "application/json");

            string Path = client.BaseAddress + $"api/Dueño/ActualizarDueño{idDueño}";
            HttpResponseMessage response = await client.PutAsync(Path, datos_dueño_actualizar);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<DtoActualizarDueño>(jsonString);
                return resultado;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var errors = jObject["errors"].First().ToString();
                return new DtoActualizarDueño
                {
                    Error = errors
                };
            }
        }

        public async Task<DtoEliminarDueño> EliminarDueño (int idDueño)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            string Path = client.BaseAddress + $"api/Dueño/EliminarDueño/{idDueño}";


            HttpResponseMessage response = await client.DeleteAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                return new DtoEliminarDueño();
            }
            else
            {
                throw new Exception($"Hubo un error al eliminar el dueño con ID {idDueño}. Código de estado HTTP: {response.StatusCode}");
            }
        }

        //MASCOTAS

        public async Task<DTOAgregarMascotas> InsertarMascotas(DTOAgregarMascotas mascotas)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            var datos_mascota = new StringContent(JsonConvert.SerializeObject(mascotas), Encoding.UTF8, "application/json");


            string Path = client.BaseAddress + "api/Mascotas/InsertarMascota";
            HttpResponseMessage response = await client.PostAsync(Path, datos_mascota);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<DTOAgregarMascotas>(jsonString);
                return resultado;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var errors = jObject["errors"].ToString();
                return new DTOAgregarMascotas
                {
                    Error_mascota = errors
                };
            }
        }

        public async Task<List<DtoMostrarMascotas>> MostrarMascota(int idDueño)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            List<DtoMostrarMascotas> lstMascota = null;
            string Path = client.BaseAddress + "api/Mascotas/MostrarMascota?idDueño=" + idDueño;

            HttpResponseMessage response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var jArray = jObject["mascotas"].ToObject<List<DtoMostrarMascotas>>();
                lstMascota = jArray;
            }

            return lstMascota;
        }

        public async Task<DtoActualizarMascotas> ActualizarMascotas(DtoActualizarMascotas mascotas_actualizar)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            var datos_mascota_actualizar = new StringContent(JsonConvert.SerializeObject(mascotas_actualizar), Encoding.UTF8, "application/json");


            string Path = client.BaseAddress + "api/Mascotas/ActualizarMascota";
            HttpResponseMessage response = await client.PutAsync(Path, datos_mascota_actualizar);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<DtoActualizarMascotas>(jsonString);
                return resultado;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var errors = jObject["errors"].ToString();
                return new DtoActualizarMascotas
                {
                    Error_mascota = errors
                };
            }
        }

        //CITAS

        public async Task<List<DtoMostrarCitas>> MostrarCitas()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            List<DtoMostrarCitas> lstCitas = null;
            string Path = client.BaseAddress + "api/Citas/MostrarCitas";

            HttpResponseMessage response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var jArray = jObject["citas"].ToObject<List<DtoMostrarCitas>>();
                lstCitas = jArray;
            }
            return lstCitas;
        }

        public async Task<DtoAgregarCita> AgregarCita(DtoAgregarCita citas)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            var datos_mascota = new StringContent(JsonConvert.SerializeObject(citas), Encoding.UTF8, "application/json");


            string Path = client.BaseAddress + "api/Citas/AgregarCita";
            HttpResponseMessage response = await client.PostAsync(Path, datos_mascota);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<DtoAgregarCita>(jsonString);
                return resultado;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var errors = jObject["errors"].ToString();
                return new DtoAgregarCita
                {
                    Error = errors
                };
            }
        }

        public async Task<DtoActualizarCita> ActualizarCita(DtoActualizarCita cita_actualizar)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMaster.Token);

            var datos_cita_actualizar = new StringContent(JsonConvert.SerializeObject(cita_actualizar), Encoding.UTF8, "application/json");


            string Path = client.BaseAddress + "api/Citas/ActualizarCitas";
            HttpResponseMessage response = await client.PutAsync(Path, datos_cita_actualizar);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<DtoActualizarCita>(jsonString);
                return resultado;
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonString);
                var errors = jObject["errors"].ToString();
                return new DtoActualizarCita
                {
                    Error = errors
                };
            }
        }


    }
}
