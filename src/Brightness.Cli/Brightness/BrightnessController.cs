using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Brightness.Cli.Monitors;

namespace Brightness.Cli.Brightness;

/// <summary>
/// Provides methods to enumerate monitors and control their brightness.
/// </summary>
public static class BrightnessController
{
	private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

	/// <summary>
	/// Enumerates all monitors and returns their current brightness levels.
	/// </summary>
	public static async Task<IReadOnlyList<MonitorEntry>> GetMonitorsAsync(
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		var monitors = (await MonitorManager.EnumerateMonitorsAsync(
			timeout ?? DefaultTimeout, cancellationToken)).ToList();
		try
		{
			var entries = new List<MonitorEntry>(monitors.Count);
			for (int i = 0; i < monitors.Count; i++)
			{
				var m = monitors[i];
				if (m.IsBrightnessSupported)
					m.UpdateBrightness();
				entries.Add(new MonitorEntry(i, m.Description, m.IsInternal, m.IsReachable, m.IsBrightnessSupported, m.Brightness));
			}
			return entries.AsReadOnly();
		}
		finally
		{
			foreach (var m in monitors)
				m.Dispose();
		}
	}

	/// <summary>
	/// Sets the brightness of a specific monitor by its index.
	/// </summary>
	/// <param name="monitorIndex">Zero-based index of the monitor.</param>
	/// <param name="brightness">Brightness level from 0 to 100.</param>
	/// <returns><c>true</c> if the brightness was set successfully; otherwise <c>false</c>.</returns>
	public static async Task<bool> SetBrightnessAsync(
		int monitorIndex,
		int brightness,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (brightness is < 0 or > 100)
			throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be between 0 and 100.");

		var monitors = (await MonitorManager.EnumerateMonitorsAsync(
			timeout ?? DefaultTimeout, cancellationToken)).ToList();
		try
		{
			if (monitorIndex < 0 || monitorIndex >= monitors.Count)
				return false;

			var monitor = monitors[monitorIndex];
			if (!monitor.IsBrightnessSupported)
				return false;

			var result = monitor.SetBrightness(brightness);
			return result.Status is AccessStatus.Succeeded;
		}
		finally
		{
			foreach (var m in monitors)
				m.Dispose();
		}
	}

	/// <summary>
	/// Sets the brightness on all monitors that support it.
	/// </summary>
	/// <param name="brightness">Brightness level from 0 to 100.</param>
	/// <returns>The number of monitors successfully updated.</returns>
	public static async Task<int> SetAllBrightnessAsync(
		int brightness,
		TimeSpan? timeout = null,
		CancellationToken cancellationToken = default)
	{
		if (brightness is < 0 or > 100)
			throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be between 0 and 100.");

		var monitors = (await MonitorManager.EnumerateMonitorsAsync(
			timeout ?? DefaultTimeout, cancellationToken)).ToList();
		try
		{
			int count = 0;
			foreach (var m in monitors)
			{
				if (!m.IsBrightnessSupported)
					continue;
				var result = m.SetBrightness(brightness);
				if (result.Status is AccessStatus.Succeeded)
					count++;
			}
			return count;
		}
		finally
		{
			foreach (var m in monitors)
				m.Dispose();
		}
	}
}
