using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class TokenRecu
    {
        public int ID {  get; set; }    
        public string StringAleatorioDelMomento { get; set; } = "";
        public int IDUsuario { get; set; }
        public DateTime FechaLimite { get; set; }
        public bool Usado { get; set; } = false;

    }
}
