using AutoBackup.ConsoleApp.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.ConsoleApp.Model.Services
{
 public   interface IUtilityService
    {
        public bool checkConnectinString(string connectionString);
        public ConnectionDetilesModel GetConnectionDetiles(string connection);
    }
}
