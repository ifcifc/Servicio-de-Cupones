using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppCupones.Data;
using AppCupones.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace AppCupones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponController : ControllerBase
    {
        private readonly DbAppContext _context;

        public CuponController(DbAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuponModel>>> GetAll()
        {
            try
            {
                var cupones = await _context.Cupones
                                        .Include(x => x.TipoCupon)
                                        .Include(x => x.CuponCategoria)
                                        .ThenInclude(x=>x.Categoria)
                                        .ToListAsync();
                Log.Information("Se llamo al endpoint <Cupon.GetAll>");
                return Ok(cupones);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }



        [HttpGet("{Id_Cupon}")]

        public async Task<IActionResult> GetByID(int Id_Cupon)
        {
            try
            {
                var cuponModel = await _context.Cupones
                                        .Include(x => x.TipoCupon)
                                        .Include(x => x.CuponCategoria)
                                        .ThenInclude(x => x.Categoria)
                                        .FirstOrDefaultAsync(x => x.Id_Cupon == Id_Cupon);
                if (cuponModel is null)
                {
                    Log.Error($"Error en el endpoint <Cupon.GetByID, {Id_Cupon}>: El cupon no existe");
                    return NotFound("El cupon no existe");
                }

                Log.Information($"Se llamo al endpoint <Cupon.GetByID, {Id_Cupon}>");
                return Ok(cuponModel);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.GetByID, {Id_Cupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Add(CuponModel cupon)
        {
            try
            {
                //Para evitar errores al crear el nuevo cupon
                cupon.Id_Cupon = 0;
                cupon.TipoCupon = null;

                //Comprueba que el Tipo_Cupon exista
                bool existeTipo_Cupon = await _context.Tipo_Cupon.AnyAsync(x => x.Id_Tipo_Cupon == cupon.Id_Tipo_Cupon);
                if (!existeTipo_Cupon)
                {
                    Log.Error($"Error en el endpoint <Cupon.Add, {cupon.ToString()}>: No existe el Tipo_Cupon asignado");
                    return NotFound("No existe el Tipo de cupon asignado");
                }

                var entityEntry = _context.Cupones.Add(cupon);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <cupon.Add, {cupon.ToString()}>");
                return Ok(cupon);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <cupon.Add, {cupon.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        [HttpPut]
        public async Task<IActionResult> Update(CuponModel cupon)
        {

            if (cupon is null)
            {
                Log.Error($"Error en el endpoint <Cupon.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(cupon.Id_Cupon);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <Cupon.Update, {cupon.ToString()}>: El cupon no existe");
                    return NotFound("El cupon no existe");
                }


                //Comprueba que el Tipo_Cupon exista
                bool existeRol = await _context.Tipo_Cupon.AnyAsync(x => x.Id_Tipo_Cupon == cupon.Id_Tipo_Cupon);
                if (!existeRol)
                {
                    Log.Error($"Error en el endpoint <Cupon.Update, {cupon.ToString()}>: No existe el Tipo_Cupon asignado");
                    return NotFound("No existe el Tipo de cupon asignado");
                }

                _context.Cupones.Update(cupon);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cupon.Update, {cupon.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.Update, {cupon.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        [HttpDelete("{Id_Cupon}")]
        public async Task<IActionResult> Delete(int Id_Cupon)
        {
            try
            {
                CuponModel? cupon = await _context.Cupones.FindAsync(Id_Cupon);

                if (cupon is null)
                {
                    Log.Error($"Error en el endpoint <Cupon.Delete, {Id_Cupon}>: El cupon no existe");
                    return BadRequest("El cupon no existe");
                }

                cupon.Activo = false;

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cupon.Delete, {Id_Cupon}>");
                return Ok("Cupon eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.Delete, {Id_Cupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        private bool Any(int id)
        {
            return _context.Cupones.Any(e => e.Id_Cupon == id);
        }
    }
}
