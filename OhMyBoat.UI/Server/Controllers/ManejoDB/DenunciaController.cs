using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Server.Services;
using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Shared.Entidades;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class DenunciaController : ControllerBase
    {

        [HttpPost]
        [Route("CargarDenuncia")]
        public async Task<IActionResult> CargarDenuncia([FromBody] Denuncia d)
        {
            using var db = new OhMyBoatUIServerContext();
            if (db.Denuncias.Where(den => den.VehiculoId == d.VehiculoId  && den.EsNavio == d.EsNavio && den.ClienteId == d.ClienteId).IsNullOrEmpty())
            {
                await db.Denuncias.AddAsync(d);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, d);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }

        [HttpPost]
        [Route("EliminarDenuncia")]
        public async Task<IActionResult> EliminarDenuncia([FromBody] Denuncia d)
        {
            using var db = new OhMyBoatUIServerContext();
            List<Denuncia> DenunciasEliminar = await db.Denuncias.Where(den => den.Id == d.Id && den.EsNavio == d.EsNavio).ToListAsync();
            if (DenunciasEliminar != null)
            {
                foreach (Denuncia dnuncia in DenunciasEliminar)
                {
                db.Denuncias.Remove(dnuncia);       // si queres logico modificalo papito lo hago a las apuradas esto
                }
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, d);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }

        [HttpGet]
        [Route("GetDenuncias")]
        public async Task<IActionResult> GetDenuncias()
        {
            using var bd = new OhMyBoatUIServerContext();
            List<Denuncia> lista_Denuncias = await bd.Denuncias.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, lista_Denuncias);
        }
    }
}
