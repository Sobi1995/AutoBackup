using AutoBackup.ConsoleApp.Model.Dto;
using AutoBackup.ConsoleApp.Model.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutoBackup.ConsoleApp
{
      class Program
    {
        static void  Main(string[] args)
        {

            //setup our DI
            var serviceProvider = new ServiceCollection()
                 .AddSingleton<IBackupService, BackupService>()
                 .AddSingleton<IUtilityService, UtilityService>()
                 .AddSingleton<IProgressBar, ProgressBar>()
                .BuildServiceProvider();

            var _progressBar = serviceProvider.GetService<IProgressBar>();
            _progressBar.ProgressBarCiz(2, 1, 100, 0, ConsoleColor.White);
            var service = new UtilityService();
            var connectionDetiles=new GetConnectionConfigInput();
        
            Console.Write("Please enter the connection string : ");
            connectionDetiles.DataBaseConnection = Console.ReadLine();
            var backupService = new BackupService(connectionDetiles.DataBaseConnection,"F:\\AutoBackup\\");
            backupService.BackupDatabase("Semicolon");
            var validConnectionString = service.checkConnectinString(connectionDetiles.DataBaseConnection);
            Console.WriteLine(validConnectionString);

            Console.ReadKey();

        }

     
    }
}
