using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Terrestre  : Vehiculo
    {
        public TipoVehiculo Tipo {  get; set; }

        public string? Marca { get; set; } = "";
   
        public string? Modelo { get; set; } = "";
   
        public int Kilometraje { get; set; } = 0;
   
        public int Anio { get; set; } = 0;
   
    }
}
