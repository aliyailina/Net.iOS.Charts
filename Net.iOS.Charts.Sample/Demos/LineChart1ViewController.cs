using System.Globalization;
using ObjCRuntime;

namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(LineChart1ViewController))]
public sealed partial class LineChart1ViewController : DemoBaseViewController, IChartViewDelegate
{
    public LineChart1ViewController()
    { }
    
    public LineChart1ViewController(NSCoder coder) : base(coder)
    { }

    public LineChart1ViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
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
            ("toggleIcons", "Toggle Icons"),
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
        ChartView.PinchZoomEnabled = true;
        ChartView.DrawGridBackgroundEnabled = false;

        var llXAxis = new ChartLimitLine(10, "Index 10");
        llXAxis.LineWidth = 4;
        llXAxis.LineDashLengths = new[] { new NSNumber(10), new NSNumber(10), new NSNumber(0) };
        llXAxis.LabelPosition = ChartLimitLabelPosition.RightBottom;
        llXAxis.ValueFont = UIFont.SystemFontOfSize(10);
        
        //ChartView.XAxis.AddLimitLine(llXAxis);

        ChartView.XAxis.GridLineDashLengths = new[] { new NSNumber(10), new NSNumber(10) };
        ChartView.XAxis.GridLineDashPhase = 0;

        var ll1 = new ChartLimitLine(150, "Upper Limit");
        ll1.LineWidth = 4;
        ll1.LineDashLengths = new[] { new NSNumber(5), new NSNumber(5) };
        ll1.LabelPosition = ChartLimitLabelPosition.RightTop;
        ll1.ValueFont = UIFont.SystemFontOfSize(10);
        
        var ll2 = new ChartLimitLine(-30, "Lower Limit");
        ll2.LineWidth = 4;
        ll2.LineDashLengths = new[] { new NSNumber(5), new NSNumber(5) };
        ll2.LabelPosition = ChartLimitLabelPosition.RightTop;
        ll2.ValueFont = UIFont.SystemFontOfSize(10);

        var leftAxis = ChartView.LeftAxis;
        leftAxis.RemoveAllLimitLines();
        leftAxis.AddLimitLine(ll1);
        leftAxis.AddLimitLine(ll2);
        leftAxis.AxisMaximum = 200;
        leftAxis.AxisMinimum = -50;
        leftAxis.GridLineDashLengths = new[] { new NSNumber(5), new NSNumber(5) };
        leftAxis.DrawZeroLineEnabled = false;
        leftAxis.DrawLimitLinesBehindDataEnabled = true;

        ChartView.RightAxis.Enabled = false;
        
        //ChartView.ViewPortHandler.SetMaximumScaleY(2);
        //ChartView.ViewPortHandler.SetMaximumScaleX(2);
        
        // TODO: marker here

        ChartView.Legend.Form = ChartLegendForm.Line;
        SliderX.Value = 45;
        SliderY.Value = 100;
        SlidersValueChanged(null);
        ChartView.AnimateWithXAxisDuration(2.5);
    }

    protected override void UpdateChartData()
    {
        if (ShouldHideData)
        {
            ChartView.Data = null;
            return;
        }
        
        SetDataCount((int)SliderX.Value, SliderY.Value);
    }

    private void SetDataCount(int count, double range)
    {
        var values = new List<ChartDataEntry>();
        for (int i = 0; i < count; i++)
        {
            var val = new Random().Next(0, (int)range);
            values.Add(new ChartDataEntry(i, val, UIImage.FromBundle("icon")));
        }

        LineChartDataSet set1;
        if (ChartView.Data?.DataSetCount > 0)
        {
            // _HOTFIX: looks like GC collects class reference with time and makes right cast impossible.
            // Maybe binding issue, review later.
            // see: https://github.com/xamarin/xamarin-macios/issues/3887.
            set1 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[0].Handle, false)!;
            set1.ReplaceEntries(values.ToArray());
            ChartView.Data.NotifyDataChanged();
            ChartView.NotifyDataSetChanged();
        }
        else
        {
            set1 = new LineChartDataSet(values.ToArray(), "DataSet 1");
            set1.DrawIconsEnabled = false;
            set1.LineDashLengths = new[] { new NSNumber(5), new NSNumber(2.5) };
            set1.HighlightLineDashLengths = new[] { new NSNumber(5), new NSNumber(2.5) };
            set1.SetColor(UIColor.Black);
            set1.SetCircleColor(UIColor.Black);
            set1.LineWidth = 1;
            set1.CircleRadius = 3;
            set1.DrawCircleHoleEnabled = false;
            set1.ValueFont = UIFont.SystemFontOfSize(9);
            set1.FormLineDashLengths = new[] { new NSNumber(5), new NSNumber(2.5) };
            set1.FormLineWidth = 1;
            set1.FormSize = 15;

            var gradientColors = new[]
            {
                ChartColorTemplates.ColorFromString("#00ff0000").CGColor,
                ChartColorTemplates.ColorFromString("#ffff0000").CGColor
            };
            
            var gradient = new CGGradient(null!, gradientColors);

            set1.FillAlpha = 1;
            set1.Fill = new ChartLinearGradientFill(gradient, 90);
            set1.DrawFilledEnabled = true;
            
            gradient.Dispose();

            var dataSets = new IChartDataSetProtocol[] { set1 };

            var data = new LineChartData(dataSets);
            
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
                set.Mode = set.Mode == LineChartMode.HorizontalBezier
                    ? LineChartMode.CubicBezier
                    : LineChartMode.HorizontalBezier;
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
        SliderTextY.Text = ((int)SliderY.Value).ToString(CultureInfo.InvariantCulture);
        
        UpdateChartData();
    }
    
    #endregion
    
    partial void OptionsButtonTapped(NSObject sender) =>
        OptionsButtonTappedHandler(sender);
}