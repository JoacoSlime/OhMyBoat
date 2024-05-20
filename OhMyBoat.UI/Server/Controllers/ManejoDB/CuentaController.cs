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
        [Route("CheckearToken")]
        public async Task<IActionResult> VerificarToken([FromBody] RecuCuentaDTO token)
        {
            using (var db = new OhMyBoatUIServerContext()) 
            {
                var resul = await db.Tokens.Where(t => ((t.StringAleatorioDelMomento == token.Hash) && (!t.Usado) && (t.FechaLimite>=DateTime.Now))).FirstOrDefaultAsync();
                if(resul!=null)// hay token valido
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict);
                }
            }
        }

        [HttpPost]
        [Route("RecuperarContrasenia")]
        public async Task<IActionResult> RecuperarContrasenia ([FromBody] RecuCuentaDTO token)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                var resul = await db.Tokens.Where(t => ((t.StringAleatorioDelMomento == token.Hash) && (!t.Usado) && (t.FechaLimite >= DateTime.Now))).FirstOrDefaultAsync();
                if (resul != null)// hay token valido
                {
                    var coso = await db.Usuarios.Where(u => u.Email == resul.Email).FirstOrDefaultAsync();
                    if(coso!= null)
                    {
                        coso.Password = token.ContraNueva;
                        resul.Usado = true;
                        db.Usuarios.Update(coso);
                        
                        db.Tokens.Update(resul);
                        await db.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status200OK);
                    }
                    else // token de cuenta eliminada (?
                    {
                        return StatusCode(StatusCodes.Status418ImATeapot);
                    }
                }
                else // token no valido o expiro 
                {
                    return StatusCode(StatusCodes.Status409Conflict);
                }
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
