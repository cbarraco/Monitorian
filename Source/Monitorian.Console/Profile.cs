using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monitorian.Console
{
	internal class Profile
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("monitors")]
		public List<Monitor> Monitors { get; set; } = new List<Monitor>();
	}
}
