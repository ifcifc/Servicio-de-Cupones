using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.DTO
{
    public class UsuarioDTO : Model
    {
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}
