namespace CloudApiVietnam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renamedatetimesofform : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Formulierens", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Formulierens", "UpdatedOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Formulierens", "DateTimeStarted");
            DropColumn("dbo.Formulierens", "DateTimeSynced");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Formulierens", "DateTimeSynced", c => c.DateTime(nullable: false));
            AddColumn("dbo.Formulierens", "DateTimeStarted", c => c.DateTime(nullable: false));
            DropColumn("dbo.Formulierens", "UpdatedOn");
            DropColumn("dbo.Formulierens", "CreatedOn");
        }
    }
}
