﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(8)]
    public class SentMessages : Migration
    {
        public override void Down()
        {
            Delete.RemoveForeignKey("SentSmss");
            Delete.Table("SentSmss");
        }

        public override void Up()
        {
            Create.Table("SentSmss")
                .WithCommonColumns()

                .WithColumn("Message").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable()
                .WithColumn("PhoneNumber").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("SentDate").AsDateTime().Nullable()
                .WithColumn("Response").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();

            Create.AddForeignKey("SentSmss");
        }
    }
}
