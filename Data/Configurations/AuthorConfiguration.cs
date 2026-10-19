using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Data.Configurations;

// Muallif jadvali ustunlari va cheklovlarini Fluent API orqali sozlash
public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        // Jadval nomi
        builder.ToTable("Authors");

        // Birlamchi kalit (Primary Key)
        builder.HasKey(x => x.Id);

        // Ism: majburiy va maksimal uzunligi 50 ta belgi
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        // Familiya: majburiy va maksimal uzunligi 50 ta belgi
        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        // Manzil: ixtiyoriy (null bo'lishi mumkin) va maksimal uzunligi 200 ta belgi
        builder.Property(x => x.Address)
            .IsRequired(false)
            .HasMaxLength(200);

        // Tug'ilgan sana: ixtiyoriy
        builder.Property(x => x.BirthDate)
            .IsRequired(false);
    }
}