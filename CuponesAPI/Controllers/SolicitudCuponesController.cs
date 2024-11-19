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
            if (clienteDTO is null)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.AsignarCupon>: No se proporciono un modelo");
                return BadRequest("No se proporciono un modelo");
            }

            if (clienteDTO.CodCliente.IsNullOrEmpty())
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.AsignarCupon, {clienteDTO.ToString()}>: Falta el codigo del cliente");
                return BadRequest("No ingreso el codigo del cliente");
            }

            try
            {
                CuponModel? cupon = await _context.Cupones.FindAsync(clienteDTO.Id_Cupon);

                if (cupon is null)
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.AsignarCupon, {clienteDTO.ToString()}>: El cupon no existe");
                    return BadRequest("El cupon no existe");
                }

                if (DateTime.Now>=cupon.FechaFin) {
                    Log.Error($"Error en el endpoint <SolicitudCupones.AsignarCupon, {clienteDTO.ToString()}>: El cupon solicitado expiro");
                    return BadRequest("El cupon solicitado expiro");
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
                Log.Information($"Se llamo al endpoint <SolicitudCupones.AsignarCupon, {clienteDTO.ToString()}>");
                return Ok(new
                {
                    Message = "El cupon fue asignado correctamente",
                    NroCupon = NroCupon
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.AsignarCupon, {clienteDTO.ToString()}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }

        }

        [HttpPost("QuemarCupon/{NroCupon}")]
        public async Task<IActionResult> QuemarCupon(string NroCupon)
        {
            try
            {

                bool anyHis = await _context.Cupones_Historial.AnyAsync(x => x.NroCupon == NroCupon);

                if(anyHis)
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: El cupon ya fue quemado");
                    return BadRequest("El cupon ya fue quemado");
                }

                var any = await _context.Cupones_Clientes.IgnoreQueryFilters().AnyAsync(x => x.NroCupon == NroCupon);
                if (!any)
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: El numero del cupon no existe");
                    return BadRequest("El numero del cupon no existe");
                }



                var cc = await _context.Cupones_Clientes
                                        .IgnoreQueryFilters()
                                        .Include(x => x.Cupon)
                                        .AsNoTracking()
                                        .FirstAsync(x => x.NroCupon == NroCupon);

                if (!cc.Cupon.Activo)
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: El cupon ya no esta disponible");
                    return BadRequest($"El cupon ya no esta disponible");
                }

                var fecha = DateTime.Now;

                if (cc.Cupon.FechaInicio > fecha)
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: El cupon aun no esta disponible");

                    return BadRequest($"El cupon aun no esta disponible");
                }

                if (fecha >= cc.Cupon.FechaFin)
                {
                    Log.Error($"Error en el endpoint <SolicitudCupones.QuemarCupon, {NroCupon}>: El cupon expiro");

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

                Log.Information($"Se llamo al endpoint <SolicitudCupones.QuemarCupon,{NroCupon}>");
                return Ok("El cupon a sido quemado");
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el endpoint <SolicitudCupones.AsignarCupon, {NroCupon}>: {ex.Message}");
                return BadRequest($"Hubo un error: {ex.Message}");
            }
        }
    }
}
