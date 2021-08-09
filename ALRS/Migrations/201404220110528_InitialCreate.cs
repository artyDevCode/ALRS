namespace ALRSSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ALRS",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ALRSName = c.String(maxLength: 100),
                        ALRSStartDate = c.DateTime(nullable: false),
                        ALRSEndDate = c.DateTime(nullable: false),
                        ALRSDuration = c.Int(nullable: false),
                        ALRSComments = c.String(maxLength: 255),
                        ALRSJudgeOnLeave = c.Boolean(nullable: false),
                        ALRSRequestorPosition = c.String(),
                        ALRSDisclaimer = c.Boolean(nullable: false),
                        ALRSStatus = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RequestorPosition",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RequestorPositionName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RequestorPosition");
            DropTable("dbo.ALRS");
        }
    }
}
