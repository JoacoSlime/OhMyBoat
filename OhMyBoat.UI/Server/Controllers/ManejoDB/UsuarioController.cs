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
        [Route("RegistrarEmpleado")]
        public async Task<IActionResult> RegistrarEmple([FromBody] Usuario c)
        {
            if (!Utils.IsValidEmail(c.Email))
            {
                return StatusCode(StatusCodes.Status418ImATeapot, null);
            }
            using (var db = new OhMyBoatUIServerContext())
            {
                if (await db.Usuarios.Where(cli => cli.Email == c.Email).AnyAsync())
                {
                    c.Rol = Roles.empleado;
                    // se tiene que mandar el email para recuperar la contraseña // <-------------------------------------------------------------//
                    await db.Usuarios.AddAsync(c);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, c);
                }
                return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
            }
        }




        [HttpPost]
        [Route("RegistrarCliente")]
        public async Task<IActionResult> RegistrarCliente([FromBody] Cliente c)
        {
            if (!Utils.IsValidEmail(c.Email))
            {
                return StatusCode(StatusCodes.Status418ImATeapot, null);
            }
            using (var db = new OhMyBoatUIServerContext())
            {
                if (db.Clientes.Where(cli => cli.Email == c.Email).IsNullOrEmpty())
                {
                    c.Rol = Roles.cliente;
                    await db.Clientes.AddAsync(c);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, c);
                }
                return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
            }
        }

        [HttpPost]
        [Route("ObtenerUsuario")]
        public async Task<IActionResult> ObtenerUsuario([FromBody] LoginDTO a)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                var clie = await db.Usuarios.Where(u => u.Email == a.Email).FirstAsync();
                if (clie != null)
                {
                    clie.Password = "que te importa lagarto";
                    return StatusCode(StatusCodes.Status200OK, clie);
                }
                else return StatusCode(StatusCodes.Status403Forbidden, null);

            }
        }

        [HttpPost]
        [Route("ActualizarUsuario")]
        public async Task<IActionResult> PostActualizarUsuario([FromBody] ActualizarUsuario parm)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                
                var clie = await db.Usuarios.Where(user => user.Email == parm.Email).FirstOrDefaultAsync();
                if (clie != null)
                {
                    clie.Nombre = parm.Nombre;
                    clie.Contacto = parm.Contacto;
                    clie.base64imagen = parm.base64imagen;
                    db.Usuarios.Update(clie);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, clie);
                }
                else return StatusCode(StatusCodes.Status403Forbidden, null);

            }
        }
        [HttpGet]
        [Route("ListarClientes")]
        public async Task<IActionResult> Get()
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                var listClientes = await db.Clientes.OrderBy(c => c.Nombre).ToListAsync();
                listClientes.ForEach(c => c.Password = "vivaracho"); // asi no hay un vivo 
                return StatusCode(StatusCodes.Status200OK, listClientes);
            }
        }
        /*
        public async Task<IActionResult> Details(String? email)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                if (db.Usuarios == null)
                {
                    return NotFound();
                }
                var usuario = await db.Usuarios
                    .FirstOrDefaultAsync(m => m.Email == email);
                if (usuario == null)
                {
                    return NotFound();
                }

            }
            
        }
        */
    }
}

