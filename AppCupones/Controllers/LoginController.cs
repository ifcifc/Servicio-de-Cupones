using AppCupones.Data;
using Microsoft.AspNetCore.Mvc;
using Common.Models;
using Common.Interfaces;
using Common.Models.DTO;

namespace AppCupones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly DbAppContext _context;
        private readonly IHashPasswordService _hashPasswordService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginController(DbAppContext context, IHashPasswordService hashPasswordService, IJwtTokenService jwtTokenService)
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

            var usuario = new UsuarioDTO()
            {
                Nombre = "Alguien",
                Email = "algo@mail.com",
                ID_Usuario = 3
            };

            string token = this._jwtTokenService.GenerarToken(usuario);

            return Ok(new
            {
                Message = "Usuario logeado correctamente",
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Token = token
            });
        }
    }
}
