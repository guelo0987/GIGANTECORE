﻿using System;
using System.Collections.Generic;
using GIGANTECORE.Models;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECORE.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Compañium> Compañia { get; set; }

    public virtual DbSet<DetalleSolicitud> DetalleSolicituds { get; set; }

    public virtual DbSet<HistorialCorreo> HistorialCorreos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Solicitud> Solicituds { get; set; }

    public virtual DbSet<SubCategorium> SubCategoria { get; set; }

    public virtual DbSet<UsuarioCliente> UsuarioClientes { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admin__3214EC07A4E8858C");

            entity.ToTable("Admin");

            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Mail).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .HasDefaultValue("Administrador");
            entity.Property(e => e.SoloLectura).HasDefaultValue(false);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carrito__3214EC0784CC7F99");

            entity.ToTable("Carrito");

            entity.HasIndex(e => e.ProductoId, "IX_Carrito_ProductoId");

            entity.HasIndex(e => e.UsuarioId, "IX_Carrito_UsuarioId");

            entity.HasOne(d => d.Producto).WithMany(p => p.Carritos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carrito__Product__6C190EBB");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Carritos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carrito__Usuario__46E78A0C");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07791AF110");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Compañium>(entity =>
        {
            entity.HasKey(e => e.Rnc).HasName("PK__Compañia__CAFF6951C669784F");

            entity.Property(e => e.Rnc)
                .HasMaxLength(11)
                .HasColumnName("RNC");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<DetalleSolicitud>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__DetalleS__E43646A58EBFD1A7");

            entity.ToTable("DetalleSolicitud");

            entity.HasIndex(e => e.IdSolicitud, "IX_DetalleSolicitud_IdSolicitud");

            entity.HasIndex(e => e.ProductoId, "IX_DetalleSolicitud_ProductoId");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.DetalleSolicituds)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleSo__IdSol__4D94879B");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleSolicituds)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleSo__Produ__6D0D32F4");
        });

        modelBuilder.Entity<HistorialCorreo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC0765966D64");

            entity.ToTable("HistorialCorreo");

            entity.HasIndex(e => e.SolicitudId, "IX_HistorialCorreo_SolicitudId");

            entity.HasIndex(e => e.UsuarioId, "IX_HistorialCorreo_UsuarioId");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Enviado");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Solicitud).WithMany(p => p.HistorialCorreos)
                .HasForeignKey(d => d.SolicitudId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Solic__66603565");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialCorreos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Usuar__656C112C");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__tmp_ms_x__06370DAD53EE836F");

            entity.HasIndex(e => e.CategoriaId, "IX_Productos_CategoriaId");

            entity.HasIndex(e => e.SubCategoriaId, "IX_Productos_SubCategoriaId");

            entity.Property(e => e.Codigo).ValueGeneratedNever();
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Stock).HasDefaultValue(true);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("FK_Productos_Categoria");

            entity.HasOne(d => d.SubCategoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.SubCategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Productos__SubCa__6EF57B66");
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__Solicitu__36899CEF05DB70D6");

            entity.ToTable("Solicitud");

            entity.HasIndex(e => e.UsuarioId, "IX_Solicitud_UsuarioId");

            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Solicituds)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__Usuar__4AB81AF0");
        });

        modelBuilder.Entity<SubCategorium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SubCateg__3214EC079C3939DB");

            entity.HasIndex(e => e.CategoriaId, "IX_SubCategoria_CategoriaId");

            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Categoria).WithMany(p => p.SubCategoria)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubCatego__Categ__3F466844");
        });

        modelBuilder.Entity<UsuarioCliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsuarioC__3214EC07D79F99F6");

            entity.ToTable("UsuarioCliente");

            entity.HasIndex(e => e.Rnc, "IX_UsuarioCliente_RNC");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Rnc)
                .HasMaxLength(11)
                .HasColumnName("RNC");
            entity.Property(e => e.Rol).HasDefaultValue("");
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.RncNavigation).WithMany(p => p.UsuarioClientes)
                .HasForeignKey(d => d.Rnc)
                .HasConstraintName("FK_UsuarioCliente_Compañia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}