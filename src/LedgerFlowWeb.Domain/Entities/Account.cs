using LedgerFlowWeb.Domain.Enums;

namespace LedgerFlowWeb.Domain.Entities;

/// <summary>
/// Trading account (HL ISA, HL LISA, IG, T212, etc.)
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User-friendly name (e.g., "My HL ISA", "My HL LISA")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Trading platform
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// Account type (ISA, LISA, Standard, etc.)
    /// </summary>
    public AccountType AccountType { get; set; }

    /// <summary>
    /// Platform's client/account number (can be same for ISA and LISA on HL)
    /// </summary>
    public string? ClientNumber { get; set; }

    /// <summary>
    /// Owner of this account
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When this account was created in the system
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<ImportLog> ImportLogs { get; set; } = new List<ImportLog>();
    public ICollection<PlatformImport> PlatformImports { get; set; } = new List<PlatformImport>();
}
