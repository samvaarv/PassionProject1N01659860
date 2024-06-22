namespace PassionProject1N01659860.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtImageUploadPicExtention : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Arts", "PicExtension", c => c.String());
            AlterColumn("dbo.Arts", "ImageURL", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Arts", "ImageURL", c => c.String());
            DropColumn("dbo.Arts", "PicExtension");
        }
    }
}
