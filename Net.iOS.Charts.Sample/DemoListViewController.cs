using Net.iOS.Charts.Sample.Demos;

namespace Net.iOS.Charts.Sample;

[Register(nameof(DemoListViewController))]
public partial class DemoListViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
{
    private ItemDef[] _itemDefs;
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Charts demonstration";

        _itemDefs = new ItemDef[]
        {
            new()
            {
                Title = "Line Chart",
                Subtitle = "A simple demonstration of the linechart.",
                ViewController = () => new LineChart1ViewController()
            },
            new()
            {
                Title = "Line Chart (Dual YAxis)",
                Subtitle = "Demonstration of the linechart with dual y-axis.",
                ViewController = () => new LineChart2ViewController()
            },
            new()
            {
                Title = "Bar Chart",
                Subtitle = "A simple demonstration of the bar chart.",
                ViewController = () => new BarChartViewController()
            },
            new()
            {
                Title = "Horizontal Bar Chart",
                Subtitle = "A simple demonstration of the horizontal bar chart.",
                ViewController = () => new HorizontalBarChartViewController()
            },
            new()
            {
                Title = "Combined Chart",
                Subtitle = "Demonstrates how to create a combined chart (bar and line in this case).",
                ViewController = () => new CombinedChartViewController()
            },
            new()
            {
                Title = "Pie Chart",
                Subtitle = "A simple demonstration of the pie chart.",
                ViewController = () => new PieChartViewController()
            },
            new()
            {
                Title = "Pie Chart with value lines",
                Subtitle = "A simple demonstration of the pie chart with polyline notes.",
                ViewController = () => new PiePolylineChartViewController()
            },
            new()
            {
                Title = "Scatter Chart",
                Subtitle = "A simple demonstration of the scatter chart.",
                ViewController = () => new ScatterChartViewController()
            },
            new()
            {
                Title = "Bubble Chart",
                Subtitle = "A simple demonstration of the bubble chart.",
                ViewController = () => new BubbleChartViewController()
            },
            new()
            {
                Title = "Stacked Bar Chart",
                Subtitle = "A simple demonstration of a bar chart with stacked bars.",
                ViewController = () => new StackedBarChartViewController()
            },
            new()
            {
                Title = "Stacked Bar Chart Negative",
                Subtitle = "A simple demonstration of stacked bars with negative and positive values.",
                ViewController = () => new NegativeStackedBarChartViewController()
            },
            new()
            {
                Title = "Another Bar Chart",
                Subtitle = "Implementation of a BarChart that only shows values at the bottom.",
                ViewController = () => new AnotherBarChartViewController()
            },
            new()
            {
                Title = "Multiple Lines Chart",
                Subtitle = "A line chart with multiple DataSet objects. One color per DataSet.",
                ViewController = () => new MultipleLinesChartViewController()
            },
            new()
            {
                Title = "Multiple Bars Chart",
                Subtitle = "A bar chart with multiple DataSet objects. One multiple colors per DataSet.",
                ViewController = () => new MultipleBarChartViewController()
            },
            new()
            {
                Title = "Candle Stick Chart",
                Subtitle = "Demonstrates usage of the CandleStickChart.",
                ViewController = () => new CandleStickChartViewController()
            },
            new()
            {
                Title = "Cubic Line Chart",
                Subtitle = "Demonstrates cubic lines in a LineChart.",
                ViewController = () => new CubicLineChartViewController()
            },
            new()
            {
                Title = "Radar Chart",
                Subtitle = "Demonstrates the use of a spider-web like (net) chart.",
                ViewController = () => new RadarChartViewController()
            },
            new()
            {
                Title = "Colored Line Chart",
                Subtitle = "Shows a LineChart with different background and line color.",
                ViewController = () => new ColoredLineChartViewController()
            },
            new()
            {
                Title = "Sinus Bar Chart",
                Subtitle = "A Bar Chart plotting the sinus function with 8.000 values.",
                ViewController = () => new SinusBarChartViewController()
            },
            new()
            {
                Title = "BarChart positive / negative",
                Subtitle =
                    "This demonstrates how to create a BarChart with positive and negative values in different colors.",
                ViewController = () => new PositiveNegativeBarChartViewController()
            },
            new()
            {
                Title = "Time Line Chart",
                Subtitle =
                    "Simple demonstration of a time-chart. This chart draws one line entry per hour originating from the current time in milliseconds.",
                ViewController = () => new LineChartTimeViewController()
            },
            new()
            {
                Title = "Filled Line Chart",
                Subtitle = "This demonstrates how to fill an area between two LineDataSets.",
                ViewController = () => new LineChartFilledViewController()
            },
            new()
            {
                Title = "Half Pie Chart",
                Subtitle = "This demonstrates how to create a 180 degree PieChart.",
                ViewController = () => new HalfPieChartViewController()
            }
        };
    }

    public override void DidReceiveMemoryWarning()
    {
        base.DidReceiveMemoryWarning();
        // Dispose of any resources that can be recreated.
    }

    #region UITableViewDataSource, UITableViewDelegate

    [Export("numberOfSectionsInTableView:")]
    public IntPtr NumberOfSections(UITableView tableView) =>
        1;
    
    [Export("tableView:heightForRowAtIndexPath:")]
    public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) =>
        70;

    public IntPtr RowsInSection(UITableView tableView, IntPtr section) =>
        _itemDefs.Length;

    [Export("tableView:didSelectRowAtIndexPath:")]
    public void SelectRow(UITableView tableView, NSIndexPath indexPath)
    {
        var def = _itemDefs[indexPath.Row];
        var vc = def.ViewController();
        NavigationController?.PushViewController(vc, true);
        tableView.DeselectRow(indexPath, true);
    }

    public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var def = _itemDefs[indexPath.Row];
        var cell = tableView.DequeueReusableCell("Cell");
        if (cell == null)
            cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "Cell");

#pragma warning disable CA1422
        cell!.TextLabel.Text = def.Title;
        cell.DetailTextLabel.Text = def.Subtitle;
        cell.DetailTextLabel.Lines = 0;
#pragma warning restore CA1422
        
        return cell;
    }

    #endregion
    
    private class ItemDef
    {
        public string Title { get; init; }
        public string Subtitle { get; init; }
        public Func<DemoBaseViewController> ViewController { get; init; }
    }
}