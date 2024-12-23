using SV21T1020254.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "AuthenticationCookie";//t�n c?a cookie
                    option.LoginPath = "/Account/Login";//URL ??n trang ??ng nh?p
                    option.AccessDeniedPath = "/Account/AccessDenined";//URL ??n trang trong tr??ng b? c?m truy c?p
                    option.ExpireTimeSpan = TimeSpan.FromDays(360);//Th?i gian t?n t?i c?a Cookie
                });
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();//ph?i vi?t tr??c
app.UseAuthorization();//sau 
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

ApplicationContext.Configure
(
    context: app.Services.GetRequiredService<IHttpContextAccessor>(),
    enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
);
//
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(@"D:\ThienBinh\SV21T1020254\ShareProduct"),
//    RequestPath = "/shared-products"
//});
//
//Kh?i t?o c?u h�nh cho BusinessLayer
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
_21T1020254.BusinessLayers.Configuration.Initialize(connectionString);
app.Run();