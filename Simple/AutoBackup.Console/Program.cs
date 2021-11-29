using AutoBackup.ConsoleApp.Model.Dto;
using AutoBackup.Core.Servises.Common;
using AutoBackup.Database;
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

            _backupService.InitBackupDatabase(connectionDetiles.DataBaseConnection, "F:\\AutoBackup\\");
            _backupService.BackupDatabase(connectionDetiles.DataBaseConnection);
    
         

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
