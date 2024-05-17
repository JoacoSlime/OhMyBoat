using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Hosting;

namespace OhMyBoat.UI.Server.Controllers.ManejoDB
{

[ApiController]
[Route("api/[controller]")]
public class uploadController : ControllerBase
{

    private readonly IWebHostEnvironment env;

    public uploadController(IWebHostEnvironment env)
    {
        this.env = env;
    }

    [HttpPost]
    public async Task Post([FromBody] ImageFile[] files)
    {
        foreach (var file in files)
        {
            var buf = Convert.FromBase64String(file.base64data);
            await System.IO.File.WriteAllBytesAsync(env.ContentRootPath + System.IO.Path.DirectorySeparatorChar + Guid.NewGuid().ToString("N") + "-" + file.fileName, buf);

        }
    }

}




}