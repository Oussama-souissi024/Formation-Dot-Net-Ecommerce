using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Formationn_Ecommerce.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class updatecouponID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartHeaders_Coupons_CouponId",
                table: "CartHeaders");

            migrationBuilder.AlterColumn<Guid>(
                name: "CouponId",
                table: "CartHeaders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CartHeaders_Coupons_CouponId",
                table: "CartHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartHeaders_Coupons_CouponId",
                table: "CartHeaders");

            migrationBuilder.AlterColumn<Guid>(
                name: "CouponId",
                table: "CartHeaders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartHeaders_Coupons_CouponId",
                table: "CartHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
