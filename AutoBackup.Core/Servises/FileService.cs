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

        public static string GetFileBaseNameUsingSplit(string path)
        {
            string[] pathArr = path.Split('\\');
            //string[] fileArr = pathArr.Last().Split('.');
            string fileBaseName = pathArr.Last().ToString();

            return fileBaseName;
        }
    }
}
