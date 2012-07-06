using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services
{
    public interface IFileService
    {
        string GetDBBackupDirector();
        bool ExistsDirectory(string path);
        void CreateDirectory(string path);
        string[] GetFilesFromDirectory(string path);
        bool ExistsFile(string path);
    }
}