using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Services;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Shared.Entidades;
using Org.BouncyCastle.Asn1.Iana;
using System.Security.Cryptography;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{  
    [Route("api/[controller]")]
        [ApiController]

    public class TurnosController : Controller
    {

        private readonly EmailService _emailService;

        public TurnosController(EmailService emailService)
        {
            _emailService = emailService;
        }

        private async Task<bool>VerificarTurnoDisponible(Turno turno) // tira false si no se superpone
        {
            if (turno == null)
            {
                return false;
            }
            else
            {
                using (var db = new OhMyBoatUIServerContext())
                {
                    return await db.Turno.Where(t => (t.TruequeId != null && 
                                                        t.SucursalId == turno.SucursalId && 
                                                        t.FechaTurno.Year == turno.FechaTurno.Year && 
                                                        t.FechaTurno.Month == turno.FechaTurno.Month && 
                                                        t.FechaTurno.Day == turno.FechaTurno.Day && 
                                                        t.FechaTurno.Hour == turno.FechaTurno.Hour && 
                                                        t.FechaTurno.Minute == turno.FechaTurno.Minute)).AnyAsync();
                }
            }
        } 
        private async Task<List<Turno>?> ObtenerTurnosReservados(DateTime dia, Sucursal suc)
        {
            using ( var db = new OhMyBoatUIServerContext())
            {
                return await db.Turno.Where(t => (t.TruequeId != null && t.SucursalId == suc.Id && t.FechaTurno.Year == dia.Year && t.FechaTurno.Month == dia.Month && t.FechaTurno.Day == dia.Day)).ToListAsync();          
            }
           // si no funciona hacer que esto devuelva null y listo            
        }
        private async Task<List<Turno>?> ObtenerTurnosDisponiblesSinReservados(List<Turno>todosLosTurnosDelDia, DateTime dia, Sucursal suc) 
        {
            var turnosReservados = await ObtenerTurnosReservados(dia, suc);
            if(turnosReservados!= null)
            {
                foreach (Turno turnoReservado in turnosReservados)
                {
                    var posibleTurnoPisado = todosLosTurnosDelDia.Where(t => t.FechaTurno.Hour == turnoReservado.FechaTurno.Hour && t.FechaTurno.Minute == turnoReservado.FechaTurno.Minute).FirstOrDefault();
                    if(posibleTurnoPisado != null)
                            todosLosTurnosDelDia.Remove(posibleTurnoPisado); // pido los turnos ocupados del dia y voy sacando de a uno los turnos reservados
                }                
            }            
            return todosLosTurnosDelDia;
            
        }
        private async Task <List<Turno>> obtenerTurnosDisponibles(DateTime dia, Sucursal suc) // 9am a 6pm los horarios de trabajo
        {
            return await Task.Run( () => {

                    if (dia.DayOfWeek == DayOfWeek.Sunday)
                    {
                        return new List<Turno>();
                    }
                    else if (dia.DayOfWeek == DayOfWeek.Saturday) // 8 a 12 hs (30 min x turno, 8 turnos)
                    {
                        var turnosSabado = new List<Turno>();
                        var diaTemp = new DateTime(dia.Year, dia.Month, dia.Day, 8, 0, 0); 
                        for (int i = 0; i < 8; i++)
                        {
                            turnosSabado.Add(new Turno() { FechaTurno = diaTemp.AddMinutes(i * 30) });
                        }
                        // si quisiese sacar los turnos reservados seria en esta linea
                        return turnosSabado;
                    }
                    else // si no es domingo o sabado, osea si es dia de semana  9am a 6pm = 9 hs = 18 turnos
                    {
                        var turnosSemana = new List<Turno>();
                        var diaTemp = new DateTime(dia.Year, dia.Month, dia.Day, 9, 0, 0); 
                        for (int i = 0; i < 18; i++)
                        {
                            turnosSemana.Add(new Turno() { FechaTurno = diaTemp.AddMinutes(i * 30) });
                        }
                        // si quisiese sacar los turnos reservados seria en esta linea
                        return turnosSemana;
                    }
                }
            );
        }

       // private List<Turno> obtenerTurnosReservadosSucursal() { return new List<Turno>(); } // simulo que no hay nada

        [HttpGet]
        [Route("ObtenerSucursales")]
        public async Task<IActionResult> ObtenerSucursales()
        {
            using var db = new OhMyBoatUIServerContext();
            var sucursales = await db.Sucursales.ToListAsync();
            if (sucursales != null)
            {
                return StatusCode(StatusCodes.Status200OK, sucursales);
            }
            else return StatusCode(StatusCodes.Status403Forbidden, null);
        }

        [HttpPost]
        [Route("ObtenerSucursal")]
        public async Task<IActionResult> ObtenerSucursal(Turno t)
        {
            using var db = new OhMyBoatUIServerContext();
            var sucursales = await db.Sucursales.Where(s => s.Id == t.SucursalId).FirstOrDefaultAsync();
            if (sucursales != null)
            {
                return StatusCode(StatusCodes.Status200OK, sucursales);
            }
            else return StatusCode(StatusCodes.Status403Forbidden, null);
        }

        [HttpPost]
        [Route("VerificarDisponibilidadDia")]
        public async Task<IActionResult> ObtenerHorarios([FromBody] ConsultaHorariosDTO tapioca)
        {
            var horarios = await obtenerTurnosDisponibles(tapioca.dia, tapioca.suc);
            if (horarios != null)               
                return StatusCode(StatusCodes.Status200OK, horarios);            
            else return StatusCode(StatusCodes.Status403Forbidden, null);
        }

        [HttpPost]
        [Route("EnviarTurnosPropuesta")]
        public async Task<IActionResult> enviarTurnos([FromBody] List<Turno> turnos)
        {
            if(turnos.Count >=1 && turnos.Count <= 3)
            {
                var numero = turnos.First().OfertaId;
                bool todoOk = true;
                foreach (var turno in turnos)
                {
                    if(!await VerificarTurnoDisponible(turno))
                    {
                        if (turno.OfertaId != numero)
                        {
                            todoOk = false; // por si algun vivo quiere explotar la api 
                        }
                    }
                    else return StatusCode(StatusCodes.Status406NotAcceptable, null); // si por X motivo se agarran el turno mientras confirman la seleccion

                }
                if (!todoOk)
                    return StatusCode(StatusCodes.Status409Conflict, null); // algun hdp tocando la api
                else
                {
                    using (var db = new OhMyBoatUIServerContext())
                    {
                        foreach (var turno in turnos)
                        {                            
                                await db.Turno.AddAsync(turno);                            
                        }                            
                       await db.SaveChangesAsync(); // explota aca por algun motivo                       
                    }
                    return StatusCode(StatusCodes.Status200OK, null);
                }
            }
            return StatusCode(StatusCodes.Status412PreconditionFailed, null); // si mandan  0 o 30 turnos porque si
        }

        [HttpGet]
        [Route("ListarTurnos")]
        public async Task<IActionResult> Get()
        {
            using var db = new OhMyBoatUIServerContext();
            var listTurnos = await db.Turno.Where( t => t.TruequeId != null).OrderBy(t => t.Id).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listTurnos);
        }
        
        [HttpPost]
        [Route("GetTurnos")]
        public async Task<IActionResult> GetTurnos(Oferta ofert) {
            using var db = new OhMyBoatUIServerContext();
            var turno = await db.Turno.Where(turno => turno.OfertaId == ofert.Id).ToListAsync();
            if (turno.IsNullOrEmpty()) {
                return StatusCode(403, null);
            }
            return StatusCode(200, turno);
        }
        
        [HttpPost]
        [Route("EliminarTurno")]
        public async Task<IActionResult> EliminarOferta([FromBody] Oferta o)
        {
            using var db = new OhMyBoatUIServerContext();
            var turn = await db.Turno.Where(turn => turn.OfertaId == o.Id).ToListAsync();
            if (turn != null)
            {
                foreach (Turno t in turn )
                {
                    db.Turno.Remove(t);
                }
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, turn);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }

        [HttpPost]
        [Route("SelectTurno")]
        public async Task<IActionResult> SelectTurno([FromBody] Turno turno) {
            using var db = new OhMyBoatUIServerContext();
            var turn = await db.Turno.Where(turn => turn.OfertaId == turno.OfertaId).ToListAsync();
            if (turn != null)
            {
                Turno turnoNuevo = new();
                foreach (Turno t in turn)
                {
                    if (t.Id != turno.Id)
                    db.Turno.Remove(t);
                    else 
                    turnoNuevo=t;
                }
                turnoNuevo.TruequeId = turno.TruequeId;
                db.Update(turnoNuevo);
                await db.SaveChangesAsync();

                Oferta ofertaRelacionada = await db.Ofertas.Where(ofer => ofer.Id == turno.OfertaId).FirstOrDefaultAsync() ?? new(); 
                String emailEnvia = ofertaRelacionada.ID_EnviaOferta ?? "";
                String emailRecibe = ofertaRelacionada.ID_RecibeOferta ?? "";
                _ = _emailService.Send(
                    to: emailEnvia,
                    subject: "¡Tu trueque tiene un turno preparado!",
                    html: $@"<h2>Nuevo turno</h2>
                    <p>Tu turno es el {turnoNuevo.FechaTurno.ToString("dd/mm/yyyy")} a las {turnoNuevo.FechaTurno.ToString("HH:mm")}<p/>
                    <br/>
                    <p>Para revisar tus trueque pendiente, ingresa a tus Ofertas Enviadas</p>"
                );
                _ = _emailService.Send(
                    to: emailRecibe,
                    subject: "¡Tu trueque tiene un turno preparado!",
                    html: $@"<h2>Nuevo turno</h2>
                    <p>Tu turno es el {turnoNuevo.FechaTurno.ToString("dd/mm/yyyy")} a las {turnoNuevo.FechaTurno.ToString("HH:mm")}<p/>
                    <br/>
                    <p>Para revisar tus trueque pendiente, ingresa a tus Ofertas Recibidas</p>"
                );
                return StatusCode(StatusCodes.Status200OK, turn);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }

    }
}
