namespace Servicio_de_Cupones.Models
{
    public class CuponClienteModel
    {
        public int Id_Cupon { get; set; }
        public string NroCupon { get; set; }
        public DateTime FechaAsignado { get; set; }
        public string CodCliente { get; set; }
    }

}
