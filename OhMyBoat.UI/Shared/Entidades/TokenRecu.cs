using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class TokenRecu
    {
        public int Id { get; set; }
        public string StringAleatorioDelMomento { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime FechaLimite { get; set; }
        public bool Usado { get; set; } = false;

        public TokenRecu(string email,string testo) { 
            Email = email;
            Usado = false;
            FechaLimite = DateTime.Now.AddDays(7);
            StringAleatorioDelMomento = testo;
        }
    }
}
