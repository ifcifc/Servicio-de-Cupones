using Common.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuponesAPI.Models
{
    public class CuponDetalleModel : Model
    {
        [Key]
        public int Id_Cupon { get; set; }
        [Key]
        public int Id_Articulo { get; set; }
        public int Cantidad { get; set; }

        
        /*[ForeignKey("Id_Cupon")]
        public virtual CuponModel? Cupon { get; set; }
        */
        [ForeignKey("Id_Articulo")]
        public virtual ArticuloModel? Articulo { get; set; }
    }

}
