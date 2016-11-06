namespace DOL.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayOrder",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false),
                        StudentID = c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false),
                        PayMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WantDropMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayTypeID = c.String(maxLength: 32, fixedLength: true, unicode: false),
                        VoucherNO = c.String(maxLength: 32, unicode: false),
                        VoucherThum = c.String(maxLength: 256, unicode: false),
                        AccountID = c.String(maxLength: 32, fixedLength: true, unicode: false),
                        WantDropDate = c.DateTime(),
                        PayTime = c.DateTime(nullable: false),
                        AccountNO = c.String(maxLength: 32, unicode: false),
                        IsConfirm = c.Int(nullable: false),
                        IsDrop = c.Int(nullable: false),
                        ConfirmUserID = c.String(maxLength: 32, fixedLength: true, unicode: false),
                        ConfirmDate = c.DateTime(),
                        CreaterID = c.String(maxLength: 32, fixedLength: true, unicode: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedTime = c.DateTime(nullable: false),
                        UpdatedID = c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false),
                        UpdatedTime = c.DateTime(nullable: false),
                        Flag = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PayOrder");
        }
    }
}
