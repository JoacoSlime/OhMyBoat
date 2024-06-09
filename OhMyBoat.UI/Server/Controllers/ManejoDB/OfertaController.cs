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

        [HttpPost]
        [Route("ListarOfertasEnviadas")]
        public async Task<IActionResult> ListSentOffers([FromBody] string Email){
            
            using var bd = new OhMyBoatUIServerContext();
            List<Oferta> offers = await bd.Ofertas.Where(o => o.ID_EnviaOferta == Email.ToLower()).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, offers);

        }

        [HttpPost]
        [Route("ListarOfertasRecibidas")]
        public async Task<IActionResult> ListReceivedOffers([FromBody] string Email){
            
            using var bd = new OhMyBoatUIServerContext();
            List<Oferta> offers = await bd.Ofertas.Where(o => o.ID_RecibeOferta == Email.ToLower()).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, offers);

        }

        [HttpPost]
        [Route("GetOferta")]
        public async Task<IActionResult> GetOferta([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            Oferta? offer = await db.Ofertas.Where(of => of.Id == o.Id).FirstOrDefaultAsync();
            if (offer != null)
            {
                return StatusCode(StatusCodes.Status200OK, offer);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }
    }
}
