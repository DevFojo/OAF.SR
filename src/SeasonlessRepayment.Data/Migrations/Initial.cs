using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeasonlessRepayment.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(),
                    EndDate = table.Column<DateTime>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSummaries",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(),
                    SeasonId = table.Column<int>(),
                    TotalRepaid = table.Column<decimal>(),
                    TotalCredit = table.Column<decimal>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerSummaries_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerSummaries_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repayments",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(),
                    SeasonId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(),
                    Amount = table.Column<decimal>(),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repayments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repayments_Repayments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Repayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Repayments_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepaymentUploads",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(),
                    SeasonId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(),
                    Amount = table.Column<decimal>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepaymentUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepaymentUploads_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepaymentUploads_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSummaries_CustomerId",
                table: "CustomerSummaries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSummaries_SeasonId",
                table: "CustomerSummaries",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_CustomerId",
                table: "Repayments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_ParentId",
                table: "Repayments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_SeasonId",
                table: "Repayments",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_RepaymentUploads_CustomerId",
                table: "RepaymentUploads",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RepaymentUploads_SeasonId",
                table: "RepaymentUploads",
                column: "SeasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerSummaries");

            migrationBuilder.DropTable(
                name: "Repayments");

            migrationBuilder.DropTable(
                name: "RepaymentUploads");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
