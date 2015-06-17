﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PH.WorkingDaysAndTimeUtility.UnitTest
{
    [TestClass]
    public class WorkingDaysAndTimeUtilityUnitTest
    {

        [TestMethod]
        public void EmptyWeek_Fail_On_Instantiate()
        {
            Exception f0 = null;
            Exception f1 = null;

            try
            {
                var fake = new WeekDaySpan();
                var utility = new WorkingDaysAndTimeUtility(fake, new List<HolyDay>());
            }
            catch (ArgumentException ex)
            {
                f0 = ex;
            }
            try
            {
                var fake = new WeekDaySpan() { WorkDays = new Dictionary<DayOfWeek, WorkDaySpan>() };
                var utility = new WorkingDaysAndTimeUtility(fake, new List<HolyDay>());
            }
            catch (ArgumentException ex)
            {
                f1 = ex;
            }

            Assert.IsNotNull(f0);
            Assert.IsNotNull(f1);

        }

        [TestMethod]
        public void Add_1_Day_From_NoWorkingDay_Fail_On_Calculate()
        {
            var sunday = new DateTime(2015, 6, 14);
            var weekConf = GetSimpleWeek();
            var utility = new WorkingDaysAndTimeUtility(weekConf, new List<HolyDay>());
            Exception f0 = null;
            try
            {
                var fake = utility.AddWorkingDays(sunday, 4);
            }
            catch (Exception ex)
            {
                f0 = ex;
            }

            Assert.IsNotNull(f0);
        }

        [TestMethod]
        public void Add_1_Day_On_Simple_Week_From_2015_06_16_With_No_Holydays()
        {
            var d = new DateTime(2015, 6, 16);
            var weekConf = GetSimpleWeek();
            var utility = new WorkingDaysAndTimeUtility(weekConf, new List<HolyDay>());

            var r = utility.AddWorkingDays(d, 1);
            var expected = new DateTime(2015, 6, 17);
            Assert.AreEqual(expected, r);

        }

        [TestMethod]
        public void Add_1_Day_On_Simple_Week_From_2015_06_19_With_No_Holydays()
        {
            var d = new DateTime(2015, 6, 19);
            var weekConf = GetSimpleWeek();
            var utility = new WorkingDaysAndTimeUtility(weekConf, new List<HolyDay>());

            var r = utility.AddWorkingDays(d, 1);
            var expected = new DateTime(2015, 6, 22);
            Assert.AreEqual(expected, r);

        }

        [TestMethod]
        public void Add_1_Day_On_Simple_With_2_HolyDays()
        {
            var d = new DateTime(2015, 6, 16);
            var holydays = new List<HolyDay>() { new HolyDay(17, 6), new HolyDay(18, 6), new EasterMonday() };
            var weekConf = GetSimpleWeek();
            var utility = new WorkingDaysAndTimeUtility(weekConf, holydays);

            var r = utility.AddWorkingDays(d, 1);
            var expected = new DateTime(2015, 6, 19);
            Assert.AreEqual(expected, r);

        }

        [TestMethod]
        public void Invalid_Start_Time_Fail_On_Calculate()
        {
            var d = new DateTime(2015, 6, 16, 2, 3, 4);
            var weekConf = GetSimpleWeek();
            var utility = new WorkingDaysAndTimeUtility(weekConf, new List<HolyDay>());
            Exception f0 = null;
            try
            {
                var fake = utility.AddWorkingHours(d, 1);
            }
            catch (Exception exception)
            {

                f0 = exception;
            }

            Assert.IsNotNull(f0);
        }

        [TestMethod]
        public void Check_Symmetrical_And_NotSummetrical_Week()
        {
            var symmetrical = GetSimpleWeek();
            var notSymm = GetAWeek();

            Assert.IsTrue(symmetrical.Symmetrical);
            Assert.IsFalse(notSymm.Symmetrical);
        }

        [TestMethod]
        public void Add_16Hours_On_8workingHoursDay_Will_Add_2_Days()
        {
            var d = new DateTime(2015, 6, 16, 9, 0, 0);
            var weekConf = GetSimpleWeek();
            var utility = new WorkingDaysAndTimeUtility(weekConf, new List<HolyDay>());
            var r = utility.AddWorkingHours(d, 16);
            var e = new DateTime(2015, 6, 18, 9, 0, 0);
            Assert.AreEqual(e, r);


        }

        private WeekDaySpan GetSimpleWeek()
        {
            var wts1 = new WorkTimeSpan() { Start = new TimeSpan(9, 0, 0), End = new TimeSpan(13, 0, 0) };
            var wts2 = new WorkTimeSpan() { Start = new TimeSpan(14, 0, 0), End = new TimeSpan(18, 0, 0) };
            var wts = new List<WorkTimeSpan>() { wts1, wts2 };
            var week = new WeekDaySpan()
            {
                WorkDays = new Dictionary<DayOfWeek, WorkDaySpan>()
                {
                    {DayOfWeek.Monday, new WorkDaySpan() {TimeSpans = wts}}
                    ,
                    {DayOfWeek.Tuesday, new WorkDaySpan() {TimeSpans = wts}}
                    ,
                    {DayOfWeek.Wednesday, new WorkDaySpan() {TimeSpans = wts}}
                    ,
                    {DayOfWeek.Thursday, new WorkDaySpan() {TimeSpans = wts}}
                    ,
                    {DayOfWeek.Friday, new WorkDaySpan() {TimeSpans = wts}}
                }
            };
            return week;
        }

        private WeekDaySpan GetAWeek()
        {
            var wts1 = new WorkTimeSpan() { Start = new TimeSpan(9, 0, 0), End = new TimeSpan(13, 0, 0) };
            var wts2 = new WorkTimeSpan() { Start = new TimeSpan(14, 0, 0), End = new TimeSpan(16, 0, 0) };
            var wts3 = new WorkTimeSpan() { Start = new TimeSpan(16, 30, 0), End = new TimeSpan(17, 0, 0) };

            var wtsA = new List<WorkTimeSpan>() { wts1, wts2 };
            var wtsB = new List<WorkTimeSpan>() { wts1, wts2, wts3 };
            var week = new WeekDaySpan()
            {
                WorkDays = new Dictionary<DayOfWeek, WorkDaySpan>()
                {
                    {DayOfWeek.Monday, new WorkDaySpan() {TimeSpans = wtsA}}
                    ,
                    {DayOfWeek.Tuesday, new WorkDaySpan() {TimeSpans = wtsB}}
                    ,
                    {DayOfWeek.Wednesday, new WorkDaySpan() {TimeSpans = wtsA}}
                    ,
                    {DayOfWeek.Thursday, new WorkDaySpan()}
                    ,
                    {DayOfWeek.Friday, new WorkDaySpan() {TimeSpans = wtsB}}
                    ,
                    {DayOfWeek.Saturday, new WorkDaySpan() {TimeSpans = wtsA}}

                }
            };
            return week;
        }

    }

}