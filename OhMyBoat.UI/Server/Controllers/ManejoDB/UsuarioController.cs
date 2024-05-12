using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OhMyBoat.UI.Shared.Entidades;

using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Server.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace OhMyBoat.UI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        

        [HttpPost]
        [Route("Login")]

        // aca me conecto a la db despues lo cambio rey
        



        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            SesionDTO sesionDTO = new SesionDTO();

           
            using (var db = new OhMyBoatUIServerContext())
            {
                var temp = db.Usuarios.Where(usuario => usuario.Email == login.Email && usuario.Password == login.Password).FirstOrDefault();
                if (temp != null)
                {
                    sesionDTO.Rol = temp.Rol.ToString();
                    sesionDTO.Email = temp.Email;
                    sesionDTO.Nombre = temp.Nombre;
                    return StatusCode(StatusCodes.Status200OK, sesionDTO);
                }
                else return StatusCode(StatusCodes.Status200OK, null);
            }
                
            /* if (login.Email == "sss" && login.Password == "aaa")
             {
                 sesionDTO.Nombre = "admin";
                 sesionDTO.Email = login.Email;
                 sesionDTO.Rol = "admin";
            }
             else
             {
                 sesionDTO.Nombre = "cliente";
                 sesionDTO.Email = login.Email;
                 sesionDTO.Rol = "cliente";
             }
             */      
        }

        private async bool verificarExistencia(string email)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                return await db.Usuarios.Where(u => u.Email == email).AnyAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            if (await verificarExistencia(usuario.Email))
            {
                return StatusCode(StatusCodes.Status200OK, false);
            }
            else
            {
                using (var db = new OhMyBoatUIServerContext())
                {
                    db.Usuarios.Add(usuario);
                    await db.SaveChangesAsync();
                }
                return StatusCode(StatusCodes.Status200OK, true);
            }
        }

    }
}
