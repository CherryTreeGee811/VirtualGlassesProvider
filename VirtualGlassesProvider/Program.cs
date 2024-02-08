using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VirtualGlassesProvider.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<GlassesStoreDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("GlassesStoreCNN")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<GlassesStoreDbContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set your desired session timeout
});
//builder.Services.AddDefaultIdentity<User>(options => {
//    //Register The Account and Validate the email
//    options.SignIn.RequireConfirmedAccount = true;
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
//    // The maximum number of failed access attempts before a user is locked out.
//    options.Lockout.MaxFailedAccessAttempts = 3;
//}).AddDefaultTokenProviders().AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<GameStoreDbContext>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
