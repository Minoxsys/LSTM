using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBackgrounder;
using System.Threading.Tasks;
using Web.Services;
using System.Data.SqlClient;

namespace Web.BackgroundJobs
{
    public class DBBackupJob : IJob
    {
        private const string JOB_NAME = "DBBackupJob";
        private TimeSpan INTERVAL = TimeSpan.FromDays(1);
        private TimeSpan TIMEOUT = TimeSpan.FromMinutes(2);

        public IFileService fileService;

        public DBBackupJob(IFileService fileService)
        {
            this.fileService = fileService;
        }

        public System.Threading.Tasks.Task Execute()
        {
            return new Task(() =>
            {
                string backupDirectory = fileService.GetDBBackupDirector();

                if (!fileService.ExistsDirectory(backupDirectory))
                    fileService.CreateDirectory(backupDirectory);
                    
                SqlConnection connect;
                string con = "Data Source=.\\sqlexpress;Initial Catalog=LSTMDB;Integrated Security=True";
                connect = new SqlConnection(con);
                connect.Open();

                SqlCommand command;
                string file = backupDirectory + "\\Backup_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".bak";
                command = new SqlCommand(@"backup database LSTMDB to disk ='" + file + "' with init,stats=10", connect);
                command.ExecuteNonQuery();

                connect.Close();
            });
        }

        public TimeSpan Interval
        {
            get { return INTERVAL; }
        }

        public string Name
        {
            get { return JOB_NAME; }
        }

        public TimeSpan Timeout
        {
            get { return TIMEOUT; }
        }
    }
}