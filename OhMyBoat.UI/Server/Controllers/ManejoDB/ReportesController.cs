using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Server.Services;
using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Shared.Entidades;
using BlazorBootstrap;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        
        [HttpGet]
        [Route("GetReporteTipoDeEmbarcacion")]
        public async Task<IActionResult> GetReporteTipoDeEmbarcacion() {
            using var db = new OhMyBoatUIServerContext();
            List<double> dataList = new();
            foreach(TipoEmbarcacion tipo in Enum.GetValues(typeof(TipoEmbarcacion))) { // Esto es una mierda -J
                dataList.Add(await db.Maritimos.Where(maritimo => maritimo.Tipo == tipo).CountAsync());
            }
            return StatusCode(StatusCodes.Status200OK, dataList);
        }
    }
}