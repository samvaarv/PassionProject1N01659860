namespace PassionProject1N01659860.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveImageUpload : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Arts", "ImageURL", c => c.String());
            DropColumn("dbo.Arts", "PicExtension");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Arts", "PicExtension", c => c.String());
            AlterColumn("dbo.Arts", "ImageURL", c => c.Boolean(nullable: false));
        }
    }
}
