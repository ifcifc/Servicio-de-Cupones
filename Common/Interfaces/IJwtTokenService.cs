using Common.Models.DTO;

namespace Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerarToken(UsuarioDTO usuario);
    }
}