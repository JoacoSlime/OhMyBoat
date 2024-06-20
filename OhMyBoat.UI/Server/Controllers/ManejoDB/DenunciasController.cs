using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Server.Services;
using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Shared.Entidades;
using System.Text.RegularExpressions;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB

{
    [Route("api/[controller]")]
    [ApiController]
    public class DenunciasController : ControllerBase
    {
        [HttpGet]
        [Route("GetDenuncias")]
        public async Task<IActionResult> QuieroSecso()
        {
            using var db = new OhMyBoatUIServerContext();
            var querysexosa = await db.Denuncias.GroupBy(denu => new { denu.VehiculoId, denu.EsNavio },
                                                                                    den => den, (aborto, denuncias) => new DenunciasDTO
                                                                                    {
                                                                                        Cantidad = denuncias.Count(),
                                                                                        VehiculoId = aborto.VehiculoId,
                                                                                        EsNavio = aborto.EsNavio,
                                                                                        
                                                                                    }).ToListAsync();
            if(querysexosa != null)
                return StatusCode(StatusCodes.Status200OK, querysexosa);
            else
                return StatusCode(StatusCodes.Status200OK, new List<DenunciasDTO>());

        }
    }
}
