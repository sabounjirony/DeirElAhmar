using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tools.Utilities
{
    public static class DateUtilities
    {
        public struct DateLimits
        {
            public DateTime FromDate;
            public DateTime ToDate;
        }

        public static string FormatTimeForDisplay(TimeSpan val)
        {
            var dayPart = val.Hours >= 12 ? "PM" : "AM";
            if (val.Hours > 12 || (val.Hours == 12 && val.Minutes != 0))
            {
                val = val.Subtract(new TimeSpan(0, 12, 0, 0));
            }
            var toRet = val.ToString("hh\\:mm\\ ") + dayPart;
            return toRet;
        }

        public static string FormatDateForDisplay(object val, bool withTime = false)
        {
            var tmpVal = CheckEmpty.Date(val);
            if (tmpVal == DateTime.MinValue)
            {
                return "N/A";
            }

            var toRet = tmpVal.ToString(withTime ? "dd-MM-yyyy hh:mm tt" : "dd-MM-yyyy");
            return toRet;
        }

        public static string GetInvariantDateTime2String(DateTime val)
        {
            var toRet = val.ToString("yyyy-MM-dd hh:mm:ss tt");
            return toRet;
        }

        public static DateTime GetInvariantDate(DateTime val)
        {
            var toRet = val;
            var value = GetInvariantDateString(val);
            if (string.IsNullOrEmpty(value)) toRet = Convert.ToDateTime(value);
            return toRet;
        }

        private static string GetInvariantDateString(DateTime val)
        {
            var toRet = string.Empty;

            var strDay = val.Day.ToUiString();
            var strMonth = val.Month.ToUiString();
            var strYear = val.Year.ToUiString();

            if ((!string.IsNullOrEmpty(strDay)) && (!string.IsNullOrEmpty(strMonth)) && (!string.IsNullOrEmpty(strYear)))
            { toRet = string.Format("{0}/{1}/{2}", strDay, strMonth, strYear); }

            return toRet;
        }

        public static DateTime GetDelayDate(DateTime startDate, string param)
        {
            var toRet = startDate;
            var arParam = param.Split(':');
            switch (arParam[1])
            {
                case "M":
                    {
                        toRet = startDate.AddMonths(Convert.ToInt16(arParam[0]));
                        break;
                    }
                case "d":
                    {
                        toRet = startDate.AddDays(Convert.ToInt16(arParam[0]));
                        break;
                    }
                case "h":
                    {
                        toRet = startDate.AddHours(Convert.ToInt16(arParam[0]));
                        break;
                    }
                case "m":
                    {
                        toRet = startDate.AddMinutes(Convert.ToInt16(arParam[0]));
                        break;
                    }
            }
            return toRet;
        }

        public static string GetDateStamp(DateTime? date = null, bool withTime = false)
        {
            //Set date default value
            var currDate = date ?? DateTime.Now;

            var datePart = string.Format("{0}_{1}_{2}", currDate.Year.ToUiString(), currDate.Month.ToUiString().PadLeft(2, '0'), currDate.Day.ToUiString().PadLeft(2, '0'));

            if (!withTime) return datePart;

            var timePart = string.Format("_{0}_{1}_{2}", currDate.Hour.ToUiString().PadLeft(2, '0'), currDate.Minute.ToUiString().PadLeft(2, '0'), currDate.Millisecond.ToUiString().PadLeft(2, '0'));

            return datePart + timePart;
        }

        public static DateLimits CalculateDate(Enumerations.DateLimit dateLimitVal, DateTime relDate)
        {
            var fromToDate = new DateLimits();

            //SET STARTING AND ENDING DATES BASED ON THE OPTION SELECTED
            switch (dateLimitVal)
            {
                case Enumerations.DateLimit.Today:
                    {
                        fromToDate.FromDate = relDate;
                        fromToDate.ToDate = relDate;
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.Yesterday:
                    {
                        fromToDate.FromDate = relDate.AddDays(-1);
                        fromToDate.ToDate = relDate.AddDays(-1);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.ThisWeek:
                    {
                        fromToDate.FromDate = relDate.AddDays(-((int)relDate.DayOfWeek-1));
                        fromToDate.ToDate = fromToDate.FromDate.AddDays(7-1);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.PreviousWeek:
                    {
                        fromToDate = CalculateDate(Enumerations.DateLimit.ThisWeek, relDate);
                        fromToDate.FromDate = fromToDate.FromDate.AddDays(-7);
                        fromToDate.ToDate = fromToDate.ToDate.AddDays(-7);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.ThisMonth:
                    {
                        fromToDate.FromDate = relDate.AddDays(-relDate.Day);
                        fromToDate.ToDate = fromToDate.FromDate.AddMonths(1);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.PreviousMonth:
                    {
                        relDate = new DateTime(relDate.Year, relDate.Month, 1);
                        fromToDate.FromDate = relDate.AddMonths(-1);
                        fromToDate.ToDate = relDate;
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.ThisQuarter:
                    {
                        switch (relDate.Month)
                        {
                            case 1:
                            case 2:
                            case 3:
                                {
                                    fromToDate.FromDate = new DateTime(relDate.Year, 1, 1);
                                    fromToDate.ToDate = new DateTime(relDate.Year, 4, 1);
                                    fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                                    return fromToDate;
                                }
                            case 4:
                            case 5:
                            case 6:
                                {
                                    fromToDate.FromDate = new DateTime(relDate.Year, 4, 1);
                                    fromToDate.ToDate = new DateTime(relDate.Year, 7, 1);
                                    fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                                    return fromToDate;
                                }
                            case 7:
                            case 8:
                            case 9:
                                {
                                    fromToDate.FromDate = new DateTime(relDate.Year, 7, 1);
                                    fromToDate.ToDate = new DateTime(relDate.Year, 10, 1);
                                    fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                                    return fromToDate;
                                }
                            case 10:
                            case 11:
                            case 12:
                                {
                                    fromToDate.FromDate = new DateTime(relDate.Year, 10, 1);
                                    fromToDate.ToDate = new DateTime(relDate.Year + 1, 1, 1);
                                    fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                                    return fromToDate;
                                }
                        }
                        break;
                    }
                case Enumerations.DateLimit.PreviousQuarter:
                    {
                        fromToDate = CalculateDate(Enumerations.DateLimit.ThisQuarter, relDate);
                        fromToDate.FromDate = fromToDate.FromDate.AddMonths(-3);
                        fromToDate.ToDate = fromToDate.ToDate.AddMonths(-3);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.ThisHalfYear:
                    {
                        switch (relDate.Month)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                {
                                    fromToDate.FromDate = new DateTime(relDate.Year, 1, 1);
                                    fromToDate.ToDate = new DateTime(relDate.Year, 7, 1);
                                    fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                                    return fromToDate;
                                }
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                                {
                                    fromToDate.FromDate = new DateTime(relDate.Year, 7, 1);
                                    fromToDate.ToDate = new DateTime(relDate.Year + 1, 1, 1);
                                    fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                                    return fromToDate;
                                }
                        }
                        break;
                    }
                case Enumerations.DateLimit.PreviousHalfYear:
                    {
                        fromToDate = CalculateDate(Enumerations.DateLimit.ThisHalfYear, relDate);
                        fromToDate.FromDate = fromToDate.FromDate.AddMonths(-6);
                        fromToDate.ToDate = fromToDate.ToDate.AddMonths(-6);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.ThisYear:
                    {
                        relDate = new DateTime(relDate.Year, 1, 1);
                        fromToDate.FromDate = relDate;
                        fromToDate.ToDate = relDate.AddYears(1);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
                case Enumerations.DateLimit.PreviousYear:
                    {
                        relDate = new DateTime(relDate.Year, 1, 1);
                        fromToDate.FromDate = relDate.AddYears(-1);
                        fromToDate.ToDate = relDate.AddDays(-1);
                        fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                        return fromToDate;
                    }
            }

            //TAKE DATE PART ONLY
            fromToDate.FromDate = fromToDate.FromDate.Date;
            fromToDate.ToDate = fromToDate.ToDate.Date;

            return fromToDate;
        }

        public static DateLimits GetDateFilter(string dateFilter, string fromDate, string toDate)
        {
            var fromToDate = new DateLimits();

            if (CheckEmpty.Numeric(dateFilter) != (long)Enumerations.DateLimit.Open)
            {
                fromToDate = CalculateDate((Enumerations.DateLimit)Convert.ToInt16(dateFilter), DateTime.Now.Date);
                //if (IsDate(fromToDate.ToDate.ToString(CultureInfo.InvariantCulture))) fromToDate.ToDate = fromToDate.ToDate.AddDays(1);
                return fromToDate;
            }

            if (CheckEmpty.Numeric(dateFilter) == (long)Enumerations.DateLimit.Open)
            {
                var from = ReadDateFromDisplay(fromDate);
                var to = ReadDateFromDisplay(toDate);

                fromToDate.FromDate = new DateTime(from.Year, from.Month, from.Day);
                fromToDate.ToDate = new DateTime(to.Year, to.Month, to.Day);
                if (IsDate(fromToDate.ToDate.ToString(CultureInfo.InvariantCulture))) fromToDate.ToDate = fromToDate.ToDate.GetMaxTimeOfDate();
                return fromToDate;
            }

            fromToDate.FromDate = DateTime.MinValue;
            fromToDate.ToDate = DateTime.MaxValue;
            return fromToDate;

        }

        public static bool IsDate(string val)
        {
            try
            {
                DateTime test;
                return DateTime.TryParse(val, out test);
            }
            catch
            {
                return false;
            }
        }

        public static DateTime ReadDateFromDisplay(this string s, bool withTime = false)
        {
            var timeofDate = new TimeSpan(0, 0, 0);
            if (withTime)
            {
                timeofDate = s.Right(s.Length - 11).ReadTimeFromDisplay();
            }

            var datePart = s.Split(' ')[0];
            if (datePart == "N/A")
            {
                datePart = new DateTime(1900, 1, 1).ToString(CultureInfo.InvariantCulture);
            }
            var arDatePart = datePart.Split(datePart.Contains('/') ? '/' : '-');
            var toRet = new DateTime(Convert.ToInt16(arDatePart[2]), Convert.ToInt16(arDatePart[1]), Convert.ToInt16(arDatePart[0]), timeofDate.Hours, timeofDate.Minutes, 0);

            return toRet;
        }

        public static TimeSpan ReadTimeFromDisplay(this string s)
        {
            var timePart = s.Left(5);
            var hours = Convert.ToInt16(s.Split(':')[0]);
            if (s.Right(2) == "PM" && hours != 12)
            {
                hours = Convert.ToInt16(hours + 12);
            }

            var minutes = Convert.ToInt16(timePart.Split(':')[1]);
            return new TimeSpan(hours, minutes, 0);
        }

        public static string ToString(this DateTime val, bool withTime = false)
        {
            if (!IsDate(val.ToString(CultureInfo.InvariantCulture)))
                return "N/A";

            return withTime ? FormatDateForDisplay(val, true) : FormatDateForDisplay(val);
        }

        public static string ToStringOrDefault(this DateTime? val, bool withTime = false)
        {
            if (withTime)
                return val.HasValue ? val.Value.ToString(true) : "N/A";
            return val.HasValue ? val.Value.ToString(false) : "N/A";
        }

        public static string SummerizePrd(double period, Enumerations.DateInterval unit)
        {
            var toRet = string.Empty;
            double dayCount = 0;
            double hourCount = 0;
            double minCount = 0;

            //Fill original values
            switch (unit)
            {
                case Enumerations.DateInterval.Minutes:
                    {
                        minCount = period;
                        break;
                    }
                case Enumerations.DateInterval.Hour:
                    {
                        hourCount = period;
                        minCount = hourCount * 60;
                        hourCount = 0;
                        break;
                    }
                case Enumerations.DateInterval.Day:
                    {
                        dayCount = period;
                        minCount = dayCount * 8 * 60;
                        dayCount = 0;
                        break;
                    }
            }

            //Summerize minutes into hours
            while (minCount >= 60)
            {
                minCount = minCount - 60;
                hourCount += 1;
            }

            //Summarize hours into days
            if (Math.Abs(hourCount - 0) > 0)
            {
                while (hourCount >= 8)
                {
                    hourCount = hourCount - 8;
                    dayCount += 1;
                }
            }

            //Fill days count
            if (Math.Abs(dayCount - 0) > 0)
                toRet += Math.Round(dayCount, 0) + " W.d. ";

            //Fill hours count
            if (Math.Abs(hourCount - 0) > 0)
                toRet += Math.Round(hourCount, 0) + " h. ";

            //Fill minutes count
            toRet += Math.Round(minCount, 0) + " min. ";

            return toRet;

        }

        public static DateTime GetMaxTimeOfDate(this DateTime val)
        {
            return new DateTime(val.Year, val.Month, val.Day, 23, 59, 59);
        }
    }
}