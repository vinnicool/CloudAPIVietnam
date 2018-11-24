namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetimeforforms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Formulierens", "DateTimeStarted", c => c.DateTime(nullable: false));
            AddColumn("dbo.Formulierens", "DateTimeSynced", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Formulierens", "DateTimeSynced");
            DropColumn("dbo.Formulierens", "DateTimeStarted");
        }
    }
}
