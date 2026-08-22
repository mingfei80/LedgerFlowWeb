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
    protected override void OnModelCreating(ModelBuilder b)
    {
    }
}
