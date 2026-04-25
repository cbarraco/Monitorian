namespace Brightness.Cli.Monitors;

internal class UnreachableMonitorItem(
	string deviceInstanceId,
	string description,
	byte displayIndex,
	byte monitorIndex,
	ConnectionType connection,
	bool isInternal) : MonitorItem(
		deviceInstanceId: deviceInstanceId,
		description: description,
		displayIndex: displayIndex,
		monitorIndex: monitorIndex,
		monitorRect: Rect.Empty,
		connection: connection,
		isInternal: isInternal,
		isReachable: false)
{
	public override AccessResult UpdateBrightness(int brightness = -1) => AccessResult.Failed;
	public override AccessResult SetBrightness(int brightness) => AccessResult.Failed;
}
