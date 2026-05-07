using System.Windows;
using PCClubAdmin.Services;

namespace PCClubAdmin
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DatabaseInitializer.Initialize();
            DemoDataSeeder.SeedIfNeeded();
            base.OnStartup(e);
        }
    }
}
