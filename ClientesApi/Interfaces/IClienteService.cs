
using Common.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ClientesApi.Interfaces
{
    public interface IClienteService
    {
        Task<HttpResponseMessage> SolicitarCupon(ClienteDTO clienteDTO);

        Task<HttpResponseMessage> QuemarCupon(string NroCupon);

        Task<ClienteDTO?> ObtenerCliente(string NroCupon);
        Task<string> ObtenerCuponesActivos(string CodCliente);
    }
}
