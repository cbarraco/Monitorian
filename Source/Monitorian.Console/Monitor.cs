using Newtonsoft.Json;

namespace Monitorian.Console
{
	internal class Monitor
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("brightness")]
		public int Brightness { get; set; }
	}
}
