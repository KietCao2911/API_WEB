﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace API_DSCS2_WEBBANGIAY.Migrations
{
    public partial class dropTableParentCate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMuc_ParentCategory_ParentCategoryID",
                table: "DanhMuc");

            migrationBuilder.DropTable(
                name: "ParentCategory");

            migrationBuilder.DropIndex(
                name: "IX_DanhMuc_ParentCategoryID",
                table: "DanhMuc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParentCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhMuc_ParentCategoryID",
                table: "DanhMuc",
                column: "ParentCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMuc_ParentCategory_ParentCategoryID",
                table: "DanhMuc",
                column: "ParentCategoryID",
                principalTable: "ParentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
