using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using N5.Permissions.Domain.Entities;

namespace N5.Permissions.Infrastructure.Persistence.Configurations;

public class PermissionTypeConfiguration : IEntityTypeConfiguration<PermissionType>
{
    public void Configure(EntityTypeBuilder<PermissionType> builder)
    {
        builder.ToTable("PermissionTypes");
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Descripcion)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasData(
            new PermissionType { Id = 1, Descripcion = "Enfermedad" },
            new PermissionType { Id = 2, Descripcion = "Tramite" },
            new PermissionType { Id = 3, Descripcion = "Mudanza" },
            new PermissionType { Id = 4, Descripcion = "Vacaciones" }
        );
    }
}

