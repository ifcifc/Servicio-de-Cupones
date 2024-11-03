using Common.Models;
using System.ComponentModel.DataAnnotations;

namespace ClientesApi.Models
{
    public class ClienteModel : Model
    {
        [Key]
        public string CodCliente { get; set; }
        public string Nombre_Cliente { get; set; }
        public string Apellido_Cliente { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
    }
}
