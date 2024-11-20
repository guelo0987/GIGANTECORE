using GIGANTECORE.Context;
using GIGANTECORE.DTO;
using GIGANTECORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIGANTECORE.Controllers;



[ApiController]
[Route("api/Permisos")]
[Authorize(Policy = "RequireAdministratorRole")]
public class PermissionController:ControllerBase
{


    private readonly MyDbContext _db;
    private readonly ILogger<PermissionController> _logger;

    public PermissionController(ILogger<PermissionController> logger, MyDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    [HttpGet]
    public IActionResult GetPermissions()
    {
        var permissions = _db.RolePermisos.ToList();
        return Ok(permissions);
    }
    
    [HttpPost]
    public IActionResult AddOrUpdatePermission([FromBody] RolePermissionDTO permission)
    {
        var existingPermission = _db.RolePermisos
            .FirstOrDefault(p => p.Role == permission.Role && p.TableName == permission.TableName);

        if (existingPermission != null)
        {
            existingPermission.CanCreate = permission.CanCreate;
            existingPermission.CanRead = permission.CanRead;
            existingPermission.CanUpdate = permission.CanUpdate;
            existingPermission.CanDelete = permission.CanDelete;
        }
        else
        {

            var Permiso = new RolePermiso
            {
                Role = permission.Role,
                TableName = permission.TableName,
                CanCreate = permission.CanCreate,
                CanDelete = permission.CanDelete,
                CanRead = permission.CanRead,
                CanUpdate = permission.CanUpdate
                
            };
            
            _db.RolePermisos.Add(Permiso);
        }

        _db.SaveChanges();
        return Ok("Permisos actualizados correctamente.");
    }
    
    
    
    
    
    
    
    
    
    
    
}