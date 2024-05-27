﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;

using OhMyBoat.UI.Shared.Entidades;


namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
    

    [HttpPost]
    [Route("CargarVehiculoTerrestre")]
    public async Task<IActionResult> CargarVehiculoTerrestre([FromBody] Terrestre v)
    {
            using var db = new OhMyBoatUIServerContext();
            if (db.Terrestres.Where(vec => vec.Matricula == v.Matricula).IsNullOrEmpty())
            {
                await db.Terrestres.AddAsync(v);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, v);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);

        }

    [HttpPost]
    [Route("CargarVehiculoMaritimo")]
    public async Task<IActionResult> CargarNavio([FromBody] Maritimo m)
    {
            using var db = new OhMyBoatUIServerContext();
            if (db.Maritimos.Where(nav => nav.Matricula == m.Matricula).IsNullOrEmpty())
            {
                await db.Maritimos.AddAsync(m);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, m);
            }
            return StatusCode(StatusCodes.Status511NetworkAuthenticationRequired, null);
        }



    [HttpPost]
    [Route("ListarVehiculosCliente")]
    public async Task<IActionResult> GetTerrestresCliente([FromBody] string Email){
            using var bd = new OhMyBoatUIServerContext();
            List<Terrestre> lista_Terrestre = await bd.Terrestres.Where(ter => ter.IDCliente == Email.ToLower()).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, lista_Terrestre);

        }
       

    [HttpPost]
    [Route("ListarNaviosCliente")]
    public async Task<IActionResult> GetMaritimosCliente([FromBody] string cliente){
            using var bd = new OhMyBoatUIServerContext();
            List<Maritimo> lista_Maritimo = await bd.Maritimos.Where(nav => nav.IDCliente == cliente).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, lista_Maritimo);
        }


    [HttpGet]
    [Route("ListarNaviosDisponibles")]
    public async Task<IActionResult> GetMaritimos()
    {
            using var db = new OhMyBoatUIServerContext();
            var listar_nav = await db.Maritimos.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listar_nav);
        }

    [HttpGet]
    [Route("ListarVehiculosDisponibles")]
    public async Task<IActionResult> GetTerrestres(){
            using var bd = new OhMyBoatUIServerContext();
            var listar_ter = await bd.Terrestres.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listar_ter);

        }
    }
}
