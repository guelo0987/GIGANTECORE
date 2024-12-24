using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;
using GIGANTECORE.Context;

namespace GIGANTECORE.Services
{
    public class AdminProductoMedia
    {
        private readonly MyDbContext _context;

        public AdminProductoMedia(MyDbContext context)
        {
            _context = context;
        }

        public string Upload(IFormFile file)
        {
            List<string> validExtensions = new List<string> { ".jpg", ".png", ".jpeg" };
            string extension = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extension))
            {
                return $"Archivo no permitido: {string.Join(", ", validExtensions)}";
            }

            if (file.Length > (5 * 1024 * 1024))
            {
                return "El archivo no puede sobrepasar los 5MB.";
            }

            string fileName = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Productos");
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

        public bool Delete(string fileName)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Imagenes", "Productos", fileName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Update(IFormFile newFile, string oldFileName)
        {
            if (!string.IsNullOrEmpty(oldFileName))
            {
                Delete(oldFileName);
            }

            return Upload(newFile);
        }
    }
}
