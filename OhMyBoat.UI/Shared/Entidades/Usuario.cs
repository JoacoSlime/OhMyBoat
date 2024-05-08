using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Usuario
    {
        public int ID { get; set; } = -1;
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public bool MayorDeEdad { get; set; }
        public bool Bloqueado { get; set; } = false;
        public string? Contacto { get; set; }
        public Roles Rol { get; set; }
        public int? IDImagen { get; set; }

    }
}
