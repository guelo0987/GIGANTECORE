using System.Security.Claims;
using GIGANTECORE.Context;
using Microsoft.AspNetCore.Mvc.Controllers;

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
        Console.WriteLine($"Ruta completa: {path}");

        // Excluir rutas específicas (por ejemplo, login)
        if (path != null && path.StartsWith("/api/Auth/login"))
        {
            await _next(context);
            return;
        }

        // Resolver el DbContext
        var db = context.RequestServices.GetRequiredService<MyDbContext>();
        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        Console.WriteLine($"Rol del usuario: {userRole}");

        if (userRole == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Acceso denegado: Usuario no autenticado.");
            return;
        }

        // Permitir acceso completo para Admin
        if (userRole == "Admin")
        {
            Console.WriteLine("Acceso permitido para el rol 'Admin'.");
            await _next(context);
            return;
        }

        // Obtener el nombre del controlador
        var endpoint = context.GetEndpoint();
        var controllerName = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>()?.ControllerName;
        Console.WriteLine($"Controller Name: {controllerName}");

        if (controllerName == null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Error en la ruta.");
            return;
        }

        // Verificar permisos en la tabla RolePermisos
        var permission = db.RolePermisos
            .FirstOrDefault(p => p.Role == userRole && p.TableName == controllerName);

        if (permission == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync($"Acceso denegado: No se encontraron permisos para el rol '{userRole}' en la tabla '{controllerName}'.");
            return;
        }

        // Validar los permisos según el método HTTP
        switch (context.Request.Method)
        {
            case "GET":
                if (!permission.CanRead)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Acceso denegado: El rol '{userRole}' no tiene permiso para leer en la tabla '{controllerName}'.");
                    return;
                }
                break;

            case "POST":
                if (!permission.CanCreate)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Acceso denegado: El rol '{userRole}' no tiene permiso para crear en la tabla '{controllerName}'.");
                    return;
                }
                break;

            case "PUT":
                if (!permission.CanUpdate)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Acceso denegado: El rol '{userRole}' no tiene permiso para actualizar en la tabla '{controllerName}'.");
                    return;
                }
                break;

            case "DELETE":
                if (!permission.CanDelete)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Acceso denegado: El rol '{userRole}' no tiene permiso para eliminar en la tabla '{controllerName}'.");
                    return;
                }
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                await context.Response.WriteAsync($"Método HTTP '{context.Request.Method}' no permitido.");
                return;
        }

        // Continuar con la solicitud
        await _next(context);
    }
}
