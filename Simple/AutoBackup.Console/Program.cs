using AutoBackup.Core.Servises.Common;
using AutoBackup.Database;
using AutoBackup.DatabaseModel.Dto;
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
            _backupService.BackupDatabase(connectionDetiles.DataBaseConnection, string.Empty,"asdasdasfcsa assa");
    
         

            Console.ReadKey();

        }

       static ServiceProvider RegisterService() {
          return  new ServiceCollection()
                    .AddSingleton<IBackupService, BackupService>()
            
                    .AddSingleton<IProgressBar, ProgressBar>()
                   .BuildServiceProvider();
        }

     
    }
}
