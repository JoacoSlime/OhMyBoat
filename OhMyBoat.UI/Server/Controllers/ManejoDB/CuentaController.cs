using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Shared.Entidades;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
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
        [Route("Recuperacion")]
        public async Task<IActionResult> EnviarCodigo([FromBody] LoginDTO log)
        {
            if (Utils.IsValidEmail(log.Email))
                using (var db = new OhMyBoatUIServerContext())
                {
                    var existe = await db.Usuarios.Where(u => u.Email == log.Email).AnyAsync();
                    if (existe)
                    {
                        //genera email y mando codigo
                    }
                    return StatusCode(StatusCodes.Status200OK);
                }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
        }

        //[HttpGet"{codigon}"]
        //[Route("Recuperacion")]
        //public async Task<>
    }
}
