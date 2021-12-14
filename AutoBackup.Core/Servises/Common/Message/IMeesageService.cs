using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Core.Servises.Common
{
  public  interface IMeesageService
    {
        void Message(string message, ConsoleColor consoleColor);
    }
}
