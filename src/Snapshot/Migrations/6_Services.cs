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

            Delete.RemoveForeignKey("Treatments");
            Delete.RemoveClientForeignKey("Treatments");
            Delete.Table("Treatments");

            //Delete.RemoveForeignKey("Conditions");
            //Delete.RemoveClientForeignKey("Conditions");
            //Delete.Table("Conditions");

            Delete.RemoveForeignKey("Advices");
            Delete.RemoveClientForeignKey("Advices");
            Delete.Table("Advices");
        }

        public override void Up()
        {
            Create.Table("Diagnosiss")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Keyword").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("Code").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();
                
            Create.AddForeignKey("Diagnosiss");
            Create.AddClientForeignKey("Diagnosiss");

            Create.Table("Treatments")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Keyword").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("Code").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();

            Create.AddForeignKey("Treatments");
            Create.AddClientForeignKey("Treatments");

            Create.Table("Conditions")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Keyword").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("Code").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();

            Create.AddForeignKey("Conditions");
            Create.AddClientForeignKey("Conditions");

            Create.Table("Advices")
                .WithCommonColumns()
                .WithClientColumn()
                .WithColumn("Keyword").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("Code").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Description").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();

            Create.AddForeignKey("Advices");
            Create.AddClientForeignKey("Advices");
        }
    }
}
