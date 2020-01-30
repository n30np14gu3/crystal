using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace crystal.io.Cron
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICronSchedule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool isValid(string expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date_time"></param>
        /// <returns></returns>
        bool isTime(DateTime date_time);
    }

    /// <summary>
    /// Schedule for planning
    /// </summary>
    public class CronSchedule : ICronSchedule
    {
        #region Readonly Class Members


        static readonly Regex divided_regex = new Regex(@"(\*/\d+)");
        static readonly Regex range_regex = new Regex(@"(\d+\-\d+)\/?(\d+)?");
        static readonly Regex wild_regex = new Regex(@"(\*)");
        static readonly Regex list_regex = new Regex(@"(((\d+,)*\d+)+)");
        static readonly Regex validation_regex = new Regex(divided_regex + "|" + range_regex + "|" + wild_regex + "|" + list_regex);

        #endregion

        #region Private Instance Members

        private readonly string _expression;
        /// <summary>
        /// 
        /// </summary>
        public List<int> minutes;

        /// <summary>
        /// 
        /// </summary>
        public List<int> hours;

        /// <summary>
        /// 
        /// </summary>
        public List<int> days_of_month;

        /// <summary>
        /// 
        /// </summary>
        public List<int> months;

        /// <summary>
        /// 
        /// </summary>
        public List<int> days_of_week;

        #endregion

        #region Public Constructors

        /// <summary>
        /// 
        /// </summary>
        public CronSchedule()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CronSchedule(string expressions)
        {
            _expression = expressions;
            generate();
        }

        #endregion

        #region Public Methods

        private bool isValid()
        {
            return isValid(_expression);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isValid(string expression)
        {
            MatchCollection matches = validation_regex.Matches(expression);
            return matches.Count > 0;//== 5;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isTime(DateTime date_time)
        {
            return minutes.Contains(date_time.Minute) &&
                   hours.Contains(date_time.Hour) &&
                   days_of_month.Contains(date_time.Day) &&
                   months.Contains(date_time.Month) &&
                   days_of_week.Contains((int)date_time.DayOfWeek);
        }

        private void generate()
        {
            if (!isValid()) return;

            MatchCollection matches = validation_regex.Matches(_expression);

            generate_minutes(matches[0].ToString());

            if (matches.Count > 1)
                generate_hours(matches[1].ToString());
            else
                generate_hours("*");

            if (matches.Count > 2)
                generate_days_of_month(matches[2].ToString());
            else
                generate_days_of_month("*");

            if (matches.Count > 3)
                generate_months(matches[3].ToString());
            else
                generate_months("*");

            if (matches.Count > 4)
                generate_days_of_weeks(matches[4].ToString());
            else
                generate_days_of_weeks("*");
        }

        private void generate_minutes(string match)
        {
            minutes = generate_values(match, 0, 60);
        }

        private void generate_hours(string match)
        {
            hours = generate_values(match, 0, 24);
        }

        private void generate_days_of_month(string match)
        {
            days_of_month = generate_values(match, 1, 32);
        }

        private void generate_months(string match)
        {
            months = generate_values(match, 1, 13);
        }

        private void generate_days_of_weeks(string match)
        {
            days_of_week = generate_values(match, 0, 7);
        }

        private List<int> generate_values(string configuration, int start, int max)
        {
            if (divided_regex.IsMatch(configuration)) return divided_array(configuration, start, max);
            if (range_regex.IsMatch(configuration)) return range_array(configuration);
            if (wild_regex.IsMatch(configuration)) return wild_array(configuration, start, max);
            if (list_regex.IsMatch(configuration)) return list_array(configuration);

            return new List<int>();
        }

        private List<int> divided_array(string configuration, int start, int max)
        {
            if (!divided_regex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();
            string[] split = configuration.Split("/".ToCharArray());
            int divisor = int.Parse(split[1]);

            for (int i = start; i < max; ++i)
                if (i % divisor == 0)
                    ret.Add(i);

            return ret;
        }

        private List<int> range_array(string configuration)
        {
            if (!range_regex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();
            string[] split = configuration.Split("-".ToCharArray());
            int start = int.Parse(split[0]);
            int end;
            if (split[1].Contains("/"))
            {
                split = split[1].Split("/".ToCharArray());
                end = int.Parse(split[0]);
                int divisor = int.Parse(split[1]);

                for (int i = start; i < end; ++i)
                    if (i % divisor == 0)
                        ret.Add(i);
                return ret;
            }
            else
                end = int.Parse(split[1]);

            for (int i = start; i <= end; ++i)
                ret.Add(i);

            return ret;
        }

        private List<int> wild_array(string configuration, int start, int max)
        {
            if (!wild_regex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();

            for (int i = start; i < max; ++i)
                ret.Add(i);

            return ret;
        }

        private List<int> list_array(string configuration)
        {
            if (!list_regex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();

            string[] split = configuration.Split(",".ToCharArray());

            foreach (string s in split)
                ret.Add(int.Parse(s));

            return ret;
        }

        #endregion
    }
}