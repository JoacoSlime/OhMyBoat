using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhMyBoat.UI.Shared.Entidades
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public int IDCliente { get; set; }
        public string Matricula { get; set; } = "";
        public int? ImagenId {  get; set; }
        public int Antiguedad { get; set; }
        public string? descripcion { get; set; }

    }
}
