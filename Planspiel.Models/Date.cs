using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planspiel.Models {
    public class Date {
        private const int DaysPerMonth = 30;
        private const int DaysPerYear = DaysPerMonth * 12;

        public int Day { get; }
        public int Month { get; }
        public int Year { get; }

        public int UnixDays => Day + (Month * DaysPerMonth) + (Year * DaysPerYear);

        public Date(int unixDays) {
            Year = unixDays / DaysPerYear;
            unixDays -= Year * DaysPerYear;

            Month = unixDays / DaysPerMonth;
            unixDays -= Month * DaysPerMonth;

            Day = unixDays;
        }

        public Date(int day, int month, int year) {
            Day = day;
            Month = month;
            Year = year;
        }

        public Date AddDays(int days) => new Date(UnixDays + days);

        public override string ToString()
            => Day.ToString().PadLeft(2, '0') + "." + Month.ToString().PadLeft(2, '0') + "." + Year.ToString().PadLeft(4, '0');
    }
}