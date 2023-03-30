using Matchbook.Db.Converters;
using Matchbook.Model;
using Microsoft.EntityFrameworkCore;

namespace Matchbook.Db
{
    public class MatchbookDbContext : DbContext
    {
        public MatchbookDbContext(DbContextOptions<MatchbookDbContext> options) : base(options) { }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SubAccount> SubAccounts { get; set; }
        public DbSet<ClearingAccount> ClearingAccounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLink> OrderLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasKey(_ => _.Code);
            modelBuilder.Entity<UnitOfMeasure>()
                .HasKey(_ => _.Code);
            modelBuilder.Entity<User>()
                .HasKey(_ => _.Id);
            modelBuilder.Entity<SubAccountProductSpecs>()
                .HasKey(_ => new { _.SubAccountId, _.ProductSpecId });
            modelBuilder.Entity<SubAccountProductSpecs>()
                .HasOne(_ => _.ProductSpec)
                .WithMany(_ => _.SubAccounts)
                .HasForeignKey(_ => _.ProductSpecId);
            modelBuilder.Entity<SubAccountProductSpecs>()
                .HasOne(_ => _.SubAccount)
                .WithMany(_ => _.TradedProducts)
                .HasForeignKey(_ => _.SubAccountId);
            modelBuilder.Entity<ProductSpecification>()
                .HasKey(_ => _.Id);
            modelBuilder.Entity<ProductSpecification>()
                .HasOne<Currency>()
                .WithMany()
                .HasForeignKey(_ => _.PriceQuoteCurrency);
            modelBuilder.Entity<ProductSpecification>()
                .HasOne<UnitOfMeasure>()
                .WithMany()
                .HasForeignKey(_ => _.ContractUoM);
            modelBuilder.Entity<ProductSpecification>()
                .Property(_ => _.MaturityMonths)
                // Possible performance issue, to be analyzed later if that's the case
                .HasConversion(MaturityMonthsValueConverter.Instance)
                .HasMaxLength(100);
            modelBuilder.Entity<ProductSpecification>()
                .HasIndex(x => x.CommodityCode)
                .IsUnique();

            var productMapping = modelBuilder.Entity<Product>();
            productMapping.HasKey(_ => _.Id);
            productMapping.HasIndex(_ => _.Symbol).IsUnique();
            productMapping.HasOne(_ => _.Specification).WithMany().OnDelete(DeleteBehavior.Restrict);

            var subAccountMapping = modelBuilder.Entity<SubAccount>();
            subAccountMapping.HasKey(_ => _.Id);
            subAccountMapping.HasIndex(_ => _.Name).IsUnique();
            subAccountMapping.HasOne(_ => _.ClearingAccount).WithMany().OnDelete(DeleteBehavior.Restrict);
            subAccountMapping.HasOne(_ => _.InternalClearingAccount).WithMany().OnDelete(DeleteBehavior.Restrict);

            var clearingAccountMapping = modelBuilder.Entity<ClearingAccount>();

            clearingAccountMapping.HasKey(_ => _.Id);
            clearingAccountMapping.HasIndex(_ => _.Code).IsUnique();
            clearingAccountMapping.Property(_ => _.Type).HasConversion<string>();

            var orderMapping = modelBuilder.Entity<Order>();

            orderMapping.HasKey(_ => _.Id);
            orderMapping.HasOne(_ => _.Product).WithMany().HasForeignKey(_ => _.ProductSymbol).HasPrincipalKey(_ => _.Symbol).OnDelete(DeleteBehavior.Restrict);
            orderMapping.HasOne(_ => _.SubAccount).WithMany().OnDelete(DeleteBehavior.Restrict);
            orderMapping.HasOne(_ => _.CounterpartySubAccount).WithMany().OnDelete(DeleteBehavior.Restrict);
            orderMapping.Property(_ => _.Price).HasColumnType("decimal(28, 10)");
            orderMapping.OwnsOne(_ => _.PhysicalContract).Property(_ => _.Quantity).HasColumnType("decimal(28, 10)");
            orderMapping.OwnsOne(_ => _.PhysicalContract).HasOne<UnitOfMeasure>().WithMany().HasForeignKey(_ => _.UnitOfMeasure);
            orderMapping.OwnsOne(_ => _.EfrpOrderDetails);
            orderMapping.Property(_ => _.Side).HasConversion<string>();
            orderMapping.Property(_ => _.OrderStatus).HasConversion<string>();
            orderMapping.Property(_ => _.OrderType).HasConversion<string>();
            orderMapping.Property(_ => _.PriceInstruction).HasConversion<string>();
            orderMapping.HasOne(_ => _.Link).WithMany(_ => _.LinkedOrders).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            var linkMapping = modelBuilder.Entity<OrderLink>();
            linkMapping.HasKey(_ => _.Id);
            linkMapping.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<OrderLink>()
                .HasIndex(_ => _.Name)
                .IsUnique();

            linkMapping.HasMany(_ => _.LinkedOrders);
        }
    }
}
