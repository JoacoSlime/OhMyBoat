using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OhMyBoat.UI.Server.Data;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
    }

    [HttpGet]
    [Route("ListarNaviosDisponibles")]
    public async Task<IActionResult> GetMaritimos()
    {
        using (var db = new OhMyBoatUIServerContext())
        {
            var listar_nav = await db.Barquitos.OrderBy(bar => bar.Matricula).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listar_nav);
        }
    }

    [HttpGet]
    [Route("ListarNaviosDisponibles")]
    public async Task<IActionResult> GetTerrestres(){
        using (var bd = new OhMyBoatUIServerContext()){
            var listar_ter = await bd.Terrestres.OrderBy(terr => terr.Matricula).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listar_ter); 
        }

    }
}
