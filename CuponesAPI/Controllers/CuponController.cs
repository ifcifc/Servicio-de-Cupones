using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuponesAPI.Data;
using CuponesAPI.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using Common.Controllers;
using Common.Services;
using Common.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using Common.Interfaces;

namespace CuponesAPI.Controllers
{

    public class CuponController(DbAppContext context) : BaseController<CuponModel, DbAppContext>(context)
    {
        protected override bool Any(int id) => _context.Cupones.Any(e => e.Id_Cupon == id);

        public override async Task<ActionResult<IEnumerable<CuponModel>>> GetAll()
        {
            try
            {
                var cupones = await _context.Cupones
                                        .Include(x => x.TipoCupon)
                                        .Include(x => x.CuponCategoria)
                                        .ThenInclude(x => x.Categoria)
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

        public override async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var cuponModel = await _context.Cupones
                                        .Include(x => x.TipoCupon)
                                        .Include(x => x.CuponCategoria)
                                        .ThenInclude(x => x.Categoria)
                                        .FirstOrDefaultAsync(x => x.Id_Cupon == Id);
                if (cuponModel is null)
                {
                    Log.Error($"Error en el endpoint <Cupon.GetByID, {Id}>: El cupon no existe");
                    return NotFound("El cupon no existe");
                }

                Log.Information($"Se llamo al endpoint <Cupon.GetByID, {Id}>");
                return Ok(cuponModel);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.GetByID, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<IActionResult> Add(CuponModel model)
        {
            try
            {
                //Para evitar errores al crear el nuevo cupon
                model.Id_Cupon = 0;
                model.TipoCupon = null;

                //Comprueba que el Tipo_Cupon exista
                bool existeTipo_Cupon = await _context.Tipo_Cupon.AnyAsync(x => x.Id_Tipo_Cupon == model.Id_Tipo_Cupon);
                if (!existeTipo_Cupon)
                {
                    Log.Error($"Error en el endpoint <Cupon.Add, {model.ToString()}>: No existe el Tipo_Cupon asignado");
                    return NotFound("No existe el tipo de cupon asignado");
                }

                await _context.Cupones.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <cupon.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <cupon.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }


        public override async Task<IActionResult> Update(CuponModel model)
        {

            if (model is null)
            {
                Log.Error($"Error en el endpoint <Cupon.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Cupon);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <Cupon.Update, {model.ToString()}>: El cupon no existe");
                    return NotFound("El cupon no existe");
                }


                //Comprueba que el Tipo_Cupon exista
                bool existeRol = await _context.Tipo_Cupon.AnyAsync(x => x.Id_Tipo_Cupon == model.Id_Tipo_Cupon);
                if (!existeRol)
                {
                    Log.Error($"Error en el endpoint <Cupon.Update, {model.ToString()}>: No existe el Tipo_Cupon asignado");
                    return NotFound("No existe el tipo de cupon asignado");
                }
                model.TipoCupon = null;
                _context.Cupones.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cupon.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        public override async Task<IActionResult> Delete(int Id)
        {
            try
            {
                CuponModel? cupon = await _context.Cupones.FindAsync(Id);

                if (cupon is null)
                {
                    Log.Error($"Error en el endpoint <Cupon.Delete, {Id}>: El cupon no existe");
                    return BadRequest("El cupon no existe");
                }

                cupon.Activo = false;

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cupon.Delete, {Id}>");
                return Ok("Cupon eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.Delete, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

        

    }
}