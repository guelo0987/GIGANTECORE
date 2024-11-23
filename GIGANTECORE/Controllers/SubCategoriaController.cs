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
public class SubCategoriaController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<SubCategoriaController> _logger;

    public SubCategoriaController(ILogger<SubCategoriaController> logger, MyDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public IActionResult GetSubCategorias()
    {
        // Retorna todas las subcategorías
        return Ok(_db.SubCategoria.ToList());
    }
    
    
    [HttpGet("{id}")]
    public IActionResult GetSubCategoriaId(int id)
    {
        var subCategorium = _db.SubCategoria
            .FirstOrDefault(u => u.Id == id);

        if (subCategorium == null)
        {
            _logger.LogError($"Subcategoria con ID {id} no encontrada.");
            return NotFound("Subcategoria no encontrada.");
        }

        return Ok(subCategorium);
    }

    [HttpPost]
    public IActionResult AddOrUpdateSubCategoria([FromBody] SubCategoriumDTO subCategoria)
    {
        if (subCategoria == null)
        {
            return BadRequest(new { Message = "El cuerpo de la solicitud no puede estar vacío." });
        }

        if (string.IsNullOrWhiteSpace(subCategoria.Nombre))
        {
            return BadRequest(new { Message = "El nombre de la subcategoría es obligatorio." });
        }

        if (subCategoria.CategoriaId <= 0 || !_db.Categoria.Any(c => c.Id == subCategoria.CategoriaId))
        {
            return BadRequest(new { Message = "La categoría asociada no es válida." });
        }

        if (subCategoria.Id > 0) // Actualizar subcategoría existente
        {
            var existingSubCategoria = _db.SubCategoria.FirstOrDefault(sc => sc.Id == subCategoria.Id);

            if (existingSubCategoria == null)
            {
                return NotFound(new { Message = "La subcategoría no fue encontrada para su actualización." });
            }

            if (_db.SubCategoria.Any(sc => sc.Id != subCategoria.Id && sc.Nombre.ToLower() == subCategoria.Nombre.ToLower()))
            {
                return Conflict(new { Message = $"Ya existe otra subcategoría con el nombre '{subCategoria.Nombre}'." });
            }

            // Actualizar los datos de la subcategoría
            existingSubCategoria.Nombre = subCategoria.Nombre;
            existingSubCategoria.CategoriaId = subCategoria.CategoriaId;

            _db.SaveChanges();
            return Ok(new { Message = "Subcategoría actualizada exitosamente.", SubCategoria = existingSubCategoria });
        }
        else // Crear nueva subcategoría
        {
            if (_db.SubCategoria.Any(sc => sc.Nombre.ToLower() == subCategoria.Nombre.ToLower()))
            {
                return Conflict(new { Message = $"Ya existe una subcategoría con el nombre '{subCategoria.Nombre}'." });
            }

            var newSubCategoria = new SubCategorium
            {
                Nombre = subCategoria.Nombre,
                CategoriaId = subCategoria.CategoriaId
            };

            _db.SubCategoria.Add(newSubCategoria);
            _db.SaveChanges();

            return Ok(new { Message = "Subcategoría creada exitosamente.", SubCategoria = newSubCategoria });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteSubCategoria(int id)
    {
        var subCategoria = _db.SubCategoria.FirstOrDefault(sc => sc.Id == id);
        if (subCategoria == null)
        {
            return NotFound(new { Message = "La subcategoría no fue encontrada." });
        }

        _db.SubCategoria.Remove(subCategoria);
        _db.SaveChanges();

        return Ok(new { Message = "Subcategoría eliminada exitosamente." });
    }
}
