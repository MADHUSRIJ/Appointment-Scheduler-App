using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application_Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class SendReinder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendReminder",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendReminder",
                table: "Users");
        }
    }
}
