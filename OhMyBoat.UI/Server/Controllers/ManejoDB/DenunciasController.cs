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
            if (querysexosa != null)
                return StatusCode(StatusCodes.Status200OK, querysexosa);
            else
                return StatusCode(StatusCodes.Status200OK, new List<DenunciasDTO>());

        }
        [HttpPost]
        [Route("EliminarPublicacion")]
        public async Task<IActionResult> DenunciadoPapi([FromBody] DenunciasDTO dto)
        {
            using var db = new OhMyBoatUIServerContext();

            Vehiculo? vehiculo = dto.EsNavio ? await db.Maritimos.Where(m => m.Id == dto.VehiculoId).FirstOrDefaultAsync() : await db.Terrestres.Where(t => t.Id == dto.VehiculoId).FirstOrDefaultAsync(); // ver si se borra bien
            if (vehiculo != null)
            {
                var ofertas = await db.Ofertas.Where(o => (o.ID_VehiculoRecibeOferta == dto.VehiculoId && o.EsNavioRecibe == dto.EsNavio) ||
                                                          (o.ID_VehiculoEnviaOferta == dto.VehiculoId && o.EsNavioEnvia == dto.EsNavio)).ToListAsync();
                if (ofertas != null)
                {
                    foreach (var oferta in ofertas)
                    {
                        var turnitos = await db.Turno.Where(t => t.OfertaId == oferta.Id).ToListAsync();
                        if (turnitos != null)
                        {
                            foreach (var turno in turnitos)
                            {
                                db.Remove(turno);
                                await db.SaveChangesAsync();
                            }
                        }
                        db.Remove(oferta);
                        await db.SaveChangesAsync();
                    }
                }
                db.Remove(vehiculo);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }


    }
}
