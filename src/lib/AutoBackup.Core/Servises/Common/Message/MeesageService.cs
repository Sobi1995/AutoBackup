using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Core.Servises.Common
{
    public class MeesageService : IMeesageService
    {
        public void Message(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine( message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
