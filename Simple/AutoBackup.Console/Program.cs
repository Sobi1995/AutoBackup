using AutoBackup.ConsoleApp.Model.Dto;
using AutoBackup.ConsoleApp.Model.Services;
using System;

namespace AutoBackup.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {



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
