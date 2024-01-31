using System.Text;
using AuthService.DataAccess;
using AuthService.Services;
using AuthService.Services.Interfaces;
using DndOnline.DataAccess;
using DndOnline.Middlewares;
using DndOnline.Services;
using DndOnline.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("default")));

builder.Services.AddDbContext<DndAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("default")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ILobbyService, LobbyService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents()
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                if (context.Request.Headers.Authorization.IsNullOrEmpty()) context.Response.Redirect("/login");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<TokenInHeaderMiddleware>();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "auth",
    pattern: "/auth/{action}",
    defaults: new { controller = "Account", action = "SignIn" });

app.MapControllerRoute(
    name: "lobby_constructor",
    pattern: "lobby-constructor",
    defaults: new { controller = "LobbyConstructor", action = "Index" });

app.MapControllerRoute(
    name: "lobby",
    pattern: "lobby/{id?}",
    defaults: new { controller = "Lobby", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" });

app.MapHub<LobbyHub>("/lobbyHub");

app.Run();