using ClientesApi.Interfaces;
using Common.Models.DTO;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Formatting.Json;

namespace ClientesApi.Services
{
    public class ClienteService : IClienteService
    {
        private ApiConnectService apiConnect {get;set;}

        //Un servicio puede recibir otro servicio
        public ClienteService(ApiConnectService apiConnect)
        {
            this.apiConnect = apiConnect;
        }

        public async Task<HttpResponseMessage> SolicitarCupon(ClienteDTO clienteDTO)
        {
            var respuesta = await this.apiConnect.FromApi("SolicitudCupones/AsignarCupon", clienteDTO);

            //var content = await respuesta.Content.ReadAsStringAsync();

            /*if (respuesta.IsSuccessStatusCode) 
            {*/
                return respuesta;
            /*}

            throw new Exception($"QuemarCupon: {clienteDTO.ToString()}, {respuesta.ToString()}");*/
        }

        public async Task<HttpResponseMessage> QuemarCupon(string NroCupon)
        {
            var respuesta = await this.apiConnect.FromApi($"SolicitudCupones/QuemarCupon/{NroCupon}");

            return respuesta;
        }

        public async Task<ClienteDTO?> ObtenerCliente(string NroCupon)
        {
            var respuesta = await this.apiConnect.FromApiGet($"CuponCliente/{NroCupon}");

            var content = await respuesta.Content.ReadAsStringAsync();

            if (respuesta.IsSuccessStatusCode)
            {

                //convierto la respuesta en un JObject
                JObject jobj = JObject.Parse(content);
                
                return new ClienteDTO() { 
                    Id_Cupon = (int)jobj["id_Cupon"],
                    CodCliente = (string)jobj["codCliente"]
                };
            }

            return null;
        }

        public async Task<string> ObtenerCuponesActivos(string CodCliente) 
        {
            var respuesta = await this.apiConnect.FromApiGet($"CuponCliente/GetAllByCodCliente/{CodCliente}");

            var content = await respuesta.Content.ReadAsStringAsync();
            if (respuesta.IsSuccessStatusCode)
            {
                //Para que el json que devuelve sea legible
                JArray jsonFormatted = JArray.Parse(content);
                string prettyJson = jsonFormatted.ToString(Newtonsoft.Json.Formatting.Indented);
                return prettyJson;
            }
            return content;
        }

    }
}
