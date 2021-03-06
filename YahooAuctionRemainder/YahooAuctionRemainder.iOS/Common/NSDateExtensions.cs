﻿using System;
using Foundation;

namespace YahooAuctionRemainder.iOS.Common
{
    public static class NSDateExtensions
    {
        public static DateTime ToDateTime(this NSDate date)
        {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return reference.AddSeconds(date.SecondsSinceReferenceDate);
        }
    }
}
