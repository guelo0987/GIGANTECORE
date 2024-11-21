using System.Security.Claims;
using GIGANTECORE.Context;
using GIGANTECORE.DTO;
using GIGANTECORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIGANTECORE.Controllers;



[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriaController:ControllerBase
{



    private readonly MyDbContext _db;
    private readonly ILogger<CategoriaController> _logger;



    public CategoriaController(ILogger<CategoriaController> logger, MyDbContext db)
    {
        _logger = logger;
        _db = db;
    }



    
    [HttpGet]
    public IActionResult GetCategorias()
    {
        // No necesitas verificar permisos aquí, ya que el middleware lo maneja
        return Ok(_db.Categoria.ToList());
    }

    
    [HttpPost]
    public IActionResult AddCategoria([FromBody] CategoriumDTO categoria)
    {
        // Validar que el objeto no sea nulo
        if (categoria == null)
        {
            return BadRequest(new { Message = "El cuerpo de la solicitud no puede estar vacío." });
        }

        // Validar que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(categoria.Nombre))
        {
            return BadRequest(new { Message = "El nombre de la categoría es obligatorio." });
        }

        // Validar que no exista otra categoría con el mismo nombre (case insensitive)
        if (_db.Categoria.Any(c => c.Nombre.ToLower() == categoria.Nombre.ToLower()))
        {
            return Conflict(new { Message = $"Ya existe una categoría con el nombre '{categoria.Nombre}'." });
        }

        // Agregar la nueva categoría a la base de datos

        var Newcat = new Categorium
        {
            Nombre = categoria.Nombre,

        };
        
        
        _db.Categoria.Add(Newcat);
        _db.SaveChanges();

        return Ok(new { Message = "Categoría agregada exitosamente.", Categoria = categoria });
    }


    
    
    
    
    
    
    


}