using BancoSoapService.Data;
using BancoSoapService.Services;
using Microsoft.EntityFrameworkCore;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<BancoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BancoConnection")));

// Registrar servicios
builder.Services.AddScoped<IBancoService, BancoService>();

// Configurar SOAP
builder.Services.AddSoapCore();

var app = builder.Build();

// Middleware para SOAP
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IBancoService>("/BancoService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
});

app.Run();
