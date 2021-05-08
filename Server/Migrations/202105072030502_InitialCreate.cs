namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.process",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        os_name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.team_process",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        id_team = c.Int(nullable: false),
                        id_process = c.Int(nullable: false),
                        team_id_teams = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Teams", t => t.team_id_teams)
                .ForeignKey("dbo.process", t => t.id_process)
                .Index(t => t.id_process)
                .Index(t => t.team_id_teams);
          
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.team_process", "id_process", "dbo.process");
            DropForeignKey("dbo.team_process", "team_id_teams", "dbo.Teams");
            DropIndex("dbo.team_process", new[] { "team_id_teams" });
            DropIndex("dbo.team_process", new[] { "id_process" });
            DropTable("dbo.team_process");
            DropTable("dbo.process");
        }
    }
}
