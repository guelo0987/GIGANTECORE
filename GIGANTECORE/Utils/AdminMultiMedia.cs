using GIGANTECORE.Context;
using GIGANTECORE.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GIGANTECORE.Utils;

public class AdminMultiMedia
{
    private readonly MyDbContext _context;
    private const string SharedImagesPath = "/Users/miguelcruz/ImageGigante/Banners"; // Ruta modificada

    public AdminMultiMedia(MyDbContext context)
    {
        _context = context;
    }

    public string Upload(IFormFile file)
    {
        List<string> validar = new List<string>{".jpg", ".png", ".jpeg"};
        string extension = Path.GetExtension(file.FileName);

        if (!validar.Contains(extension))
        {
            return $"Archivo no permitido: {string.Join(',', validar)}";
        }

        long size = file.Length;
        if (size > (5 * 1024 * 1024))
        {
            return "El archivo no puede sobrepasar los 5mb";
        }

        string fileName = Guid.NewGuid().ToString() + extension;
        
        // Ruta modificada
        if (!Directory.Exists(SharedImagesPath))
        {
            Directory.CreateDirectory(SharedImagesPath);
        }

        using (var stream = new FileStream(Path.Combine(SharedImagesPath, fileName), FileMode.Create))
        {
            file.CopyTo(stream);
        }

        var banner = new Banner
        {
            ImageUrl = fileName,
            Active = true,
            OrderIndex = _context.Banners.Max(b => (int?)b.OrderIndex) + 1 ?? 1
        };

        _context.Banners.Add(banner);
        _context.SaveChanges();

        return fileName;
    }

    public bool Delete(int id)
    {
        try
        {
            var banner = _context.Banners.Find(id);
            if (banner == null) return false;

            // Ruta modificada
            string path = Path.Combine(SharedImagesPath, banner.ImageUrl);
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            _context.Banners.Remove(banner);
            _context.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Los demás métodos permanecen igual (no usan rutas de archivo)
    public bool ReorderImages(List<(int id, int newOrder)> newOrders)
    {
        try
        {
            foreach (var (id, newOrder) in newOrders)
            {
                var banner = _context.Banners.Find(id);
                if (banner != null)
                {
                    banner.OrderIndex = newOrder;
                }
            }
            
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<Banner> GetImages()
    {
        return _context.Banners
            .OrderBy(b => b.OrderIndex)
            .ToList();
    }

    public bool ToggleActive(int id)
    {
        try
        {
            var banner = _context.Banners.Find(id);
            if (banner == null) return false;

            banner.Active = !banner.Active;
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}