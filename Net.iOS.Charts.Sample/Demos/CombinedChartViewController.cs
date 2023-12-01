namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(CombinedChartViewController))]
public sealed partial class CombinedChartViewController : DemoBaseViewController, IChartViewDelegate,
    IChartAxisValueFormatter
{
    private const int ItemCount = 12;
    
    private string[] _months;
    public CombinedChartViewController()
    { }
    
    public CombinedChartViewController(NSCoder coder) : base(coder)
    { }

    public CombinedChartViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    { }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Combined chart";
        
        Options = new(string key, string label)[]
        {
            ("toggleLineValues", "Toggle Line Values"),
            ("toggleBarValues", "Toggle Bar Values"),
            ("saveToGallery", "Save to Camera Roll"),
            ("toggleData", "Toggle Data"),
            ("removeDataSet", "Remove random set")
        };
        
        _months = new[]
        {
            "Jan", "Feb", "Mar",
            "Apr", "May", "Jun",
            "Jul", "Aug", "Sep",
            "Oct", "Nov", "Dec"
        };
    
        ChartView.Delegate = this;
    
        ChartView.ChartDescription.Enabled = false;
    
        ChartView.DrawGridBackgroundEnabled = false;
        ChartView.DrawBarShadowEnabled = false;
        ChartView.HighlightFullBarEnabled = false;

        ChartView.DrawOrder = new NSNumber[]
        {
            (int)CombinedChartDrawOrder.Bar,
            // TODO: add (int)CombinedChartDrawOrder.Bubble,
            // TODO: add (int)CombinedChartDrawOrder.Candle,
            (int)CombinedChartDrawOrder.Line,
            // TODO: add (int)CombinedChartDrawOrder.Scatter
        };
    
    
        var l = ChartView.Legend;
        l.WordWrapEnabled = true;
        l.HorizontalAlignment = ChartLegendHorizontalAlignment.Center;
        l.VerticalAlignment = ChartLegendVerticalAlignment.Bottom;
        l.Orientation = ChartLegendOrientation.Horizontal;
        l.DrawInside = false;
    
        var rightAxis = ChartView.RightAxis;
        rightAxis.DrawGridLinesEnabled = false;
        rightAxis.AxisMinimum = 0; // this replaces startAtZero = YES
    
        var leftAxis = ChartView.LeftAxis;
        leftAxis.DrawGridLinesEnabled = false;
        leftAxis.AxisMinimum = 0; // this replaces startAtZero = YES
    
        var xAxis = ChartView.XAxis;
        xAxis.LabelPosition = XAxisLabelPosition.BothSided;
        xAxis.AxisMinimum = 0;
        xAxis.Granularity = 1;
        xAxis.ValueFormatter = this;
    
        UpdateChartData();
    }

    protected override void UpdateChartData()
    {
        if (ShouldHideData)
        {
            ChartView.Data = null;
            return;
        }

        SetChartData();
    }

    private void SetChartData()
    {
        var data = new CombinedChartData();
        data.LineData = GenerateLineData();
        data.BarData = GenerateBarData();
        // TODO: add data.BubbleData = GenerateBubbleData();
        // TODO: add data.ScatterData = GenerateScatterData();
        // TODO: add data.CandleData = GenerateCandleData();
    
        ChartView.XAxis.AxisMaximum = data.XMax + 0.25;

        ChartView.Data = data;
    }

    protected override void OptionTapped(string key)
    {
        if (key == "toggleLineValues")
        {
            foreach(var set in ChartView.Data!.DataSets)
            {
                if (set is LineChartDataSet lineSet)
                {
                    lineSet.DrawValuesEnabled = !set.IsDrawValuesEnabled;
                }
            }
        
            ChartView.SetNeedsDisplay();
            return;
        }
        
        if (key == "toggleBarValues")
        {
            foreach(var set in ChartView.Data!.DataSets)
            {
                if (set is BarChartDataSet lineSet)
                {
                    lineSet.DrawValuesEnabled = !set.IsDrawValuesEnabled;
                }
            }
        
            ChartView.SetNeedsDisplay();
            return;
        }
        
        if (key == "removeDataSet")
        {
            var rnd = new Random().Next((int)ChartView.Data!.DataSetCount);
            ChartView.Data.RemoveDataSet(ChartView.Data.DataSetAtIndex(rnd)!);
            ChartView.Data.NotifyDataChanged();
            ChartView.NotifyDataSetChanged();
            return;
        }
        
        HandleOption(key, ChartView);
    }
    
    private LineChartData GenerateLineData()
    {
        var d = new LineChartData();
    
        var entries = new List<ChartDataEntry>();
    
        for (int index = 0; index < ItemCount; index++) 
        {
            entries.Add(new ChartDataEntry(index + 0.5, new Random().Next(15) + 5));
        }
        
        var set = new LineChartDataSet(entries.ToArray(), "Line DataSet");
        set.SetColor(UIColor.FromRGBA(240 / 255f, 238 / 255f, 70 / 255f, 1)); 
        set.LineWidth = 2.5f; 
        set.SetCircleColor(UIColor.FromRGBA(240 / 255f, 238 / 255f, 70 / 255f, 1)); 
        set.CircleRadius = 5; 
        set.CircleHoleRadius = 2.5f; 
        set.FillColor = UIColor.FromRGBA(240 / 255f, 238 / 255f, 70 / 255f, 1); 
        set.Mode = LineChartMode.CubicBezier; 
        set.DrawValuesEnabled = true; 
        set.ValueFont = UIFont.SystemFontOfSize(10); 
        set.ValueTextColor = UIColor.FromRGBA(240 / 255f, 238 / 255f, 70 / 255f, 1);
        set.AxisDependency = AxisDependency.Left;
        d.AddDataSet(set);
        
        return d;
    }

    private BarChartData GenerateBarData()
    {
        var entries1 = new List<BarChartDataEntry>();
        var entries2 = new List<BarChartDataEntry>();
    
        for (int index = 0; index < ItemCount; index++)
        {
            entries1.Add(new BarChartDataEntry(0, new Random().Next(25) + 25));
        
            // stacked
            entries2.Add(new BarChartDataEntry(0, new NSNumber[] { new Random().Next(13) + 12, new Random().Next(13) + 12 }));
        }

        var set1 = new BarChartDataSet(entries1.ToArray<ChartDataEntry>(), "Bar 1");
        set1.SetColor(UIColor.FromRGBA(60 / 255f, 220 / 255f, 78 / 255f, 1));
        set1.ValueTextColor = UIColor.FromRGBA(60 / 255f, 220/255f, 78 / 255f, 1);
        set1.ValueFont = UIFont.SystemFontOfSize(10);
        set1.AxisDependency = AxisDependency.Right;
    
        var set2 = new BarChartDataSet(entries2.ToArray<ChartDataEntry>(), "");
        set2.StackLabels = new [] { "Stack 1", "Stack 2" };
        set2.Colors = new[]
        {
            UIColor.FromRGBA(61 / 255f, 165 / 255f, 255 / 255f, 1),
            UIColor.FromRGBA(23 / 255f, 197 / 255f, 255 / 255f, 1)
        };
        set2.ValueTextColor = UIColor.FromRGBA(61 / 255f, 165 / 255f, 255 / 255f, 1);
        set2.ValueFont = UIFont.SystemFontOfSize(10);
        set2.AxisDependency = AxisDependency.Right;

        const float groupSpace = 0.06f;
        const float barSpace = 0.02f; // x2 dataset
        const float barWidth = 0.45f; // x2 dataset
        // (0.45 + 0.02) * 2 + 0.06 = 1.00 -> interval per "group"
    
        var d = new BarChartData(new IChartDataSetProtocol[]{set1, set2});
        d.BarWidth = barWidth;
    
        // make this BarData object grouped
        d.GroupBarsFromX(0, groupSpace, barSpace); // start at x = 0
    
        return d;
    }
    
    // TODO: add GenerateScatterData()
    // TODO: add GenerateCandleData()
    // TODO: add GenerateBubbleData()
    
    partial void OptionsButtonTapped(NSObject sender) =>
        OptionsButtonTappedHandler(sender);

    #region AxisValueFormatter
    
    public string StringForValue(double value, ChartAxisBase axis) =>
        _months[(int)value % _months.Length];
    
    #endregion
}