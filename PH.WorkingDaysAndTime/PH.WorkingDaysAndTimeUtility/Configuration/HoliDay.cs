﻿/* 
 * Copyright (c) 2015, paonath
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * 3. Neither the name of PH.WorkingDaysAndTime nor the names of its
 *    contributors may be used to endorse or promote products derived from
 *    this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */
using System;

namespace PH.WorkingDaysAndTimeUtility.Configuration
{
    /// <summary>
    /// Holiday: a non-working day.
    /// 
    /// This is a generic-instance, with <see cref="HoliDay.Day">Day</see> 
    /// and <see cref="HoliDay.Month">Month</see>.
    /// </summary>
    public class HoliDay
    {
        /// <summary>
        /// Day
        /// </summary>
        public int Day { get; private set; }
        /// <summary>
        /// Month
        /// </summary>
        public int Month { get; private set; }
        

        public HoliDay(int day,int mont)
        {
            Day = day;
            Month = mont;
            
        }

        /// <summary>
        /// It returns an instance of the data by year provided
        /// </summary>
        /// <param name="year">year provided</param>
        /// <returns>Holiday DateTime</returns>
        public virtual DateTime Calculate(int year)
        {
            return new DateTime(year,this.Month,this.Day);
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Day: {0} Month: {1}", this.Day, this.Month);

        }
    }
}