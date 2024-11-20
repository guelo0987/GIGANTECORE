using System;
using System.Collections.Generic;

namespace GIGANTECORE.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Rol { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public string? Telefono { get; set; }

    public bool? SoloLectura { get; set; }
}
