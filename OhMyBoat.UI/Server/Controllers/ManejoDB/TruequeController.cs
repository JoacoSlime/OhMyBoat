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
        //AgregarTrueque
        [HttpPost]
        [Route("AgregarTrueque")]
        public async Task<IActionResult> AgregarTrueque([FromBody] Oferta ofe){
            using var db = new OhMyBoatUIServerContext();
            var ofeDB = await db.Ofertas.Where(o => o.Id == ofe.Id).FirstOrDefaultAsync();
            if (ofeDB != null)
            {
                ofeDB.EstadoOferta = EstadoOferta.Programada;
                db.Update(ofeDB);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

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
                reportesito.Patente_VehiculoRecibeOferta = vehiculoRecibe?.Matricula ?? "";

                Turno? turnito = await db.Turno.Where( t => t.OfertaId == o.Id).FirstOrDefaultAsync();

                reportesito.FechaTurno = turnito?.FechaTurno ?? DateTime.Now;

                var idSuck = turnito?.SucursalId ?? 0;

                Sucursal? suck = await db.Sucursales.Where(s => s.Id == idSuck).FirstOrDefaultAsync();

                reportesito.Sucursal = suck?.NombreSuck ?? "Punta indio";
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
                reportesito.Patente_VehiculoRecibeOferta = vehiculoRecibe?.Matricula ?? "";

                Turno? turnito = await db.Turno.Where(t => t.OfertaId == o.Id).FirstOrDefaultAsync();

                reportesito.FechaTurno = turnito?.FechaTurno ?? DateTime.Now;

                var idSuck = turnito?.SucursalId ?? 0;

                Sucursal? suck = await db.Sucursales.Where(s => s.Id == idSuck).FirstOrDefaultAsync();

                reportesito.Sucursal = suck?.NombreSuck ?? "La Pampa";
                reportes.Add(reportesito);

            }

            return StatusCode(StatusCodes.Status200OK, reportes);
        }
        
        [HttpPost]
        [Route("ActualizarEstadoTrueque")]
        public async Task<IActionResult> DiosSoyYoDeNuevo([FromBody] ReporteTrueque reporte)
        {
            using var db = new OhMyBoatUIServerContext();
            
                    var oferta = await db.Ofertas.Where(o =>(o.Id == reporte.IdOferta) && (o.EstadoOferta==EstadoOferta.Programada)).FirstOrDefaultAsync();
                    if (oferta == null)
                        return StatusCode(StatusCodes.Status200OK);
                    if (reporte.Estado == EstadoOferta.Concretada)
                    {
                        oferta.EstadoOferta = EstadoOferta.Concretada;
                        db.Update(oferta);
                        await db.SaveChangesAsync();
                      
                            if (reporte.EsNavioEnvia)
                            {
                                Maritimo? mari1 = await db.Maritimos.Where(m => m.Id == reporte.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                                if (mari1 != null)
                                {
                                    mari1.Deuda = 0;// parece medio de goma pero bueno
                                    mari1.IDCliente = reporte.ID_RecibeOferta != null ? reporte.ID_RecibeOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(mari1);
                                    await db.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                Terrestre? ter1 = await db.Terrestres.Where(t => t.Id == reporte.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                                if (ter1 != null)
                                {

                                    ter1.IDCliente = reporte.ID_RecibeOferta != null ? reporte.ID_RecibeOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(ter1);
                                    await db.SaveChangesAsync();

                                }
                            }
                            if (reporte.EsNavioRecibe)
                            {
                                Maritimo? mari2 = await db.Maritimos.Where(m => m.Id == reporte.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                                if (mari2 != null)
                                {
                                    mari2.Deuda = 0;
                                    mari2.IDCliente = reporte.ID_EnviaOferta != null ? reporte.ID_EnviaOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(mari2);
                                    await db.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                Terrestre? ter2 = await db.Terrestres.Where(t => t.Id == reporte.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                                if (ter2 != null)
                                {
                                    ter2.IDCliente = reporte.ID_EnviaOferta != null ? reporte.ID_EnviaOferta : "ERROR DE TRASLADO DE IDS";
                                    db.Update(ter2);
                                    await db.SaveChangesAsync();
                                }
                            }
                            return StatusCode(StatusCodes.Status200OK);
                    }else
                    {
                        oferta.EstadoOferta = EstadoOferta.Inconclusa;
                        db.Update(oferta);
                        await db.SaveChangesAsync();
                    }
           return StatusCode(StatusCodes.Status200OK);
        }                       


         
            
        }
    }
        

