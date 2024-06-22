using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Server.Services;
using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Shared.Entidades;
using System.Collections.Immutable;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
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