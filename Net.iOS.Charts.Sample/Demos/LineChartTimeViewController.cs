using System.Globalization;
using Net.iOS.Charts.Sample.Formatters;
using ObjCRuntime;

namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(LineChartTimeViewController))]
public sealed partial class LineChartTimeViewController : DemoBaseViewController, IChartViewDelegate
{
    public LineChartTimeViewController()
    { }
    
    public LineChartTimeViewController(NSCoder coder) : base(coder)
    { }

    public LineChartTimeViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    { }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Line Chart 1";
        
        Options = new(string key, string label)[]
        {
            ("toggleValues", "Toggle Values"),
            ("toggleFilled", "Toggle Filled"),
            ("toggleCircles", "Toggle Circles"),
            ("toggleCubic", "Toggle Cubic"),
            ("toggleHorizontalCubic", "Toggle Horizontal Cubic"),
            ("toggleStepped", "Toggle Stepped"),
            ("toggleHighlight", "Toggle Highlight"),
            ("animateX", "Animate X"),
            ("animateY", "Animate Y"),
            ("animateXY", "Animate XY"),
            ("saveToGallery", "Save to Camera Roll"),
            ("togglePinchZoom", "Toggle PinchZoom"),
            ("toggleAutoScaleMinMax", "Toggle auto scale min/max"),
            ("toggleData", "Toggle Data")
        };

        ChartView.Delegate = this;
        
        ChartView.ChartDescription.Enabled = false;
        
        ChartView.DragEnabled = true;
        ChartView.SetScaleEnabled(true);
        ChartView.PinchZoomEnabled = false;
        ChartView.DrawGridBackgroundEnabled = false;
        ChartView.HighlightPerDragEnabled = true;

        ChartView.BackgroundColor = UIColor.White;

        ChartView.Legend.Enabled = false;

        var xAxis = ChartView.XAxis;
        xAxis.LabelPosition = XAxisLabelPosition.TopInside;
        xAxis.LabelFont = UIFont.FromName("HelveticaNeue-Light", 10);
        xAxis.LabelTextColor = UIColor.FromRGBA(255 / 255f, 192 / 255f, 56 / 255f, 1);
        xAxis.DrawAxisLineEnabled = false;
        xAxis.DrawGridLinesEnabled = true;
        xAxis.CenterAxisLabelsEnabled = true;
        xAxis.Granularity = 3600;
        xAxis.ValueFormatter = new DateValueFormatter();

        var leftAxis = ChartView.LeftAxis;
        leftAxis.LabelPosition = YAxisLabelPosition.InsideChart;
        leftAxis.LabelFont = UIFont.FromName("HelveticaNeue-Light", 12);
        leftAxis.LabelTextColor = UIColor.FromRGBA(51/255f, 181 / 255f, 229 / 255f, 1);
        leftAxis.DrawGridLinesEnabled = true;
        leftAxis.GranularityEnabled = true;
        leftAxis.AxisMinimum = 0;
        leftAxis.AxisMaximum = 170;
        leftAxis.YOffset = -9;
        leftAxis.LabelTextColor = UIColor.FromRGBA(255 / 255f, 192 / 255f, 56 / 255f, 1);

        ChartView.RightAxis.Enabled = false;

        ChartView.Legend.Form = ChartLegendForm.Line;

