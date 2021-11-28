using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.ConsoleApp.Model.Services
{
 public   interface IProgressBar
    {
            void ProgressBarCiz(int sol, int ust, int deger, int isaret, ConsoleColor color);
    }
}
