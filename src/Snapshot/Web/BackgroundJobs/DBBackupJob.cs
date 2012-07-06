using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class DBBackupJob : IJob
    {
        private const string JOB_NAME = "DBBackupJob";
        private TimeSpan INTERVAL = TimeSpan.FromMinutes(1);
        private TimeSpan TIMEOUT = TimeSpan.FromSeconds(90);

        public System.Threading.Tasks.Task Execute()
        {
            throw new NotImplementedException();
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