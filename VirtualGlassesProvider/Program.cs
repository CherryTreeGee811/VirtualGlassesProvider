using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Services;


// ToDo: Remove all Identity Framework features that are not being used
// Remove all imports that are not being used

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<GlassesStoreDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("GlassesStoreCNN")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddDefaultTokenProviders().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<GlassesStoreDbContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IEmailSender>(provider =>
{
    return new EmailSender(
        smtpServer: "in-v3.mailjet.com",
        smtpPort: 587,                  
        smtpUsername: "55d6922101509219c3e7510e235d5ac3", 
        smtpPassword: "0ae07aee173a2d7a23d193042fe36397"
    );
});
// Generate AES Key and IV
var (key, iv) = AesKeyGenerator.GenerateAesKeyAndIV();

// Register AesEncryptionService with the generated key and IV
builder.Services.AddSingleton<AesEncryptionService>(new AesEncryptionService(key, iv));
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

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await GlassesStoreDbContext.CreateAdminUser(serviceProvider);
    // Call the CreateMemberUser method to create the member user.
    //await GlassesStoreDbContext.DeleteTestClient(serviceProvider);
}
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
