using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Cliente : Usuario
    {
        public Cliente (String nombre, String email, String passw){
            Nombre = nombre;
            Email = email;
            Password = passw;
            Bloqueado = false;
            Contacto = null;
            Rol = Roles.Cliente;
        }
        public List<Terrestre>? Terrestres {  get; set; }
        public List<Maritimo>? Maritimos {  get; set; }
    }
}
