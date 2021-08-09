namespace ALRSSystem.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ALRSSystem.Models;
    using System.Collections.Generic;
    internal sealed class Configuration : DbMigrationsConfiguration<ALRSSystem.Models.ALRSDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ALRSSystem.Models.ALRSDB context)
        {
            //  This method will be called after migrating to the latest version.

            //var requestorpositions = new List<RequestorPosition>
            //{
            //    new RequestorPosition{RequestorPositionName="Associates"},
            //    new RequestorPosition{RequestorPositionName="Tipstaff"},
            //};

            //requestorpositions.ForEach(s => context.RequestorPosition.Add(s));
            //context.SaveChanges();

        }
    }
}
