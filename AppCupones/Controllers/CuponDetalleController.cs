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
    public class CuponDetalleController(DbAppContext context) : BaseController<CuponDetalleModel>(context)
    {
        protected override bool Any(int id) => _context.Cupones_Detalle.Any(e => e.Id_Cupon == id);

        public override async Task<IActionResult> Add(CuponDetalleModel model)
        {
            model.Articulo = null;
            //model.Id_Cupon = null;
            try
            {
                var entityEntry = await _context.Cupones_Detalle.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponDetalle.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        public override async Task<ActionResult<IEnumerable<CuponDetalleModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Cupones_Detalle.ToListAsync();
                Log.Information("Se llamo al endpoint <CuponDetalle.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [HttpGet("GetByIdCupon/{Id_Cupon}")]
        public async Task<ActionResult<IEnumerable<CuponDetalleModel>>> GetByIdCupon(int Id_Cupon)
        {
            try
            {
                var tc = await _context.Cupones_Detalle.Where(x => x.Id_Cupon == Id_Cupon).ToListAsync();
                Log.Information($"Se llamo al endpoint <CuponDetalle.GetByIdCupon, {Id_Cupon}>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.GetByIdCupon, {Id_Cupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> Update(CuponDetalleModel model) => NotFound("Endpoint no encontrado.");
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");
    }
}