        SliderX.Value = 100;
        SlidersValueChanged(null);
    }

    protected override void UpdateChartData()
    {
        if (ShouldHideData)
        {
            ChartView.Data = null;
            return;
        }
        
        SetDataCount((int)SliderX.Value, 30);
    }

    private void SetDataCount(int count, double range)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hourSeconds = 3600;
    
        var values = new List<ChartDataEntry>();

        var from = now - (count / 2f) * hourSeconds;
        var to = now + (count / 2f) * hourSeconds;
    
        for (var x = from; x < to; x += hourSeconds)
        {
            double y = new Random().Next((int)range) + 50;
            values.Add(new ChartDataEntry(x, y));
        }

        LineChartDataSet set1;
        if (ChartView.Data?.DataSetCount > 0)
        {
            set1 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[0].Handle, false)!;
            set1.ReplaceEntries(values.ToArray());
            ChartView.Data.NotifyDataChanged();
            ChartView.NotifyDataSetChanged();
        }
        else
        {
            set1 = new LineChartDataSet(values.ToArray(), "DataSet 1");
            set1.AxisDependency = AxisDependency.Left;
            set1.ValueTextColor = UIColor.FromRGBA(51 / 255f, 181 / 255f, 229 / 255f, 1);
            set1.LineWidth = 1.5f;
            set1.DrawCirclesEnabled = false;
            set1.DrawValuesEnabled = false;
            set1.FillAlpha = 0.26f;
            set1.FillColor = UIColor.FromRGBA(51 / 255f, 181 / 255f, 229 / 255f, 1);
            set1.HighlightColor = UIColor.FromRGBA(224 / 255, 117 / 255f, 117 / 255f, 1);
            set1.DrawCircleHoleEnabled = false;

            var dataSets = new IChartDataSetProtocol[] { set1 };

            var data = new LineChartData(dataSets);
            
            data.SetValueTextColor(UIColor.White);
            data.SetValueFont(UIFont.FromName("HelveticaNeue-Light", 9));
        
            ChartView.Data = data;
        }
    }

    protected override void OptionTapped(string key)
    {
        if (key == "toggleFilled")
        {
            foreach (var chartDataSetProtocol in ChartView.Data!.DataSets)
            {
                var set = (ILineChartDataSetProtocol)chartDataSetProtocol;
                set.DrawFilledEnabled = !set.IsDrawFilledEnabled;
            }

            ChartView.SetNeedsDisplay();
            return;
        }

        if (key == "toggleCircles")
        {
            foreach (var chartDataSetProtocol in ChartView.Data!.DataSets)
            {
                var set = (ILineChartDataSetProtocol)chartDataSetProtocol;
                set.DrawCirclesEnabled = !set.IsDrawCirclesEnabled;
            }

            ChartView.SetNeedsDisplay();
            return;
        }

        if (key == "toggleCubic")
        {
            foreach (var chartDataSetProtocol in ChartView.Data!.DataSets)
            {
                var set = (ILineChartDataSetProtocol)chartDataSetProtocol;
                set.Mode = set.Mode == LineChartMode.CubicBezier ? LineChartMode.Linear : LineChartMode.CubicBezier;
            }

            ChartView.SetNeedsDisplay();
            return;
        }

        if (key == "toggleStepped")
        {
            foreach (var chartDataSetProtocol in ChartView.Data!.DataSets)
            {
                var set = (ILineChartDataSetProtocol)chartDataSetProtocol;
                switch (set.Mode)
                {
                    case LineChartMode.Linear:
                    case LineChartMode.CubicBezier:
                    case LineChartMode.HorizontalBezier:
                        set.Mode = LineChartMode.Stepped;
                        break;
                    case LineChartMode.Stepped:
                        set.Mode = LineChartMode.Linear;
                        break;
                }

                ChartView.SetNeedsDisplay();
            }
        }
        
        if (key == "toggleHorizontalCubic")
        {
            foreach (var chartDataSetProtocol in ChartView.Data!.DataSets)
            {
                var set = (ILineChartDataSetProtocol)chartDataSetProtocol;
                set.Mode = set.Mode == LineChartMode.CubicBezier
                    ? LineChartMode.HorizontalBezier
                    : LineChartMode.CubicBezier;
            }

            ChartView.SetNeedsDisplay();
            return;
        }
            
        HandleOption(key, ChartView);
    }

    #region Actions

    partial void SlidersValueChanged(NSObject sender)
    {
        SliderTextX.Text = ((int)SliderX.Value).ToString(CultureInfo.InvariantCulture);
        
        UpdateChartData();
    }
    
    #endregion
    
    partial void OptionsButtonTapped(NSObject sender) =>
        OptionsButtonTappedHandler(sender);
}