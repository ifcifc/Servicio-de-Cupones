﻿using Microsoft.EntityFrameworkCore;
using CuponesAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CuponesAPI.Data
{
    public class DbAppContext : DbContext
    {

        public DbSet<ArticuloModel> Articulos { get; set; }
        public DbSet<CategoriaModel> Categorias { get; set; }
        public DbSet<CuponCategoriaModel> Cupones_Categorias { get; set; }
        public DbSet<CuponClienteModel> Cupones_Clientes { get; set; }
        public DbSet<CuponDetalleModel> Cupones_Detalle { get; set; }
        public DbSet<CuponHistorialModel> Cupones_Historial { get; set; }
        public DbSet<PrecioModel> Precios { get; set; }
        public DbSet<CuponModel> Cupones { get; set; }
        public DbSet<TipoCuponModel> Tipo_Cupon { get; set; }

        public DbAppContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Para el borrado logico
            modelBuilder.Entity<ArticuloModel>().HasQueryFilter(x => x.Activo);
            modelBuilder.Entity<CuponModel>().HasQueryFilter(x => x.Activo);

            //Para definir las keys
            modelBuilder.Entity<CuponDetalleModel>().HasKey(x => new { x.Id_Cupon, x.Id_Articulo});
            modelBuilder.Entity<CuponHistorialModel>().HasKey(x => new { x.Id_Cupon, x.NroCupon });

            base.OnModelCreating(modelBuilder);
        }
    }
}
