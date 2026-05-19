using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingAppUserProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserSubscriptions");

            migrationBuilder.RenameColumn(
                name: "SubscriptionPlanId",
                table: "UserSubscriptions",
                newName: "PlanId");

            migrationBuilder.RenameColumn(
                name: "ReactivatedAt",
                table: "UserSubscriptions",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscriptions_SubscriptionPlanId",
                table: "UserSubscriptions",
                newName: "IX_UserSubscriptions_PlanId");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "Users",
                newName: "ProfilePicture");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "UserSubscriptions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ReferralCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionPlanId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentDetails",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanId",
                table: "Payments",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_PlanId",
                table: "UserSubscriptions",
                column: "PlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_PlanId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UserSubscriptions",
                newName: "ReactivatedAt");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "UserSubscriptions",
                newName: "SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscriptions_PlanId",
                table: "UserSubscriptions",
                newName: "IX_UserSubscriptions_SubscriptionPlanId");

            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "Users",
                newName: "ProfilePictureUrl");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "UserSubscriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "UserSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "UserSubscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UserSubscriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferralCode",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionPlanId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentDetails",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SubscriptionPlans_SubscriptionPlanId",
                table: "Payments",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "UserSubscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
