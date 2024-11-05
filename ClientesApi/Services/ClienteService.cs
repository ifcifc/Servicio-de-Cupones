﻿using ClientesApi.Interfaces;
using Common.Models.DTO;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;

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

        public async Task<string> SolicitarCupon(ClienteDTO clienteDTO)
        {
            var respuesta = await this.apiConnect.FromApi("Cupon/AsignarCupon", clienteDTO);

            var content = await respuesta.Content.ReadAsStringAsync();

            if (respuesta.IsSuccessStatusCode) 
            {
                return content;
            }

            throw new Exception(content);
        }

        public async Task<string> QuemarCupon(string NroCupon)
        {
            var respuesta = await this.apiConnect.FromApi($"Cupon/QuemarCupon/{NroCupon}");
            var content = await respuesta.Content.ReadAsStringAsync();

            if (respuesta.IsSuccessStatusCode)
            {
                return content;
            }

            throw new Exception(respuesta.ToString());
        }

        public async Task<ClienteDTO> ObtenerCliente(string NroCupon)
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

            throw new Exception(respuesta.ToString());
        }

    }
}
