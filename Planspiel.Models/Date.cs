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

        public string ToDB() => Year + "|" + Month + "|" + Day;

        public static Date FromDB(string str) {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(nameof(str));

            var split = str.Split('|');
            if (split.Length != 3)
                throw new FormatException("The supplied string was not in a correct format!");

            if (!int.TryParse(split[0], out var year))
                throw new FormatException("The supplied string was not in a correct format!");
            if (!int.TryParse(split[1], out var month))
                throw new FormatException("The supplied string was not in a correct format!");
            if (!int.TryParse(split[2], out var day))
                throw new FormatException("The supplied string was not in a correct format!");

            return new Date(day, month, year);
        }

        public override string ToString()
            => Day.ToString().PadLeft(2, '0') + "." + Month.ToString().PadLeft(2, '0') + "." + Year.ToString().PadLeft(4, '0');
    }
}