namespace _00_Asset.StompyRobot.SRDebugger.Scripts.Services
{
    public interface IDebugTriggerService
    {
        bool IsEnabled { get; set; }
        PinAlignment Position { get; set; }
    }
}
