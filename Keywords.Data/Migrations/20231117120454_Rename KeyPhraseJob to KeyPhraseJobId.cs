using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keywords.Data.Migrations
{
    public partial class RenameKeyPhraseJobtoKeyPhraseJobId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeyPhraseJob",
                table: "IndexerEntities",
                newName: "KeyPhraseJobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeyPhraseJobId",
                table: "IndexerEntities",
                newName: "KeyPhraseJob");
        }
    }
}
