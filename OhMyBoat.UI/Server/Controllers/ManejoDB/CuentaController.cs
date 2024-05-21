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
    public class CuentaController : ControllerBase
    {
        private readonly EmailService _emailService;

        public CuentaController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [Route("RegistrarEmpleado")]
        public async Task<IActionResult> RegistrarEmple([FromBody] Usuario c)
        {
            if (!Utils.IsValidEmail(c.Email))
            {
                return StatusCode(StatusCodes.Status418ImATeapot, null);
            }
            using var db = new OhMyBoatUIServerContext();
            if (db.Usuarios.Where(cli => cli.Email == c.Email.ToLower()).IsNullOrEmpty())
            {
                c.Rol = Roles.empleado;
                TokenRecu Token = new()
                {
                    Email = c.Email.ToLower(),
                    StringAleatorioDelMomento = UniqueId.CreateRandomId(),
                    FechaLimite = DateTime.Now.AddDays(7)                        
                };
                await db.TokenRecu.AddAsync(Token);
                await db.SaveChangesAsync();
                await _emailService.Send(
                        to: c.Email,
                        subject: "Aquí está tu clave para terminar de configurar tu cuenta.", 
                        html: $@"<h2>Verificación de cuenta</h2>
                        <p>Tu token de verificación de cuenta es: {Token.StringAleatorioDelMomento}<p/>
                        <p>Haz click <a href=""http://localhost:5047/recovery/{Token.StringAleatorioDelMomento}"">aquí</a> para ir directamente.<p/>
                        <p>Si no has sido tu quien pidió esta clave, ignora este mensaje.</p>"
                    );
                await db.Usuarios.AddAsync(c);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, c);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }




        [HttpPost]
        [Route("RegistrarCliente")]
        public async Task<IActionResult> RegistrarCliente([FromBody] Cliente c)
        {
            if (!Utils.IsValidEmail(c.Email)) {
                return StatusCode(StatusCodes.Status418ImATeapot, null);
            }
            using var db = new OhMyBoatUIServerContext();
            if (db.Clientes.Where(cli => cli.Email == c.Email.ToLower()).IsNullOrEmpty())
            {
                c.Rol = Roles.cliente;
                await db.Clientes.AddAsync(c);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, c);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
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
        [Route("CheckearToken")]
        public async Task<IActionResult> VerificarToken([FromBody] RecuCuentaDTO token)
        {
            using var db = new OhMyBoatUIServerContext();
            var resul = await db.Tokens.Where(t => ((t.StringAleatorioDelMomento == token.Hash) && (!t.Usado) && (t.FechaLimite >= DateTime.Now))).FirstOrDefaultAsync();
            if (resul != null)// hay token valido
            {
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
        }

        [HttpPost]
        [Route("RecuperarContrasenia")]
        public async Task<IActionResult> RecuperarContrasenia ([FromBody] RecuCuentaDTO token)
        {
            using var db = new OhMyBoatUIServerContext();
            var resul = await db.Tokens.Where(t => ((t.StringAleatorioDelMomento == token.Hash) && (!t.Usado) && (t.FechaLimite >= DateTime.Now))).FirstOrDefaultAsync();
            if (resul != null)// hay token valido
            {
                var coso = await db.Usuarios.Where(u => u.Email == resul.Email).FirstOrDefaultAsync();
                if (coso != null)
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

        [HttpPost]
        [Route("SwitchUserBlock")]
        public async Task<IActionResult> BloaquearUsuario([FromBody] LoginDTO log)
        {
            using var db = new OhMyBoatUIServerContext();
            var user = await db.Usuarios.Where(u => u.Email == log.Email).FirstOrDefaultAsync();
            if (user != null){
                user.Bloqueado = user.Bloqueado;
                db.Usuarios.Update(user);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
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
            if (Utils.IsValidEmail(log.Email)){
                using var db = new OhMyBoatUIServerContext();
                var existe = await db.Usuarios.Where(u => u.Email == log.Email).AnyAsync();
                if (existe)
                {
                    // Generar el token
                    TokenRecu Token = new()
                    {
                        Email = log.Email.ToLower(),
                        StringAleatorioDelMomento = UniqueId.CreateRandomId(),
                        FechaLimite = DateTime.Now.AddDays(7)                        
                    };
                    await db.TokenRecu.AddAsync(Token);
                    await db.SaveChangesAsync();
                    // Enviar Mail
                    await _emailService.Send(
                        to: log.Email,
                        subject: "Aquí está tu clave de recuperación de cuenta", 
                        html: $@"<h2>Verificación de cuenta</h2>
                        <p>Tu token de verificación de cuenta es: {Token.StringAleatorioDelMomento}<p/>
                        <p>Haz click <a href=""http://localhost:5047/recovery/{Token.StringAleatorioDelMomento}"">aquí</a> para ir directamente.<p/>
                        <p>Si no has sido tu quien pidió esta clave, ignora este mensaje.</p>"
                    );
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
        }
    }
}
