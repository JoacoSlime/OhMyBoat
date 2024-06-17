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
        [Route("AceptarOferta")]
        public async Task<IActionResult> AceptarOferta([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            var oferta = await db.Ofertas.Where(of => of.Id == o.Id).FirstOrDefaultAsync();
            if (oferta != null){
                oferta.EstadoOferta = EstadoOferta.Aceptada;
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
            List<Oferta> offers = await bd.Ofertas.Where(o => (!o.EstadoOferta.Equals(EstadoOferta.Concretada) || !o.EstadoOferta.Equals(EstadoOferta.Inconclusa)) && o.ID_EnviaOferta == Email.Email.ToLower()).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, offers);

        }

        [HttpPost]
        [Route("ListarOfertasRecibidas")]
        public async Task<IActionResult> ListReceivedOffers([FromBody] Usuario Email){
            
            using var bd = new OhMyBoatUIServerContext();
            List<Oferta> offers = await bd.Ofertas.Where(o => (!o.EstadoOferta.Equals(EstadoOferta.Concretada) || !o.EstadoOferta.Equals(EstadoOferta.Inconclusa)) && o.ID_RecibeOferta == Email.Email.ToLower()).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, offers);

        }

        [HttpPost]
        [Route("ListarTrueques")]
        public async Task<IActionResult> ListOffers(){
            
            using var bd = new OhMyBoatUIServerContext();
            List<Oferta> offers = await bd.Ofertas.Where(o => (o.EstadoOferta.Equals(EstadoOferta.Concretada) || o.EstadoOferta.Equals(EstadoOferta.Inconclusa) || o.EstadoOferta.Equals(EstadoOferta.Programada))).ToListAsync();
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
                (of =>!of.EstadoOferta.Equals(EstadoOferta.Enviada) && ((of.ID_VehiculoEnviaOferta == o.ID_VehiculoEnviaOferta && of.ID_VehiculoRecibeOferta == o.ID_VehiculoRecibeOferta) ||
                (of.ID_VehiculoEnviaOferta == o.ID_VehiculoRecibeOferta && of.ID_VehiculoRecibeOferta == o.ID_VehiculoEnviaOferta )))
                .FirstOrDefaultAsync();
            if (offer != null)
            {
                return StatusCode(StatusCodes.Status200OK, offer);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }

        [HttpPost]
        [Route("ActualizarEstadoOferta")]
        public async Task<IActionResult> DiosSoyYoDeNuevo([FromBody] ReporteTrueque reporte)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Ofertas.Where(o => o.Id == reporte.IdOferta).FirstOrDefaultAsync();

            if (trueque != null) {

                    var turno = await db.Turno.Where(t => t.TruequeId == trueque.Id).FirstOrDefaultAsync();
                    if (turno != null)
                    {
                        var oferta = await db.Ofertas.Where(o => o.Id == turno.OfertaId).FirstOrDefaultAsync();
                        if (oferta != null)
                        {
                            oferta.archivada = true;
                            db.Update(oferta);
                            await db.SaveChangesAsync();

                            trueque.Concreto = reporte.Concreto ?? false;
                            db.Update(trueque);
                            await db.SaveChangesAsync();

                            if (reporte.Concreto ?? false)
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
                             return StatusCode(StatusCodes.Status200OK);
                    }
                }
                /*
                else
                {
                    trueque.Concreto = false;
                    db.Update(trueque);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK);
                }
                */
                        
                    
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}

