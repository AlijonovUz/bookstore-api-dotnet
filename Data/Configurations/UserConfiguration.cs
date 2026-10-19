using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Data.Configurations;

// Foydalanuvchi jadvali ustunlari va cheklovlarini Fluent API orqali sozlash
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Birlamchi kalit
        builder.HasKey(x => x.Id);
        
        // Username: majburiy, maksimal 32 belgi va unikal
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.HasIndex(x => x.Username)
            .IsUnique();

        // Ism va familiya (ixtiyoriy)
        builder.Property(x => x.FirstName)
            .HasMaxLength(50);
        
        builder.Property(x => x.LastName)
            .HasMaxLength(50);
        
        // Email: maksimal 100 belgi va unikal
        builder.Property(x => x.Email)
            .HasMaxLength(100);
        
        builder.HasIndex(x => x.Email)
            .IsUnique();
            
        // Foydalanuvchi roli (string ko'rinishida saqlanadi)
        builder.Property(x => x.Role)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(5);
    }
}