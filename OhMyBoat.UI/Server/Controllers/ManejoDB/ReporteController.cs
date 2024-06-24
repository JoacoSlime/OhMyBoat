using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Shared.Entidades;
using System.ComponentModel;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/reporte")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        [HttpGet]
        [Route("a")]
        public async Task<IActionResult> a()
        {
            return StatusCode(StatusCodes.Status200OK, new { gordas = "si porfavor", lama = "mentoplus" });
        }

        private bool pijaParaQueElEntityFrameworkNoTireExcepcion(DateTime turno, DateTime inicio, DateTime fin)
        {
            return true;
        }

        [HttpPost]
        [Route("ReporteCliente")]
        public async Task<IActionResult> reporteCliente([FromBody] RangoDTO rango)
        {
            DateTime inicio = new DateTime(rango.inicio.Year, rango.inicio.Month, rango.inicio.Day);
            DateTime fin = new DateTime(rango.fin.Year, rango.fin.Month, rango.fin.Day);
            
            using var db = new OhMyBoatUIServerContext();           
            var turnosQueVan = await db.Turno.Where(turno => (turno.FechaTurno.Year>inicio.Year||(turno.FechaTurno.Year == inicio.Year && (turno.FechaTurno.Month>inicio.Month || (turno.FechaTurno.Month==inicio.Month && turno.FechaTurno.Day>=inicio.Day)))) &&
                                                        (turno.FechaTurno.Year < fin.Year || (turno.FechaTurno.Year == fin.Year && (turno.FechaTurno.Month < fin.Month || (turno.FechaTurno.Month == fin.Month && turno.FechaTurno.Day <= fin.Day))))).ToListAsync(); // este filtrado demencial lo tengo que hacer si o si porque sino el entity framework explota pq no puedo usar operadores de fechas ni llamar a una funcion, que paja
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
                if (cantidad >= 1)
                {
                    diccionarioCremoso.Add(u.Email, cantidad);
                }
            }
            return StatusCode(StatusCodes.Status200OK, diccionarioCremoso);
        }

        [HttpPost]
        [Route("ReportarNaviosMasUsados")]
        public async Task<IActionResult> ReporteNaviosMasUsados([FromBody] RangoDTO rango)
        {
            DateTime inicio = new DateTime(rango.inicio.Year, rango.inicio.Month, rango.inicio.Day);
            DateTime fin = new DateTime(rango.fin.Year, rango.fin.Month, rango.fin.Day);

            using var db = new OhMyBoatUIServerContext();
            var turnosQueVan = await db.Turno.Where(turno => (turno.FechaTurno.Year > inicio.Year || (turno.FechaTurno.Year == inicio.Year && (turno.FechaTurno.Month > inicio.Month || (turno.FechaTurno.Month == inicio.Month && turno.FechaTurno.Day >= inicio.Day)))) &&
            (turno.FechaTurno.Year < fin.Year || (turno.FechaTurno.Year == fin.Year && (turno.FechaTurno.Month < fin.Month || (turno.FechaTurno.Month == fin.Month && turno.FechaTurno.Day <= fin.Day))))).ToListAsync(); // este filtrado demencial lo tengo que hacer si o si porque sino el entity framework explota pq no puedo usar operadores de fechas ni llamar a una funcion, que paja
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
            foreach (var t in Enum.GetNames(typeof(TipoEmbarcacion))) // cambia aca moro por tipoVehiculo
            {
                if (t != null)
                    diccionarioCremoso.Add(t, 0);// agrego todo en 0
            }

            foreach (Oferta o in ofertasFiltradas)
            {
                if (o.EsNavioEnvia)  // tenes que cambiar esto por !esNavio y hacer las mismas consultas pero a la db de Terrestres
                {
                    Maritimo? si = await db.Maritimos.Where(m => m.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync(); 
                    if (si != null)
                    {
                        if (diccionarioCremoso.ContainsKey(si.Tipo.ToString()))
                        {
                            diccionarioCremoso[si.Tipo.ToString()] = diccionarioCremoso[si.Tipo.ToString()] + 1;
                        }
                    }
                }
                if (o.EsNavioRecibe)// tenes que cambiar esto por !esNavio y hacer las mismas consultas pero a la db de Terrestres
                {
                    Maritimo? si = await db.Maritimos.Where(m => m.Id == o.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                    if (si != null)
                    {
                        if (diccionarioCremoso.ContainsKey(si.Tipo.ToString()))
                        {
                            diccionarioCremoso[si.Tipo.ToString()] = diccionarioCremoso[si.Tipo.ToString()] + 1;
                        }
                    }
                }
            }
            return StatusCode(StatusCodes.Status200OK, diccionarioCremoso);
        }
<<<<<<< HEAD


        [HttpGet]
        [Route("GetReporteIntercambiosVehiculosMasUsados")]
        public async Task<IActionResult> GetReporteIntercambiosVehiculosMasUsados([FromBody] RangoDTO rango)
        {
            DateTime inicio = new DateTime(rango.inicio.Year, rango.inicio.Month, rango.inicio.Day);
            DateTime fin = new DateTime(rango.fin.Year, rango.fin.Month, rango.fin.Day);

            using var db = new OhMyBoatUIServerContext();
            var turnosQueVan = await db.Turno.Where(turno => (turno.FechaTurno.Year > inicio.Year || (turno.FechaTurno.Year == inicio.Year && (turno.FechaTurno.Month > inicio.Month || (turno.FechaTurno.Month == inicio.Month && turno.FechaTurno.Day >= inicio.Day)))) &&
            (turno.FechaTurno.Year < fin.Year || (turno.FechaTurno.Year == fin.Year && (turno.FechaTurno.Month < fin.Month || (turno.FechaTurno.Month == fin.Month && turno.FechaTurno.Day <= fin.Day))))).ToListAsync(); // este filtrado demencial lo tengo que hacer si o si porque sino el entity framework explota pq no puedo usar operadores de fechas ni llamar a una funcion, que paja
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
            Dictionary<String, int> DiccionarioDatos = new();
            foreach (var t in Enum.GetNames(typeof(TipoVehiculo))) 
            {
                if (t != null)
                    DiccionarioDatos.Add(t, 0);// agrego todo en 0
            }

            foreach (Oferta o in ofertasFiltradas)
            {
                if (!o.EsNavioEnvia) 
                {
                    Terrestre? si = await db.Terrestres.Where(m => m.Id == o.ID_VehiculoEnviaOferta).FirstOrDefaultAsync(); 
                    if (si != null)
                    {
                        if (DiccionarioDatos.ContainsKey(si.Tipo.ToString()))
                        {
                            DiccionarioDatos[si.Tipo.ToString()] = DiccionarioDatos[si.Tipo.ToString()] + 1;
                        }
                    }
                }
                if (!o.EsNavioRecibe)
                {
                    Terrestre? si = await db.Terrestres.Where(m => m.Id == o.ID_VehiculoRecibeOferta).FirstOrDefaultAsync();
                    if (si != null)
                    {
                        if (DiccionarioDatos.ContainsKey(si.Tipo.ToString()))
                        {
                            DiccionarioDatos[si.Tipo.ToString()] = DiccionarioDatos[si.Tipo.ToString()] + 1;
                        }
                    }
                }
            }
            return StatusCode(StatusCodes.Status200OK, DiccionarioDatos);
        }


=======
>>>>>>> 78180c9 (Hay filtro funcional por fecha gente pero si fuera bueno)
    }
}