using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(6)]
    public class Services : Migration
    {
        public override void Down()
        {
            Delete.RemoveForeignKey("Diagnosiss");
            Delete.RemoveClientForeignKey("Diagnosiss");

            Delete.Table("Diagnosiss");
        }

        public override void Up()
        {
            Create.Table("Diagnosiss")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("ServiceNeeded").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("Code").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Description").AsString(ConstraintUtility.NAME_LENGTH).Nullable();
                
                

            Create.AddForeignKey("Diagnosiss");
            Create.AddClientForeignKey("Diagnosiss");
        }
    }
}
