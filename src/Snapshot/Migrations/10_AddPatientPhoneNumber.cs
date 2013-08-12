using FluentMigrator;

namespace Migrations
{
    [Migration(10)]
    public class AddPatientPhoneNumber: Migration
    {
        public override void Up()
        {
            Alter.Table("MessageFromDrugShops").
                  AddColumn("PatientPhoneNumber").AsString().Nullable();
        }

        public override void Down()
        {
        }
    }
}
