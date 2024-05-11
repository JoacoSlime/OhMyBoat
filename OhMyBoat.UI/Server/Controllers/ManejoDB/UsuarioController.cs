using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OhMyBoat.UI.Shared.Entidades;

using OhMyBoat.UI.Shared;

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

            if (login.Email == "sss" && login.Password == "aaa")
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
        
            return StatusCode(StatusCodes.Status200OK, sesionDTO);
        }

    }
}
