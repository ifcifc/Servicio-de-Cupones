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


        [ForeignKey("Id_Cupon")]
        public virtual CuponModel? Cupon { get; set; }
    }

}
