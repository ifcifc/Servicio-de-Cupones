using AppCupones.Data;
using AppCupones.Data;
using AppCupones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System.Runtime.InteropServices;

namespace AppCupones.Controllers
{
    public class CuponClienteController(DbAppContext context) : BaseController<CuponClienteModel>(context)
    {
        protected override bool Any(int id) => _context.Cupones_Clientes.Any(e => e.Id_Cupon == id);
        private bool Any(string NroCupon) => _context.Cupones_Clientes.Any(e => e.NroCupon == NroCupon);
        public override async Task<IActionResult> Add(CuponClienteModel model)
        {
            try
            {
                model.Cupon = null;
                model.FechaAsignado = DateTime.Now;
             
                var entityEntry = await _context.Cupones_Clientes.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponCliente.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCliente.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        
        public override async Task<ActionResult<IEnumerable<CuponClienteModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Cupones_Clientes
                    .Include(x => x.Cupon)
                    .ToListAsync();
                Log.Information("Se llamo al endpoint <CuponCliente.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCliente.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<IActionResult> Update(CuponClienteModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <CuponCliente.Update>: No se proporciono un cliente cupon");
                return BadRequest("No se proporciono un cliente cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.NroCupon);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <CuponCliente.Update, {model.ToString()}>: El cliente cupon no existe");
                    return NotFound("El cliente cupon no existe");
                }

                _context.Cupones_Clientes.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponCliente.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCliente.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpDelete("{NroCupon}")]
        public async Task<IActionResult> Delete(string NroCupon) {
            try
            {
                var tc = await _context.Cupones_Clientes.FindAsync(NroCupon);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponCliente.Delete, {NroCupon}>: El precio no existe");
                    return BadRequest("El precio no existe");
                }

                _context.Cupones_Clientes.Remove(tc);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponCliente.Delete, {NroCupon}>");
                return Ok("Cupon cliente eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCliente.Delete, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpGet("{NroCupon}")]
        public async Task<IActionResult> GetByNroCupon(string NroCupon) {
            try
            {
                var tc = await _context.Cupones_Clientes.AsNoTracking()
                    .Include(x => x.Cupon)
                    .FirstOrDefaultAsync(x => x.NroCupon == NroCupon);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponCliente.GetByID, {NroCupon}>: El precio no existe");
                    return NotFound("El tipo de cliente cupon no existe");
                }

                Log.Information($"Se llamo al endpoint <CuponCliente.GetByID, {NroCupon}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCliente.GetByID, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Borrado_1{NroCupon}")]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_2/{Id}")]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");
    }
}
