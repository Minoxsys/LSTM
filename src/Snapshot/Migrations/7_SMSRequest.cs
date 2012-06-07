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

            Delete.Table("MessageFromDrugShopServiceNeededs");
            Delete.Table("AdviceMessageFromDispensarys");
            Delete.Table("DiagnosisMessageFromDispensarys");
            Delete.Table("MessageFromDispensaryTreatments");

            Delete.RemoveForeignKey("MessageFromDrugShops");
            Delete.Table("MessageFromDrugShops");

            Delete.RemoveForeignKey("MessageFromDispensarys");
            Delete.RemoveForeignKey("MessageFromDispensarys", "MessageFromDrugShop_FK", "MessageFromDispensarys");
            Delete.Table("MessageFromDispensarys");
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
                .WithColumn("OutpostType").AsInt32().Nullable()
                .WithColumn("ParseSucceeded").AsBoolean().Nullable()
                .WithColumn("ParseErrorMessage").AsString(ConstraintUtility.NAME_LENGTH).Nullable();

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

            Create.Table("MessageFromDrugShopServiceNeededs")
                .WithColumn("ServiceNeeded_FK").AsGuid().NotNullable()
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
