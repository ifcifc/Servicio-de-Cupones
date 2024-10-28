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
        public override async Task<IActionResult> Add(CuponClienteModel model)
        {
            try
            {
                model.Cupon = null;
             
                var entityEntry = await _context.Cupones_Clientes.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <TipoCupon.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <TipoCupon.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        
        public override async Task<ActionResult<IEnumerable<CuponClienteModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Cupones_Clientes.ToListAsync();
                Log.Information("Se llamo al endpoint <TipoCupon.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <TipoCupon.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> Update(CuponClienteModel model) => NotFound("Endpoint no encontrado.");
    }
}
