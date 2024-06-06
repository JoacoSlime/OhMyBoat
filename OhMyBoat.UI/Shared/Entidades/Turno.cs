using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Turno
    {
        public int Id { get; set; }
        public int IDTrueque {  get; set; }
        public int IDSucursal { get; set; }
        public DateTime FechaTurno {  get; set; }

    }
}
