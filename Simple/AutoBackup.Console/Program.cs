using AutoBackup.ConsoleApp.Model.Dto;
using System;

namespace AutoBackup.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionDetiles=new GetConnectionConfigInput();
            Console.Write("Please enter the connection string : ");
            connectionDetiles.DataBaseConnection = Console.ReadLine();


        }
    }
}
