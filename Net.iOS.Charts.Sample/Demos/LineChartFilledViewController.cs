using System.Globalization;
using ObjCRuntime;

namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(LineChartFilledViewController))]
public sealed partial class LineChartFilledViewController : DemoBaseViewController, IChartViewDelegate
{
    public LineChartFilledViewController()
    { }
    
    public LineChartFilledViewController(NSCoder coder) : base(coder)
    { }

    public LineChartFilledViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    { }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Filled Line Chart";

        ChartView.Delegate = this;
        
        ChartView.BackgroundColor = UIColor.White;
        ChartView.GridBackgroundColor = UIColor.FromRGBA(51 / 255f, 181 / 255f, 229 / 255f, 1);
        ChartView.DrawGridBackgroundEnabled = true;

        ChartView.DrawBordersEnabled = true;

        ChartView.ChartDescription.Enabled = false;

        ChartView.PinchZoomEnabled = false;
        ChartView.DragEnabled = true;
        ChartView.SetScaleEnabled(true);

        var l = ChartView.Legend;
        l.Enabled = false;

        var xAxis = ChartView.XAxis;
        xAxis.Enabled = false;

        var leftAxis = ChartView.LeftAxis;
        leftAxis.AxisMaximum = 900;
        leftAxis.AxisMinimum = -250;
        leftAxis.DrawAxisLineEnabled = false;
        leftAxis.DrawZeroLineEnabled = false;
        leftAxis.DrawGridLinesEnabled = false;

        ChartView.RightAxis.Enabled = false;

        SliderX.Value = 100;
        SliderX.Value = 60;
        SlidersValueChanged(null);
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
        var yVals1 = new List<ChartDataEntry>(); 
        var yVals2 = new List<ChartDataEntry>();;
        
        for (int i = 0; i < count; i++)
        {
            var val = new Random().Next((int)range) + 50;
            yVals1.Add(new ChartDataEntry(i, val));
        }
        
        for (int i = 0; i < count; i++)
        {
            var val = new Random().Next((int)range) + 450;
            yVals2.Add(new ChartDataEntry(i, val));
        }
        
        LineChartDataSet set1;
        LineChartDataSet set2;
    
        if (ChartView.Data?.DataSetCount > 0)
        {
            set1 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[0].Handle, false);
            set2 = Runtime.GetINativeObject<LineChartDataSet>(ChartView.Data.DataSets[1].Handle, false);
        
            set1!.ReplaceEntries(yVals1.ToArray());
            set2!.ReplaceEntries(yVals2.ToArray());

            ChartView.Data.NotifyDataChanged();
            ChartView.NotifyDataSetChanged();
        }
        else
        {
            set1 = new LineChartDataSet(yVals1.ToArray(), "DataSet 1");
            set1.AxisDependency = AxisDependency.Left;
            set1.SetColor(UIColor.FromRGBA(255 / 255f, 241 / 255f, 46 / 255f, 1));
            set1.DrawCirclesEnabled = false;
            set1.LineWidth = 2;
            set1.CircleRadius = 3;
            set1.FillAlpha = 1;
            set1.DrawFilledEnabled = true;
            set1.FillColor = UIColor.White;
            set1.HighlightColor = UIColor.FromRGBA(244/255f, 117/255f, 117/255f, 1);
            set1.DrawCircleHoleEnabled = true;
            set1.FillFormatter = ChartDefaultFillFormatter.WithBlock((_, _) => (nfloat)ChartView.LeftAxis.AxisMinimum);
        
            set2 = new LineChartDataSet(yVals2.ToArray(), "DataSet 2");
            set2.AxisDependency = AxisDependency.Left;
            set2.SetColor(UIColor.FromRGBA(255 / 255f, 241 / 255f, 46 / 255f, 1));
            set2.DrawCirclesEnabled = false;
            set2.LineWidth = 2;
            set2.CircleRadius = 3;
            set2.FillAlpha = 1;
            set2.DrawFilledEnabled = true;
            set2.FillColor = UIColor.White;
            set2.HighlightColor = UIColor.FromRGBA(244/255f, 117/255f, 117/255f, 1);
            set2.DrawCircleHoleEnabled = true;
            set2.FillFormatter = ChartDefaultFillFormatter.WithBlock((_, _) => (nfloat)ChartView.LeftAxis.AxisMaximum);

            var dataSets = new IChartDataSetProtocol[] { set1, set2 };
            var data = new LineChartData(dataSets);
            data.SetDrawValues(false);
            ChartView.Data = data;
        }
    }

    #region Actions

    partial void SlidersValueChanged(NSObject sender)
    {
        SliderTextX.Text = ((int)SliderX.Value).ToString(CultureInfo.InvariantCulture);
        SliderTextY.Text = ((int)SliderY.Value).ToString(CultureInfo.InvariantCulture);
        
        UpdateChartData();
    }
    
    #endregion
}