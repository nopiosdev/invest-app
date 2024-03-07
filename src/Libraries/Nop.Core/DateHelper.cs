using System;

public static class DateHelper
{
    public static DateTime GetLastDayOfCurrentMonth()
    {
        DateTime now = DateTime.Now;
        int year = now.Year;
        int month = now.Month;

        // Get the first day of the next month
        DateTime firstDayOfNextMonth = (month == 12) ? new DateTime(year + 1, 1, 1) : new DateTime(year, month + 1, 1);

        // Subtract one day to get the last day of the current month
        return firstDayOfNextMonth.AddDays(-1);
    }

    public static string ToFormattedDate(DateTime dateTime)
    {
        return dateTime.ToString("MMMM dd, yyyy");
    }
}
