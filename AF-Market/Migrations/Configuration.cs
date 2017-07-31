namespace AF_Market.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AF_Market.Models.AF_MarketContext>
    {
        public Configuration()
        {
            // AFRAGO: En producción no se recomienda tener habilitadas las migraciones automáticas
            AutomaticMigrationsEnabled = true;
            // AFRAGO: Posibilidad de perdida de datos en caso de que una de las migraciones lo necesite
            AutomaticMigrationDataLossAllowed = true; 
            ContextKey = "AF_Market.Models.AF_MarketContext";
            // AFRAGO: Modificar Global.asax => Cada vez que arrancas verifica si el modelo ha cambiado
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<Models.AF_MarketContext , Migrations.Configuration>);
            // ¡Al generar nuevos campos la aplicación no debe fallar!
        }

        protected override void Seed(AF_Market.Models.AF_MarketContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
