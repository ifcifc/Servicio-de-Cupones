using CuponesAPI.Data;
using CuponesAPI.Data;
using CuponesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System.Runtime.InteropServices;
using Common.Controllers;

namespace CuponesAPI.Controllers
{
    public class CuponHistorialController(DbAppContext context) : BaseController<CuponHistorialModel, DbAppContext>(context)
    {
        protected override bool Any(int id) => _context.Cupones_Historial.Any(e => e.Id_Cupon == id);
        private bool Any(int Id_Cupon, string NroCupon) => _context.Cupones_Historial.Any(e => e.Id_Cupon == Id_Cupon && e.NroCupon == NroCupon);
        public override async Task<IActionResult> Add(CuponHistorialModel model)
        {
            //model.Cliente = null;
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
                var tc = await _context.Cupones_Historial
                    //.Include(x=>x.Cliente)
                    .ToListAsync();
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
                var tc = await _context.Cupones_Historial.Where(x => x.NroCupon == NroCupon)
                    //.Include(x => x.Cliente)
                    .ToListAsync();
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
                var tc = await _context.Cupones_Historial.Where(x => x.Id_Cupon == Id_Cupon)
                    //.Include(x => x.Cliente)
                    .ToListAsync();
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
                var tc = await _context.Cupones_Historial.Where(x => x.CodCliente == CodCliente)
                    //.Include(x => x.Cliente)
                    .ToListAsync();
                Log.Information($"Se llamo al endpoint <CuponHistorial.GetByCodCliente, {CodCliente}>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.GetByCodCliente, {CodCliente}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpGet("{Id_Cupon}, {NroCupon}")]
        public async Task<IActionResult> GetByID(int Id_Cupon, string NroCupon) 
        {
            try
            {
                var tc = await _context.Cupones_Historial.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Cupon == Id_Cupon && x.NroCupon==NroCupon);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponHistorial.GetByID, [{Id_Cupon}, {NroCupon}]>: El cupon historial no existe");
                    return NotFound("El cupon historial no existe");
                }

                Log.Information($"Se llamo al endpoint <CuponHistorial.GetByID, [{Id_Cupon}, {NroCupon}]>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.GetByID, [{Id_Cupon}, {NroCupon}]>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<IActionResult> Update(CuponHistorialModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.Update>: No se proporciono un cupon historial");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Cupon, model.NroCupon);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <CuponHistorial.Update, {model.ToString()}>: El historial no existe");
                    return NotFound("El historial no existe");
                }

                _context.Cupones_Historial.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponHistorial.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        [HttpDelete("{Id_Cupon}, {NroCupon}")]
        public async Task<IActionResult> Delete(int Id_Cupon, string NroCupon)
        {
            try
            {
                var tc = await _context.Cupones_Historial.FirstOrDefaultAsync(x => x.Id_Cupon == Id_Cupon && x.NroCupon == NroCupon);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponHistorial.Delete, [{Id_Cupon}, {NroCupon}]>: El cupon historial no existe");
                    return BadRequest("El cupon historial no existe");
                }

                _context.Cupones_Historial.Remove(tc);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponHistorial.Delete, [{Id_Cupon}, {NroCupon}]>");
                return Ok("Cupon historial eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponHistorial.Delete, [{Id_Cupon}, {NroCupon}]>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_1/{Id}")]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("Borrado_2/{Id}")]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");
    }
}
