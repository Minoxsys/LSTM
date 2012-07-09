using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Web.Hosting;

namespace Web.Services
{
    public class FileService : IFileService
    {
        public string GetDBBackupDirector()
        {
            return HostingEnvironment.MapPath("~\\Backup");
        }


        public bool ExistsDirectory(string path)
        {
            return Directory.Exists(path);
        }


        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }


        public string[] GetFilesFromDirectory(string path)
        {
            return Directory.GetFiles(path);
        }


        public bool ExistsFile(string path)
        {
            return File.Exists(path);
        }


        public void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
}
