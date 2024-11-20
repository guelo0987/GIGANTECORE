using GIGANTECORE.Context;
using Microsoft.AspNetCore.Mvc;

namespace GIGANTECORE.Controllers;



[ApiController]
[Route("api/CategoriApi")]
public class CategoriaController:ControllerBase
{



    private readonly MyDbContext _db;
    private readonly ILogger<CategoriaController> _logger;



    public CategoriaController(ILogger<CategoriaController> logger, MyDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    
    
    
    


}