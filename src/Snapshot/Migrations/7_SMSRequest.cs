using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(7)]
    public class SMSRequest : Migration
    {
        public override void Down()
        {
            Delete.RemoveForeignKey("RawSmsReceiveds");
            Delete.Table("RawSmsReceiveds");
        }

        public override void Up()
        {
            Create.Table("RawSmsReceiveds")
                .WithCommonColumns()
                .WithColumn("Sender").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Content").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Credits").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("ReceivedDate").AsDateTime().Nullable()
                .WithColumn("OutpostId").AsGuid().Nullable()
                .WithColumn("OutpostName").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("IsDispensary").AsBoolean().Nullable()
                .WithColumn("ParseSucceeded").AsBoolean().Nullable()
                .WithColumn("ParseErrorMessage").AsString(ConstraintUtility.NAME_LENGTH).Nullable();

            Create.AddForeignKey("RawSmsReceiveds");
        }
    }
}
