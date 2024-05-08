
using Microsoft.EntityFrameworkCore;
using OhMyBoat.UI.Server.Data;

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

    }
}
