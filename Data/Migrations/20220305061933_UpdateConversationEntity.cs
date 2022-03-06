using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeAssistant.API.Data.Migrations
{
    public partial class UpdateConversationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Conversations",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Conversations",
                newName: "Name");
        }
    }
}
