using AutoBackup.Database;
using AutoBackup.Http.GoogleDrive;
using AutoBackup.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{
    public static class ZarinPalServiceCollectionExtensions
    {
        public static IServiceCollection ConfZarinPal(this IServiceCollection services, Action<AutoBackupOption> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));


            var autoBackupOptions = new AutoBackupOption();
            configure?.Invoke(autoBackupOptions);

            // Register zarinpal generator service
            services.AddTransient<IBackupService, BackupService>();
            services.AddTransient<IGoogleDriveHttpService, GoogleDriveHttpService>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton(autoBackupOptions);
            return services;
        }




    }
}
