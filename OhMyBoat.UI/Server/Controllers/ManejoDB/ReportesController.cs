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
    public class ReportesController : ControllerBase
    {

        [HttpGet]
        [Route("GetReporteTipoDeTerrestre")]
        public async Task<IActionResult> GetReporteTipoDeTerrestre() {
            using var db = new OhMyBoatUIServerContext();
            List<double> dataList = new();
            foreach(TipoVehiculo tipo in Enum.GetValues(typeof(TipoVehiculo))) {
                dataList.Add(await db.Terrestres.Where(t => t.Tipo == tipo).CountAsync());
            }
            return StatusCode(StatusCodes.Status200OK, dataList);
        }
        
        [HttpPost]
        [Route("TruequesInconclusosPorSede")]
        public async Task<IActionResult> TruequesNoConcluidosPorSede([FromBody] RangoDTO rango)
        {
            DateTime inicio = new DateTime(rango.inicio.Year, rango.inicio.Month, rango.inicio.Day);
            DateTime fin = new DateTime(rango.fin.Year, rango.fin.Month, rango.fin.Day);

            using var db = new OhMyBoatUIServerContext();

        /*
            var TurnosTotales = await db.Turno.Where
            (t => (t.FechaTurno.Year > inicio.Year || (t.FechaTurno.Year == inicio.Year && (t.FechaTurno.Month > inicio.Month || (t.FechaTurno.Month == inicio.Month && t.FechaTurno.Day >= inicio.Day))))
            && (t.FechaTurno.Year < fin.Year || (t.FechaTurno.Year == fin.Year && (t.FechaTurno.Month < fin.Month || (t.FechaTurno.Month == fin.Month && t.FechaTurno.Day <= fin.Day))))).ToListAsync();
            // este filtrado demencial lo tengo que hacer si o si porque sino el entity framework explota pq no puedo usar operadores de fechas ni llamar a una funcion, que paja
        */

            //y no funca asi?
            var TurnosTotales = await db.Turno.Where
            (t => (t.FechaTurno >= inicio) && (t.FechaTurno <= fin)).ToListAsync();
            
            var TruequesInconclusos = await db.Ofertas.Where
            (o => o.EstadoOferta == EstadoOferta.Inconclusa).ToListAsync();

            List<Turno> TurnosOfertasInconclusas = new();

            //Simulo un Inner Join OFERTA_INCONCLUSA x TURNO_EN_RANGO
            if (TruequesInconclusos != null) {
                foreach (Oferta inconcluso in TruequesInconclusos)
                {
                    Turno? t = TurnosTotales.Where(turno => turno.OfertaId == inconcluso.Id).FirstOrDefault();
                    if (t != null) TurnosOfertasInconclusas.Add(t);
                }
            }

        /*
            Hay que conseguir los TURNOS de OFERTAS INCONCLUSAS, porque es TURNOS
            el que tiene el IDSucursal que nos llevara al nombre de la SUCURSAL
        */

            List<Sucursal> SucursalesTotales = new();
            SucursalesTotales = await db.Sucursales.ToListAsync();

            Dictionary<String, int> diccionario = new();
            foreach (var s in SucursalesTotales)
            {
                diccionario.Add(s.NombreSuck, 0);
            }

            foreach (Turno t in TurnosOfertasInconclusas)
            {
                var mi_sucursal = SucursalesTotales.Where(s => s.Id == t.SucursalId).FirstOrDefault();
                if (mi_sucursal != null){
                    if (diccionario.ContainsKey(mi_sucursal.NombreSuck)) {
                            diccionario[mi_sucursal.NombreSuck] = diccionario[mi_sucursal.NombreSuck] + 1;
                    }
                }
            }

            return StatusCode(StatusCodes.Status200OK, diccionario);
        }


        [HttpPost]
        [Route("TruequesConcretadosPorSede")]
        public async Task<IActionResult> TruequesConcretadosPorSede([FromBody] RangoDTO rango)
        {
            DateTime inicio = new DateTime(rango.inicio.Year, rango.inicio.Month, rango.inicio.Day);
            DateTime fin = new DateTime(rango.fin.Year, rango.fin.Month, rango.fin.Day);

            using var db = new OhMyBoatUIServerContext();

            var TurnosTotales = await db.Turno.Where
            (t => (t.FechaTurno >= inicio) && (t.FechaTurno <= fin)).ToListAsync();
            
            var TruequesConcretados = await db.Ofertas.Where
            (o => o.EstadoOferta == EstadoOferta.Concretada).ToListAsync();

            List<Turno> TurnosOfertasConcretadas = new();

            if (TruequesConcretados != null) {
                foreach (Oferta concretado in TruequesConcretados)
                {
                    Turno? t = TurnosTotales.Where(turno => turno.OfertaId == concretado.Id).FirstOrDefault();
                    if (t != null) TurnosOfertasConcretadas.Add(t);
                }
            }

            List<Sucursal> SucursalesTotales = new();
            SucursalesTotales = await db.Sucursales.ToListAsync();

            Dictionary<String, int> diccionario = new();
            foreach (var s in SucursalesTotales)
            {
                diccionario.Add(s.NombreSuck, 0);
            }

            foreach (Turno t in TurnosOfertasConcretadas)
            {
                var mi_sucursal = SucursalesTotales.Where(s => s.Id == t.SucursalId).FirstOrDefault();
                if (mi_sucursal != null){
                    if (diccionario.ContainsKey(mi_sucursal.NombreSuck)) {
                            diccionario[mi_sucursal.NombreSuck] = diccionario[mi_sucursal.NombreSuck] + 1;
                    }
                }
            }

            return StatusCode(StatusCodes.Status200OK, diccionario);
        }





    }
}