using Common.Controllers;
using Common.Interfaces;
using Common.Models.DTO;
using CuponesAPI.Data;
using CuponesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace CuponesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudCuponesController : ControllerBase
    {
        private readonly DbAppContext _context;
        private IGenerateCuponService generateCuponService { get; set; }

        public SolicitudCuponesController(DbAppContext context, IGenerateCuponService generateCuponService)
        {
            this._context = context;
            this.generateCuponService = generateCuponService;
        }

        private bool Any(int id) => _context.Cupones.Any(e => e.Id_Cupon == id);

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
                do
                {
                    NroCupon = this.generateCuponService.GenerateCode();
                } while (_context.Cupones_Historial.Any(x => x.NroCupon == NroCupon) || _context.Cupones_Clientes.Any(x => x.NroCupon == NroCupon));


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
                return Ok(new
                {
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
                var any = await _context.Cupones_Clientes.IgnoreQueryFilters().AnyAsync(x => x.NroCupon == NroCupon);
                if (!any)
                {
                    Log.Error($"Error en el endpoint <Cupon.QuemarCupon, {NroCupon}>: El Nro cupon no existe");
                    return BadRequest("El Nro cupon no existe");
                }

                var cc = await _context.Cupones_Clientes
                                        .IgnoreQueryFilters()
                                        .Include(x => x.Cupon)
                                        .AsNoTracking()
                                        .FirstAsync(x => x.NroCupon == NroCupon);

                if (!cc.Cupon.Activo)
                {
                    Log.Error($"Error en el endpoint <Cupon.QuemarCupon, {NroCupon}>: El cupon ya no esta disponible");
                    return BadRequest($"El cupon ya no esta disponible");
                }

                var fecha = DateTime.Now;


                if (!(fecha >= cc.Cupon.FechaInicio && fecha <= cc.Cupon.FechaFin))
                {
                    Log.Error($"Error en el endpoint <Cupon.QuemarCupon, {NroCupon}>: El cupon expiro");

                    return BadRequest($"El cupon expiro");
                }

                await _context.Cupones_Historial.AddAsync(new CuponHistorialModel()
                {
                    Id_Cupon = cc.Id_Cupon,
                    NroCupon = cc.NroCupon,
                    FechaUso = fecha,
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
