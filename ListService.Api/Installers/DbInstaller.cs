namespace ListService.Api.Installers
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Service;

    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DefaultConnection"];

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 7, 12)), m =>
                    {
                        m.MigrationsAssembly("BattleAuth.Api");
                        m.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null!);
                    }).UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole().AddFilter(level => level >= LogLevel.Debug)))
                    .EnableSensitiveDataLogging().EnableDetailedErrors();
            });
        }
    }
}