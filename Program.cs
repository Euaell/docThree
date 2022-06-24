using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using QuizApi.Models;
using Newtonsoft.Json.Serialization;
using ScottBrady91.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<QuizDbContext>( options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// JSON Serializer
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
        = new DefaultContractResolver());

// // cookie
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//     {
//         options.Cookie.Name = "UserLoginCookie";
//         options.SlidingExpiration = true;
//         options.ExpireTimeSpan = new TimeSpan(1, 0, 0); // Expires in 1 hour
//         options.Events.OnRedirectToLogin = (context) =>
//         {
//             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//             return Task.CompletedTask;
//         };
//
//         options.Cookie.HttpOnly = true;
//         // Only use this when the sites are on different domains
//         options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
//     });


var app = builder.Build();

// Add cors
app.UseCors(options =>
    options
        // .WithOrigins("http://localhost:3000")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

// serve static files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Images")),
    RequestPath = "/Images"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// // Tells the app to transmit the cookie through both HTTP and HTTPS.
// app.UseCookiePolicy(
//     new CookiePolicyOptions
//     {
//         Secure = CookieSecurePolicy.SameAsRequest
//     });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
