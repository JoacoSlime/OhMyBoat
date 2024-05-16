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
    public class UsuarioController : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        // aca me conecto a la db despues lo cambio rey -Agus
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            SesionDTO sesionDTO = new SesionDTO();

            using (var db = new OhMyBoatUIServerContext())
            {
                var temp = await db.Usuarios.Where(usuario => usuario.Email == login.Email && usuario.Password == login.Password).FirstOrDefaultAsync();
                if (temp != null)
                {
                    sesionDTO.Rol = temp.Rol.ToString();
                    if (temp.Rol == Roles.cliente && temp.Bloqueado)
                    {
                        return StatusCode(StatusCodes.Status506VariantAlsoNegotiates);
                    }

                    sesionDTO.Email = temp.Email;
                    sesionDTO.Nombre = temp.Nombre;
                    return StatusCode(StatusCodes.Status200OK, sesionDTO);
                }
                else return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired);
            }
        }

        [HttpPost]
        [Route("Registrar")]
        public async Task<IActionResult> RegistrarCliente([FromBody] Cliente c)
        {
            if (!Utils.IsValidEmail(c.Email)) {
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

