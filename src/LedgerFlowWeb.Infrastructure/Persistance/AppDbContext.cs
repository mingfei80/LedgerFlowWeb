using LedgerFlowWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlowWeb.Infrastructure.Persistance;


/// <summary>
/// No repository layer wraps this. DbContext already is a Unit of Work and DbSet
/// already is a repository; wrapping it either leaks IQueryable (a fake
/// abstraction) or hides it (losing Include, projections and bulk operations)
/// Complex queries get their own named query class instead -- see the API features.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Security> Securities => Set<Security>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionDetail> TransactionDetails => Set<TransactionDetail>();
    public DbSet<ImportLog> ImportLogs => Set<ImportLog>();
    public DbSet<PlatformImport> PlatformImports => Set<PlatformImport>();
    public DbSet<SkippedTransaction> SkippedTransactions => Set<SkippedTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.ExternalProvider).HasMaxLength(50);
            entity.Property(e => e.ExternalSubjectId).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.ExternalProvider, e.ExternalSubjectId }).IsUnique();
        });

        // Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Platform).IsRequired();
            entity.Property(e => e.AccountType).IsRequired();
            entity.Property(e => e.ClientNumber).HasMaxLength(100);
            entity.Property(e => e.Metadata).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UserId, e.Platform, e.AccountType, e.ClientNumber });
        });

        // Security
        modelBuilder.Entity<Security>(entity =>
        {
            entity.ToTable("Securities", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
            entity.Property(e => e.ISIN).HasMaxLength(12);
            entity.Property(e => e.Ticker).HasMaxLength(20);
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
            entity.Property(e => e.Exchange).HasMaxLength(50);
            entity.Property(e => e.Metadata).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => e.ISIN);
            entity.HasIndex(e => e.Ticker);
            entity.HasIndex(e => e.Name);
        });

        // Currency
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currencies", "dbo");
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(3).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Symbol).HasMaxLength(5).IsRequired();
            entity.Property(e => e.DecimalPlaces).IsRequired();
        });

        // Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionType).IsRequired();
            entity.Property(e => e.TransactionDate).IsRequired();
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,6)").IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,6)").IsRequired();
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18,6)");
            entity.Property(e => e.Fees).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PlatformReference).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Metadata).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Security)
                .WithMany(s => s.Transactions)
                .HasForeignKey(e => e.SecurityId)
                .OnDelete(DeleteBehavior.SetNull);

            // Composite index for idempotency check
            entity.HasIndex(e => new { e.UserId, e.AccountId, e.TransactionDate, e.TransactionType, e.SecurityId, e.Quantity, e.UnitPrice })
                .HasDatabaseName("IX_Transaction_Idempotency");

            // Index for common queries
            entity.HasIndex(e => new { e.UserId, e.AccountId, e.TransactionDate });
            entity.HasIndex(e => new { e.SecurityId, e.TransactionDate });
        });

        // TransactionDetail
        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.ToTable("TransactionDetails", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Platform).IsRequired();
            entity.Property(e => e.SourceRowType).HasMaxLength(64);
            entity.Property(e => e.RawRowJson).HasMaxLength(4000);
            entity.Property(e => e.ParsedExtrasJson).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Transaction)
                .WithMany(t => t.Details)
                .HasForeignKey(e => e.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.TransactionId);
            entity.HasIndex(e => new { e.Platform, e.SourceRowType });
        });

        // ImportLog
        modelBuilder.Entity<ImportLog>(entity =>
        {
            entity.ToTable("ImportLogs", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Platform).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(256).IsRequired();
            entity.Property(e => e.ImportDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.ImportedCount).IsRequired();
            entity.Property(e => e.SkippedCount).IsRequired();
            entity.Property(e => e.ErrorCount).IsRequired();
            entity.Property(e => e.DurationMs).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.ImportLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.ImportLogs)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UserId, e.ImportDate });
            entity.HasIndex(e => e.AccountId);
        });

        // PlatformImport
        modelBuilder.Entity<PlatformImport>(entity =>
        {
            entity.ToTable("PlatformImports", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Platform).IsRequired();
            entity.Property(e => e.SourceRowType).HasMaxLength(64);
            entity.Property(e => e.RawRowJson).HasMaxLength(4000).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.ImportLog)
                .WithMany(l => l.PlatformImports)
                .HasForeignKey(e => e.ImportLogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.PlatformImports)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.PlatformImports)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ImportLogId);
            entity.HasIndex(e => new { e.UserId, e.AccountId, e.Platform });
        });

        // SkippedTransaction
        modelBuilder.Entity<SkippedTransaction>(entity =>
        {
            entity.ToTable("SkippedTransactions", "stock");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Platform).IsRequired();
            entity.Property(e => e.SkipReason).IsRequired();
            entity.Property(e => e.SkippedDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TransactionDate).IsRequired();
            entity.Property(e => e.TransactionType).IsRequired();
            entity.Property(e => e.SecurityName).HasMaxLength(256).IsRequired();
            entity.Property(e => e.SecurityISIN).HasMaxLength(12);
            entity.Property(e => e.SecurityTicker).HasMaxLength(20);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,6)").IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,6)").IsRequired();
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Currency).HasMaxLength(3).IsRequired();
            entity.Property(e => e.OriginalCsvRow).HasMaxLength(4000);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.ImportLog)
                .WithMany(i => i.SkippedTransactions)
                .HasForeignKey(e => e.ImportLogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.MatchedTransaction)
                .WithMany()
                .HasForeignKey(e => e.MatchedTransactionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ImportLogId);
            entity.HasIndex(e => new { e.UserId, e.Status });
        });
    }
}
