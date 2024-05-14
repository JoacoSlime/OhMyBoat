﻿using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            SesionDTO sesionDTO = new SesionDTO();

            using (var db = new OhMyBoatUIServerContext())
            {
                var temp = db.Usuarios.Where(usuario => usuario.Email == login.Email && usuario.Password == login.Password).FirstOrDefault();
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

        private async Task<bool> VerificarExistencia(string email)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                return await db.Usuarios.Where(u => u.Email == email).AnyAsync();
            }
        }

        [HttpPost]
        [Route("Registrar")]
        public async Task<IActionResult> RegistrarCliente([FromBody] Cliente c)
        {
            using (var db = new OhMyBoatUIServerContext())
            {
                if (db.Clientes.Where(cli => cli.Email == c.Email).IsNullOrEmpty())
                {
                    c.Rol = Roles.cliente;
                  
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, c);
                }
                return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
            }

        }




    }
    }

