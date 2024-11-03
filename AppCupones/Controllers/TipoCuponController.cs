using AppCupones.Data;
using AppCupones.Data;
using AppCupones.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System.Runtime.InteropServices;
using Common.Controllers;

namespace AppCupones.Controllers
{
    //[Authorize]
    public class TipoCuponController(DbAppContext context) : BaseController<TipoCuponModel, DbAppContext>(context)
    {
        protected override bool Any(int id) => _context.Tipo_Cupon.Any(e => e.Id_Tipo_Cupon == id);
        public override async Task<IActionResult> Add(TipoCuponModel model)
        {
            model.Id_Tipo_Cupon = 0;

            try
            {
                var entityEntry = await _context.Tipo_Cupon.AddAsync(model);
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
        public override async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var tc = await _context.Tipo_Cupon.FindAsync(Id);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <TipoCupon.Delete, {Id}>: El tipo de cupon no existe");
                    return BadRequest("El tipo de cupon no existe");
                }

                _context.Tipo_Cupon.Remove(tc);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <TipoCupon.Delete, {Id}>");
                return Ok("Tipo de cupon eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <TipoCupon.Delete, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<ActionResult<IEnumerable<TipoCuponModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Tipo_Cupon.ToListAsync();
                Log.Information("Se llamo al endpoint <TipoCupon.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <TipoCupon.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var tc = await _context.Tipo_Cupon.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Tipo_Cupon == Id);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <TipoCupon.GetByID, {Id}>: El tipo de cupon no existe");
                    return NotFound("El tipo de tipo de cupon no existe");
                }

                Log.Information($"Se llamo al endpoint <TipoCupon.GetByID, {Id}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <TipoCupon.GetByID, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Update(TipoCuponModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <TipoCupon.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Tipo_Cupon);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <TipoCupon.Update, {model.ToString()}>: El tipo de cupon no existe");
                    return NotFound("El tipo de cupon no existe");
                }
                _context.Tipo_Cupon.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <TipoCupon.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <TipoCupon.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
    }
}
