using Microsoft.EntityFrameworkCore;
using InventoryManagement;
using InventoryManagement.IRepository;
using InventoryManagement.SqlRepository;
using DinkToPdf;
using OfficeOpenXml;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
builder.Services.AddScoped<IOrderOutRepository, SqlOrderOutRepository>();
builder.Services.AddScoped<IReturnInRepository, SqlReturnInRepository>();
// Add DinkToPdf service configuration
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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