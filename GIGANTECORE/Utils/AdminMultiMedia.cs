using GIGANTECORE.Context;
using GIGANTECORE.Models;

namespace GIGANTECORE.Utils;

public class AdminMultiMedia
{
    private readonly MyDbContext _context;

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
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Banners");
        FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
        file.CopyTo(stream);

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

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Banners", banner.ImageUrl);
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