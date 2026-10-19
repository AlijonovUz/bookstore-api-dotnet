using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Data.Configurations;

// Kitob jadvali ustunlari, cheklovlari va bog'liqliklarini Fluent API orqali sozlash
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Jadval nomi
        builder.ToTable("Books");

        // Birlamchi kalit (Primary Key)
        builder.HasKey(x => x.Id);
        
        // Kitob nomi: majburiy va maksimal uzunligi 150 ta belgi
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);
        
        // Tavsif: maksimal 1000 ta belgi va boshlang'ich qiymat bo'sh satr
        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .HasDefaultValue(string.Empty);

        // Narx: aniqligi (12 ta raqam, shundan 2 tasi verguldan keyin)
        builder.Property(x => x.Price)
            .HasPrecision(12, 2);
        
        // Sahifalar soni: majburiy
        builder.Property(x => x.PageCount)
            .IsRequired();
        
        // Omborda mavjudligi: boshlang'ich qiymat false
        builder.Property(x => x.InStock)
            .HasDefaultValue(false);
        
        // ISBN: majburiy, maksimal 20 ta belgi
        builder.Property(x => x.Isbn)
            .IsRequired()
            .HasMaxLength(20);
        
        // ISBN uchun unikal indeks (bazada bir xil ISBN takrorlanmasligi uchun)
        builder.HasIndex(x => x.Isbn)
            .IsUnique();

        // Rasm havolasi: ixtiyoriy, maksimal 500 ta belgi
        builder.Property(x => x.ImageUrl)
            .IsRequired(false)
            .HasMaxLength(500);
        
        // Aloqa (Relationship): Har bir kitob bitta muallifga tegishli (One-to-Many)
        // Agar muallif o'chirilsa, uning barcha kitoblari ham o'chiriladi (Cascade)
        builder.HasOne(a => a.Author)
            .WithMany(b => b.Books)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}