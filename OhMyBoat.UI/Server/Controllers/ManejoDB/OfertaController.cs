using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OhMyBoat.UI.Shared.Entidades;

using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Server.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace OhMyBoat.UI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

 public class OfertaController : ControllerBase
    {
        [HttpPost]
        [Route("RegistrarOferta")]
        public async Task<IActionResult> RegistrarOferta([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            var cli = db.Usuarios.Where(cli => cli.Email == o.ID_RecibeOferta).FirstAsync();
        if (cli != null)
            {
                await db.Ofertas.AddAsync(o);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, o);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }
    }
}
