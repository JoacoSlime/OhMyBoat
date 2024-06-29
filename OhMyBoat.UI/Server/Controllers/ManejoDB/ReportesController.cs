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


    }
}
   