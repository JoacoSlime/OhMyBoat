using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{

    
    public class ActualizarUsuario
    {
        public string Nombre { get; set; } = "";
        public string Contacto { get; set; } = "";
        public string Email { get; set; }  = "";
        public string Base64imagen { get; set; } = "";
    }

}
