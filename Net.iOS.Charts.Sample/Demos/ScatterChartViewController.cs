using System.Globalization;

namespace Net.iOS.Charts.Sample.Demos;

[Register(nameof(ScatterChartViewController))]
public sealed partial class ScatterChartViewController : DemoBaseViewController, IChartViewDelegate
{
    public ScatterChartViewController()
    { }
    
    public ScatterChartViewController(NSCoder coder) : base(coder)
    { }

    public ScatterChartViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    { }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Line Chart 1";
        
        Options = new(string key, string label)[]
        {
            ("toggleValues", "Toggle Values"),
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
        
        ChartView.DrawGridBackgroundEnabled = false;
        ChartView.DragEnabled = true;
        ChartView.SetScaleEnabled(true);
        ChartView.MaxVisibleCount = 200;
        ChartView.PinchZoomEnabled = true;
        
        var l = ChartView.Legend;
        l.HorizontalAlignment = ChartLegendHorizontalAlignment.Right;
        l.VerticalAlignment = ChartLegendVerticalAlignment.Top;
        l.Orientation = ChartLegendOrientation.Vertical;
        l.DrawInside = false;
        l.Font = UIFont.FromName("HelveticaNeue-Light", 10);
        l.XOffset = 5;
        
        var yl = ChartView.LeftAxis;
        yl.LabelFont = UIFont.FromName("HelveticaNeue-Light", 10);
        yl.AxisMinimum = 0; // this replaces startAtZero = YES
           
        ChartView.RightAxis.Enabled = false;
           
        var xl = ChartView.XAxis;
        xl.LabelFont = UIFont.FromName("HelveticaNeue-Light", 10);
        xl.DrawGridLinesEnabled = false;
           
        SliderX.Value = 45;
        SliderY.Value = 100;
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
         var yVals1 = new List<ChartDataEntry>(); 
         var yVals2 = new List<ChartDataEntry>();
         var yVals3 = new List<ChartDataEntry>();
           
         for (int i = 0; i < count; i++)
         {
             var val = (double) new Random().Next((int)range) + 3;
             yVals1.Add(new ChartDataEntry(i, val));
           
             val = (double) new Random().Next((int)range) + 3;
             yVals2.Add(new ChartDataEntry(i + 0.33, val));
           
             val = (double) new Random().Next((int)range) + 3;
             yVals3.Add(new ChartDataEntry(i + 0.66, val));
         }
           
         var set1 = new ScatterChartDataSet(yVals1.ToArray(), "DS 1");
         
         set1.SetScatterShape(ScatterShape.Square);
         set1.SetColor(ChartColorTemplates.Colorful[0]);
           
         var set2 = new ScatterChartDataSet(yVals2.ToArray(), "DS 2");
         set2.SetScatterShape(ScatterShape.Circle);
         set2.ScatterShapeHoleColor = ChartColorTemplates.Colorful[3];
         set2.ScatterShapeHoleRadius = 3.5f;
         set2.SetColor(ChartColorTemplates.Colorful[1]);
           
         var set3 = new ScatterChartDataSet(yVals3.ToArray(), "DS 3");
         set3.SetScatterShape(ScatterShape.Cross);
         set3.SetColor(ChartColorTemplates.Colorful[2]);
           
         set1.ScatterShapeSize = 8;
         set2.ScatterShapeSize = 8;
         set3.ScatterShapeSize = 8;
           
         var dataSets = new IChartDataSetProtocol[] { set1, set2, set3 };

         var data = new ScatterChartData(dataSets);
         data.SetValueFont(UIFont.FromName("HelveticaNeue-Light", 7));
           
         ChartView.Data = data;
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