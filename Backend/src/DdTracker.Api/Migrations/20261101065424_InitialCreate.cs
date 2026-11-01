using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DdTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TariffTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChargeType = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    FromDay = table.Column<int>(type: "INTEGER", nullable: false),
                    ToDay = table.Column<int>(type: "INTEGER", nullable: true),
                    RatePerDay = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false, defaultValue: "USD")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffTiers", x => x.Id);
                    table.CheckConstraint("CK_TariffTier_Range", "ToDay IS NULL OR ToDay >= FromDay");
                    table.ForeignKey(
                        name: "FK_TariffTiers_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerNo = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    BookingRef = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ContainerTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationPortId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Containers_Ports_DestinationPortId",
                        column: x => x.DestinationPortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ChargeType = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Acknowledged = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContainerEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerEvents_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Acknowledged_ContainerId_Severity",
                table: "Alerts",
                columns: new[] { "Acknowledged", "ContainerId", "Severity" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ContainerId",
                table: "Alerts",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerEvents_ContainerId_EventType",
                table: "ContainerEvents",
                columns: new[] { "ContainerId", "EventType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerNo",
                table: "Containers",
                column: "ContainerNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerTypeId",
                table: "Containers",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_DestinationPortId",
                table: "Containers",
                column: "DestinationPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_Status",
                table: "Containers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerTypes_Code",
                table: "ContainerTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ports_Code",
                table: "Ports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffTiers_ChargeType_ContainerTypeId_FromDay",
                table: "TariffTiers",
                columns: new[] { "ChargeType", "ContainerTypeId", "FromDay" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffTiers_ContainerTypeId",
                table: "TariffTiers",
                column: "ContainerTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "ContainerEvents");

            migrationBuilder.DropTable(
                name: "TariffTiers");

            migrationBuilder.DropTable(
                name: "Containers");

            migrationBuilder.DropTable(
                name: "ContainerTypes");

            migrationBuilder.DropTable(
                name: "Ports");
        }
    }
}
