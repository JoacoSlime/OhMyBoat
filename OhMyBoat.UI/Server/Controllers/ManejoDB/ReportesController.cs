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
        
    }
}