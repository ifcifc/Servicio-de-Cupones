using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using Common.Models.DTO;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ClientesApi.Controllers
{
    public class SolicitudCuponesController : ControllerBase
    {
        protected readonly DbAppContext _context; 
        private readonly EmailService emailService;

        private IClienteService clienteService { get; set; }

        public SolicitudCuponesController(DbAppContext context, IClienteService clienteService, EmailService emailService)
        {
            _context = context;
            this.clienteService = clienteService;
            this.emailService = emailService;
        }

        [HttpPost("EnviarSolicitudCupon")]
        public async Task<IActionResult> EnviarSolicitudCupon([FromBody] ClienteDTO clienteDTO)
        {
            if (clienteDTO == null)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.EnviarSolicitudCupon>: No se proporciono un ClienteDTO");
                return BadRequest("No se proporciono un ClienteDTO");
            }

            try
            {
                if (!this.Any(clienteDTO.CodCliente))
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.EnviarSolicitudCupon, {clienteDTO.ToString()}>: El cliente no existe");
                    return NotFound("El cliente no existe");
                }

                string respuesta = await this.clienteService.SolicitarCupon(clienteDTO);
                Log.Information($"Se llamo al endpoint <SolicitudCupones.EnviarSolicitudCupon, {clienteDTO.ToString()}>");

                //Obtengo el cliente
                ClienteModel? clienteModel = _context.Clientes.Find(clienteDTO.CodCliente);

                //convierto la respuesta en un JObject para obtener el NroCupon
                JObject jobj = JObject.Parse(respuesta);
                var NroCupon = jobj["nroCupon"];

                //Envio el email
                await this.emailService.EnviarEmail(clienteModel.Email, "Solicitud de cupon", $"Se le a dado el cupon: {NroCupon}");

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.EnviarSolicitudCupon, {clienteDTO.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpPost("QuemarCupon/{NroCupon}")]
        public async Task<IActionResult> QuemarCupon(string NroCupon)
        {
            if (NroCupon.IsNullOrEmpty())
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: No se proporciono un ClienteDTO");
                return BadRequest("No se proporciono un ClienteDTO");
            }

            try
            {
                ClienteDTO clienteDTO = await this.clienteService.ObtenerCliente(NroCupon);
                ClienteModel? clienteModel = _context.Clientes.Find(clienteDTO.CodCliente);


                string result = await this.clienteService.QuemarCupon(NroCupon);
                Log.Information($"Se llamo al endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>");

                //Envio el email
                await this.emailService.EnviarEmail(clienteModel.Email, "Quemar cupon", $"Se a quemado el cupon: {NroCupon}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        private bool Any(string CodCliente) => _context.Clientes.Any(e => e.CodCliente == CodCliente);

    }
}
