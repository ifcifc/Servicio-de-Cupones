using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using Common.Controllers;
using Common.Interfaces;
using Common.Models.DTO;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ClientesApi.Controllers
{
    public class ClienteController : BaseController<ClienteModel, DbAppContext>
    {
        private readonly EmailService emailService; 
        private IClienteService clienteService { get; set; }
        public ClienteController(DbAppContext context, EmailService emailService, IClienteService clienteService) : base(context) 
        {
            this.emailService = emailService;
            this.clienteService = clienteService;

        }

        public override async Task<IActionResult> Add(ClienteModel model)
        {
            try
            {
                var entityEntry = await _context.Clientes.AddAsync(model);
                await _context.SaveChangesAsync();

                //Tenia que usarlo para algo mas
                this.EnviarEmail(model.Email, "Registro", $"Bienvenido {model.Nombre_Cliente}");

                Log.Information($"Se llamo al endpoint <Cliente.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cliente.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        [HttpDelete("{CodCliente}")]
        public async Task<IActionResult> Delete(string CodCliente)
        {
            try
            {
                var tc = await _context.Clientes.FindAsync(CodCliente);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Cliente.Delete, {CodCliente}>: El cliente no existe");
                    return BadRequest("El cliente no existe");
                }

                _context.Clientes.Remove(tc);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cliente.Delete, {CodCliente}>");
                return Ok("Cliente eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cliente.Delete, {CodCliente}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<ActionResult<IEnumerable<ClienteModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Clientes.ToListAsync();
                Log.Information("Se llamo al endpoint <Cliente.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cliente.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        [HttpGet("{CodCliente}")]
        public async Task<IActionResult> GetByID(string CodCliente)
        {
            try
            {
                var tc = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.CodCliente == CodCliente);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Cliente.GetByID, {CodCliente}>: El cliente no existe");
                    return NotFound("El cliente no existe");
                }

                Log.Information($"Se llamo al endpoint <Cliente.GetByID, {CodCliente}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cliente.GetByID, {CodCliente}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<IActionResult> Update(ClienteModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <Cliente.Update>: No se proporciono un cliente");
                return BadRequest("No se proporciono un cliente");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.CodCliente);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <Cliente.Update, {model.ToString()}>: El cliente no existe");
                    return NotFound("El cliente no existe");
                }
                _context.Clientes.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cliente.Update, {model.ToString()}>");
                return Ok("Cliente modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cliente.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [HttpPost("ReclamarCupon")]
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
                this.EnviarEmail(clienteModel.Email, "Solicitud de cupon", $"Se le a dado el cupon: {NroCupon}");

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.EnviarSolicitudCupon, {clienteDTO.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpPost("UsarCupon/{NroCupon}")]
        public async Task<IActionResult> QuemarCupon(string NroCupon)
        {
            if (NroCupon.IsNullOrEmpty())
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: No se proporciono un NroCupon");
                return BadRequest("No se proporciono un NroCupon");
            }

            try
            {
                ClienteDTO clienteDTO = await this.clienteService.ObtenerCliente(NroCupon);
                ClienteModel? clienteModel = _context.Clientes.Find(clienteDTO.CodCliente);


                string result = await this.clienteService.QuemarCupon(NroCupon);
                Log.Information($"Se llamo al endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>");
               
                //Envio el email
                this.EnviarEmail(clienteModel.Email, "Quemar cupon", $"Se a quemado el cupon: {NroCupon}");
                
                

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpGet("ObtenerCuponesActivos/{CodCliente}")]
        public async Task<IActionResult> ObtenerCuponesActivos(string CodCliente)
        {

            if (CodCliente.IsNullOrEmpty())
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.ObtenerCuponesActivos, {CodCliente}>: No se proporciono un CodCliente");
                return BadRequest("No se proporciono un CodCliente");
            }

            try
            {
                if (!this.Any(CodCliente))
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.ObtenerCuponesActivos, {CodCliente.ToString()}>: El cliente no existe");
                    return NotFound("El cliente no existe");
                }

                var cc = await this.clienteService.ObtenerCuponesActivos(CodCliente);
                return Ok(cc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.ObtenerCuponesActivos, {CodCliente}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        protected override bool Any(int id) => false;

        private bool Any(string CodCliente) => _context.Clientes.Any(e => e.CodCliente==CodCliente);


        private async void EnviarEmail(string email, string subject, string body) 
        {
            try
            {
                //Envio el email
                await this.emailService.EnviarEmail(email, subject, body);
            }
            catch (Exception ex)
            {
                Log.Error($"Hubo un problema al enviar el email: {ex.Message}");
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_1/{Id}")]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_2/{Id}")]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");
    }
}
