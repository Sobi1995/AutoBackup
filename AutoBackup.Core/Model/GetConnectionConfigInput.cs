using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.DatabaseModel.Dto
{
   public class GetConnectionConfigInput
    {
        public string DataBaseConnection { get; set; }
        public string GoogleDriveKey { get; set; }
        public string FilePatchToBackup { get; set; }
    }
}
