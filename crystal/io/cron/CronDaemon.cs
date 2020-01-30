using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace crystal.io.Cron
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICronDaemon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="action"></param>
        void AddJob(string schedule, ThreadStart action);

        /// <summary>
        /// 
        /// </summary>
        void Start();

        /// <summary>
        /// 
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 
    /// </summary>
    public class CronDaemon : ICronDaemon
    {
        private readonly System.Timers.Timer timer = new System.Timers.Timer(30000);
        private readonly List<ICronJob> cron_jobs = new List<ICronJob>();
        private DateTime _last = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        public CronDaemon()
        {
            timer.AutoReset = true;
            timer.Elapsed += timer_elapsed;
        }

        /// <summary>
        /// Add job to queue
        /// </summary>
        /// <param name="schedule">
        /// <example>
        ///*    *    *    *    *  
        ///┬    ┬    ┬    ┬    ┬
        ///│    │    │    │    │
        ///│    │    │    │    │
        ///│    │    │    │    └───── day of week(0 - 6) (Sunday=0 )
        ///│    │    │    └────────── month(1 - 12)
        ///│    │    └─────────────── day of month(1 - 31)
        ///│    └──────────────────── hour(0 - 23)
        ///└───────────────────────── min(0 - 59)
        /// </example>
        /// </param>
        /// <param name="action">Thread to word</param>
        public void AddJob(string schedule, ThreadStart action)
        {
            var cj = new CronJob(schedule, action);
            cron_jobs.Add(cj);
        }

        /// <summary>
        /// Start jobs
        /// </summary>
        public void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// Stop All Jobs
        /// </summary>
        public void Stop()
        {
            timer.Stop();

            foreach (var cronJob in cron_jobs)
            {
                var job = (CronJob) cronJob;
                job.abort();
            }
        }

        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Minute != _last.Minute)
            {
                _last = DateTime.Now;
                foreach (ICronJob job in cron_jobs)
                    job.execute(DateTime.Now);
            }
        }
    }
}