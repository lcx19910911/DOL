namespace DOL.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reference", "EnteredPointIDStr", c => c.String(maxLength: 5120, unicode: false));
            DropColumn("dbo.Reference", "ShopIDStr");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reference", "ShopIDStr", c => c.String(maxLength: 5120, unicode: false));
            DropColumn("dbo.Reference", "EnteredPointIDStr");
        }
    }
}
