﻿using System;
using System.Collections.Generic;

namespace GIGANTECORE.Models;

public partial class HistorialCorreo
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int SolicitudId { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public string? Estado { get; set; }

    public virtual Solicitud Solicitud { get; set; } = null!;

    public virtual UsuarioCliente Usuario { get; set; } = null!;
}