using System.Security.Claims;
using GIGANTECORE.Context;
using GIGANTECORE.DTO;
using GIGANTECORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECORE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<SolicitudController> _logger;

    public SolicitudController(ILogger<SolicitudController> logger, MyDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public IActionResult GetSolicitudes()
    {
        // Retorna todos los productos
        return Ok(_db.Solicituds.ToList());
    }



    [HttpGet("{Id}")]
    public IActionResult GetSolicitudesId(int Id)
    {
        var solicitud = _db.Solicituds
            .FirstOrDefault(u => u.IdSolicitud == Id);

        if (solicitud == null)
        {
            _logger.LogError($"Producto con ID {Id} no encontrada.");
            return NotFound("Producto no encontrada.");
        }

        return Ok(solicitud);
    }

}