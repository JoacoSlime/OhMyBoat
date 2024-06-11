using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Trueque
    {
          public int Id { get; set; }
          public int MaritimoId {  get; set; }
          public int VehiculoId { get; set; }
          public bool? Concreto { get; set; }

    }
}
