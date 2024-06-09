using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Oferta 
    { 
        public int Id { get; set; }
        public string? ID_RecibeOferta {  get; set; }
        public string? ID_EnviaOferta {  get; set; }
        public string PatenteVehiculoRecibeOferta  {  get; set; }
        public string  PatenteVehiculoEnviaOferta {  get; set; }

        public bool Estado {get;set;}

    }
}
