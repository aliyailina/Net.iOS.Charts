// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Net.iOS.Charts.Sample.Demos
{
	partial class BarChartViewController
	{
		[Outlet]
		Net.iOS.Charts.BarChartView ChartView { get; set; }

		[Outlet]
		UIKit.UITextField SliderTextX { get; set; }

		[Outlet]
		UIKit.UITextField SliderTextY { get; set; }

		[Outlet]
		UIKit.UISlider SliderX { get; set; }

		[Outlet]
		UIKit.UISlider SliderY { get; set; }

		[Action ("optionsButtonTapped:")]
		partial void OptionsButtonTapped (Foundation.NSObject sender);

		[Action ("slidersValueChanged:")]
		partial void SlidersValueChanged (Foundation.NSObject sender);

		void ReleaseDesignerOutlets ()
		{
			if (ChartView != null) {
				ChartView.Dispose ();
				ChartView = null;
			}

			if (SliderTextX != null) {
				SliderTextX.Dispose ();
				SliderTextX = null;
			}

			if (SliderTextY != null) {
				SliderTextY.Dispose ();
				SliderTextY = null;
			}

			if (SliderX != null) {
				SliderX.Dispose ();
				SliderX = null;
			}

			if (SliderY != null) {
				SliderY.Dispose ();
				SliderY = null;
			}

		}
	}
}
