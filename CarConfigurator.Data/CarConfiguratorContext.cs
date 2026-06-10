using System;
using System.Collections.Generic;
using CarConfigurator.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CarConfigurator.Data;

public partial class CarConfiguratorContext : DbContext
{
    public CarConfiguratorContext(DbContextOptions<CarConfiguratorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<CarComponent> CarComponents { get; set; }

    public virtual DbSet<CarModel> CarModels { get; set; }

    public virtual DbSet<ComponentCompatibility> ComponentCompatibilities { get; set; }

    public virtual DbSet<ComponentType> ComponentTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<UserConfiguration> UserConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.IdApplicationUser).HasName("PK__Applicat__4CA7007470E168DC");

            entity.ToTable("ApplicationUser");

            entity.HasIndex(e => e.Username, "UQ__Applicat__536C85E4FF57C540").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__Applicat__737584F6D5783919").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Applicat__A9D10534AFC4A097").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PwdHash).HasMaxLength(255);
            entity.Property(e => e.PwdSalt).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<CarComponent>(entity =>
        {
            entity.HasKey(e => e.IdCarComponent).HasName("PK__CarCompo__7F5C0E8ABE345C8D");

            entity.ToTable("CarComponent");

            entity.HasIndex(e => e.Name, "UQ__CarCompo__737584F69C07B33F").IsUnique();

            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ComponentType).WithMany(p => p.CarComponents)
                .HasForeignKey(d => d.ComponentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarCompon__Compo__04E4BC85");
        });

        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasKey(e => e.IdCarModel).HasName("PK__CarModel__0732E775978AE91B");

            entity.ToTable("CarModel");

            entity.HasIndex(e => e.Name, "UQ__CarModel__737584F67E768480").IsUnique();

            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Manufacturer).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasMany(d => d.CarComponents).WithMany(p => p.CarModels)
                .UsingEntity<Dictionary<string, object>>(
                    "CarModelCarComponent",
                    r => r.HasOne<CarComponent>().WithMany()
                        .HasForeignKey("CarComponentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CarModelC__CarCo__08B54D69"),
                    l => l.HasOne<CarModel>().WithMany()
                        .HasForeignKey("CarModelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CarModelC__CarMo__07C12930"),
                    j =>
                    {
                        j.HasKey("CarModelId", "CarComponentId").HasName("PK__CarModel__9B5123094FA9DE03");
                        j.ToTable("CarModelCarComponent");
                    });
        });

        modelBuilder.Entity<ComponentCompatibility>(entity =>
        {
            entity.HasKey(e => new { e.ComponentId, e.CompatibleWithId }).HasName("PK__Componen__10EF70A93F52F213");

            entity.ToTable("ComponentCompatibility");

            entity.HasOne(d => d.CompatibleWith).WithMany(p => p.ComponentCompatibilityCompatibleWiths)
                .HasForeignKey(d => d.CompatibleWithId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Component__Compa__0C85DE4D");

            entity.HasOne(d => d.Component).WithMany(p => p.ComponentCompatibilityComponents)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Component__Compo__0B91BA14");
        });

        modelBuilder.Entity<ComponentType>(entity =>
        {
            entity.HasKey(e => e.IdComponentType).HasName("PK__Componen__60039BD32D6A0B5E");

            entity.ToTable("ComponentType");

            entity.HasIndex(e => e.Name, "UQ__Componen__737584F6E52912E5").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.IdImage).HasName("PK__Image__C8C63E4A204D26D6");

            entity.ToTable("Image");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.CarComponent).WithMany(p => p.Images)
                .HasForeignKey(d => d.CarComponentId)
                .HasConstraintName("FK__Image__CarCompon__17F790F9");

            entity.HasOne(d => d.CarModel).WithMany(p => p.Images)
                .HasForeignKey(d => d.CarModelId)
                .HasConstraintName("FK__Image__CarModelI__18EBB532");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK__Log__0C54DBC673BB4021");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(20);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserConfiguration>(entity =>
        {
            entity.HasKey(e => e.IdUserConfiguration).HasName("PK__UserConf__3F24B8D5CA60F19C");

            entity.ToTable("UserConfiguration");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CarModel).WithMany(p => p.UserConfigurations)
                .HasForeignKey(d => d.CarModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserConfi__CarMo__10566F31");

            entity.HasOne(d => d.User).WithMany(p => p.UserConfigurations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserConfi__UserI__0F624AF8");

            entity.HasMany(d => d.CarComponents).WithMany(p => p.UserConfigurations)
                .UsingEntity<Dictionary<string, object>>(
                    "UserConfigurationComponent",
                    r => r.HasOne<CarComponent>().WithMany()
                        .HasForeignKey("CarComponentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserConfi__CarCo__151B244E"),
                    l => l.HasOne<UserConfiguration>().WithMany()
                        .HasForeignKey("UserConfigurationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserConfi__UserC__14270015"),
                    j =>
                    {
                        j.HasKey("UserConfigurationId", "CarComponentId").HasName("PK__UserConf__7DFFAB6AE8182176");
                        j.ToTable("UserConfigurationComponent");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
