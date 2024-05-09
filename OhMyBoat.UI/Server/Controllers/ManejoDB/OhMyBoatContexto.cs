﻿
using Microsoft.EntityFrameworkCore;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Shared.Entidades;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    public class OhMyBoatContexto { 
        static OhMyBoatContexto() { 
            using (var context = new OhMyBoatUIServerContext())
            {
                context.Database.EnsureCreated();
                var connection = context.Database.GetDbConnection();
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA journal_mode=DELETE;";
                    command.ExecuteNonQuery();
                }
            }
        }

        // Aca se sufre

        public void agregarCliente(Cliente cliente_nuevo){
            using (var db = new OhMyBoatUIServerContext()){
                db.Clientes.Add(cliente_nuevo);
                db.SaveChanges();
            }
        }
    
    }

}
