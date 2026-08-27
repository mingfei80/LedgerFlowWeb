using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerFlowWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialStockSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stock");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "dbo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DecimalPlaces = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Securities",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISIN = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Ticker = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Exchange = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Securities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExternalProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    ClientNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportLogs",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ImportedCount = table.Column<int>(type: "int", nullable: false),
                    SkippedCount = table.Column<int>(type: "int", nullable: false),
                    ErrorCount = table.Column<int>(type: "int", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportLogs_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "stock",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    SecurityId = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlatformReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "stock",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Securities_SecurityId",
                        column: x => x.SecurityId,
                        principalSchema: "stock",
                        principalTable: "Securities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlatformImports",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportLogId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    SourceRowNumber = table.Column<int>(type: "int", nullable: true),
                    SourceRowType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RawRowJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformImports_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "stock",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformImports_ImportLogs_ImportLogId",
                        column: x => x.ImportLogId,
                        principalSchema: "stock",
                        principalTable: "ImportLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformImports_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkippedTransactions",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportLogId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    SkipReason = table.Column<int>(type: "int", nullable: false),
                    MatchedTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    SkippedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    SecurityName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SecurityISIN = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    SecurityTicker = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    OriginalCsvRow = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkippedTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkippedTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "stock",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkippedTransactions_ImportLogs_ImportLogId",
                        column: x => x.ImportLogId,
                        principalSchema: "stock",
                        principalTable: "ImportLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkippedTransactions_Transactions_MatchedTransactionId",
                        column: x => x.MatchedTransactionId,
                        principalSchema: "stock",
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SkippedTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionDetails",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<long>(type: "bigint", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    SourceRowType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RawRowJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ParsedExtrasJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionDetails_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalSchema: "stock",
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId_Platform_AccountType_ClientNumber",
                schema: "stock",
                table: "Accounts",
                columns: new[] { "UserId", "Platform", "AccountType", "ClientNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportLogs_AccountId",
                schema: "stock",
                table: "ImportLogs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportLogs_UserId_ImportDate",
                schema: "stock",
                table: "ImportLogs",
                columns: new[] { "UserId", "ImportDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformImports_AccountId",
                schema: "stock",
                table: "PlatformImports",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformImports_ImportLogId",
                schema: "stock",
                table: "PlatformImports",
                column: "ImportLogId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformImports_UserId_AccountId_Platform",
                schema: "stock",
                table: "PlatformImports",
                columns: new[] { "UserId", "AccountId", "Platform" });

            migrationBuilder.CreateIndex(
                name: "IX_Securities_ISIN",
                schema: "stock",
                table: "Securities",
                column: "ISIN");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Name",
                schema: "stock",
                table: "Securities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Ticker",
                schema: "stock",
                table: "Securities",
                column: "Ticker");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedTransactions_AccountId",
                schema: "stock",
                table: "SkippedTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedTransactions_ImportLogId",
                schema: "stock",
                table: "SkippedTransactions",
                column: "ImportLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedTransactions_MatchedTransactionId",
                schema: "stock",
                table: "SkippedTransactions",
                column: "MatchedTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SkippedTransactions_UserId_Status",
                schema: "stock",
                table: "SkippedTransactions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_Platform_SourceRowType",
                schema: "stock",
                table: "TransactionDetails",
                columns: new[] { "Platform", "SourceRowType" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_TransactionId",
                schema: "stock",
                table: "TransactionDetails",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Idempotency",
                schema: "stock",
                table: "Transactions",
                columns: new[] { "UserId", "AccountId", "TransactionDate", "TransactionType", "SecurityId", "Quantity", "UnitPrice" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                schema: "stock",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SecurityId_TransactionDate",
                schema: "stock",
                table: "Transactions",
                columns: new[] { "SecurityId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_AccountId_TransactionDate",
                schema: "stock",
                table: "Transactions",
                columns: new[] { "UserId", "AccountId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "dbo",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ExternalProvider_ExternalSubjectId",
                schema: "dbo",
                table: "Users",
                columns: new[] { "ExternalProvider", "ExternalSubjectId" },
                unique: true,
                filter: "[ExternalProvider] IS NOT NULL AND [ExternalSubjectId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlatformImports",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "SkippedTransactions",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "TransactionDetails",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "ImportLogs",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "Securities",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");
        }
    }
}
