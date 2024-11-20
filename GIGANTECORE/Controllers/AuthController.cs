using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GIGANTECORE.Context;
using GIGANTECORE.DTO;
using GIGANTECORE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PassHash;

namespace GIGANTECORE.Controllers;




[Route("api/[controller]")]
[ApiController]
public class AuthController:ControllerBase
{


    private readonly MyDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger, MyDbContext db, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _db = db;
    }
    
    [HttpPost("register")]
    public IActionResult Register([FromBody] AdminDTO adminDto)
    {
        // Verificar si el correo ya existe
        if (_db.Admins.Any(a => a.Mail == adminDto.Mail))
        {
            _logger.LogError("El correo ya esta registrado");
            return BadRequest("El correo ya está registrado.");
        }

        // Validar que el rol sea "Admin" o "Empleado"
        if (adminDto.Rol != "Admin" && adminDto.Rol != "Empleado")
        {
            _logger.LogError("El rol debe ser Admin o Empleado");
            return BadRequest("El rol debe ser 'Admin' o 'Empleado'.");
        }

        // Crear el usuario
        var newAdmin = new Admin
        {
            Nombre = adminDto.Nombre,
            Mail = adminDto.Mail,
            Password = PassHasher.HashPassword(adminDto.Password), // Almacenar contraseña hasheada
            Rol = adminDto.Rol,
            FechaIngreso = DateTime.Now
        };

        // Guardar en la base de datos
        _db.Admins.Add(newAdmin);
        _db.SaveChanges();
        _logger.LogInformation("El Usuario ha sido creado exitosamente");

        return Ok(new { Message = "Usuario registrado exitosamente.", UserId = newAdmin.Id });
    }


    
    
    
    
    // Autenticar Admin con DTO
    [HttpPost("login")]
    public IActionResult AuthenticateAdmin([FromBody] LoginRequest loginRequest)
    {
        // Buscar el administrador por correo
        var admin = _db.Admins.FirstOrDefault(a => a.Mail == loginRequest.Mail);

        if (admin == null)
        {
            _logger.LogError("Usuario no encontrado.");
            return Unauthorized("Credenciales inválidas.");
        }

        // Verificar contraseña
        if (!PassHasher.VerifyPassword(loginRequest.Password, admin.Password))
        {
            _logger.LogError("Contraseña incorrecta.");
            return Unauthorized("Credenciales inválidas.");
        }

        // Validar rol
        if (admin.Rol != "Admin" && admin.Rol != "Empleado")
        {
            _logger.LogError("Rol no válido.");
            return Unauthorized("El usuario no tiene un rol válido.");
        }

        // Crear los claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Id", admin.Id.ToString()),
            new Claim(ClaimTypes.Role, admin.Rol) // Rol dinámico
        };
        
        _logger.LogInformation("Log in Exitoso");

        // Generar y devolver el token
        return GenerateToken(claims, new ()
        {
            Nombre = admin.Nombre,
            Telefono = admin.Telefono,
            Mail = admin.Mail,
            Rol = admin.Rol,
            SoloLectura = admin.SoloLectura
        }, admin.Rol);
        
        
    }


        // Generar Token JWT
        private IActionResult GenerateToken(Claim[] claims, AdminDTO adminDto, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
            );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Login exitoso");

            return Ok(new { Token = tokenValue, User = adminDto, Role = role });
        }
    
    

}

public class LoginRequest
{
    public string Mail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
