namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// User entity for multi-user support
/// Prepared for Microsoft Identity integration
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier (will map to Microsoft Identity UserId)
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User email (from Microsoft Identity)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// External identity provider name (for example, Microsoft).
    /// </summary>
    public string? ExternalProvider { get; set; }

    /// <summary>
    /// External subject/object identifier from the provider.
    /// </summary>
    public string? ExternalSubjectId { get; set; }

    /// <summary>
    /// Display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Date when user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<ImportLog> ImportLogs { get; set; } = new List<ImportLog>();
    public ICollection<PlatformImport> PlatformImports { get; set; } = new List<PlatformImport>();
}
