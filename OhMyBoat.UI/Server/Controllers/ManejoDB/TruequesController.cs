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

