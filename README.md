# Known issues

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
You can check open .NET iOS issue that is (probably) causing this: https://github.com/xamarin/xamarin-macios/issues/3217

### Missing methods
1. `SetColors(UIColor[] colors)` in `ChartBaseDataSet`.
<br> Workaround: use `SetColors(UIColor[] colors, nfloat alpha)` with alpha 1.
