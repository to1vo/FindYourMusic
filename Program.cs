using FindYourMusic.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FindYourMusic.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Authentication";
        options.LogoutPath = "/Home";
        options.AccessDeniedPath = "/Home";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.Cookie.Name = "FindYourMusicAuth";
    });

builder.Services.AddScoped<DatabaseContext>();
string connectionString = builder.Configuration.GetConnectionString("DefaultConnectionMySQL");
builder.Services.AddDbContextPool<DatabaseContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddHttpClient<APIService>();
builder.Services.AddScoped<TrackService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
