namespace Servicio_de_Cupones.Models
{
    public class CuponHistorialModel
    {
        public int Id_Cupon { get; set; }
        public string NroCupon { get; set; }
        public DateTime FechaUso { get; set; }
        public string CodCliente { get; set; }
    }

}
