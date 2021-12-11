using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Core.Servises
{
    public static class FileService
    {
        public static Stream GetFileStream(string fileName) => File.OpenRead(fileName);
    }
}
