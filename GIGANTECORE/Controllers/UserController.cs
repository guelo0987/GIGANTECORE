using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GIGANTECORE.Context;
using GIGANTECORE.DTO;
using GIGANTECORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PassHash;

namespace GIGANTECORE.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "RequireAdministratorRole")]
public class UserController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, MyDbContext db, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _db = db;
    }

    // Crear o Editar un  usuario
    
  [HttpPost]  
public IActionResult AddOrUpdate([FromBody] AdminDTO adminDto)
{
    if (adminDto == null)
    {
        _logger.LogError("El cuerpo de la solicitud está vacío.");
        return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
    }

    if (string.IsNullOrEmpty(adminDto.Mail))
    {
        _logger.LogError("El correo es obligatorio.");
        return BadRequest("El correo es obligatorio.");
    }

    // Verificamos si es una actualización (ID presente y válido)
    if (adminDto.Id > 0)
    {
        var existingUser = _db.Admins.FirstOrDefault(u => u.Id == adminDto.Id);

        if (existingUser == null)
        {
            _logger.LogError($"Usuario con ID {adminDto.Id} no encontrado.");
            return NotFound("Usuario no encontrado.");
        }

        // Actualizamos los valores
        existingUser.Nombre = adminDto.Nombre;
        existingUser.Mail = adminDto.Mail;
        existingUser.Rol = adminDto.Rol;
        existingUser.Telefono = adminDto.Telefono;
        existingUser.SoloLectura = adminDto.SoloLectura;

        // Solo actualizamos la contraseña si se envía una nueva
        if (!string.IsNullOrEmpty(adminDto.Password))
        {
            existingUser.Password = PassHasher.HashPassword(adminDto.Password);
        }

        _db.SaveChanges();
        _logger.LogInformation($"Usuario con ID {adminDto.Id} actualizado exitosamente.");
        return Ok(new { Message = "Usuario actualizado exitosamente.", UserId = existingUser.Id });
    }
    else
    {
        // Validaciones para un nuevo registro
        if (_db.Admins.Any(a => a.Mail == adminDto.Mail))
        {
            _logger.LogError("El correo ya está registrado.");
            return BadRequest("El correo ya está registrado.");
        }

        if (adminDto.Rol != "Admin" && adminDto.Rol != "Empleado")
        {
            _logger.LogError("El rol debe ser Admin o Empleado.");
            return BadRequest("El rol debe ser 'Admin' o 'Empleado'.");
        }

        var newAdmin = new Admin
        {
            Nombre = adminDto.Nombre,
            Mail = adminDto.Mail,
            Password = PassHasher.HashPassword(adminDto.Password), // Hasheamos la contraseña
            Rol = adminDto.Rol,
            FechaIngreso = DateTime.Now,
            Telefono = adminDto.Telefono,
            SoloLectura = adminDto.SoloLectura
        };

        _db.Admins.Add(newAdmin);
        _db.SaveChanges();
        _logger.LogInformation("Usuario creado exitosamente.");

        return Ok(new { Message = "Usuario registrado exitosamente.", UserId = newAdmin.Id });
    }
}


    // Obtener todos los usuarios
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _db.Admins.ToList();

        return Ok(users);
    }

    // Obtener un usuario por ID
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _db.Admins
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            _logger.LogError($"Usuario con ID {id} no encontrado.");
            return NotFound("Usuario no encontrado.");
        }

        return Ok(user);
    }

    
   

    // Eliminar un usuario
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _db.Admins.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            _logger.LogError($"Usuario con ID {id} no encontrado.");
            return NotFound("Usuario no encontrado.");
        }

        _db.Admins.Remove(user);
        _db.SaveChanges();
        _logger.LogInformation($"Usuario con ID {id} eliminado exitosamente.");

        return Ok("Usuario eliminado exitosamente.");
    }
}
