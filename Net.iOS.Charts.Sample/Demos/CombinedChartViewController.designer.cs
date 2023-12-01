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
	partial class CombinedChartViewController
	{
		[Outlet]
		Net.iOS.Charts.CombinedChartView ChartView { get; set; }

		[Action ("optionsButtonTapped:")]
		partial void OptionsButtonTapped (Foundation.NSObject sender);

		void ReleaseDesignerOutlets ()
		{
			if (ChartView != null) {
				ChartView.Dispose ();
				ChartView = null;
			}

		}
	}
}
