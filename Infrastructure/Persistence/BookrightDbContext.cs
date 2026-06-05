using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
        public class BookrightDbContext : DbContext
        {
                public BookrightDbContext(DbContextOptions<BookrightDbContext> options) : base(options) { }

                public DbSet<Booking> Bookings { get; set; }
                public DbSet<Treatment> Treatments { get; set; }

                protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
                {
                        base.ConfigureConventions(configurationBuilder);
                        configurationBuilder.Properties<decimal>().HavePrecision(Config.DIGITS_BEFORE_DOT_IN_MONEY, Config.DIGITS_AFTER_DOT_IN_MONEY);
                        configurationBuilder.Properties<Currency>().HaveConversion<string>();
                }
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                        base.OnModelCreating(modelBuilder);

                        modelBuilder.Entity<Booking>().OwnsOne(b => b.Paid, m =>
                        {
                                m.Property(p => p.Value).HasColumnName("PaidValue");
                                m.Property(p => p.Currency).HasColumnName("PaidCurrency");
                        });

                        modelBuilder.Entity<Treatment>().OwnsOne(t => t.Price, m =>
                        {
                                m.Property(p => p.Value).HasColumnName("PriceValue");
                                m.Property(p => p.Currency).HasColumnName("PriceCurrency");
                        });
                }
        }
}
