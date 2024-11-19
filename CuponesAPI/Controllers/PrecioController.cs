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
    public class PrecioController(DbAppContext context) : BaseController<PrecioModel, DbAppContext>(context)
    {
        protected override bool Any(int id) => _context.Precios.Any(e => e.Id_Precio == id);
        public override async Task<IActionResult> Add(PrecioModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <Precio.Add>: No se proporciono un modelo");
                return BadRequest("No se proporciono un modelo");
            }

            model.Id_Precio = 0;
            model.Articulo = null;

            try
            {
                /*//Remueve el precio para el articulo si ya existe
                if (_context.Precios.Any(x => x.Id_Articulo == model.Id_Articulo))
                {
                    var p = await _context.Precios.FirstAsync(x => x.Id_Articulo == model.Id_Articulo);
                    _context.Precios.Remove(p);
                }*/

                await _context.Precios.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Precio.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Precio.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var tc = await _context.Precios.FindAsync(Id);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Precio.Delete, {Id}>: El precio no existe");
                    return BadRequest("El precio no existe");
                }

                //_context.Precios.Remove(tc);

                tc.Precio = 0;

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Precio.Delete, {Id}>");
                return Ok("Precio eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Precio.Delete, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<ActionResult<IEnumerable<PrecioModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Precios
                    .Include(x=>x.Articulo)
                    .ToListAsync();
                Log.Information("Se llamo al endpoint <Precio.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Precio.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var tc = await _context.Precios.AsNoTracking()
                    .Include(x => x.Articulo)
                    .FirstOrDefaultAsync(x => x.Id_Precio == Id);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Precio.GetByID, {Id}>: El precio no existe");
                    return NotFound("El precio no existe");
                }

                Log.Information($"Se llamo al endpoint <Precio.GetByID, {Id}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Precio.GetByID, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Update(PrecioModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <Precio.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Precio);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <Precio.Update, {model.ToString()}>: El precio no existe");
                    return NotFound("El precio no existe");
                }

                model.Articulo = null;

                _context.Precios.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Precio.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Precio.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
    }
}
