using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(9)]
    public class NoOfWrongMesssages : Migration
    {

        public override void Down()
        {
            Delete.RemoveForeignKey("WrongMessages");
            Delete.Table("WrongMessages");
        }

        public override void Up()
        {
            Create.Table("WrongMessages")
                .WithCommonColumns()

                .WithColumn("PhoneNumber").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("NoOfWrongMessages").AsInt32().Nullable()
                .WithColumn("SentDate").AsDate();

            Create.AddForeignKey("WrongMessages");
        }
    }
}
