using System.Security.Claims;
using GIGANTECORE.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIGANTECORE.Controllers;



[ApiController]
[Route("api/CategoriApi")]
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


    
    
    
    
    
    
    


}