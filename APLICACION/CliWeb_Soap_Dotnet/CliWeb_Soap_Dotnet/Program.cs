using ClienteWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar la BaseUrl del API REST
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                 ?? "http://localhost:5001/api";
ApiService.BaseUrl = apiBaseUrl;

// Servicios MVC
builder.Services.AddControllersWithViews();

// Registrar servicios
builder.Services.AddSingleton<BancoSoapService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
