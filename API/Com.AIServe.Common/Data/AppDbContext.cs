using Com.AIServe.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Com.AIServe.Common.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 只保留特殊配置（默认值SQL）
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreateTime).HasDefaultValueSql("GETDATE()");
        });
    }
}
