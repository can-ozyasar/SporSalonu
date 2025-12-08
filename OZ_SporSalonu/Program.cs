using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using OZ_SporSalonu.Services; // Servisleri ekledik
using Npgsql.EntityFrameworkCore; // PostgreSQL için bu satırı ekleyin

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext ve SQL Server Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));



// 2. Identity (Kimlik Doğrulama) ve Roller
// IdentityUser yerine bizim ApplicationUser modelimizi kullan
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<TurkceIdentityErrorDescriber>() 
    .AddDefaultUI(); // Login, Register sayfaları için

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages(); // Identity UI için gerekli

// 3. Yapay Zeka Servisini Kaydet (Dependency Injection)
builder.Services.AddScoped<IYapayZekaService, GeminiService>(); 


builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           



var app = builder.Build();

// Veritabanı ve Admin Rolü Oluşturma (Seed)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    await SeedRolesAndAdminAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Identity sayfaları için

app.Run();

async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // YENİ ŞİFREmiz
    string yeniAdminSifresi = "Aa1.123";
    // ==================================

    // 1. Roller
    string[] roleNames = { "Admin", "Uye" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2. Admin kullanıcısını oluştur (veya bul)
    var adminUserEmail = "muhammed.ozyasar@ogr.sakarya.edu.tr";
    var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

    if (adminUser == null)
    {
        // Kullanıcı yoksa, yeni şifreyle oluştur
        adminUser = new ApplicationUser
        {
            UserName = adminUserEmail,
            Email = adminUserEmail,
            EmailConfirmed = true,
            Ad = "Admin",
            Soyad = "User"
        };
        var result = await userManager.CreateAsync(adminUser, yeniAdminSifresi); 

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        // Kullanıcı varsa, rolünü kontrol et
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

       if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

}