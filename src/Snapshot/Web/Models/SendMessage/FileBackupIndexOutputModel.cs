using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.SendMessage
{
    public class FileBackupIndexOutputModel
    {
        public FileBackupModel[] Files { get; set; }
        public int TotalItems { get; set; }
    }
}