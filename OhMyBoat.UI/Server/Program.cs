using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OhMyBoat.UI.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//builder.Services.AddDbContext<OhMyBoatUIServerContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("OhMyBoatUIServerContext") ?? throw new InvalidOperationException("Connection string 'OhMyBoatUIServerContext' not found.")));
builder.Services.AddDbContext<OhMyBoatUIServerContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

using (var context = new OhMyBoatUIServerContext()) // ESTO ES PARA QUE NO EXPLOTE CREO
{
    if(context.Usuarios != null)
    {
        context.Usuarios.Add(new OhMyBoat.UI.Shared.Entidades.Usuario() { Email = "admin@admin.com" });
    }
    context.Database.EnsureCreated();
    var connection = context.Database.GetDbConnection();
    connection.Open();
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "PRAGMA journal_mode=DELETE;";
        command.ExecuteNonQuery();
    }
}
    

    app.Run();
