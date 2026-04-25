using System;
using System.Collections.Generic;
using System.Linq;

using MonitorBrightnessCli.Support;

namespace MonitorBrightnessCli.Monitors;

/// <summary>
/// Physical monitor managed by WMI (internal monitor)
/// </summary>
internal class WmiMonitorItem(
	string deviceInstanceId,
	string description,
	byte displayIndex,
	byte monitorIndex,
	Rect monitorRect,
	ConnectionType connection,
	bool isInternal,
	IEnumerable<byte> brightnessLevels) : MonitorItem(
		deviceInstanceId: deviceInstanceId,
		description: description,
		displayIndex: displayIndex,
		monitorIndex: monitorIndex,
		monitorRect: monitorRect,
		connection: connection,
		isInternal: isInternal,			
		isReachable: true)
{
	private readonly byte[] _brightnessLevels = brightnessLevels?.ToArray() ?? throw new ArgumentNullException(nameof(brightnessLevels));

	public override AccessResult UpdateBrightness(int brightness = -1)
	{
		if (IsInternal && !PowerManagement.IsIgnored)
		{
			this.Brightness = PowerManagement.GetActiveSchemeBrightness();

			this.BrightnessSystemAdjusted = !PowerManagement.IsAdaptiveBrightnessEnabled
				? -1 // Default
				: (0 <= brightness)
					? brightness
					: MSMonitor.GetBrightness(DeviceInstanceId);
		}
		else
		{
			this.Brightness = (0 <= brightness)
				? brightness
				: MSMonitor.GetBrightness(DeviceInstanceId);
		}
		return (0 <= this.Brightness) ? AccessResult.Succeeded : AccessResult.Failed;
	}

	public override AccessResult SetBrightness(int brightness)
	{
		if (brightness is < 0 or > 100)
			throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "The brightness must be from 0 to 100.");

		if (IsInternal && !PowerManagement.IsIgnored)
		{
			if (PowerManagement.SetActiveSchemeBrightness(brightness))
			{
				this.Brightness = brightness;
				return AccessResult.Succeeded;
			}
		}
		else
		{
			brightness = ArraySearch.GetNearest(_brightnessLevels, (byte)brightness);

			if (MSMonitor.SetBrightness(DeviceInstanceId, brightness))
			{
				this.Brightness = brightness;
				return AccessResult.Succeeded;
			}
		}
		return AccessResult.Failed;
	}
}
