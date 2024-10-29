﻿using AppCupones.Data;
using AppCupones.Data;
using AppCupones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System.Runtime.InteropServices;

namespace AppCupones.Controllers
{
    public class CuponCategoriaController(DbAppContext context) : BaseController<CuponCategoriaModel>(context)
    {
        protected override bool Any(int id) => _context.Cupones_Categorias.Any(e => e.Id_Cupones_Categorias == id);
        public override async Task<IActionResult> Add(CuponCategoriaModel model)
        {
            model.Id_Cupones_Categorias = 0;
            model.Categoria = null;
            model.Cupon = null;
            try
            {
                var entityEntry = await _context.Cupones_Categorias.AddAsync(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponCategoria.Add, {model.ToString()}>");
                return Ok(model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCategoria.Add, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var tc = await _context.Cupones_Categorias.FindAsync(Id);

                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponCategoria.Delete, {Id}>: El cupon categoria no existe");
                    return BadRequest("El cupon categoria no existe");
                }

                _context.Cupones_Categorias.Remove(tc);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponCategoria.Delete, {Id}>");
                return Ok("Precio eliminado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCategoria.Delete, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<ActionResult<IEnumerable<CuponCategoriaModel>>> GetAll()
        {
            try
            {
                var tc = await _context.Cupones_Categorias
                        .Include(x => x.Cupon)
                        .Include(x => x.Categoria)
                        .ToListAsync();
                Log.Information("Se llamo al endpoint <CuponCategoria.GetAll>");
                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCategoria.GetAll>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var tc = await _context.Cupones_Categorias.AsNoTracking()
                        .Include(x => x.Cupon)
                        .Include(x => x.Categoria)
                        .FirstOrDefaultAsync(x => x.Id_Cupones_Categorias == Id);
                if (tc is null)
                {
                    Log.Error($"Error en el endpoint <CuponCategoria.GetByID, {Id}>: El cupon categoria no existe");
                    return NotFound("El cupon categoria no existe");
                }

                Log.Information($"Se llamo al endpoint <CuponCategoria.GetByID, {Id}>");

                return Ok(tc);
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCategoria.GetByID, {Id}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
        public override async Task<IActionResult> Update(CuponCategoriaModel model)
        {
            if (model is null)
            {
                Log.Error($"Error en el endpoint <CuponCategoria.Update>: No se proporciono un cupon");
                return BadRequest("No se proporciono un cupon");
            }

            try
            {
                //Any -> Devuelve true si encuentra un registro en la DB
                bool cuponExiste = this.Any(model.Id_Cupones_Categorias);
                if (!cuponExiste)
                {
                    Log.Error($"Error en el endpoint <CuponCategoria.Update, {model.ToString()}>: El cupon categoria no existe");
                    return NotFound("El cupon categoria no existe");
                }

                model.Categoria = null;
                model.Cupon = null;

                _context.Cupones_Categorias.Update(model);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <CuponCategoria.Update, {model.ToString()}>");
                return Ok("Cupon modificado correctamente");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <CuponCategoria.Update, {model.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
    }
}