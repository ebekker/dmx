using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMX.AppDB.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DmxDomain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmxDomain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    PosX = table.Column<int>(type: "INTEGER", nullable: true),
                    PoxY = table.Column<int>(type: "INTEGER", nullable: true),
                    ZOrder = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmxAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    DomainId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsPrimaryKey = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DmxAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DmxAttribute_DmxDomain_DomainId",
                        column: x => x.DomainId,
                        principalTable: "DmxDomain",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DmxAttribute_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DmxAttribute_DomainId",
                table: "DmxAttribute",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DmxAttribute_EntityId",
                table: "DmxAttribute",
                column: "EntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DmxAttribute");

            migrationBuilder.DropTable(
                name: "DmxDomain");

            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}
