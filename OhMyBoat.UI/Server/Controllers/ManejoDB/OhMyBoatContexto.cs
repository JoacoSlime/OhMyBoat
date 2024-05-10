
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public async void AgregarCliente(Cliente cliente_nuevo){
            await
            using (var db = new OhMyBoatUIServerContext()){
                if (db.Clientes.Where(cli => cli.Email == cliente_nuevo.Email).IsNullOrEmpty()){
                    db.Clientes.Add(cliente_nuevo);
                    db.SaveChanges();
                }
            }
        }
    }
}
