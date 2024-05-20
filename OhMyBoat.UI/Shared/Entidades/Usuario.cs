using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool Bloqueado { get; set; } = false;
        public string Contacto { get; set; } = "";
        public Roles Rol { get; set; }
        public string? Base64imagen { get; set; }

    }
}
