using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie.Migrations
{
    /// <inheritdoc />
    public partial class FixOfferRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeComments_EpisodeComments_ParentCommentId",
                table: "EpisodeComments");

            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeComments_Users_UserId",
                table: "EpisodeComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_SubscriptionPlans_SubscriptionPlanId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Episodes_EpisodeId",
                table: "UserActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Movies_MovieId",
                table: "UserActivities");

            migrationBuilder.AlterColumn<string>(
                name: "OfferCode",
                table: "Offers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Offers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeComments_EpisodeComments_ParentCommentId",
                table: "EpisodeComments",
                column: "ParentCommentId",
                principalTable: "EpisodeComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeComments_Users_UserId",
                table: "EpisodeComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_SubscriptionPlans_SubscriptionPlanId",
                table: "Offers",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Episodes_EpisodeId",
                table: "UserActivities",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Movies_MovieId",
                table: "UserActivities",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeComments_EpisodeComments_ParentCommentId",
                table: "EpisodeComments");

            migrationBuilder.DropForeignKey(
                name: "FK_EpisodeComments_Users_UserId",
                table: "EpisodeComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_SubscriptionPlans_SubscriptionPlanId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Episodes_EpisodeId",
                table: "UserActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Movies_MovieId",
                table: "UserActivities");

            migrationBuilder.AlterColumn<string>(
                name: "OfferCode",
                table: "Offers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Offers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeComments_EpisodeComments_ParentCommentId",
                table: "EpisodeComments",
                column: "ParentCommentId",
                principalTable: "EpisodeComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EpisodeComments_Users_UserId",
                table: "EpisodeComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_SubscriptionPlans_SubscriptionPlanId",
                table: "Offers",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Episodes_EpisodeId",
                table: "UserActivities",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Movies_MovieId",
                table: "UserActivities",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }
    }
}
