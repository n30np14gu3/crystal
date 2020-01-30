using System;
using System.Threading;

namespace crystal.io.Cron
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICronJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date_time"></param>
        void execute(DateTime date_time);

        /// <summary>
        /// 
        /// </summary>
        void abort();
    }

    /// <summary>
    /// 
    /// </summary>
    public class CronJob : ICronJob
    {
        private readonly ICronSchedule _cron_schedule;
        private readonly ThreadStart _thread_start;
        private Thread _thread;

        /// <summary>
        /// 
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
        /// <param name="thread_start"></param>
        public CronJob(string schedule, ThreadStart thread_start)
        {
            _cron_schedule = new CronSchedule(schedule);
            _thread_start = thread_start;
            _thread = new Thread(thread_start);
        }

        private object _lock = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date_time"></param>
        public void execute(DateTime date_time)
        {
            lock (_lock)
            {
                if (!_cron_schedule.isTime(date_time))
                    return;

                if (_thread.ThreadState == ThreadState.Running)
                    return;

                _thread = new Thread(_thread_start);
                _thread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void abort()
        {
            _thread.Abort();
        }

    }
}