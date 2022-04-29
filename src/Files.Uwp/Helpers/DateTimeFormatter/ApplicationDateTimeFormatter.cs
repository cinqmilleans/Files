﻿using Microsoft.Toolkit.Uwp;
using System;

namespace Files.Uwp.Helpers
{
    internal class ApplicationDateTimeFormatter : AbstractDateTimeFormatter
    {
        public override string Name { get; }

        public ApplicationDateTimeFormatter(string name) => Name = name;

        public override string ToShortLabel(DateTimeOffset offset)
        {
            var elapsed = DateTimeOffset.Now - offset;

            if (offset.Year <= 1601 || offset.Year >= 9999)
            {
                return " ";
            }
            if (elapsed.TotalDays >= 7)
            {
                return offset.ToLocalTime().ToString("D");
            }
            if (elapsed.TotalDays >= 2)
            {
                return string.Format("DaysAgo".GetLocalized(), elapsed.Days);
            }
            if (elapsed.TotalDays >= 1)
            {
                return string.Format("DayAgo".GetLocalized(), elapsed.Days);
            }
            if (elapsed.TotalHours >= 2)
            {
                return string.Format("HoursAgo".GetLocalized(), elapsed.Hours);
            }
            if (elapsed.TotalHours >= 1)
            {
                return string.Format("HourAgo".GetLocalized(), elapsed.Hours);
            }
            if (elapsed.TotalMinutes >= 2)
            {
                return string.Format("MinutesAgo".GetLocalized(), elapsed.Minutes);
            }
            if (elapsed.TotalMinutes >= 1)
            {
                return string.Format("MinuteAgo".GetLocalized(), elapsed.Minutes);
            }
            return string.Format("SecondsAgo".GetLocalized(), elapsed.Seconds);
        }

        public override string ToLongLabel(DateTimeOffset offset)
        {
            var elapsed = DateTimeOffset.Now - offset;

            if (offset.Year <= 1601 || offset.Year >= 9999)
            {
                return " ";
            }
            if (elapsed.TotalDays < 7)
            {
                return offset.ToLocalTime().ToString("D t") + " (" + ToShortLabel(offset) + ")";
            }
            return offset.ToLocalTime().ToString("D t");
        }
    }
}
