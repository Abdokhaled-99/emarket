﻿namespace emarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reADDCartTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cart",
                c => new
                    {
                        Product_id = c.Int(nullable: false),
                        Added_at = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Product_id)
                .ForeignKey("dbo.Product", t => t.Product_id)
                .Index(t => t.Product_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cart", "Product_id", "dbo.Product");
            DropIndex("dbo.Cart", new[] { "Product_id" });
            DropTable("dbo.Cart");
        }
    }
}
