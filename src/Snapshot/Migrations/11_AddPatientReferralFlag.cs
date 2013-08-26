using FluentMigrator;

namespace Migrations
{
    [Migration(11)]
    public class AddPatientReferralFlag : Migration
    {
        public override void Up()
        {
            Alter.Table("MessageFromDrugShops").AddColumn("PatientReferralConsumed").AsBoolean().Nullable();
            Alter.Table("MessageFromDrugShops").AddColumn("PatientReferralReminderSentDate").AsDateTime().Nullable().WithDefaultValue(false);
            Alter.Table("MessageFromDrugShops").AddColumn("ReminderAnswer").AsInt32().Nullable().WithDefaultValue(0);
        }

        public override void Down()
        {
        }
    }
}
