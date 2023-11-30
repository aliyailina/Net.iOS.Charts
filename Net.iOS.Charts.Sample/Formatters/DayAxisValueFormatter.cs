namespace Net.iOS.Charts.Sample.Formatters;

public sealed class DayAxisValueFormatter : NSObject, IChartAxisValueFormatter
{
    private readonly string[] _months;
    private readonly BarLineChartViewBase _chart;

    public DayAxisValueFormatter(BarLineChartViewBase chart)
    {
        _chart = chart;

        _months = new[]
        {
            "Jan", "Feb", "Mar",
            "Apr", "May", "Jun",
            "Jul", "Aug", "Sep",
            "Oct", "Nov", "Dec"
        };
    }
    
    public string StringForValue(double value, ChartAxisBase axis)
    {
        var days = (int)value;
        var year = DetermineYearForDays(days);
        var month = DetermineMonthForDayOfYear(days);
        var monthName = _months[month % _months.Length];
        var yearName = year.ToString();
        if (_chart.VisibleXRange > 30 * 6)
        {
            return $"{monthName} {yearName}";
        }
        else
        {
            var dayOfMonth = DetermineDayOfMonthForDays(days, month + 12 * (year - 2016));

            var appendix = "th";
            switch (dayOfMonth)
            {
                case 1:
                    appendix = "st";
                    break;
                case 2:
                    appendix = "nd";
                    break;
                case 3:
                    appendix = "rd";
                    break;
                case 21:
                    appendix = "st";
                    break;
                case 22:
                    appendix = "nd";
                    break;
                case 23:
                    appendix = "rd";
                    break;
                case 31:
                    appendix = "st";
                    break;
            }

            return dayOfMonth == 0 ? string.Empty : $"{dayOfMonth}{appendix} {monthName}";
        }
    }

    private int DetermineDayOfMonthForDays(int days, int month)
    {
        int count = 0;
        int daysForMonths = 0;
       
        while (count < month)
        {
            int year = DetermineYearForDays(days);
            daysForMonths += DaysForMonth(count % 12, year);
            count++;
        }
       
        return days - daysForMonths;
    }

    private int DaysForMonth(int month, int year)
    {
        if (month == 1)
        {
            var is29Feb = false;
        
            if (year < 1582)
            {
                is29Feb = (year < 1 ? year + 1 : year) % 4 == 0;
            }
            else if (year > 1582)
            {
                is29Feb = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
            }
        
            return is29Feb ? 29 : 28;
        }
    
        if (month == 3 || month == 5 || month == 8 || month == 10)
        {
            return 30;
        }
    
        return 31;
    }

    private int DetermineMonthForDayOfYear(int dayOfYear)
    {
        int month = -1;
        int days = 0;
    
        while (days < dayOfYear)
        {
            month = month + 1;
        
            if (month >= 12)
                month = 0;
        
            int year = DetermineYearForDays(days);
            days += DaysForMonth(month, year);
        }
    
        return Math.Max(month, 0);
    }

    private int DetermineYearForDays(int days)
    {
        if (days <= 366)
        {
            return 2016;
        }
        else if (days <= 730)
        {
            return 2017;
        }
        else if (days <= 1094)
        {
            return 2018;
        }
        else if (days <= 1458)
        {
            return 2019;
        }
    
        return 2020;
    }
        
}