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
        [Route("GetReporteCantidadNaviosPorSucursal")]
        public async Task<IActionResult> GetReporteCantidadNaviosPorSucursal() {
            using var db = new OhMyBoatUIServerContext();
            List<double> dataList = new();
            foreach(Sucursal s in await db.Sucursales.ToListAsync()) { // Esto es una mierda parte 2 -J
                dataList.Add(await db.Maritimos.Where(maritimo => maritimo.SucursalId == s.Id).CountAsync());
            }
            return StatusCode(StatusCodes.Status200OK, dataList);
        }
    }
}