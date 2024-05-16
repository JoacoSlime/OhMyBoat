using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Cliente : Usuario
    {
        public List<Terrestre>? Terrestres {  get; set; }
        public List<Maritimo>? Maritimos {  get; set; }
    }
}
