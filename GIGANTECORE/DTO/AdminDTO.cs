namespace GIGANTECORE.DTO;

public class AdminDTO
{
   
    public string Nombre { get; set; } = null!;
    public string Mail { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Rol { get; set; }
  
    public string? Telefono { get; set; }
    public bool? SoloLectura { get; set; }
}
