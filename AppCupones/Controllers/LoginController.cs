using AppCupones.Data;
using AppCupones.Models;
using AppCupones.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Controllers;

namespace AppCupones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly DbAppContext _context;
        private readonly HashPasswordService _hashPasswordService;
        private readonly JwtTokenService _jwtTokenService;

        public LoginController(DbAppContext context, HashPasswordService hashPasswordService, JwtTokenService jwtTokenService)
        {
            _context = context;
            _hashPasswordService = hashPasswordService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginDTO loginDTO)
        {

            /*string pass = this._hashPasswordService.HashPassword(loginDTO.Password);

            UsuarioModel? usuario = await this._context.Usuarios
                                                    .Where(x => x.Email == loginDTO.Email && x.Password == pass)
                                                    .FirstOrDefaultAsync();

            if (usuario == null) return BadRequest("Los datos son incorrectos");*/

            var usuario = new UsuarioModel()
            {
                Nombre = "Alguien",
                Apellido = "Alguno",
                Email = "algo@mail.com",
                Password = "123"
            };

            string token = this._jwtTokenService.GenerarToken(usuario);

            return Ok(new
            {
                Message = "Usuario logeado correctamente",
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Token = token
            });
        }
    }
}
