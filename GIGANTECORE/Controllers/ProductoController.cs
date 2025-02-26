using System.Security.Claims;
using GIGANTECORE.Context;
using GIGANTECORE.DTO;
using GIGANTECORE.Models;
using GIGANTECORE.Utils;
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
    [Consumes("multipart/form-data")]
    public IActionResult AddOrUpdateProducto([FromForm] ProductoDTO producto, IFormFile? imageFile)
    {
    if (producto == null || string.IsNullOrWhiteSpace(producto.Nombre))
    {
        return BadRequest(new { Message = "Los datos del producto son obligatorios." });
    }

    var adminMedia = new AdminProductoMedia(_db);

    // Si el producto ya existe, actualizamos
    var existingProducto = _db.Productos.FirstOrDefault(p => p.Codigo == producto.Codigo);

    if (existingProducto != null)
    {
        // Actualizar imagen si se proporciona una nueva
        if (imageFile != null)
        {
            existingProducto.ImageUrl = adminMedia.Update(imageFile, existingProducto.ImageUrl);
        }

        existingProducto.Nombre = producto.Nombre;
        existingProducto.Marca = producto.Marca;
        existingProducto.Stock = producto.Stock;
        existingProducto.Descripcion = producto.Descripcion;
        existingProducto.SubCategoriaId = producto.SubCategoriaId;
        existingProducto.CategoriaId = producto.CategoriaId;
        existingProducto.EsDestacado = producto.EsDestacado;

        _db.SaveChanges();
        return Ok(new { Message = "Producto actualizado exitosamente.", Producto = existingProducto });
    }
    else
    {
        // Crear nuevo producto
        if (_db.Productos.Any(p => p.Nombre.ToLower() == producto.Nombre.ToLower()))
        {
            return Conflict(new { Message = $"Ya existe un producto con el nombre '{producto.Nombre}'." });
        }

        string fileName = null;
        if (imageFile != null)
        {
            fileName = adminMedia.Upload(imageFile);
        }

        var newProducto = new Producto
        {
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Marca = producto.Marca,
            Stock = producto.Stock,
            Descripcion = producto.Descripcion,
            SubCategoriaId = producto.SubCategoriaId,
            CategoriaId = producto.CategoriaId,
            ImageUrl = fileName,
            EsDestacado = producto.EsDestacado
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

        var adminMedia = new AdminProductoMedia(_db);
        adminMedia.Delete(producto.ImageUrl);

        _db.Productos.Remove(producto);
        _db.SaveChanges();

        return Ok(new { Message = "Producto eliminado exitosamente." });
    }

}
