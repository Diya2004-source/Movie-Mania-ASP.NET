using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToShowReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ShowReviews");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "MovieReviews");

            migrationBuilder.RenameColumn(
                name: "ReviewDate",
                table: "ShowReviews",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ReviewDate",
                table: "MovieReviews",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "ShowReviews",
                type: "int",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,1)",
                oldPrecision: 3,
                oldScale: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShowReviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "MovieReviews",
                type: "int",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,1)",
                oldPrecision: 3,
                oldScale: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MovieReviews",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ShowReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MovieReviews");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ShowReviews",
                newName: "ReviewDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "MovieReviews",
                newName: "ReviewDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "ShowReviews",
                type: "decimal(3,1)",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 3,
                oldScale: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ShowReviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "MovieReviews",
                type: "decimal(3,1)",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 3,
                oldScale: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "MovieReviews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
