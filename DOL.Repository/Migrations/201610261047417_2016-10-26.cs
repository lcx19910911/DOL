namespace DOL.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20161026 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "IsAddCertificate", c => c.Int(nullable: false));
            AddColumn("dbo.Students", "OldCertificate", c => c.String(maxLength: 32, unicode: false));
            AlterColumn("dbo.Students", "CertificateID", c => c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Students", "CertificateID", c => c.String(maxLength: 32, fixedLength: true, unicode: false));
            DropColumn("dbo.Students", "OldCertificate");
            DropColumn("dbo.Students", "IsAddCertificate");
        }
    }
}
