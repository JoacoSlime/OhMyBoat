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
        [Route("EliminarOferta")]
        public async Task<IActionResult> EliminarOferta([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            var cli = db.Usuarios.Where(cli => cli.Email == o.ID_RecibeOferta).FirstAsync();
        if (cli != null)
            {
                db.Ofertas.Remove(o);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, o);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }

        [HttpPost]
        [Route("SwitchEstado")]
        public async Task<IActionResult> SwitchEstado([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            var oferta = await db.Ofertas.Where(of => of.Id == o.Id).FirstOrDefaultAsync();
            if (oferta != null){
                oferta.Estado = ! oferta.Estado;
                db.Ofertas.Update(oferta);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
        }


        [HttpPost]
        [Route("AceptarOferta")]
        public async Task<IActionResult> AceptarOferta([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            var oferta = await db.Ofertas.Where(of => of.Id == o.Id).FirstOrDefaultAsync();
            if (oferta != null){
                oferta.Estado = true;
                db.Ofertas.Update(oferta);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
        }

        [HttpPost]
        [Route("ListarOfertasEnviadas")]
        public async Task<IActionResult> ListSentOffers([FromBody] Usuario Email){
            
            using var bd = new OhMyBoatUIServerContext();
            List<Oferta> offers = await bd.Ofertas.Where(o => !o.archivada && o.ID_EnviaOferta == Email.Email.ToLower()).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, offers);

        }

        [HttpPost]
        [Route("ListarOfertasRecibidas")]
        public async Task<IActionResult> ListReceivedOffers([FromBody] Usuario Email){
            
            using var bd = new OhMyBoatUIServerContext();
            List<Oferta> offers = await bd.Ofertas.Where(o => !o.archivada && o.ID_RecibeOferta == Email.Email.ToLower()).ToListAsync();
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
        
        [HttpPost]
        [Route("ChekearOfertaExiste")]
        public async Task<IActionResult> chekearExsiste([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            Oferta? offer = await db.Ofertas.Where
                (of =>!of.archivada && ((of.ID_VehiculoEnviaOferta == o.ID_VehiculoEnviaOferta && of.ID_VehiculoRecibeOferta == o.ID_VehiculoRecibeOferta) ||
                (of.ID_VehiculoEnviaOferta == o.ID_VehiculoRecibeOferta && of.ID_VehiculoRecibeOferta == o.ID_VehiculoEnviaOferta )))
                .FirstOrDefaultAsync();
            if (offer != null)
            {
                return StatusCode(StatusCodes.Status200OK, offer);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }
    }
}
