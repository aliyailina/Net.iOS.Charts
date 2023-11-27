using CoreImage;

namespace Net.iOS.Charts.Sample;

public partial class DemoBaseViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
{
    protected UITableView OptionsTableView;
    protected bool ShouldHideData;
    protected string[] Parties;
    protected (string key, string label)[] Options { get; set; }

    protected DemoBaseViewController()
    { }
    
    protected DemoBaseViewController(NSCoder coder) : base(coder)
    {
        Initialize();
    }

    protected DemoBaseViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
    {
        Initialize();
    }

    private void Initialize()
    {
        EdgesForExtendedLayout = UIRectEdge.None;

        Parties = new[]
        {
            "Party A", "Party B", "Party C", "Party D", "Party E", "Party F",
            "Party G", "Party H", "Party I", "Party J", "Party K", "Party L",
            "Party M", "Party N", "Party O", "Party P", "Party Q", "Party R",
            "Party S", "Party T", "Party U", "Party V", "Party W", "Party X",
            "Party Y", "Party Z"
        };
    }

    protected virtual void OptionTapped(string key)
    { }

    protected void HandleOption(string key, ChartViewBase chartView)
    {
        if (key == "toggleValues")
        {
            foreach (var set in chartView.Data!.DataSets)
            {
                set.DrawValuesEnabled = !set.IsDrawValuesEnabled;
            }
            chartView.SetNeedsDisplay();
        }

        if (key == "toggleIcons")
        {
            foreach (var set in chartView.Data!.DataSets)
            {
                set.DrawIconsEnabled = !set.IsDrawIconsEnabled;
            }

            chartView.SetNeedsDisplay();
        }

        if (key == "toggleHighlight")
        {
            chartView.Data!.IsHighlightEnabled = !chartView.Data.IsHighlightEnabled;
            chartView.SetNeedsDisplay();
        }

        if (key == "animateX")
        {
            chartView.AnimateWithXAxisDuration(3.0);
        }
        
        if (key == "animateY")
        {
            chartView.AnimateWithYAxisDuration(3.0);
        }

        if (key == "animateXY")
        {
            chartView.AnimateWithXAxisDuration(3.0, yAxisDuration: 3.0);
        }

        if (key == "saveToGallery")
        {
            chartView.GetChartImageWithTransparent(false)!.SaveToPhotosAlbum(null);
        }

        if (key == "togglePinchZoom")
        {
            var barLineChart = (BarLineChartViewBase)chartView;
            barLineChart.PinchZoomEnabled = !barLineChart.IsPinchZoomEnabled;
            chartView.SetNeedsDisplay();
        }

        if (key == "toggleAutoScaleMinMax")
        {
            var barLineChart = (BarLineChartViewBase)chartView;
            barLineChart.AutoScaleMinMaxEnabled = !barLineChart.IsAutoScaleMinMaxEnabled;
            chartView.NotifyDataSetChanged();
        }

        if (key == "toggleData")
        {
            ShouldHideData = !ShouldHideData;
            UpdateChartData();
        }

        if (key == "toggleBarBorders")
        {
            foreach (var set in chartView.Data!.DataSets)
            {
                if (set is IBarChartDataSetProtocol barChartSet)
                {
                    barChartSet.BarBorderWidth = barChartSet.BarBorderWidth == 1 ? 0 : 1;
                }
            }

            chartView.SetNeedsDisplay();
        }
    }

    protected void OptionsButtonTappedHandler(NSObject sender)
    {
        if (OptionsTableView != null)
        {
            OptionsTableView.RemoveFromSuperview();
            OptionsTableView = null;
            return;
        }

        OptionsTableView = new UITableView
        {
            BackgroundColor = UIColor.FromWhiteAlpha(0, 0.9f),
            Delegate = this,
            DataSource = this
        };
        OptionsTableView.RegisterClassForCellReuse(typeof(UITableViewCell), "Cell");
        OptionsTableView.TranslatesAutoresizingMaskIntoConstraints = false;
        var constraints = new NSMutableArray<NSLayoutConstraint>();
        constraints.AddObjects(NSLayoutConstraint.Create(OptionsTableView, NSLayoutAttribute.Leading,
            NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 40));
        constraints.AddObjects(NSLayoutConstraint.Create(OptionsTableView, NSLayoutAttribute.Trailing,
            NSLayoutRelation.Equal, sender, NSLayoutAttribute.Trailing, 1, 0));
        constraints.AddObjects(NSLayoutConstraint.Create(OptionsTableView, NSLayoutAttribute.Top,
            NSLayoutRelation.Equal, sender, NSLayoutAttribute.Bottom, 1, 0));
        View!.AddSubview(OptionsTableView);
        View.AddConstraints(constraints.ToArray<NSLayoutConstraint>());
        OptionsTableView.AddConstraint(NSLayoutConstraint.Create(OptionsTableView, NSLayoutAttribute.Height,
            NSLayoutRelation.Equal, null, NSLayoutAttribute.Height, 1, 220));
    }
    
