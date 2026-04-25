using System;
using System.Runtime.Serialization;

namespace MonitorBrightnessCli.Monitors;

[DataContract]
public struct Point(double x, double y)
{
	[DataMember(Order = 0)]
	public double X { get; set; } = x;

	[DataMember(Order = 1)]
	public double Y { get; set; } = y;

	public static Point Empty => new(double.NaN, double.NaN);

	public bool IsEmpty => double.IsNaN(X) || double.IsNaN(Y);

	public override string ToString() => IsEmpty ? "Empty" : $"{X},{Y}";
}

[DataContract]
public struct Size(double width, double height)
{
	[DataMember(Order = 0)]
	public double Width { get; set; } = width;

	[DataMember(Order = 1)]
	public double Height { get; set; } = height;

	public static Size Empty => new(double.NaN, double.NaN);

	public bool IsEmpty => double.IsNaN(Width) || double.IsNaN(Height);

	public override string ToString() => IsEmpty ? "Empty" : $"{Width},{Height}";
}

[DataContract]
public struct Rect(double x, double y, double width, double height)
{
	[DataMember(Order = 0)]
	public double X { get; set; } = x;

	[DataMember(Order = 1)]
	public double Y { get; set; } = y;

	[DataMember(Order = 2)]
	public double Width { get; set; } = width;

	[DataMember(Order = 3)]
	public double Height { get; set; } = height;

	public static Rect Empty => new(double.NaN, double.NaN, double.NaN, double.NaN);

	public bool IsEmpty => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Width) || double.IsNaN(Height);

	public double Left => X;

	public double Top => Y;

	public double Right => X + Width;

	public double Bottom => Y + Height;

	public Point Location => IsEmpty ? Point.Empty : new Point(X, Y);

	public Size Size => IsEmpty ? Size.Empty : new Size(Width, Height);

	public override string ToString() => IsEmpty ? "Empty" : $"{X},{Y},{Width},{Height}";
}
