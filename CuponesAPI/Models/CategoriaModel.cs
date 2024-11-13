using Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuponesAPI.Models
{
    public class CategoriaModel : Model
    {
        [Key]
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; }

    }

}
