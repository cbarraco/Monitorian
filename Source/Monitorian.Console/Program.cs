namespace Monitorian.Console
{
	using Monitorian.Core.Models.Monitor;
	using System.Linq;
	using System;
	using System.Threading.Tasks;
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using System.IO;

	internal class Program
	{
		static async Task Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Invalid number of arguments");
				return;
			}

			Console.WriteLine("Getting list of connected monitors");
			var monitors = await MonitorManager.EnumerateMonitorsAsync().ConfigureAwait(false);
			var monitorsList = monitors.ToList();

			string command = args[0];
			if (command.Equals("get"))
			{
				int monitorId = int.Parse(args[1]);
				var monitor = monitorsList[monitorId];
				monitor.UpdateBrightness();
				Console.WriteLine(monitor.Brightness);
			}
			else if (command.Equals("set"))
			{
				int monitorId = int.Parse(args[1]);
				int brightnessLevel = int.Parse(args[2]);
				monitorsList[monitorId].SetBrightness(brightnessLevel);
			}
			else if (command.Equals("save"))
			{
				string profileName = args[1];
				SaveMonitorProfile(profileName, monitorsList);
			}
			else if (command.Equals("load"))
			{
				string profileName = args[1];
				LoadMonitorProfile(profileName, monitorsList);
			}
			else
			{
				Console.WriteLine("Invalid command specified");
			}
		}

		private static void SaveMonitorProfile(string profileName, List<IMonitor> monitorsList)
		{
			Dictionary<string, Profile> profiles = LoadProfilesFromFile();
			if (profiles == null)
				profiles = new Dictionary<string, Profile>();

			var profile = new Profile();
			profile.Name = profileName;
			for (int i = 0; i < monitorsList.Count; i++)
			{
				IMonitor monitor = monitorsList[i];
				monitor.UpdateBrightness();
				profile.Monitors.Add(new Monitor()
				{
					Id = i,
					Brightness = monitor.Brightness
				});
			}
			profiles[profileName] = profile;

			string profilesJson = JsonConvert.SerializeObject(profiles);
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/monitorianProfiles.json";
			File.WriteAllText(path, profilesJson);
		}

		private static void LoadMonitorProfile(string profileName, List<IMonitor> monitorsList)
		{
			var profiles = LoadProfilesFromFile();
			var profile = profiles[profileName];
			for (int i = 0; i < profile.Monitors.Count; i++)
			{
				var monitor = profile.Monitors[i];
				monitorsList[monitor.Id].SetBrightness(monitor.Brightness);
			}
		}

		private static Dictionary<string, Profile> LoadProfilesFromFile()
		{
			try
			{
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/monitorianProfiles.json";
				var profileJson = File.ReadAllText(path);
				return JsonConvert.DeserializeObject<Dictionary<string, Profile>>(profileJson);
			} catch {
				Console.WriteLine("Could not read profiles");
				return null;
			}
		}
	}
}
