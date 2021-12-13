using AutoBackup.Core.Servises;
using AutoBackup.Core.Servises.Common;
using AutoBackup.Database;
using AutoBackup.DatabaseModel.Dto;
using AutoBackup.Http.GoogleDrive;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoBackup.ConsoleApp
{
    class Program
    {
        static void  Main(string[] args)
        {
            var serviceProvider = RegisterService();
            var _backupService = serviceProvider.GetService<IBackupService>();
 
            var connectionDetiles=new GetConnectionConfigInput();
        
            Console.Write("Please enter the connection string : ");
           connectionDetiles.DataBaseConnection = Console.ReadLine();
            connectionDetiles.DataBaseConnection = "Server=.;Database=Coffeete_db;Trusted_Connection=True;MultipleActiveResultSets=true;";
            _backupService.BackupDatabase(connectionDetiles.DataBaseConnection );
    
         

            Console.ReadKey();

        }

       static ServiceProvider RegisterService() {
          return  new ServiceCollection()
                    .AddSingleton<IBackupService, BackupService>()
                    .AddSingleton<IGoogleDriveHttpService, GoogleDriveHttpService>()
                    .AddSingleton<IProgressBar, ProgressBar>()
                    .AddSingleton<IFileService, FileService>()
                   .BuildServiceProvider();
        }

     
    }
}
