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
using GIGANTECORE.Utils;
using Microsoft.OpenApi.Models;


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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GIGANTE CORE API", Version = "v1" });

    // Configuración para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Configuración para manejar archivos
    c.OperationFilter<SwaggerFileOperationFilter>();
});



builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

//Configuración Cors para poder hacerlo con ReactJS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173") // URL de tu app React
            .AllowAnyMethod()     // Permite todos los métodos HTTP (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

var productosPath = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Productos");
if (!Directory.Exists(productosPath))
{
    Directory.CreateDirectory(productosPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(productosPath),
    RequestPath = "/api/Imagenes/Productos"
});


var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Imagenes")),
    RequestPath = "/api/Imagenes"
});



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowReactApp");
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

