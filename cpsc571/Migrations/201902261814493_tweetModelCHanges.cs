namespace cpsc571.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tweetModelCHanges : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Tweets");
            AddColumn("dbo.Tweets", "Keyword", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Tweets", "Count", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Tweets", "Keyword");
            DropColumn("dbo.Tweets", "Id");
            DropColumn("dbo.Tweets", "FullText");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tweets", "FullText", c => c.String());
            AddColumn("dbo.Tweets", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Tweets");
            DropColumn("dbo.Tweets", "Count");
            DropColumn("dbo.Tweets", "Keyword");
            AddPrimaryKey("dbo.Tweets", "Id");
        }
    }
}
