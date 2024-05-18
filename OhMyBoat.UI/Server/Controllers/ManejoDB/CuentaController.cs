using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        [Route("RegistrarEmpleado")]
        public async Task<IActionResult> RegistrarEmple([FromBody] Usuario c)
        {
            if (!Utils.IsValidEmail(c.Email))
            {
                return StatusCode(StatusCodes.Status418ImATeapot, null);
            }
            using (var db = new OhMyBoatUIServerContext())
            {
                if (await db.Usuarios.Where(cli => cli.Email == c.Email.ToLower()).AnyAsync())
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
            if (!Utils.IsValidEmail(c.Email)) {
                return StatusCode(StatusCodes.Status418ImATeapot, null);
            }
            using (var db = new OhMyBoatUIServerContext())
            {
                if (db.Clientes.Where(cli => cli.Email == c.Email.ToLower()).IsNullOrEmpty())
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
        [Route("Login")]
        // aca me conecto a la db despues lo cambio rey -Agus
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            SesionDTO sesionDTO = new();

            using var db = new OhMyBoatUIServerContext();
            var temp = await db.Usuarios.Where(usuario => usuario.Email == login.Email.ToLower() && usuario.Password == login.Password).FirstOrDefaultAsync();
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
        
        [HttpPost]
        [Route("RecuperarContra")]
        public async Task<IActionResult> EnviarCodigo([FromBody] RecuDTO papanatas)
        {
            if (Utils.IsValidEmail(papanatas.Email.ToLower()))
                    using (var db = new OhMyBoatUIServerContext())
                    {
                        var persona = await db.Usuarios.Where(u => u.Email == papanatas.Email.ToLower() && ((u.Password == papanatas.HashViejo) ||(u.Password == papanatas.HashNuevo))).FirstOrDefaultAsync();
                        if (persona != null)
                        {
                        if (persona.Password == papanatas.HashViejo)
                        {
                            if ((papanatas.HashNuevo == papanatas.HashViejo) || (persona.Password == papanatas.HashNuevo)) // el boludo quiso cambiar la contrase;a por la misma que ya tenia, tremendo salame
                                {
                                    return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired);
                                }else
                            {
                                persona.Password = papanatas.HashNuevo;
                                db.Usuarios.Update(persona);
                                await db.SaveChangesAsync();
                                return StatusCode(StatusCodes.Status200OK);
                            }
                        //tengo que cambiar contraseña 
                        
                            
                        }
                                            
                        
                        }
                        return StatusCode(StatusCodes.Status401Unauthorized);

                }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
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
    }
}
