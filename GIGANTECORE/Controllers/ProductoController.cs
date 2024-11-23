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
public class ProductoController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<ProductoController> _logger;

    public ProductoController(ILogger<ProductoController> logger, MyDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public IActionResult GetProductos()
    {
        // Retorna todos los productos
        return Ok(_db.Productos.ToList());
    }
    
    
    
    [HttpGet("{codigo}")]
    public IActionResult GetProductoId(int codigo)
    {
        var producto = _db.Productos
            .FirstOrDefault(u => u.Codigo == codigo);

        if (producto == null)
        {
            _logger.LogError($"Producto con ID {codigo} no encontrada.");
            return NotFound("Producto no encontrada.");
        }

        return Ok(producto);
    }

    [HttpPost]
    public IActionResult AddOrUpdateProducto([FromBody] ProductoDTO producto)
    {
        if (producto == null)
        {
            return BadRequest(new { Message = "El cuerpo de la solicitud no puede estar vacío." });
        }

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            return BadRequest(new { Message = "El nombre del producto es obligatorio." });
        }

        if (producto.SubCategoriaId <= 0 || !_db.SubCategoria.Any(sc => sc.Id == producto.SubCategoriaId))
        {
            return BadRequest(new { Message = "La subcategoría asociada no es válida." });
        }

        if (producto.CategoriaId != null && !_db.Categoria.Any(c => c.Id == producto.CategoriaId))
        {
            return BadRequest(new { Message = "La categoría asociada no es válida." });
        }

        // Buscar si el producto ya existe basado en el código
        var existingProducto = _db.Productos.FirstOrDefault(p => p.Codigo == producto.Codigo);


        if (producto.Codigo == 0)
        {
            return Conflict(new { Message = $"El codigo No puede ser 0" });
        }

        if (existingProducto != null) // Actualizar producto existente
        {
            if (_db.Productos.Any(p => p.Codigo != producto.Codigo && p.Nombre.ToLower() == producto.Nombre.ToLower()))
            {
                return Conflict(new { Message = $"Ya existe otro producto con el nombre '{producto.Nombre}'." });
            }

            // Actualizar los datos del producto
            existingProducto.Nombre = producto.Nombre;
            existingProducto.Marca = producto.Marca;
            existingProducto.Stock = producto.Stock;
            existingProducto.SubCategoriaId = producto.SubCategoriaId;
            existingProducto.ImageUrl = producto.ImageUrl;
            existingProducto.CategoriaId = producto.CategoriaId;

            _db.SaveChanges();
            return Ok(new { Message = "Producto actualizado exitosamente.", Producto = existingProducto });
        }
        else // Crear nuevo producto
        {
            if (_db.Productos.Any(p => p.Nombre.ToLower() == producto.Nombre.ToLower()))
            {
                return Conflict(new { Message = $"Ya existe un producto con el nombre '{producto.Nombre}'." });
            }

            var newProducto = new Producto
            {
                Codigo = producto.Codigo, // Usar el código generado externamente
                Nombre = producto.Nombre,
                Marca = producto.Marca,
                Stock = producto.Stock,
                SubCategoriaId = producto.SubCategoriaId,
                ImageUrl = producto.ImageUrl,
                CategoriaId = producto.CategoriaId
            };

            _db.Productos.Add(newProducto);
            _db.SaveChanges();

            return Ok(new { Message = "Producto creado exitosamente.", Producto = newProducto });
        }
    }

    [HttpDelete("{codigo}")]
    public IActionResult DeleteProducto(int codigo)
    {
        var producto = _db.Productos.FirstOrDefault(p => p.Codigo == codigo);
        if (producto == null)
        {
            return NotFound(new { Message = "El producto no fue encontrado." });
        }

        _db.Productos.Remove(producto);
        _db.SaveChanges();

        return Ok(new { Message = "Producto eliminado exitosamente." });
    }
}
