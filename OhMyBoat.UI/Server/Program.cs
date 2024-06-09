using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OhMyBoat.UI.Server.Data;
using OhMyBoat.UI.Server.Services;
using Microsoft.Extensions.Configuration;
using OhMyBoat.UI.Server.Helpers;
using OhMyBoat.UI.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//builder.Services.AddDbContext<OhMyBoatUIServerContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("OhMyBoatUIServerContext") ?? throw new InvalidOperationException("Connection string 'OhMyBoatUIServerContext' not found.")));
builder.Services.AddDbContext<OhMyBoatUIServerContext>();

builder.Configuration.AddJsonFile("emailsettings.json", optional: false, reloadOnChange: true);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();


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
        context.Usuarios.Add(new OhMyBoat.UI.Shared.Entidades.Usuario() { Email = "empleado@empleado.com",Password="0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Nombre="Mariano Moreno",Rol=OhMyBoat.UI.Shared.Entidades.Roles.empleado});// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "cliente@cliente.com", Nombre="Juan Francisco Franks", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente});// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "clientebloq1@cliente.com", Bloqueado = true, Nombre = "Pedro Ramirez", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente});// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "clientebloq2@cliente.com", Bloqueado = true, Nombre ="Silvia Silva", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente});// contra Agua1234.
        context.Clientes.Add(new OhMyBoat.UI.Shared.Entidades.Cliente() { Email = "cliente2@cliente.com", Nombre="Tom Holland", Password = "0eb6f152d72087e15b6d61aa17ddafdce855f96330d56f004f129e4504d60c5d", Rol = OhMyBoat.UI.Shared.Entidades.Roles.cliente});// contra Agua1234.
        context.Tokens.Add(new OhMyBoat.UI.Shared.Entidades.TokenRecu() { Email = "admin@admin.com" ,StringAleatorioDelMomento = "TokenRecuAdmin", Usado=false, FechaLimite= DateTime.Now.AddDays(7)}); // token valido que vence en 3 dias
        context.Tokens.Add(new OhMyBoat.UI.Shared.Entidades.TokenRecu() { Email = "empleado@empleado.com", StringAleatorioDelMomento = "TokenRecuEmpleado", Usado=false, FechaLimite= DateTime.Now.AddDays(-5)}); // token valido vencido
        context.Tokens.Add(new OhMyBoat.UI.Shared.Entidades.TokenRecu() { Email = "cliente@cliente.com", StringAleatorioDelMomento = "TokenRecuCliente", Usado=true, FechaLimite= DateTime.Now.AddDays(7)}); // token usado que vence en 3 dias
        context.Tokens.Add(new OhMyBoat.UI.Shared.Entidades.TokenRecu() { Email = "cliente2@cliente.com", StringAleatorioDelMomento = "TokenRecuCliente2", Usado=true, FechaLimite= DateTime.Now.AddDays(-5)}); // token vencido usado
        context.Sucursales.Add(new OhMyBoat.UI.Shared.Entidades.Sucursal() { NombreSuck = "Punta Indio" });
        context.Sucursales.Add(new OhMyBoat.UI.Shared.Entidades.Sucursal() { NombreSuck = "La Plata" });
        context.Sucursales.Add(new OhMyBoat.UI.Shared.Entidades.Sucursal() { NombreSuck = "Berisso" });

        context.SaveChanges();
    }
    var connection = context.Database.GetDbConnection();
    connection.Open();
    using var command = connection.CreateCommand();
    command.CommandText = "PRAGMA journal_mode=DELETE;";
    command.ExecuteNonQuery();
}
    

    app.Run();
