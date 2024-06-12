using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OhMyBoat.UI.Shared.Entidades;

using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Server.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.Net;

namespace OhMyBoat.UI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TruequesController : ControllerBase {

        [HttpGet]
        [Route("ListarTrueques")]
        public async Task<IActionResult> Get()
        {
            using var db = new OhMyBoatUIServerContext();
            var listTrueques = await db.Trueques.OrderBy(c => c.Id).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listTrueques);
        }

        [HttpPost]
        [Route("AgregarTrueque")]
        public async Task<IActionResult> AgregarTrueque([FromBody] Trueque t)
        {
            using var db = new OhMyBoatUIServerContext();
            await db.Trueques.AddAsync(t);
            await db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("DeleteTrueque")]
        public async Task<IActionResult> AgregDeleteTruequearTrueque([FromBody] Trueque t)
        {
            using var db = new OhMyBoatUIServerContext();
            db.Trueques.Remove(t);
            await db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("GetTrueque")]
        public async Task<IActionResult> GetTrueque([FromBody] Trueque tru)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Trueques.Where(t => t.MaritimoId == tru.MaritimoId && t.VehiculoId == tru.VehiculoId).FirstOrDefaultAsync();
            if (trueque == null) {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            } else {
                return StatusCode(StatusCodes.Status200OK, trueque);
            }
        }

        [HttpPost]
        [Route("SwitchConcretar")]
        public async Task<IActionResult> SwitchConcretar([FromBody] Trueque t)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Trueques.Where(tr => tr.Id == t.Id).FirstOrDefaultAsync();
            if (trueque != null){
                trueque.Concreto = !trueque.Concreto;
                db.Trueques.Update(trueque);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
        }

        [HttpPost]
        [Route("ActualizarEstadoTrueque")]
        public async Task<IActionResult> DiosSoyYoDeNuevo([FromBody] ReporteTrueque reporte)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Trueques.Where(trueq => trueq.Id == reporte.IdTrueque).FirstOrDefaultAsync();

            if (trueque != null) {
                if (reporte.Concreto ?? false)
                {

                    trueque.Concreto= true;
                    db.Update(trueque);
                    await db.SaveChangesAsync();

                    var turno = await db.Turno.Where(t => t.TruequeId == trueque.Id).FirstOrDefaultAsync();
                    if (turno != null)
                    {
                        var oferta = await db.Ofertas.Where(o => o.Id == turno.OfertaId).FirstOrDefaultAsync();
                        if (oferta != null)
                        {
                            if (oferta.EsNavioEnvia)
                            {
                                Maritimo? mari1 = await db.Maritimos.Where(m => m.Id == oferta.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                                if (mari1 != null)
                                {
                                    mari1.Deuda = 0;// parece medio de goma pero bueno
                                    mari1.IDCliente = oferta.ID_RecibeOferta != null ? oferta.ID_RecibeOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(mari1);
                                    await db.SaveChangesAsync();
                                    
                                }
                            }
                            else
                            {
                                Terrestre? ter1 = await db.Terrestres.Where(t => t.Id == oferta.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                                if (ter1 != null)
                                {

                                    ter1.IDCliente = oferta.ID_RecibeOferta != null ? oferta.ID_RecibeOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(ter1);
                                    await db.SaveChangesAsync();
                                    
                                }
                            }
                            if (oferta.EsNavioRecibe)
                            {
                                Maritimo? mari2 = await db.Maritimos.Where(m => m.Id == oferta.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                                if (mari2 != null)
                                {
                                    mari2.Deuda = 0;
                                    mari2.IDCliente = oferta.ID_EnviaOferta != null ? oferta.ID_EnviaOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(mari2);
                                    await db.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                Terrestre? ter2 = await db.Terrestres.Where(t => t.Id == oferta.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                                if (ter2 != null)
                                {
                                    ter2.IDCliente = oferta.ID_EnviaOferta != null ? oferta.ID_EnviaOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(ter2);
                                    await db.SaveChangesAsync();
                                }
                            }
                            return StatusCode(StatusCodes.Status200OK);
                        }
                    }
                }
                else
                {
                    trueque.Concreto = false;
                    db.Update(trueque);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK);
                }
                        
                    
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }

}

