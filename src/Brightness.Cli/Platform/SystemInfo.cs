using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Brightness.Cli.Platform;

/// <summary>
/// System information
/// </summary>
public static class SystemInfo
{
	private class SystemInfoBase
	{
		public string Manufacturer { get; private set; } = string.Empty;
		public string Model { get; private set; } = string.Empty;

		public SystemInfoBase()
		{
			const string keyName = @"SYSTEM\CurrentControlSet\Control\SystemInformation"; // HKLM

			using var key = Registry.LocalMachine.OpenSubKey(keyName);

			Manufacturer = key?.GetValue("SystemManufacturer") as string ?? string.Empty;
			Model = key?.GetValue("SystemProductName") as string ?? string.Empty;

			// Fallback
			if (string.IsNullOrEmpty(Manufacturer) ||
				string.IsNullOrEmpty(Model))
			{
				try
				{
					using var @class = new ManagementClass("Win32_ComputerSystem");
					using var instance = @class.GetInstances().Cast<ManagementObject>().FirstOrDefault();

					Manufacturer = instance?["Manufacturer"] as string ?? string.Empty;
					Model = instance?["Model"] as string ?? string.Empty;
				}
				catch (ManagementException)
				{
					Debug.WriteLine("Failed to get instances by Win32_ComputerSystem.");
				}
				catch (COMException ce) when ((uint)ce.HResult is 0x80070422)
				{
					// Error message: The service cannot be started, either because it is disabled or because it has no enabled devices associated with it.
					// Error code: 0x80070422
					// This error occurs when WMI is disabled.
				}
			}
		}
	}

	private static readonly Lazy<SystemInfoBase> _instance = new(() => new());

	public static string Manufacturer => _instance.Value.Manufacturer;
	public static string Model => _instance.Value.Model;
}
