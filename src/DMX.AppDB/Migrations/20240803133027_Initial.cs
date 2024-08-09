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
                name: "dom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ent",
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
                    table.PrimaryKey("PK_ent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shape",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    PosX = table.Column<int>(type: "INTEGER", nullable: true),
                    PoxY = table.Column<int>(type: "INTEGER", nullable: true),
                    PosZ = table.Column<int>(type: "INTEGER", nullable: true),
                    DimW = table.Column<int>(type: "INTEGER", nullable: true),
                    DimH = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shape", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "att",
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
                    table.PrimaryKey("PK_att", x => x.Id);
                    table.ForeignKey(
                        name: "FK_att_dom_DomainId",
                        column: x => x.DomainId,
                        principalTable: "dom",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_att_ent_EntityId",
                        column: x => x.EntityId,
                        principalTable: "ent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    ChildId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildEdge = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentEdge = table.Column<int>(type: "INTEGER", nullable: false),
                    ChildEdgeOffset = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentEdgeOffset = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rel_ent_ChildId",
                        column: x => x.ChildId,
                        principalTable: "ent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rel_ent_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rel_pair",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelationshipId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_pair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rel_pair_att_ChildId",
                        column: x => x.ChildId,
                        principalTable: "att",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rel_pair_att_ParentId",
                        column: x => x.ParentId,
                        principalTable: "att",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rel_pair_rel_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "rel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_att_DomainId",
                table: "att",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_att_EntityId",
                table: "att",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_rel_ChildId",
                table: "rel",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_rel_ParentId",
                table: "rel",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_rel_pair_ChildId",
                table: "rel_pair",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_rel_pair_ParentId",
                table: "rel_pair",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_rel_pair_RelationshipId",
                table: "rel_pair",
                column: "RelationshipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rel_pair");

            migrationBuilder.DropTable(
                name: "shape");

            migrationBuilder.DropTable(
                name: "att");

            migrationBuilder.DropTable(
                name: "rel");

            migrationBuilder.DropTable(
                name: "dom");

            migrationBuilder.DropTable(
                name: "ent");
        }
    }
}
