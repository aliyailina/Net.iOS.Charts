using System.Globalization;
using ObjCRuntime;

namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(LineChart2ViewController))]
public sealed partial class LineChart2ViewController : DemoBaseViewController, IChartViewDelegate
{
    public LineChart2ViewController()
    { }
    
    public LineChart2ViewController(NSCoder coder) : base(coder)
    { }

    public LineChart2ViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    { }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Line Chart 2";
        
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
        ChartView.PinchZoomEnabled = true;
        ChartView.DrawGridBackgroundEnabled = false;

        ChartView.BackgroundColor = UIColor.FromWhiteAlpha(204 / 255f, 1);

        var l = ChartView.Legend;
        l.Form = ChartLegendForm.Line;
        l.Font = UIFont.FromName("HelveticaNeue-Light", 11);
        l.TextColor = UIColor.White;
        l.HorizontalAlignment = ChartLegendHorizontalAlignment.Left;
        l.VerticalAlignment = ChartLegendVerticalAlignment.Bottom;
        l.Orientation = ChartLegendOrientation.Horizontal;
        l.DrawInside = false;

        var xAxis = ChartView.XAxis;
        xAxis.LabelFont = UIFont.SystemFontOfSize(11);
        xAxis.LabelTextColor = UIColor.White;
        xAxis.DrawGridLinesEnabled = false;
        xAxis.DrawAxisLineEnabled = false;

        var leftAxis = ChartView.LeftAxis;
        leftAxis.LabelTextColor = UIColor.FromRGBA(51 / 255f, 181 / 255f, 229 / 255f, 1);
        leftAxis.AxisMaximum = 200;
        leftAxis.AxisMinimum = 0;
        leftAxis.DrawGridLinesEnabled = true;
        leftAxis.DrawZeroLineEnabled = false;
        leftAxis.GranularityEnabled = true;

        var rightAxis = ChartView.RightAxis;
        rightAxis.LabelTextColor = UIColor.Red;
        rightAxis.AxisMaximum = 900;
        rightAxis.AxisMinimum = -200;
        rightAxis.DrawGridLinesEnabled = false;
        rightAxis.GranularityEnabled = false;

        SliderX.Value = 20;
        SliderY.Value = 30;
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
        
        SetDataCount((int)SliderX.Value + 1, SliderY.Value);
    }

    private void SetDataCount(int count, double range)
    {
        var yVals1 = new List<ChartDataEntry>();
        var yVals2 = new List<ChartDataEntry>();
        var yVals3 = new List<ChartDataEntry>();
        
        for (int i = 0; i < count; i++)
        {
            var mult = range / 2f;
            var val = new Random().Next((int)mult) + 50;
            yVals1.Add(new ChartDataEntry(i, val));
        }
        
        for (int i = 0; i < count - 1; i++)
        {
            var mult = range;
            var val = new Random().Next((int)mult) + 450;
            yVals2.Add(new ChartDataEntry(i, val));
        }
        
        for (int i = 0; i < count; i++)
        {
            var mult = range;
            var val = new Random().Next((int)mult) + 500;
            yVals3.Add(new ChartDataEntry(i, val));
        }

        LineChartDataSet set1, set2, set3;
        
        if (ChartView.Data?.DataSetCount > 0)
        {
            // _HOTFIX: looks like GC collects class reference with time and makes right cast impossible.
            // Maybe binding issue, review later.
            // see: https://github.com/xamarin/xamarin-macios/issues/3887.
            set1 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[0].Handle, false)!;
            set2 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[1].Handle, false)!;
            set3 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[2].Handle, false)!;
        
            set1.ReplaceEntries(yVals1.ToArray());
            set2.ReplaceEntries(yVals2.ToArray());
            set3.ReplaceEntries(yVals3.ToArray());
            ChartView.Data.NotifyDataChanged();
            ChartView.NotifyDataSetChanged();
        }
        else
        {
            set1 = new LineChartDataSet(yVals1.ToArray(), "DataSet 1");
            set1.AxisDependency = AxisDependency.Left;
            set1.SetColor(UIColor.FromRGBA(51 / 255f, 181 / 255f, 229 / 255f, 1));
            set1.SetCircleColor(UIColor.White);
            set1.LineWidth = 2;
            set1.CircleRadius = 3;
            set1.FillAlpha = 65 / 255f;
            set1.FillColor = UIColor.FromRGBA(51 / 255f, 181 / 255f, 229 / 255f, 1);
            set1.HighlightColor = UIColor.FromRGBA(244 / 255f, 117 / 255f, 117 / 255f, 1);
            set1.DrawCircleHoleEnabled = false;
            
            set2 = new LineChartDataSet(yVals2.ToArray(), "DataSet 2");
            set2.AxisDependency = AxisDependency.Right;
            set2.SetColor(UIColor.Red);
            set2.SetCircleColor(UIColor.White);
            set2.LineWidth = 2;
            set2.CircleRadius = 3;
            set2.FillAlpha = 65 / 255f;
            set2.FillColor = UIColor.Red;
            set2.HighlightColor = UIColor.FromRGBA(244 / 255f, 117 / 255f, 117 / 255f, 1);
            set2.DrawCircleHoleEnabled = false;
            
            set3 = new LineChartDataSet(yVals3.ToArray(), "DataSet 3");
            set3.AxisDependency = AxisDependency.Right;
            set3.SetColor(UIColor.Yellow);
            set3.SetCircleColor(UIColor.White);
            set3.LineWidth = 2;
            set3.CircleRadius = 3;
            set3.FillAlpha = 65 / 255f;
            set3.FillColor = UIColor.Yellow.ColorWithAlpha(200 / 255f);
            set3.HighlightColor = UIColor.FromRGBA(244 / 255f, 117 / 255f, 117 / 255f, 1);
            set3.DrawCircleHoleEnabled = false;

            var dataSets = new IChartDataSetProtocol[] { set1, set2, set3 };
            var data = new LineChartData(dataSets);
            data.SetValueTextColor(UIColor.White);
            data.SetValueFont(UIFont.SystemFontOfSize(9));
            
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
        SliderTextY.Text = ((int)SliderY.Value).ToString(CultureInfo.InvariantCulture);
        
        UpdateChartData();
    }
    
    #endregion
    
    #region ChartViewDelegate

    [Export("chartValueSelected:entry:highlight:")]
    public void ChartValueSelected(ChartViewBase chartView, ChartDataEntry entry, ChartHighlight highlight)
    {
        ChartView.CenterViewToAnimatedWithXValue(entry.X, entry.Y,
            ChartView.Data!.DataSetAtIndex(highlight.DataSetIndex)!.AxisDependency, 1);
        // ChartView.MoveViewToAnimatedWithXValue(entry.X, entry.Y,
        //     ChartView.Data.DataSetAtIndex(highlight.DataSetIndex)!.AxisDependency, 1);
        // ChartView.ZoomAndCenterViewAnimatedWithScaleX(1.8f, 1.8f, entry.X, entry.Y,
        //     ChartView.Data.DataSetAtIndex(highlight.DataSetIndex)!.AxisDependency, 1);
    }
    
    #endregion
    
    partial void OptionsButtonTapped(NSObject sender) =>
        OptionsButtonTappedHandler(sender);
}