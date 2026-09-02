using System.IO;

namespace LedgerFlowWeb.Api.Features.Import.IG;

public interface IIGImportService
{
    Task<ImportResult> ImportAsync(Guid userId, int accountId, Stream csvStream, string fileName, CancellationToken cancellationToken = default);
}
