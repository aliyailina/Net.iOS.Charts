using System.Globalization;
using Net.iOS.Charts.Sample.Formatters;
using ObjCRuntime;

namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(BarChartViewController))]
public sealed partial class BarChartViewController : DemoBaseViewController, IChartViewDelegate
{
    public BarChartViewController()
    { }
    
    public BarChartViewController(NSCoder coder) : base(coder)
    { }

    public BarChartViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    { }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Bar Chart";
        
        Options = new(string key, string label)[]
        {
            ("toggleValues", "Toggle Values"),
            ("toggleIcons", "Toggle Icons"),
            ("toggleHighlight", "Toggle Highlight"),
            ("animateX", "Animate X"),
            ("animateY", "Animate Y"),
            ("animateXY", "Animate XY"),
            ("saveToGallery", "Save to Camera Roll"),
            ("togglePinchZoom", "Toggle PinchZoom"),
            ("toggleAutoScaleMinMax", "Toggle auto scale min/max"),
            ("toggleData", "Toggle Data"),
            ("toggleBarBorders", "Show Bar Borders")
        };
        
        SetupBarLineChartView(ChartView);

        ChartView.DrawBarShadowEnabled = false;
        ChartView.DrawValueAboveBarEnabled = true;

        ChartView.MaxVisibleCount = 60;

        var xAxis = ChartView.XAxis;
        xAxis.LabelPosition = XAxisLabelPosition.Bottom;
        xAxis.LabelFont = UIFont.SystemFontOfSize(10);
        xAxis.DrawGridLinesEnabled = false;
        xAxis.Granularity = 1; // only intervals of 1 day
        xAxis.LabelCount = 7;
        xAxis.ValueFormatter = new DayAxisValueFormatter(ChartView);

        var leftAxisFormatter = new NSNumberFormatter();
        leftAxisFormatter.MinimumFractionDigits = 0;
        leftAxisFormatter.MaximumFractionDigits = 1;
        leftAxisFormatter.NegativeSuffix = " $";
        leftAxisFormatter.PositiveSuffix = " $";
    
        var leftAxis = ChartView.LeftAxis;
        leftAxis.LabelFont = UIFont.SystemFontOfSize(10);
        leftAxis.LabelCount = 8;
        leftAxis.ValueFormatter = new ChartDefaultAxisValueFormatter(leftAxisFormatter);
        leftAxis.LabelPosition = YAxisLabelPosition.OutsideChart;
        leftAxis.SpaceTop = 0.15f;
        leftAxis.AxisMinimum = 0; // this replaces startAtZero = YES
        
        var rightAxis = ChartView.RightAxis;
        rightAxis.Enabled = true;
        rightAxis.DrawGridLinesEnabled = false;
        rightAxis.LabelFont = UIFont.SystemFontOfSize(10);
        rightAxis.LabelCount = 8;
        rightAxis.ValueFormatter = leftAxis.ValueFormatter;
        rightAxis.SpaceTop = 0.15f;
        rightAxis.AxisMinimum = 0; // this replaces startAtZero = YES
        
        var l = ChartView.Legend;
        l.HorizontalAlignment = ChartLegendHorizontalAlignment.Left;
        l.VerticalAlignment = ChartLegendVerticalAlignment.Bottom;
        l.Orientation = ChartLegendOrientation.Horizontal;
        l.DrawInside = false;
        l.Form = ChartLegendForm.Square;
        l.FormSize = 9;
        l.Font = UIFont.FromName("HelveticaNeue-Light", 11);
        l.XEntrySpace = 4;
        
        // TODO: marker here
    
        SliderX.Value = 12;
        SliderY.Value = 50;
        SlidersValueChanged(null);
    }

    protected override void UpdateChartData()
    {
        if (ShouldHideData)
        {
            ChartView.Data = null;
            return;
        }
        
        SetDataCount((int)SliderX.Value + 1, SliderY.Value);
    }

    private void SetDataCount(int count, double range)
    {
        var start = 1;
        var yVals = new List<ChartDataEntry>();
        for (var i = start; i < start + count + 1; i++)
        {
            var mult = range + 1;
            var val = new Random().Next((int)mult);
            if (new Random().Next(100) < 25)
                yVals.Add(new BarChartDataEntry(i, val, UIImage.FromBundle("icon")));
            else
                yVals.Add(new BarChartDataEntry(i, val));
        }

        BarChartDataSet set1 = null;
        if (ChartView.Data?.DataSetCount > 0)
        {
            // _HOTFIX: looks like GC collects class reference with time and makes right cast impossible.
            // Maybe binding issue, review later.
            // see: https://github.com/xamarin/xamarin-macios/issues/3887.
            set1 = Runtime.GetINativeObject<BarChartDataSet>(ChartView.Data.DataSets[0].Handle, false);
            set1!.ReplaceEntries(yVals.ToArray());
            ChartView.Data.NotifyDataChanged();
            ChartView.NotifyDataSetChanged();
        }
        else
        {
            set1 = new BarChartDataSet(yVals.ToArray(), "The year 2017");
            // _HOTFIX: looks like SetColors(UIColor[] colors) is missing
            set1.SetColors(ChartColorTemplates.Material, 1);
            set1.DrawIconsEnabled = false;

            var dataSets = new IChartDataSetProtocol[] { set1 };

            var data = new BarChartData(dataSets);
            data.SetValueFont(UIFont.FromName("HelveticaNeue-Light", 10));

            data.BarWidth = 0.9;

            ChartView.Data = data;
        }
    }

    protected override void OptionTapped(string key) =>
        HandleOption(key, ChartView);

    #region Actions

    partial void SlidersValueChanged(NSObject sender)
    {
        SliderTextX.Text = ((int)SliderX.Value).ToString(CultureInfo.InvariantCulture);
        SliderTextY.Text = ((int)SliderY.Value).ToString(CultureInfo.InvariantCulture);
        
        UpdateChartData();
    }
    
    #endregion
    
    partial void OptionsButtonTapped(NSObject sender) =>
        OptionsButtonTappedHandler(sender);
}