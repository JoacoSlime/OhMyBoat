using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class ProposicionTurno
    {
        public int ID { get; set; }
        public int IDSucursal { get; set; }
        public int IDTrueque {  get; set; }
        public DateTime turnoObligatorio { get; set; }
        public DateTime? turnoOpcional { get; set; }
        public DateTime? turnoOpcional2 { get; set; }
    }
}
