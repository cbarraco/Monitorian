namespace Brightness.Cli.Brightness;

/// <summary>
/// Represents a monitor with its current brightness state.
/// </summary>
public class MonitorEntry
{
	/// <summary>Zero-based index of the monitor in the enumerated list.</summary>
	public int Index { get; }

	/// <summary>Display name of the monitor.</summary>
	public string Name { get; }

	/// <summary>Whether the monitor is an internal (built-in) display.</summary>
	public bool IsInternal { get; }

	/// <summary>Whether the monitor is reachable via DDC/CI or WMI.</summary>
	public bool IsReachable { get; }

	/// <summary>Whether the monitor supports brightness control.</summary>
	public bool SupportsBrightness { get; }

	/// <summary>Current brightness level (0–100), or -1 if unavailable.</summary>
	public int Brightness { get; }

	internal MonitorEntry(int index, string name, bool isInternal, bool isReachable, bool supportsBrightness, int brightness)
	{
		Index = index;
		Name = name;
		IsInternal = isInternal;
		IsReachable = isReachable;
		SupportsBrightness = supportsBrightness;
		Brightness = brightness;
	}
}
