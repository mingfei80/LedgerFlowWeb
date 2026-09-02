namespace LedgerFlowWeb.Api.Features.Import.IG;

public sealed class ImportResult
{
    public long ImportLogId { get; set; }
    public int Created { get; set; }
    public int Skipped { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public long DurationMs { get; set; }
}
