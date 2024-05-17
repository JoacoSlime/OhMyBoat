using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;

using OhMyBoat.UI.Shared.Entidades;


namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
    

    [HttpPost]
    [Route("CargarVehiculoTerrestre")]
    public async Task<IActionResult> cargarVehiculoTerrestre([FromBody] Terrestre v)
    {               
            using (var db = new OhMyBoatUIServerContext())
            {
                if ( db.Terrestres.Where(vec => vec.Matricula == v.Matricula).IsNullOrEmpty())
                {                    
                    await db.Terrestres.AddAsync(v);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, v);
                }
                return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
            }

    }

    [HttpPost]
    [Route("CargarNavio")]
    public async Task<IActionResult> cargarNavio([FromBody] Maritimo m)
    {               
            using (var db = new OhMyBoatUIServerContext())
            {
                if ( db.Navios.Where(nav => nav.Matricula == m.Matricula).IsNullOrEmpty())
                {                    
                    await db.Navios.AddAsync(m);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, m);
                }
                return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
            }
    }

    [HttpGet]
    [Route("ListarNaviosDisponibles")]
    public async Task<IActionResult> GetMaritimos()
    {
        using (var db = new OhMyBoatUIServerContext())
        {
            var listar_nav = await db.Navios.OrderBy(bar => bar.Matricula).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listar_nav);
        }
    }

    [HttpGet]
    [Route("ListarTerrestresDisponibles")]
    public async Task<IActionResult> GetTerrestres(){
        using (var bd = new OhMyBoatUIServerContext()){
            var listar_ter = await bd.Terrestres.OrderBy(terr => terr.Matricula).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listar_ter); 
        }

    }
    }
}
