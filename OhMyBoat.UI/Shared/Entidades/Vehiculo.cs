using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class  Vehiculo
    {
        public int Id { get; set; }
        public string IDCliente { get; set; } ="";
        public string Matricula { get; set; } = "";
        public string? base64imagen {  get; set; }
        public int Antiguedad { get; set; }
        public string? descripcion { get; set; }
        public bool Visible { get; set; } = true;

    }
}
