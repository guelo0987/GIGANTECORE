﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GIGANTECORE.Models;

public partial class Producto
{
    public int Codigo { get; set; }

    
    public string Nombre { get; set; } = null!;

    public string? Marca { get; set; }

    public bool? Stock { get; set; }

    public int SubCategoriaId { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoriaId { get; set; }

    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

    public virtual Categorium? Categoria { get; set; }

    public virtual ICollection<DetalleSolicitud> DetalleSolicituds { get; set; } = new List<DetalleSolicitud>();

    public virtual SubCategorium SubCategoria { get; set; } = null!;
}