using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCupones.Models
{
    public class CuponCategoriaModel : Model
    {
        [Key]
        public int Id_Cupones_Categorias { get; set; }
        public int Id_Cupon { get; set; }
        public int Id_Categoria { get; set; }

        
        [ForeignKey("Id_Cupon")]
        public virtual CuponModel? Cupon { get; set; }


        [ForeignKey("Id_Categoria")]

        public virtual CategoriaModel? Categoria { get; set; }
    }

}
