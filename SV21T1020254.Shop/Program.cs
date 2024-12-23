//var builder = WebApplication.CreateBuilder(args);

using SV21T1020254.Shop;
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
                    option.Cookie.Name = "AuthenticationCookie";//tên c?a cookie
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

//

//Kh?i t?o c?u hình cho BusinessLayer
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
_21T1020254.BusinessLayers.Configuration.Initialize(connectionString);
app.Run();
//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//}
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
