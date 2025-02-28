// Program.cs
using CrudDesarrollo.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrar MySqlDataAccess como un servicio singleton
builder.Services.AddSingleton<Acceso>();

//builder.ServicesAddDbContext<>

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Privacy}/{id?}");
app.Run();
//-------------------------------------------------
//using Microsoft.Extensions.Configuration;
//using MySql.Data.MySqlClient;


//var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//);

//// Add services to the container.
//builder.Services.AddControllersWithViews();


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//}

//app.UseHttpsRedirection();
//app.MapControllers();

//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
