using ObjCRuntime;

[assembly: LinkWith("DGCharts.framework",
    target: LinkTarget.Arm64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator | LinkTarget.Simulator64,
    SmartLink = true, 
    ForceLoad = true,
    LinkerFlags = "-lxml2 -ObjC")]