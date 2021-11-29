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
     
 
            var service = new ConnectionService();
            var connectionDetiles=new GetConnectionConfigInput();
        
            Console.Write("Please enter the connection string : ");
            connectionDetiles.DataBaseConnection = Console.ReadLine();
            var backupService = new BackupService(connectionDetiles.DataBaseConnection,"F:\\AutoBackup\\");
            backupService.BackupDatabase("Semicolon");
            var validConnectionString = service.checkConnectinString(connectionDetiles.DataBaseConnection);
            Console.WriteLine(validConnectionString);

            Console.ReadKey();

        }

       static ServiceProvider RegisterService() {
          return  new ServiceCollection()
                    .AddSingleton<IBackupService, BackupService>()
                    .AddSingleton<IConnectionService, ConnectionService>()
                    .AddSingleton<IProgressBar, ProgressBar>()
                   .BuildServiceProvider();
        }

     
    }
}
