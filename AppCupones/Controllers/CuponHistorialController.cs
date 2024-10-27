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
    public class CuponHistorialController(DbAppContext context) : BaseController<CuponHistorialModel>(context)
    {
        protected override bool Any(int id) => _context.Cupones_Historial.Any(e => e.Id_Cupon == id);

        public override async Task<IActionResult> Add(CuponHistorialModel model)
        {
            model.Cliente = null;
            model.FechaUso = DateTime.Now;
            try
            {
                var entityEntry = await _context.Cupones_Historial.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponHistorial.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        public override async Task<ActionResult<IEnumerable<CuponHistorialModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Cupones_Historial.ToListAsync();
                Log.Information("Se llamo al endpoint <CuponHistorial.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpGet("GetByNroCupon/{NroCupon}")]
        public async Task<ActionResult<IEnumerable<CuponHistorialModel>>> GetByNroCupon(string NroCupon) {
            try
            {
                var tc = await _context.Cupones_Historial.Where(x=>x.NroCupon==NroCupon).ToListAsync();
                Log.Information($"Se llamo al endpoint <CuponHistorial.GetByNroCupon, {NroCupon}>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.GetByNroCupon, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpGet("GetByIdCupon/{Id_Cupon}")]
        public async Task<ActionResult<IEnumerable<CuponHistorialModel>>> GetByIdCupon(int Id_Cupon)
        {
            try
            {
                var tc = await _context.Cupones_Historial.Where(x => x.Id_Cupon == Id_Cupon).ToListAsync();
                Log.Information($"Se llamo al endpoint <CuponHistorial.GetByIdCupon, {Id_Cupon}>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.GetByIdCupon, {Id_Cupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [HttpGet("GetByCodCliente/{CodCliente}")]
        public async Task<ActionResult<IEnumerable<CuponHistorialModel>>> GetByCodCliente(string CodCliente)
        {
            try
            {
                var tc = await _context.Cupones_Historial.Where(x => x.CodCliente == CodCliente).ToListAsync();
                Log.Information($"Se llamo al endpoint <CuponHistorial.GetByCodCliente, {CodCliente}>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.GetByCodCliente, {CodCliente}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> Update(CuponHistorialModel model) => NotFound("Endpoint no encontrado.");
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");
    }
}
