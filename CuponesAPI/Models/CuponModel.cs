using Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuponesAPI.Models
{
    public class CuponModel : Model
    {
        [Key]
        public int Id_Cupon { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? PorcentajeDto { get; set; }
        public decimal? ImportePromo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Id_Tipo_Cupon { get; set; }

        [DefaultValue(true)]
        public bool Activo { get; set; }


        [ForeignKey("Id_Tipo_Cupon")]

        public virtual TipoCuponModel? TipoCupon { get; set; }

        public virtual ICollection<CuponCategoriaModel>? CuponCategoria { get; private set; }

    }

}
