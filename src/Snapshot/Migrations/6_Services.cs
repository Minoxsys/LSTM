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
            Delete.RemoveForeignKey("Diagnosis");

            Delete.Table("Diagnosiss");
        }

        public override void Up()
        {
            Create.Table("Diagnosiss")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Name").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("CodeDS").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Display").AsString(ConstraintUtility.NAME_LENGTH);

            
        }
    }
}
