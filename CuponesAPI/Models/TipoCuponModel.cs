﻿using Common.Models;
using System.ComponentModel.DataAnnotations;

namespace CuponesAPI.Models
{
    public class TipoCuponModel : Model
    {
        [Key]
        public int Id_Tipo_Cupon { get; set; }
        public string Nombre { get; set; }
    }

}
