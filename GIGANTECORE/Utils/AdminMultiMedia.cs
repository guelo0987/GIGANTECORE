using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GIGANTECORE.Models;

public class AdminMultiMedia
{
    private readonly DbContext _context;

    public AdminMultiMedia(DbContext context)
    {
        _context = context;
    }

    public async Task<string> Upload(IFormFile file)
    {
        List<string> validar = new List<string>{".jpg", ".png", ".jpeg"};
        string extension = Path.GetExtension(file.FileName);

        if (!validar.Contains(extension))
        {
            return $"Archivo no permitido: {string.Join(',', validar)}";
        }

        long size = file.Length;
        if (size > (5 * 1024 * 1024)) // Corregido a 5MB
        {
            return "El archivo no puede sobrepasar los 5mb";
        }

        string fileName = Guid.NewGuid().ToString() + extension;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Banners");
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Crear nuevo banner en la base de datos
        var banner = new Banner
        {
            ImageUrl = fileName,
            Active = true,
            OrderIndex = await _context.Set<Banner>().MaxAsync(b => (int?)b.OrderIndex) + 1 ?? 1
        };

        _context.Set<Banner>().Add(banner);
        await _context.SaveChangesAsync();

        return fileName;
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var banner = await _context.Set<Banner>().FindAsync(id);
            if (banner == null) return false;

            // Eliminar archivo físico
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Banners", banner.ImageUrl);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // Eliminar de la base de datos
            _context.Set<Banner>().Remove(banner);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ReorderImages(List<(int id, int newOrder)> newOrders)
    {
        try
        {
            foreach (var (id, newOrder) in newOrders)
            {
                var banner = await _context.Set<Banner>().FindAsync(id);
                if (banner != null)
                {
                    banner.OrderIndex = newOrder;
                }
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Banner>> GetImages()
    {
        return await _context.Set<Banner>()
            .Where(b => b.Active)
            .OrderBy(b => b.OrderIndex)
            .ToListAsync();
    }

    public async Task<bool> ToggleActive(int id)
    {
        try
        {
            var banner = await _context.Set<Banner>().FindAsync(id);
            if (banner == null) return false;

            banner.Active = !banner.Active;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}