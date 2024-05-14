This is .NET iOS binding of [danielgindi iOS Charts (DGCharts) library](https://github.com/danielgindi/Charts)

## Be aware, library is beta

Not all functionality may be available (like, some setters missing or something) and not all demos and no tests was
ported to C#. If you observe any problem – open new issue to prioritize its fix.

## Known issues

### Sometimes GC can remove class reference of value of properties, which declared type is protocol interfaces.
You can observe this issue in DataSets property of any ChartDataSet subclass. Create some set
(e.g. LineChartDataSet), wrap it into array and then set to DataSets property of you LineChartView.Data property.
Then after some time try to get this set from DataSets property and cast it to LineChartDataSet and you will observe that
it is impossible, as type of this set is ChartDataSetWrapper instead of LineChartDataSet.
There is (looks like) [similar issue](https://github.com/xamarin/xamarin-macios/issues/3887) on Xamarin GitHub and
workaround for this situation can be found there: instead of using C# casting use
```csharp
Runtime.GetINativeObject<IProtocolName> (handler.WeakClient.Handle, false);
```

### Launch screen does not work
Suppose it is due to fact that it is in non-standard path for .NET iOS (by default it is in root, here in Resources folder).

### ApiDefinition contains setters for properties in protocol interfaces, that doesn't has setters in source protocols.
Example of such protocol is ChartDataProvider: the `Data` property in ApiDefinition has setter, while correspondent
`data` property in original protocol doesn't. It is because `Data` property in `LineChartView` class 
(that conforms that ChartDataProvider protocol) has setter.
You can see in ApiDefinition that `ChartViewBase` (one of `LineChartView`'s base classes) conforms `ChartDataProvider`
protocol and, moreover, has setter for `Data` property, but `LineChartView`, that inherits `ChartViewBase` doesn't.
It looks like it is because of reimplementation of `IChartViewProvider` interface (of `ChartViewProvider` protocol) and,
hence, inlining all its required properties (such as `Data`) with required signature (so without setter).
Unfortunately, I haven't found any way to prevent this behavior and looks like there is no one for now.
You can check open [.NET iOS issue](https://github.com/xamarin/xamarin-macios/issues/3217) that is (probably) causing this.

### ChartsVersionNumber and ChartsVersionString are not available
Including these two properties may result in build fail for app that uses library. Looks like it is because of recent fix for .NET (see same [.NET 8 issue](https://github.com/xamarin/xamarin-macios/issues/20458))
Recommendation from .NET was to use .xcframework to fix issue, but it didn't work. It was decided to exclude these properties as they are not expected to be frequently used. If you really require them, please, open an issue please.
```csharp
[Static]
partial interface Constants
{
    // extern double ChartsVersionNumber;
    [Field ("ChartsVersionNumber", "__Internal")]
    double ChartsVersionNumber { get; }
    
    // extern const unsigned char[] ChartsVersionString;
    [Field ("ChartsVersionString", "__Internal")]
    NSString ChartsVersionString { get; }
}
```