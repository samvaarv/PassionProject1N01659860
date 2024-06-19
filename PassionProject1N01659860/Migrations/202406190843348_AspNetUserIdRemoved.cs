namespace PassionProject1N01659860.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspNetUserIdRemoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "AspNetUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AspNetUserId", c => c.String(maxLength: 128));
        }
    }
}
