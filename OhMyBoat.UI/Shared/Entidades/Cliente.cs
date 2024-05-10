using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Cliente : Usuario
    {

        public Cliente(){}

        //es necesario o no el constructor? genera ambiguedades? necesito ese constructor vacio para que me deje de joder?
        public Cliente (String nombre, String email, String passw){
            Nombre = nombre;
            Email = email;
            Password = passw;
            UserName = email.Remove(email.IndexOf('@'), email.Length-1);
            MayorDeEdad = true;
            Bloqueado = false;
            Contacto = null;
            Rol = Roles.Cliente;
        }
        public List<Terrestre>? Terrestres {  get; set; }
        public List<Maritimo>? Maritimos {  get; set; }
    }
}
