namespace Net.iOS.Charts.Sample.Formatters;

public sealed class DateValueFormatter : NSObject, IChartAxisValueFormatter
{
    private readonly NSDateFormatter _dateFormatter = new() { DateFormat = "dd MMM HH:mm" };
    
    public string StringForValue(double value, ChartAxisBase axis) =>
        _dateFormatter.ToString(NSDate.FromTimeIntervalSince1970(value));
}