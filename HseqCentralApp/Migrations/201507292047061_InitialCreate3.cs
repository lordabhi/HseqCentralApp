namespace HseqCentralApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Comments", "AssociatedType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "AssociatedType", c => c.String());
        }
    }
}
