using ClientesApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ClientesApi.Data
{
    public class DbAppContext : DbContext
    {
        public DbSet<ClienteModel> Clientes { get; set; }


        //public DbSet<ArticuloModel> Articulos { get; set; }
        public DbAppContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            base.OnModelCreating(modelBuilder);
        }
    }
}
