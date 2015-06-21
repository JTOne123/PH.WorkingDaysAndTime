﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PH.WorkingDaysAndTimeUtility
{
    public class WorkingDaysAndTimeUtility : IWorkingDaysAndTimeUtility
    {
        private WeekDaySpan _workWeekConfiguration;
        private List<HoliDay> _holidays;
        
       
        /// <summary>
        /// Create a new instance of the utility with given configuration.
        /// </summary>
        /// <param name="workWeekConfiguration">work-week configuration</param>
        /// <param name="holidays">List of holidays</param>
        /// <exception cref="ArgumentNullException">Trown if null workWeekConfiguration</exception>
        /// <exception cref="ArgumentException">Thrown if workWeekConfiguration without working days defined</exception>
        public WorkingDaysAndTimeUtility(WeekDaySpan workWeekConfiguration
            , List<HoliDay> holidays
            )
        {
            if(null == workWeekConfiguration)
                throw new ArgumentNullException("workWeekConfiguration","Week configuration mandatory");
            try
            {
                CheckWeek(workWeekConfiguration);

                _workWeekConfiguration = workWeekConfiguration;
                _holidays = holidays;

            }
            catch (ArgumentException agEx)
            {

                throw new ArgumentException("Invalid workWeekConfiguration",agEx);
            }
            
        }

        public DateTime AddWorkingDays(DateTime start, int days)
        {
            CheckWorkDayStart(start);

            List<DateTime> toExclude = CalculateDaysForExclusions(start.Year);

            DateTime end = start;
            for (int i = 0; i < days; i++)
            {
                end = AddOneDay(end,ref toExclude);
            }
            return end;
        }
        public DateTime AddWorkingDays(DateTime start, int days, out List<DateTime> resultListOfHoliDays)
        {
            CheckWorkDayStart(start);

            List<DateTime> toExclude = CalculateDaysForExclusions(start.Year);

            DateTime end = start;
            for (int i = 0; i < days; i++)
            {
                end = AddOneDay(end, ref toExclude);
            }
            resultListOfHoliDays = toExclude;
            return end;
        }
        public DateTime AddWorkingHours(DateTime start, double hours)
        {
            CheckWorkDayStart(start);
            DateTime r = start;
            var totMinutes = CheckWorkTimeStartandGetTotalWorkingHoursForTheDay(start);
            List<DateTime> toExclude = CalculateDaysForExclusions(start.Year);
            double hh = hours * 60;

            if (totMinutes <= hh && _workWeekConfiguration.Symmetrical )
            {
                #region Just for "Symmetrical" week

                
                var days = (int) (hh/totMinutes);
                var otherMinutes = hh % totMinutes;
                r = AddWorkingDays(r, days, out toExclude);
                if (otherMinutes > (double) 0)
                {
                    r = AddWorkingMinutes(r, otherMinutes, toExclude);
                }
                

                #endregion
            }
            else
            {
                r = AddWorkingMinutes(r, totMinutes, toExclude);

            }
            
            return r;
        }

        public List<DateTime> GetWorkingDaysBetweenTwoDateTimes(DateTime start, DateTime end, bool includeStartAndEnd = true)
        {
            CheckWorkDayStart(start);
            List<DateTime> result = new List<DateTime>() {};
            if (includeStartAndEnd)
            {
                result.Add(start);
                result.Add(end);
            }
            List<DateTime> toExclude = CalculateDaysForExclusions(start.Year);

            while (start.Date < end.Date)
            {
                start = AddOneDay(start, ref toExclude);
                if (start.Date < end.Date || includeStartAndEnd)
                    result.Add(start);
            }
            return result.Distinct().OrderByDescending(x => x.Date).ToList();
        }


        private DateTime AddWorkingMinutes(DateTime start, double otherMinutes, List<DateTime> toExclude)
        {
            DateTime r = start;
            while (otherMinutes > 0)
            {
                r = AddOneMinute(r, ref toExclude);
                otherMinutes--;
            }
            return r;
        }

        private DateTime AddOneMinute(DateTime start, ref List<DateTime> toExclude)
        {
            DateTime r = start.AddMinutes(1);
            
            //check if in work-interval
            WorkTimeSpan nextInterval;
            bool isInWorkInterval = CheckIfWorkTime(r, out nextInterval);
            if (!isInWorkInterval)
            {
  
                if (null != nextInterval)
                {
                    var ts = nextInterval.Start;
                    r = new DateTime(r.Year, r.Month, r.Day, ts.Hours,ts.Minutes,ts.Seconds);
                }
                else
                {
                    r = AddOneDay(r, ref toExclude);
                    var ts = GetFirstTimeSpanOfTheWorkingDay(r);
                    r = new DateTime(r.Year, r.Month, r.Day, ts.Hours, ts.Minutes, ts.Seconds);
                }
            }

            return r;
        }

        private TimeSpan GetFirstTimeSpanOfTheWorkingDay(DateTime d)
        {
            var workDaySpan = _workWeekConfiguration.WorkDays[d.DayOfWeek];
            return workDaySpan.TimeSpans
                .OrderBy(x => x.Start).Select(x => x.Start).FirstOrDefault();
        }

        private bool CheckIfWorkTime(DateTime d, out WorkTimeSpan nextInterval)
        {
            var workDaySpan = _workWeekConfiguration.WorkDays[d.DayOfWeek];
            bool r = false;
            nextInterval = null;

            if (null == workDaySpan)
            {
                
                return false;
            }
            else
            {
                var orderdTimes = (from t in workDaySpan.TimeSpans
                    orderby t.Start ascending, t.End ascending
                    select t
                    ).ToArray();
                int counter = -1;
                foreach (var ts in orderdTimes)
                {
                    counter++;

                    var s = new DateTime(d.Year, d.Month, d.Day, ts.Start.Hours, ts.Start.Minutes,
                        ts.Start.Seconds);
                    var e = new DateTime(d.Year, d.Month, d.Day, ts.End.Hours, ts.End.Minutes,
                        ts.End.Seconds);
                    if (s <= d && d <= e)
                    {
                        r = true;
                        nextInterval = counter == orderdTimes.Length - 1 ? null : orderdTimes[counter + 1];
                        break;
                    }
                }
            }
            
            return r;
        }

        private void CheckWorkDayStart(DateTime start)
        {
            if (!(_workWeekConfiguration.WorkDays.ContainsKey(start.DayOfWeek)))
            {
                var err = "Invalid DateTime start given: give a workingday for start or check your configuration";
                throw new ArgumentException(err, "start");
            }
        }


        private double GetTotalWorkingHoursForTheDay(DateTime d)
        {
            double r = (double) 0;
            var workDaySpan = _workWeekConfiguration.WorkDays[d.DayOfWeek];
            workDaySpan.TimeSpans.OrderBy(x => x.Start).ToList()
                .ForEach(ts =>
                {
                    var s = new DateTime(d.Year, d.Month, d.Day, ts.Start.Hours, ts.Start.Minutes,
                        ts.Start.Seconds);
                    var e = new DateTime(d.Year, d.Month, d.Day, ts.End.Hours, ts.End.Minutes,
                        ts.End.Seconds);
                    if (s <= d && d <= e)
                    {
                        
                        r = workDaySpan.WorkingMinutesPerDay;

                    }
                });
            return r;
        }

        private double CheckWorkTimeStartandGetTotalWorkingHoursForTheDay(DateTime start)
        {
            double ret = GetTotalWorkingHoursForTheDay(start);
            var inError = ret == (double)0;
            
            if (inError)
            {
                var err = "Invalid DateTime start given: give a valid time for start or check your configuration";
                throw new ArgumentException(err, "start");
            }
            else
            {
                return ret;
            }
        }

        private void CheckWeek(WeekDaySpan weekDaySpan)
        {
            bool throwMy = true;
            if (null != weekDaySpan.WorkDays)
            {
                foreach (DayOfWeek dow in Enum.GetValues(typeof (DayOfWeek)))
                {
                    if (weekDaySpan.WorkDays.ContainsKey(dow))
                        throwMy = false;
                }
            }
            if (throwMy)
            {
                var err = "WeekDaySpan without working days defined, check your configuration";
                throw new ArgumentException(err, "weekDaySpan");
            }
        }

        private DateTime AddOneDay(DateTime d,ref List<DateTime> toExclude)
        {
            int y = d.Year;
            DateTime r = d.AddDays(1);
            if (y < r.Year)
            {
                toExclude.AddRange(CalculateDaysForExclusions(r.Year));
            }
            bool addAnother = false;
            //check if current is a workingDay
            if (_workWeekConfiguration.WorkDays.ContainsKey(r.DayOfWeek))
            {
                if (!(_workWeekConfiguration.WorkDays[r.DayOfWeek].IsWorkingDay))
                {
                    addAnother = true;
                }
            }
            else
            {
                addAnother = true;
            }

            if(addAnother)
                r = AddOneDay(r,ref toExclude);
            

            if (null != toExclude && toExclude.Count > 0 
                && DateTime.MinValue != toExclude.FirstOrDefault(x => x.Date == r.Date))
            {
                toExclude.Remove(r);
                r = AddOneDay(r,ref toExclude);
            }
            
            return r;
        }


        private List<DateTime> CalculateDaysForExclusions(int year)
        {
            List<DateTime> r = new List<DateTime>();
            _holidays.ForEach(day =>
            {
                r.Add(day.Calculate(year));
            });
            return r.OrderByDescending(x => x.Date).ToList();
        }




    }
}
