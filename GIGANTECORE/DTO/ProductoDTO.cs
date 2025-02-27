namespace GIGANTECORE.DTO;

public class ProductoDTO
{
    public int Codigo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Marca { get; set; }
    public bool? Stock { get; set; }
    public int SubCategoriaId { get; set; }
    public string? ImageUrl { get; set; }
    public int? CategoriaId { get; set; }
    
    public string? Descripcion { get; set; }
    
    public bool? EsDestacado { get; set; }
    
    public string? Medidas { get; set; }
}
