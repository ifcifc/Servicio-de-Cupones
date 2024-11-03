using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace AppCupones.Models
{
    public class UsuarioModel : Model
    {
        [Key]
        public int ID_Usuario { get; set; }
        //public int ID_Rol { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public string? Telefono { get; set; }
        //public DateTime FechaNacimiento { get; set; }
        //public int Edad { get; set; }

        /*[ForeignKey("ID_Rol")]
        public virtual RolModel? Rol { get; set; }*/
    }
}
