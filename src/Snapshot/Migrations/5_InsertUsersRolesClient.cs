using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Migrations
{
    [Migration(5)]
    public class InsertUsersRolesClient: Migration
    {
        public override void Down()
        {
        }

        public override void Up()
        {
            this.IfDatabase("sqlserver").Execute.EmbeddedScript(@"Migrations.Scripts.sqlserver_InsertIntoTables.sql");
        }
    }
}
