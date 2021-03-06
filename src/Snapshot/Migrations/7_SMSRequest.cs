﻿using System;
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

            Delete.Table("ConditionMessageFromDrugShops");
            Delete.Table("AdviceMessageFromDispensarys");
            Delete.Table("DiagnosisMessageFromDispensarys");
            Delete.Table("MessageFromDispensaryTreatments");
            Delete.Table("AppointmentMessageFromDrugShops");
            

            //Delete.RemoveForeignKey("MessageFromDispensarys", "MessageFromDrugShop_FK", "MessageFromDispensarys");
            Delete.RemoveForeignKey("MessageFromDispensarys");
            Delete.Table("MessageFromDispensarys");

            Delete.RemoveForeignKey("MessageFromDrugShops");
            Delete.Table("MessageFromDrugShops");
        }

        public override void Up()
        {
            Create.Table("RawSmsReceiveds")
                .WithCommonColumns()

                .WithColumn("Sender").AsString(ConstraintUtility.NAME_LENGTH)
                .WithColumn("Content").AsString(ConstraintUtility.DESCRIPTION_LENGTH)
                .WithColumn("ReceivedDate").AsDateTime().Nullable()
                .WithColumn("OutpostId").AsGuid().Nullable()
                .WithColumn("OutpostType").AsInt32().Nullable()
                .WithColumn("ParseSucceeded").AsBoolean().Nullable()
                .WithColumn("ParseErrorMessage").AsString(ConstraintUtility.DESCRIPTION_LENGTH).Nullable();

            Create.AddForeignKey("RawSmsReceiveds");

            Create.Table("MessageFromDrugShops")
                .WithCommonColumns()
                .WithColumn("Initials").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("BirthDate").AsDateTime().Nullable()
                .WithColumn("Gender").AsString(ConstraintUtility.NAME_LENGTH).Nullable()
                .WithColumn("OutpostId").AsGuid().Nullable()
                .WithColumn("SentDate").AsDateTime().Nullable()
                .WithColumn("IDCode").AsString(ConstraintUtility.NAME_LENGTH);

            Create.AddForeignKey("MessageFromDrugShops");

            Create.Table("MessageFromDispensarys")
                .WithCommonColumns()
                .WithColumn("OutpostId").AsGuid().Nullable()
                .WithColumn("OutpostType").AsInt32().Nullable()
                .WithColumn("SentDate").AsDateTime().Nullable()
                .WithColumn("MessageFromDrugShop_FK").AsGuid();

            Create.AddForeignKey("MessageFromDispensarys");
            Create.AddForeignKey("MessageFromDispensarys", "MessageFromDrugShop_FK", "MessageFromDrugShops");

            Create.Table("ConditionMessageFromDrugShops")
                .WithColumn("Condition_FK").AsGuid().NotNullable()
                .WithColumn("MessageFromDrugShop_FK").AsGuid().NotNullable();

            Create.Table("AppointmentMessageFromDrugShops")
                .WithColumn("Appointment_FK").AsGuid().NotNullable()
                .WithColumn("MessageFromDrugShop_FK").AsGuid().NotNullable();

            Create.Table("AdviceMessageFromDispensarys")
                .WithColumn("Advice_FK").AsGuid().NotNullable()
                .WithColumn("MessageFromDispensary_FK").AsGuid().NotNullable();

            Create.Table("DiagnosisMessageFromDispensarys")
                .WithColumn("Diagnosis_FK").AsGuid().NotNullable()
                .WithColumn("MessageFromDispensary_FK").AsGuid().NotNullable();

             Create.Table("MessageFromDispensaryTreatments")
                .WithColumn("MessageFromDispensary_FK").AsGuid().NotNullable()
                .WithColumn("Treatment_FK").AsGuid().NotNullable();
        }
    }
}
