using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OhMyBoat.UI.Shared.Entidades;

using OhMyBoat.UI.Shared;
using OhMyBoat.UI.Server.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.Net;

namespace OhMyBoat.UI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TruequesController : ControllerBase {

        [HttpGet]
        [Route("ListarTrueques")]
        public async Task<IActionResult> Get()
        {
            using var db = new OhMyBoatUIServerContext();
            var listTrueques = await db.Trueques.OrderBy(c => c.Id).ToListAsync();
            return StatusCode(StatusCodes.Status200OK, listTrueques);
        }

        [HttpPost]
        [Route("AgregarTrueque")]
        public async Task<IActionResult> AgregarTrueque([FromBody] Trueque t)
        {
            using var db = new OhMyBoatUIServerContext();
            await db.Trueques.AddAsync(t);
            await db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK, await GetTrueque(t));
        }

        [HttpPost]
        [Route("DeleteTrueque")]
        public async Task<IActionResult> AgregDeleteTruequearTrueque([FromBody] Trueque t)
        {
            using var db = new OhMyBoatUIServerContext();
            db.Trueques.Remove(t);
            await db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("GetTrueque")]
        public async Task<IActionResult> GetTrueque([FromBody] Trueque tru)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Trueques.Where(t => t.MaritimoId == tru.MaritimoId && t.VehiculoId == tru.VehiculoId).FirstOrDefaultAsync();
            if (trueque == null || trueque == new Trueque()) {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            } else {
                return StatusCode(StatusCodes.Status200OK, trueque);
            }
        }

        [HttpPost]
        [Route("SwitchConcretar")]
        public async Task<IActionResult> SwitchConcretar([FromBody] Trueque t)
        {
            using var db = new OhMyBoatUIServerContext();
            var trueque = await db.Trueques.Where(tr => tr.Id == t.Id).FirstOrDefaultAsync();
            if (trueque != null){
                trueque.Concreto = !trueque.Concreto;
                db.Trueques.Update(trueque);
                await db.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status406NotAcceptable);
        }
    }

}

