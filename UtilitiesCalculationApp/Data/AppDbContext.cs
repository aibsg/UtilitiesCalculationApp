using Microsoft.EntityFrameworkCore;
using UtilitiesCalculationApp.Models;

namespace UtilitiesCalculationApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Variables>  Variables { get; set; }
    public DbSet<ResidentsNumber> ResidentsNumbers { get; set; }
    public DbSet<Readings> Readings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Variables>()
            .HasIndex(v => v.UtilityName)
            .IsUnique();
    }
    
}