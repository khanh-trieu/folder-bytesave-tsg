using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransferWorker.Models;

namespace TransferWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    var appSettingsSection = configuration.GetSection("Settings");
                    services.Configure<Settings>(appSettingsSection);

                    //var optionsBuilder = new DbContextOptionsBuilder<SoteriaDbContext>();
                    //optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=Soteria;Trusted_Connection=True;");//,
                    //services.AddScoped<SoteriaDbContext>(s => new SoteriaDbContext(optionsBuilder.Options));

                    //var folderConfigsSection = configuration.GetSection("Folders");
                    //services.Configure<List<FolderConfig>>(folderConfigsSection);

                    services.AddHostedService<Worker>();
                    //services.AddSingleton<IAzureBlobHelper, AzureBlobHelper>();
                });

    }
}