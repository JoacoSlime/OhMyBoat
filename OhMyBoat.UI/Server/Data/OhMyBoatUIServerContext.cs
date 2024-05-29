using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OhMyBoat.UI.Shared.Entidades;

namespace OhMyBoat.UI.Server.Data
{
    public class OhMyBoatUIServerContext: DbContext
    {
#nullable disable
        /// <summary> asi pones el modelo en el EFC 
        /// public DbSet<Titular> Titulares { get; set; } 
        ///  public DbSet<Vehiculo> Vehiculos { get; set; }
        ///   public DbSet<Poliza> Polizas { get; set; }
        ///   public DbSet<Siniestro> Siniestros { get; set; }
        ///  public DbSet<Tercero> Terceros { get; set; }
        /// 
        /// 
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Denuncia> Denuncias { get; set; }
        public DbSet<TokenRecu> Tokens { get; set; }
        public DbSet<Trueque> Trueques { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<Terrestre> Terrestres { get; set; }
        public DbSet<Maritimo> Maritimos { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<TokenRecu> TokenRecu { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }



#nullable restore
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("data source=DataBase.sqlite");
        }

    }
}
