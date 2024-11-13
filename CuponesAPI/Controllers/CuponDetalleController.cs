using CuponesAPI.Data;
using CuponesAPI.Data;
using CuponesAPI.Models;
using Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System.Runtime.InteropServices;

namespace CuponesAPI.Controllers
{
    public class CuponDetalleController(DbAppContext context) : BaseController<CuponDetalleModel,DbAppContext>(context)
    {
        protected override bool Any(int id) => false;
        private bool Any(int Id_Cupon, int Id_Articulo) => _context.Cupones_Detalle.Any(x => x.Id_Cupon == Id_Cupon && x.Id_Articulo == Id_Articulo);

        public override async Task<IActionResult> Add(CuponDetalleModel model)
        {
            model.Articulo = null;
            //model.Id_Cupon = null;
            try
            {

                if (this.Any(model.Id_Cupon, model.Id_Articulo))
                {
                    Log.Error($"Error en el endpoint <CuponDetalle.Add, {model.ToString()}>: Ya existe un cupon detalle para este cupon");
                    return BadRequest($"Ya existe un cupon detalle para este cupon");
                }

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
                var tc = await _context.Cupones_Detalle
                    .Include(x => x.Articulo)
                    .ToListAsync();
                Log.Information("Se llamo al endpoint <CuponDetalle.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [HttpGet("{Id_Cupon}")]
        public async Task<ActionResult<IEnumerable<CuponDetalleModel>>> GetByIdCupon(int Id_Cupon)
        {
            try
            {
                var tc = await _context.Cupones_Detalle.Where(x => x.Id_Cupon == Id_Cupon)
                    .Include(x => x.Articulo)
                    .ToListAsync();
                Log.Information($"Se llamo al endpoint <CuponDetalle.GetByIdCupon, {Id_Cupon}>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.GetByIdCupon, {Id_Cupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        //Inesperadamente funciona
        public override async Task<IActionResult> Update(CuponDetalleModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Cupon, model.Id_Articulo);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <CuponDetalle.Update, {model.ToString()}>: El tipo de cupon no existe");
                    return NotFound("El tipo de cupon no existe");
                }

                //var _model = await _context.Cupones_Detalle.FirstAsync(x => x.Id_Cupon == Id_Cupon && x.Id_Articulo == Id_Articulo);

                /*_model.Id_Articulo = model.Id_Articulo;
                _model.Id_Cupon = model.Id_Cupon;
                _model.Cantidad = model.Cantidad;*/

                _context.Cupones_Detalle.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponDetalle.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpDelete("{Id_Cupon}, {Id_Articulo}")]
        public async Task<IActionResult> Delete(int Id_Cupon, int Id_Articulo)
        {
            try
            {
                var tc = await _context.Cupones_Detalle.FirstOrDefaultAsync(x => x.Id_Cupon == Id_Cupon && x.Id_Articulo == Id_Articulo);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponDetalle.Delete, [{Id_Cupon}, {Id_Articulo}]>: El cupon detalle no existe");
                    return BadRequest("El tipo de cupon no existe");
                }

                _context.Cupones_Detalle.Remove(tc);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponDetalle.Delete, [{Id_Cupon}, {Id_Articulo}]>");
                return Ok("Cupon detalle eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponDetalle.Delete, [{Id_Cupon}, {Id_Articulo}]>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("Borrado_1/{Id}")]
        public override async Task<IActionResult> Delete(int Id) => NotFound("Endpoint no encontrado.");

        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<IActionResult> GetByID(int Id) => NotFound("Endpoint no encontrado.");
    }
}