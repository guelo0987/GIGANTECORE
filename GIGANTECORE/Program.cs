using System.Text;
using GIGANTECORE.Context;
using Serilog;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Filters;
using System.IO;


var builder = WebApplication.CreateBuilder(args);


//Logs Configuración
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo
    .File("logs/GiganteCoreLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();




//Configuracion de la base de datos 
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//Controlador Servicios
builder.Services.AddControllers(option => option.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
    .AddXmlDataContractSerializerFormatters();




//Configuracion JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;  // Asegúrate de que RequireHttpsMetadata esté configurado correctamente
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        }; 
        
    });


//Configuracion de politicas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireEmpleadoRole", policy => policy.RequireRole("Empleado"));
});




//Configuracion Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

//Configuración Cors para poder hacerlo con ReactJS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyHeader().AllowAnyOrigin()));

var app = builder.Build();

var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/Imagenes"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();
app.UseHttpsRedirection();
//Usamos la autenticación antes de la autorización  
app.UseAuthentication();
app.UseMiddleware<RolePermissionMiddleware>();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

