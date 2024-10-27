using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCupones.Models
{
    public class CuponHistorialModel : Model
    {
        [Key]
        public int Id_Cupon { get; set; }
        [Key]
        public string NroCupon { get; set; }
        public DateTime FechaUso { get; set; }
        public string CodCliente { get; set; }

        //Redundante Ya se puede acceder al cupon desde cliente
        /*[ForeignKey("Id_Cupon")]
        public virtual CuponModel? Cupon { get; private set; }*/


        [ForeignKey("NroCupon")]
        public virtual CuponClienteModel? Cliente { get; set; }
    }

}
