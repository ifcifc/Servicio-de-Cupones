using Common.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CuponesAPI.Models
{
    public class ArticuloModel : Model
    {
        [Key]
        public int Id_Articulo { get; set; }
        public string Nombre_Articulo { get; set; }
        public string Descripcion_Articulo { get; set; }
        [DefaultValue(true)]
        public bool Activo { get; set; }
    }
}
