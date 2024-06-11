using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class ReporteTrueque
    {
        public int IdTrueque { get; set; }
        public int MaritimoId {  get; set; }
        public int VehiculoId { get; set; }
        public String Sucursal { get; set; }
        public DateTime FechaTurno {  get; set; }
        public bool Concreto { get; set; }

    }
}