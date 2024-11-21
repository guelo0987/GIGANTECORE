using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using GIGANTECORE.Context;

public class RolePermissionMiddleware
{
    private readonly RequestDelegate _next;

    public RolePermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
        var path = context.Request.Path.Value;
        if (path != null && (path.StartsWith("/api/Auth/login")))
        {
            await _next(context);
            return;
        }
        
        // Resolver el DbContext dentro del alcance de la solicitud
        var db = context.RequestServices.GetRequiredService<MyDbContext>();
        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        
        Console.WriteLine(userRole);

        if (userRole == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Acceso denegado: Usuario no autenticado.");
            return;
        }

        // Si el rol es Admin, permitir acceso completo
        if (userRole == "Admin")
        {
            await _next(context);
            return;
        }

        // Extraer el nombre del controlador desde la ruta
        var controllerName = context.Request.Path.Value?.Split('/')[2];
        if (controllerName == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Error en la ruta.");
            return;
        }

        // Verificar permisos en la tabla RolePermisos
        var permission = db.RolePermisos
            .FirstOrDefault(p => p.Role == userRole && p.TableName == controllerName);

        if (permission == null || !permission.CanRead)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync($"Acceso denegado: El rol '{userRole}' no tiene permiso para leer la tabla '{controllerName}'.");
            return;
        }

        // Continuar con la solicitud
        await _next(context);
    }
}