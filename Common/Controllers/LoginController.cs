using Common.Interfaces;
using Common.Models;
using Common.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuponesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Esta preparada para ser usada, pero oculta por que fue descartado del proyecto final
    public class LoginController<T> : ControllerBase where T : DbContext
    {
        protected readonly T _context;
        protected readonly IHashPasswordService _hashPasswordService;
        protected readonly IJwtTokenService _jwtTokenService;

        public LoginController(T context, IHashPasswordService hashPasswordService, IJwtTokenService jwtTokenService)
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
                Nombre = "admin",
                Email = "admin",
                ID_Usuario = 0
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
