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

    public class TruequesController : ControllerBase
    {

        [HttpGet]
        [Route("GetReporte")]
        public async Task<IActionResult> GetReportes()
        {
            var reportes = new List<ReporteTrueque>();
            using var db = new OhMyBoatUIServerContext();

            var query = await db.Ofertas.Where(o => o.EstadoOferta == EstadoOferta.Programada ||
                                              o.EstadoOferta == EstadoOferta.Concretada ||
                                              o.EstadoOferta == EstadoOferta.Inconclusa).ToListAsync();

            foreach (Oferta o in query) {
                var reportesito = new ReporteTrueque() { IdOferta = o.Id ,
                                                         EsNavioEnvia = o.EsNavioEnvia,
                                                         EsNavioRecibe = o.EsNavioRecibe,
                                                         ID_EnviaOferta = o.ID_EnviaOferta,
                                                         ID_RecibeOferta = o.ID_RecibeOferta,
                                                         ID_VehiculoEnviaOferta = o.ID_VehiculoEnviaOferta,
                                                         ID_VehiculoRecibeOferta = o.ID_VehiculoRecibeOferta,
                                                         Estado = o.EstadoOferta};

                Vehiculo? vehiculoEnvia = o.EsNavioEnvia ? await db.Maritimos.Where(x => x.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync() : await db.Terrestres.Where(x => x.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                reportesito.Patente_VehiculoEnviaOferta = vehiculoEnvia?.Matricula ?? "";

                Vehiculo? vehiculoRecibe = o.EsNavioRecibe ? await db.Maritimos.Where(x => x.Id == o.ID_VehiculoRecibeOferta).FirstOrDefaultAsync() : await db.Terrestres.Where(x => x.Id == o.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                reportesito.Patente_VehiculoEnviaOferta = vehiculoRecibe?.Matricula ?? "";

                Turno? turnito = await db.Turno.Where( t => t.OfertaId == o.Id).FirstOrDefaultAsync();

                reportesito.FechaTurno = turnito?.FechaTurno ?? DateTime.Now;

                var idSuck = turnito?.SucursalId ?? 0;

                Sucursal? suck = await db.Sucursales.Where(s => s.Id == idSuck).FirstOrDefaultAsync();

                reportesito.Sucursal = suck?.NombreSuck ?? "La Pampa";
                reportes.Add(reportesito);

            }

           return StatusCode(StatusCodes.Status200OK,reportes); 
        }


        [HttpPost]
        [Route("GetReporteCliente")]
        public async Task<IActionResult> GetReportesCliente([FromBody] LoginDTO chiripiorca)
        {
            var reportes = new List<ReporteTrueque>();
            using var db = new OhMyBoatUIServerContext();

            var query = await db.Ofertas.Where(o =>(o.ID_EnviaOferta == chiripiorca.Email || o.ID_RecibeOferta == chiripiorca.Email) && (o.EstadoOferta == EstadoOferta.Programada ||
                                              o.EstadoOferta == EstadoOferta.Concretada ||
                                              o.EstadoOferta == EstadoOferta.Inconclusa)).ToListAsync();

            foreach (Oferta o in query)
            {
                var reportesito = new ReporteTrueque()
                {
                    IdOferta = o.Id,
                    EsNavioEnvia = o.EsNavioEnvia,
                    EsNavioRecibe = o.EsNavioRecibe,
                    ID_EnviaOferta = o.ID_EnviaOferta,
                    ID_RecibeOferta = o.ID_RecibeOferta,
                    ID_VehiculoEnviaOferta = o.ID_VehiculoEnviaOferta,
                    ID_VehiculoRecibeOferta = o.ID_VehiculoRecibeOferta,
                    Estado = o.EstadoOferta
                };

                Vehiculo? vehiculoEnvia = o.EsNavioEnvia ? await db.Maritimos.Where(x => x.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync() : await db.Terrestres.Where(x => x.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                reportesito.Patente_VehiculoEnviaOferta = vehiculoEnvia?.Matricula ?? "";

                Vehiculo? vehiculoRecibe = o.EsNavioRecibe ? await db.Maritimos.Where(x => x.Id == o.ID_VehiculoRecibeOferta).FirstOrDefaultAsync() : await db.Terrestres.Where(x => x.Id == o.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                reportesito.Patente_VehiculoEnviaOferta = vehiculoRecibe?.Matricula ?? "";

                Turno? turnito = await db.Turno.Where(t => t.OfertaId == o.Id).FirstOrDefaultAsync();

                reportesito.FechaTurno = turnito?.FechaTurno ?? DateTime.Now;

                var idSuck = turnito?.SucursalId ?? 0;

                Sucursal? suck = await db.Sucursales.Where(s => s.Id == idSuck).FirstOrDefaultAsync();

                reportesito.Sucursal = suck?.NombreSuck ?? "La Pampa";
                reportes.Add(reportesito);

            }

            return Ok(reportes);
        }
        /*
        [HttpPost]
        [Route("ActualizarEstadoTrueque")]
        public async Task<IActionResult> DiosSoyYoDeNuevo([FromBody] ReporteTrueque reporte)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Trueques.Where(trueq => trueq.Id == reporte.IdTrueque).FirstOrDefaultAsync();

            if (trueque != null)
            {

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
                


            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
        */

    }
}