    #region UITableViewDataSource, UITableViewDelegate

    [Export("numberOfSectionsInTableView:")]
    public IntPtr NumberOfSections(UITableView tableView)
    {
        if (tableView.Equals(OptionsTableView))
            return 1;

        return 0;
    }
    
    public IntPtr RowsInSection(UITableView tableView, IntPtr section)
    {
        if (tableView.Equals(OptionsTableView))
            return Options.Length;

        return 0;
    }

    [Export("tableView:heightForRowAtIndexPath:")]
    public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
    {
        if (tableView.Equals(OptionsTableView))
            return 40;

        return 44;
    }

    public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell("Cell", indexPath);
        cell.BackgroundView = null;
        cell.BackgroundColor = UIColor.Clear;
#pragma warning disable CA1422
        cell.TextLabel.TextColor = UIColor.White;
        cell.TextLabel.Text = Options[indexPath.Row].label;
        return cell;
#pragma warning restore CA1422
    }
    
    [Export("tableView:didSelectRowAtIndexPath:")]
    public void SelectRow(UITableView tableView, NSIndexPath indexPath)
    {
        if (tableView.Equals(OptionsTableView))
        {
            tableView.DeselectRow(indexPath, true);

            if (OptionsTableView != null)
            {
                OptionsTableView.RemoveFromSuperview();
                OptionsTableView = null;
            }
            
            OptionTapped(Options[indexPath.Row].key);
        }
    }
    
    #endregion

    #region Stubs for chart view
    
    protected virtual void UpdateChartData()
    { }

    protected virtual void SetupPieChartView(PieChartView chartView)
    {
        chartView.UsePercentValuesEnabled = true;
        chartView.DrawSlicesUnderHoleEnabled = false;
        chartView.HoleRadiusPercent = 0.58f;
        chartView.TransparentCircleRadiusPercent = 0.61f;
        chartView.ChartDescription.Enabled = false;
        chartView.SetExtraOffsetsWithLeft(5, 10, 5, 5);
        chartView.DrawCenterTextEnabled = true;
        var paragraphStyle = NSParagraphStyle.Default.MutableCopy() as NSMutableParagraphStyle;
        paragraphStyle!.LineBreakMode = UILineBreakMode.TailTruncation;
        paragraphStyle.Alignment = UITextAlignment.Center;

        var centerText = new NSMutableAttributedString("Charts\\nby Daniel Cohen Gindi (.NET port by Aliya Ilina");
        centerText.SetAttributes(
            new NSMutableDictionary
            {
                { UIStringAttributeKey.Font, UIFont.FromName("HelveticaNeue-Light", 13) },
                { UIStringAttributeKey.ParagraphStyle, paragraphStyle }
            }, new NSRange(0, centerText.Length));

        centerText.AddAttributes(
            new NSMutableDictionary
            {
                { UIStringAttributeKey.Font, UIFont.FromName("HelveticaNeue-Light", 11) },
                { UIStringAttributeKey.ForegroundColor, UIColor.Gray }
            }, new NSRange(10, centerText.Length - 10));

        centerText.AddAttributes(
            new NSMutableDictionary
            {
                { UIStringAttributeKey.Font, UIFont.FromName("HelveticaNeue-LightItalic", 11) },
                { UIStringAttributeKey.ForegroundColor, UIColor.FromRGBA(51 / 255, 181 / 255, 229 / 255, 1) }
            }, new NSRange(centerText.Length - 19, 19));

        chartView.CenterAttributedText = centerText;
        chartView.DrawHoleEnabled = true;
        chartView.RotationAngle = 0.0f;
        chartView.RotationEnabled = true;
        chartView.HighlightPerTapEnabled = true;
        var l = chartView.Legend;
        l.HorizontalAlignment = ChartLegendHorizontalAlignment.Right;
        l.VerticalAlignment = ChartLegendVerticalAlignment.Top;
        l.Orientation = ChartLegendOrientation.Vertical;
        l.DrawInside = false;
        l.XEntrySpace = 7.0f;
        l.YEntrySpace = 0.0f;
        l.YOffset = 0.0f;
    }

    protected virtual void SetupRadarChartView(RadarChartView chartView) =>
        chartView.ChartDescription.Enabled = false;

    protected virtual void SetupBarLineChartView(BarLineChartViewBase chartView)
    {
        chartView.ChartDescription.Enabled = false;
        chartView.DrawGridBackgroundEnabled = false;
        chartView.DragEnabled = true;
        chartView.SetScaleEnabled(true);
        chartView.PinchZoomEnabled = false;

        // var leftAxis = chartView.LeftAxis;

        var xAxis = chartView.XAxis;
        xAxis.LabelPosition = XAxisLabelPosition.Bottom;

        chartView.RightAxis.Enabled = false;
    }

    #endregion
}