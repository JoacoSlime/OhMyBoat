using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Maritimo : Vehiculo
    { 
        public int IDSucursal {  get; set; }
        public float Eslora {  get; set; }
        public float Puntal {  get; set; }
        public float Deuda {  get; set; }

    }
}
