using CuponesAPI.Data;
using CuponesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Common.Controllers;
namespace CuponesAPI.Controllers
{
    public class ArticuloController(DbAppContext context) : BaseController<ArticuloModel, DbAppContext>(context)
    {
        protected override bool Any(int id) => _context.Articulos.Any(e => e.Id_Articulo == id);
        public override async Task<IActionResult> Add(ArticuloModel model)
        {
            model.Id_Articulo = 0;

            try
            {
                var entityEntry = await _context.Articulos.AddAsync(model);

                await _context.SaveChangesAsync();

                await _context.Precios.AddAsync(new PrecioModel()
                {
                    Id_Articulo = model.Id_Articulo,
                    Precio = 0
                }) ;

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Articulo.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Articulo.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var tc = await _context.Articulos.FindAsync(Id);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Articulo.Delete, {Id}>: El articulo no existe");
                    return BadRequest("El articulo no existe");
                }

                var precio = await _context.Precios.Where(x => x.Id_Articulo == Id).FirstAsync();

                precio.Precio = 0;

                tc.Activo = false;

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Articulo.Delete, {Id}>");
                return Ok("Articulo eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Articulo.Delete, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<ActionResult<IEnumerable<ArticuloModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Articulos.ToListAsync();
                Log.Information("Se llamo al endpoint <Articulo.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Articulo.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var tc = await _context.Articulos.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Articulo == Id);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Articulo.GetByID, {Id}>: El articulo no existe");
                    return NotFound("El articulo no existe");
                }

                Log.Information($"Se llamo al endpoint <Articulo.GetByID, {Id}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Articulo.GetByID, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Update(ArticuloModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <Articulo.Update>: No se proporciono un articulo");
                return BadRequest("No se proporciono un articulo");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool articuloExiste = this.Any(model.Id_Articulo);
                if (!articuloExiste)
                {
                    Log.Error($"Error en el endpoint <Articulo.Update, {model.ToString()}>: El articulo no existe");
                    return NotFound("El articulo no existe");
                }
                _context.Articulos.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Articulo.Update, {model.ToString()}>");
                return Ok("Articulo modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Articulo.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
    }
}
