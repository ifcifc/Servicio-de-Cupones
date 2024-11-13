using CuponesAPI.Data;
using CuponesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Common.Controllers;

namespace CuponesAPI.Controllers
{
    public class CategoriaController(DbAppContext context) : BaseController<CategoriaModel, DbAppContext>(context)
    {
        protected override bool Any(int id) => _context.Categorias.Any(e => e.Id_Categoria == id);
        public override async Task<IActionResult> Add(CategoriaModel model)
        {

            model.Id_Categoria = 0;

            try
            {
                var entityEntry = await _context.Categorias.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Categoria.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Categoria.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
      
        public override async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var tc = await _context.Categorias.FindAsync(Id);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Categoria.Delete, {Id}>: El categoria no existe");
                    return BadRequest("El categoria no existe");
                }

                _context.Categorias.Remove(tc);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Categoria.Delete, {Id}>");
                return Ok("categoria eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Categoria.Delete, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
     
        public override async Task<ActionResult<IEnumerable<CategoriaModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Categorias.ToListAsync();
                Log.Information("Se llamo al endpoint <Categoria.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Categoria.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
  
        public override async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var tc = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Categoria == Id);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <Categoria.GetByID, {Id}>: El categoria no existe");
                    return NotFound("El tipo de categoria no existe");
                }

                Log.Information($"Se llamo al endpoint <Categoria.GetByID, {Id}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Categoria.GetByID, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<IActionResult> Update(CategoriaModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <Categoria.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Categoria);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <Categoria.Update, {model.ToString()}>: El categoria no existe");
                    return NotFound("El categoria no existe");
                }
                _context.Categorias.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Categoria.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Categoria.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
    }
}
