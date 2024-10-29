using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCupones.Models
{
    public class CuponClienteModel : Model
    {
        [Key]
        public string NroCupon { get; set; }
        public int Id_Cupon { get; set; }
        public DateTime FechaAsignado { get; set; }
        public string CodCliente { get; set; }



        [ForeignKey("Id_Cupon")]
        public virtual CuponModel? Cupon { get; set; }
    }

}
