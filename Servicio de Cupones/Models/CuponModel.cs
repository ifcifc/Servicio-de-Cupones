namespace Servicio_de_Cupones.Models
{
    public class CuponModel
    {
        public int Id_Cupon { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? PorcentajeDto { get; set; }
        public decimal? ImportePromo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Id_Tipo_Cupon { get; set; }
        public bool Activo { get; set; }
    }

}
