using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Matchbook.Db.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClearingAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Type = table.Column<string>(nullable: false),
                    BrokerCode = table.Column<string>(maxLength: 50, nullable: false),
                    BrokerTerm = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClearingAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasure",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasure", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    DsId = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    AccountOwner = table.Column<string>(maxLength: 50, nullable: false),
                    ClearingAccountId = table.Column<int>(nullable: false),
                    InternalClearingAccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubAccounts_ClearingAccounts_ClearingAccountId",
                        column: x => x.ClearingAccountId,
                        principalTable: "ClearingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubAccounts_ClearingAccounts_InternalClearingAccountId",
                        column: x => x.InternalClearingAccountId,
                        principalTable: "ClearingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ExchangeName = table.Column<string>(maxLength: 20, nullable: false),
                    CommodityCode = table.Column<string>(maxLength: 20, nullable: false),
                    JPMCode = table.Column<string>(maxLength: 20, nullable: false),
                    ContractSize = table.Column<int>(nullable: false),
                    ContractUoM = table.Column<string>(maxLength: 10, nullable: false),
                    PriceQuoteCurrency = table.Column<string>(maxLength: 10, nullable: false),
                    MaturityMonths = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_UnitsOfMeasure_ContractUoM",
                        column: x => x.ContractUoM,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_Currencies_PriceQuoteCurrency",
                        column: x => x.PriceQuoteCurrency,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(maxLength: 20, nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    SpecificationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.UniqueConstraint("AK_Products_Symbol", x => x.Symbol);
                    table.ForeignKey(
                        name: "FK_Products_ProductSpecifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalTable: "ProductSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubAccountProductSpecs",
                columns: table => new
                {
                    SubAccountId = table.Column<int>(nullable: false),
                    ProductSpecId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAccountProductSpecs", x => new { x.SubAccountId, x.ProductSpecId });
                    table.ForeignKey(
                        name: "FK_SubAccountProductSpecs_ProductSpecifications_ProductSpecId",
                        column: x => x.ProductSpecId,
                        principalTable: "ProductSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubAccountProductSpecs_SubAccounts_SubAccountId",
                        column: x => x.SubAccountId,
                        principalTable: "SubAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Side = table.Column<string>(nullable: false),
                    ProductSymbol = table.Column<string>(maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28, 10)", nullable: true),
                    PriceInstruction = table.Column<string>(nullable: true),
                    SubAccountId = table.Column<int>(nullable: false),
                    CounterpartySubAccountId = table.Column<int>(nullable: false),
                    PhysicalContract_Number = table.Column<string>(maxLength: 50, nullable: true),
                    PhysicalContract_Quantity = table.Column<decimal>(type: "decimal(28, 10)", nullable: true),
                    PhysicalContract_UnitOfMeasure = table.Column<string>(maxLength: 10, nullable: true),
                    SourceOrderId = table.Column<string>(maxLength: 100, nullable: true),
                    SourceSystem = table.Column<string>(maxLength: 100, nullable: true),
                    ExternalReferenceId = table.Column<string>(maxLength: 100, nullable: true),
                    AdditionalNotes = table.Column<string>(maxLength: 5000, nullable: true),
                    CargillContact = table.Column<string>(maxLength: 100, nullable: true),
                    OrderType = table.Column<string>(nullable: false),
                    OrderStatus = table.Column<string>(nullable: false),
                    EfrpOrderDetails_AgreementDate = table.Column<DateTime>(nullable: true),
                    EfrpOrderDetails_OppositeFirmName = table.Column<string>(maxLength: 100, nullable: true),
                    EfrpOrderDetails_AccountHolder = table.Column<string>(maxLength: 100, nullable: true),
                    EfrpOrderDetails_CustomerName = table.Column<string>(maxLength: 100, nullable: true),
                    EfrpOrderDetails_CustomerClearingAccount = table.Column<string>(maxLength: 100, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    LastUpdatedBy = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_SubAccounts_CounterpartySubAccountId",
                        column: x => x.CounterpartySubAccountId,
                        principalTable: "SubAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductSymbol",
                        column: x => x.ProductSymbol,
                        principalTable: "Products",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_SubAccounts_SubAccountId",
                        column: x => x.SubAccountId,
                        principalTable: "SubAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_UnitsOfMeasure_PhysicalContract_UnitOfMeasure",
                        column: x => x.PhysicalContract_UnitOfMeasure,
                        principalTable: "UnitsOfMeasure",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClearingAccounts_Code",
                table: "ClearingAccounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CounterpartySubAccountId",
                table: "Orders",
                column: "CounterpartySubAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductSymbol",
                table: "Orders",
                column: "ProductSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SubAccountId",
                table: "Orders",
                column: "SubAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PhysicalContract_UnitOfMeasure",
                table: "Orders",
                column: "PhysicalContract_UnitOfMeasure");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecificationId",
                table: "Products",
                column: "SpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Symbol",
                table: "Products",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_CommodityCode",
                table: "ProductSpecifications",
                column: "CommodityCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_ContractUoM",
                table: "ProductSpecifications",
                column: "ContractUoM");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_PriceQuoteCurrency",
                table: "ProductSpecifications",
                column: "PriceQuoteCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_SubAccountProductSpecs_ProductSpecId",
                table: "SubAccountProductSpecs",
                column: "ProductSpecId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_ClearingAccountId",
                table: "SubAccounts",
                column: "ClearingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_InternalClearingAccountId",
                table: "SubAccounts",
                column: "InternalClearingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SubAccounts_Name",
                table: "SubAccounts",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "SubAccountProductSpecs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "SubAccounts");

            migrationBuilder.DropTable(
                name: "ProductSpecifications");

            migrationBuilder.DropTable(
                name: "ClearingAccounts");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasure");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
