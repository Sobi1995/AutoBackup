using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Core.Model
{
   public class BuildBackupPathWithFilenameModel
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string FolderName { get; set; }
    }
}
