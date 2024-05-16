using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
    context.Database.EnsureCreated();
    if (context.Usuarios.IsNullOrEmpty())
    {
        context.Usuarios.Add(new OhMyBoat.UI.Shared.Entidades.Usuario() { Email = "admin@admin.com",Password="925427659676ada68e35f680a1f4c8da1cee5955d9bd014772d4b798c5bae13a",Nombre="Daniel",Rol=OhMyBoat.UI.Shared.Entidades.Roles.jefe}); // contra Admin1234.
        context.Usuarios.Add(new OhMyBoat.UI.Shared.Entidades.Usuario() { Email = "emple@gmail.com",Password="0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Nombre="Mariano",Rol=OhMyBoat.UI.Shared.Entidades.Roles.empleado});// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "cliente1@yahoo.com", Nombre="pepo Ramallo", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente });// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "clientebloq1@yahoo.com", Bloqueado = true, Nombre = "pepa Ramallo", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente }) ;// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "clientebloq2@yahoo.com", Bloqueado = true, Nombre ="pepi Ramallo", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente });// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "cliente2@yahoo.com", Nombre="pepu Ramallo", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente });// contra Agua1234.
        context.SaveChanges();
    }
    var connection = context.Database.GetDbConnection();
    connection.Open();
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "PRAGMA journal_mode=DELETE;";
        command.ExecuteNonQuery(); }
}
    

    app.Run();
