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
using System.Runtime.InteropServices;
using Common.Controllers;
using Common.Services;
using Common.Models.DTO;
using Microsoft.IdentityModel.Tokens;

namespace AppCupones.Controllers
{

    public class CuponController : BaseController<CuponModel, DbAppContext>
    {
        private GenerateCuponService generateCuponService { get; set; }

        public CuponController(DbAppContext context, GenerateCuponService generateCuponService):base(context) {
            this.generateCuponService = generateCuponService;
        }

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
                    return NotFound("No existe el Tipo de cupon asignado");
                }

                var entityEntry = _context.Cupones.Add(model);
                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <cupon.Add, {model.ToString()}>");
                return Ok(entityEntry);
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
                    return NotFound("No existe el Tipo de cupon asignado");
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

        [HttpPost("AsignarCupon")]
        public async Task<IActionResult> AsignarCupon(ClienteDTO clienteDTO)
        {
            if (clienteDTO.CodCliente.IsNullOrEmpty()) 
            {
                Log.Error($"Error en el endpoint <Cupon.AsignarCupon, {clienteDTO.ToString()}>: Falta el codigo de cliente");
                return BadRequest("No se a pasado un codigo de cliente");
            }

            try
            {
                if (!this.Any(clienteDTO.Id_Cupon))
                {
                    Log.Error($"Error en el endpoint <Cupon.AsignarCupon, {clienteDTO.ToString()}>: El cupon no existe");
                    return BadRequest("El cupon no existe");
                }

               var NroCupon = "";

                //Verifica que no exista ningun cupon con el numero generado, si existe se vuelve a generar
                do {
                    NroCupon = this.generateCuponService.GenerateCode();
                } while (_context.Cupones_Historial.Any(x=>x.NroCupon==NroCupon) || _context.Cupones_Clientes.Any(x => x.NroCupon == NroCupon));


                var cc = new CuponClienteModel()
                {
                    Id_Cupon = clienteDTO.Id_Cupon,
                    CodCliente = clienteDTO.CodCliente,
                    FechaAsignado = DateTime.Now,
                    NroCupon = NroCupon
                };

                await _context.Cupones_Clientes.AddAsync(cc);
                await _context.SaveChangesAsync();
                Log.Information($"Se llamo al endpoint <Cupon.AsignarCupon, {clienteDTO.ToString()}>");
                return Ok(new {
                    Message = "El cupon fue asignado correctamente",
                    NroCupon = NroCupon
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.AsignarCupon, {clienteDTO.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }

        }

        [HttpPost("QuemarCupon/{NroCupon}")]
        public async Task<IActionResult> QuemarCupon(string NroCupon)
        {
            try
            {
                var any = await _context.Cupones_Clientes.AnyAsync(x => x.NroCupon == NroCupon);
                if (!any)
                {
                    Log.Error($"Error en el endpoint <Cupon.QuemarCupon, {NroCupon}>: El Nro cupon no existe");
                    return BadRequest("El Nro cupon no existe");
                }

                var cc = await _context.Cupones_Clientes.FirstAsync(x => x.NroCupon == NroCupon);

                await _context.Cupones_Historial.AddAsync(new CuponHistorialModel()
                {
                    Id_Cupon = cc.Id_Cupon,
                    NroCupon = cc.NroCupon,
                    FechaUso = DateTime.Now,
                    CodCliente = cc.CodCliente
                });

                _context.Cupones_Clientes.Remove(cc);

                await _context.SaveChangesAsync();

                Log.Information($"Se llamo al endpoint <Cupon.QuemarCupon,{NroCupon}>");
                return Ok("El cupon a sido quemado");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <Cupon.AsignarCupon, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }

    }
}