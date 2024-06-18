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
        public int ID_VehiculoRecibeOferta  {  get; set; }
        public int ID_VehiculoEnviaOferta {  get; set; }
        public bool EsNavioRecibe { get; set; }
        public bool EsNavioEnvia { get; set; }
        public EstadoOferta EstadoOferta { get; set; } = EstadoOferta.Enviada;

    }
}
