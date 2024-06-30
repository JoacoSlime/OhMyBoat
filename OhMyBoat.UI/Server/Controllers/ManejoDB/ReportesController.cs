using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Server.Services;
using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Shared.Entidades;
using BlazorBootstrap;
using System.Collections.Immutable;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        [HttpPost]
        [Route("GetReportenaviosDeudoresSucursal")]
        public async Task<IActionResult> GetReporteNaviosDeudoresSucursal([FromBody] Sucursal sucursal )
        {
           
            using var db = new OhMyBoatUIServerContext();
            List<double> dataList = new();
            foreach(TipoEmbarcacion tipo in Enum.GetValues(typeof(TipoEmbarcacion))) { 
                dataList.Add(await db.Maritimos.Where(maritimo => maritimo.Tipo == tipo && maritimo.SucursalId == sucursal.Id && maritimo.Deuda > 0).CountAsync());
            }
            return StatusCode(StatusCodes.Status200OK, dataList);
        }

        [HttpGet]
        [Route("GetReporteCantidadNaviosPorSucursal")]
        public async Task<IActionResult> GetReporteCantidadNaviosPorSucursal() {
            using var db = new OhMyBoatUIServerContext();
            List<double> dataList = new();
            foreach(Sucursal s in await db.Sucursales.ToListAsync()) { // Esto es una mierda parte 2 -J
                dataList.Add(await db.Maritimos.Where(maritimo => maritimo.SucursalId == s.Id).CountAsync());
            }
            return StatusCode(StatusCodes.Status200OK, dataList);
        }

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
        
        [HttpGet]
        [Route("GetReporteTipoDeEmbarcacion")]
        public async Task<IActionResult> GetReporteTipoDeEmbarcacion() {
            using var db = new OhMyBoatUIServerContext();
            List<double> dataList = new();
            foreach(TipoEmbarcacion tipo in Enum.GetValues(typeof(TipoEmbarcacion))) { // Esto es una mierda -J
                dataList.Add(await db.Maritimos.Where(maritimo => maritimo.Tipo == tipo).CountAsync());
            }
            return StatusCode(StatusCodes.Status200OK, dataList);

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
        [HttpPost]
        [Route("ReporteTruequesClientes")]

        private async Task<IActionResult> ReporteClientesTrueques([FromBody] RangoDTO rango)
        {
            using var db = new OhMyBoatUIServerContext();
            var turnosQueVan = await db.Turno.Where(t => DateOnly.FromDateTime(t.FechaTurno) >= rango.inicio && DateOnly.FromDateTime(t.FechaTurno) <= rango.fin).ToListAsync();
            List<Oferta> ofertasFiltradas = new();

            var ofertasSinFiltrar = await db.Ofertas.Where(o => o.EstadoOferta == EstadoOferta.Concretada ||
                                                          o.EstadoOferta == EstadoOferta.Inconclusa || o.EstadoOferta == EstadoOferta.Programada).ToListAsync();
            if (ofertasSinFiltrar != null)
                foreach (Turno t in turnosQueVan)
                {
                    Oferta? o = ofertasSinFiltrar.Where(o => o.Id == t.OfertaId).FirstOrDefault();
                    if (o != null)
                        ofertasFiltradas.Add(o);
                }
            var usuarios = await db.Usuarios.Where(u => u.Rol == Roles.cliente).ToListAsync();

            Dictionary<String, int> diccionarioCremoso = new();
            foreach (Usuario u in usuarios)
            {
                int cantidad = ofertasFiltradas.Where(o => o.ID_EnviaOferta == u.Email || o.ID_RecibeOferta == u.Email).Count();
                if (cantidad > 1)
                {
                    diccionarioCremoso.Add(u.Email, cantidad);
                }
            }
            return StatusCode(StatusCodes.Status200OK, diccionarioCremoso);
        }

        [HttpPost]
        [Route("ReportarNaviosMasUsados")]
        private async Task<IActionResult> ReporteNaviosMasUsados([FromBody] RangoDTO rango)
        {
            using var db = new OhMyBoatUIServerContext();
            var turnosQueVan = await db.Turno.Where(t => DateOnly.FromDateTime(t.FechaTurno) >= rango.inicio && DateOnly.FromDateTime(t.FechaTurno) <= rango.fin).ToListAsync();
            List<Oferta> ofertasFiltradas = new();

            var ofertasSinFiltrar = await db.Ofertas.Where(o => o.EstadoOferta == EstadoOferta.Concretada ||
                                                          o.EstadoOferta == EstadoOferta.Inconclusa || o.EstadoOferta == EstadoOferta.Programada).ToListAsync();
            if (ofertasSinFiltrar != null)
                foreach (Turno t in turnosQueVan)
                {
                    Oferta? o = ofertasSinFiltrar.Where(o => o.Id == t.OfertaId).FirstOrDefault();
                    if (o != null)
                        ofertasFiltradas.Add(o);
                }
            Dictionary<String, int> diccionarioCremoso = new();
            foreach(var t in Enum.GetNames(typeof(TipoEmbarcacion)))
            {
                if(t!=null)
                    diccionarioCremoso.Add(t,0);// agrego todo en 0
            }

            foreach (Oferta o in ofertasFiltradas)
            {
                if (o.EsNavioEnvia)
                {
                    Maritimo? si = await db.Maritimos.Where(m => m.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync();
                    if(si != null)
                        {
                            if (diccionarioCremoso.ContainsKey(si.Tipo.ToString()))
                            {

                            }
                        }
                        
                }
                if (o.EsNavioRecibe)
                {

                }
            }
        }
    }
}
   