using System;
using System.Threading.Tasks;

using Brightness.Cli.Brightness;

namespace Brightness.Cli;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		if (args.Length == 0)
		{
			PrintHelp();
			return 0;
		}

		return args[0].ToLowerInvariant() switch
		{
			"list" or "get" => await RunGetCommand(args),
			"set"           => await RunSetCommand(args),
			_               => UnknownCommand(args[0]),
		};
	}

	private static async Task<int> RunGetCommand(string[] args)
	{
		int? filterIndex = null;
		if (args.Length > 1)
		{
			if (!int.TryParse(args[1], out int idx))
			{
				Console.Error.WriteLine($"Invalid monitor index: {args[1]}");
				return 1;
			}
			filterIndex = idx;
		}

		var monitors = await BrightnessController.GetMonitorsAsync();

		if (monitors.Count == 0)
		{
			Console.WriteLine("No monitors found.");
			return 0;
		}

		foreach (var m in monitors)
		{
			if (filterIndex.HasValue && m.Index != filterIndex.Value)
				continue;

			string brightness = m.SupportsBrightness && m.Brightness >= 0
				? $"{m.Brightness}%"
				: "N/A";
			string type = m.IsInternal ? "internal" : "external";
			Console.WriteLine($"[{m.Index}] {m.Name} ({type}) - brightness: {brightness}");
		}

		return 0;
	}

	private static async Task<int> RunSetCommand(string[] args)
	{
		if (args.Length < 3)
		{
			Console.Error.WriteLine("Usage: brightness set <index|all> <0-100>");
			return 1;
		}

		if (!int.TryParse(args[2], out int brightness) || brightness is < 0 or > 100)
		{
			Console.Error.WriteLine($"Invalid brightness '{args[2]}'. Must be an integer between 0 and 100.");
			return 1;
		}

		if (string.Equals(args[1], "all", StringComparison.OrdinalIgnoreCase))
		{
			int count = await BrightnessController.SetAllBrightnessAsync(brightness);
			Console.WriteLine($"Brightness set to {brightness}% on {count} monitor(s).");
			return 0;
		}

		if (!int.TryParse(args[1], out int monitorIndex))
		{
			Console.Error.WriteLine($"Invalid monitor index '{args[1]}'. Use an integer or 'all'.");
			return 1;
		}

		bool success = await BrightnessController.SetBrightnessAsync(monitorIndex, brightness);
		if (success)
		{
			Console.WriteLine($"Brightness set to {brightness}% on monitor {monitorIndex}.");
			return 0;
		}

		Console.Error.WriteLine($"Failed to set brightness on monitor {monitorIndex}. " +
			"The monitor may not exist or may not support brightness control.");
		return 1;
	}

	private static int UnknownCommand(string command)
	{
		Console.Error.WriteLine($"Unknown command: {command}");
		PrintHelp();
		return 1;
	}

	private static void PrintHelp()
	{
		Console.WriteLine("brightness - Control monitor brightness from the command line");
		Console.WriteLine();
		Console.WriteLine("Usage:");
		Console.WriteLine("  brightness list                   List all monitors with current brightness");
		Console.WriteLine("  brightness get [<index>]          Get brightness of all or a specific monitor");
		Console.WriteLine("  brightness set all <0-100>        Set brightness on all monitors");
		Console.WriteLine("  brightness set <index> <0-100>    Set brightness on a specific monitor");
		Console.WriteLine();
		Console.WriteLine("Examples:");
		Console.WriteLine("  brightness list");
		Console.WriteLine("  brightness set all 50");
		Console.WriteLine("  brightness set 0 75");
		Console.WriteLine("  brightness get 1");
	}
}
