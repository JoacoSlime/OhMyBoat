using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class ReporteTrueque
    {
        public int IdOferta { get; set; }
        public int MaritimoId {  get; set; }
        public string MaritimoPatente { get; set; } = "";
        public string VehiculoPatente { get; set; } = "";
        public int VehiculoId { get; set; }
        public String Sucursal { get; set; }
        public DateTime FechaTurno {  get; set; }
        public EstadoOferta? Estado { get; set; }

    }
}