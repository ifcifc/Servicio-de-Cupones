using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Models;
using Common.Controllers;
using Common.Models.DTO;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ClientesApi.Controllers
{
    public class ClienteController : BaseController<ClienteModel, DbAppContext>
    {
        private readonly EmailService emailService;
        public ClienteController(DbAppContext context, EmailService emailService) : base(context) 
        {
            this.emailService = emailService;
        }

        public override async Task<IActionResult> Add(ClienteModel model)
        {
            try
            {
                var entityEntry = await _context.Clientes.AddAsync(model);
                await _context.SaveChangesAsync();

                //Tenia que usarlo para algo mas
                await this.emailService.EnviarEmail(model.Email, "Registro", $"Bienvenido {model.Nombre_Cliente}");

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
                    return NotFound("El tipo de cliente no existe");
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
                Log.Error($"Error en el endpoint <Cliente.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
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
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cliente.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        

        protected override bool Any(int id) => false;

        private bool Any(string CodCliente) => _context.Clientes.Any(e => e.CodCliente==CodCliente);

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_1/{Id}")]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_2/{Id}")]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");
    }
}
