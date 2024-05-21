using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OhMyBoat.UI.Shared.Entidades;

using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Server.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace OhMyBoat.UI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //          Esta api la usamos para login y usuarios en general
    public class UsuarioController : ControllerBase
    {
        [HttpPost]
        [Route("ObtenerUsuario")]
        public async Task<IActionResult> ObtenerUsuario([FromBody] LoginDTO a)
        {
            using var db = new OhMyBoatUIServerContext();
            var clie = await db.Usuarios.Where(u => u.Email == a.Email).FirstAsync();
            if (clie != null)
            {
                clie.Password = "que te importa lagarto";
                return StatusCode(StatusCodes.Status200OK, clie);
            }
            else return StatusCode(StatusCodes.Status403Forbidden, null);
        }

        [HttpPost]
        [Route("ActualizarUsuario")]
        public async Task<IActionResult> PostActualizarUsuario([FromBody] ActualizarUsuario parm)
        {
            using var db = new OhMyBoatUIServerContext();
            var clie = await db.Usuarios.Where(user => user.Email == parm.Email).FirstOrDefaultAsync();
            if (clie != null)
            {
                clie.Nombre = parm.Nombre;
                clie.Contacto = parm.Contacto;
                clie.Base64imagen = parm.Base64imagen;
                db.Usuarios.Update(clie);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, clie);
            }
            else return StatusCode(StatusCodes.Status403Forbidden, null);
        }
        
        [HttpGet]
        [Route("ListarClientes")]
        public async Task<IActionResult> Get()
        {
            using var db = new OhMyBoatUIServerContext();
            var listClientes = await db.Clientes.OrderBy(c => c.Nombre).ToListAsync();
            listClientes.ForEach(c => c.Password = "vivaracho"); // asi no hay un vivo 
            return StatusCode(StatusCodes.Status200OK, listClientes);
        }
    }
}

