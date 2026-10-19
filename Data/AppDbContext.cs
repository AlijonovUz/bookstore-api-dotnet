using Microsoft.EntityFrameworkCore;
using FirstBackendApp.Entities;

namespace FirstBackendApp.Data;

// Ma'lumotlar bazasi bilan aloqa qiluvchi asosiy DbContext klassi
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    // Foydalanuvchilar jadvali (Users table)
    public DbSet<User> Users => Set<User>();
    
    // Mualliflar jadvali (Authors table)
    public DbSet<Author> Authors => Set<Author>();

    // Kitoblar jadvali (Books table)
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Data/Configurations papkasidagi barcha IEntityTypeConfiguration klasslarini avtomatik qo'llash
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}