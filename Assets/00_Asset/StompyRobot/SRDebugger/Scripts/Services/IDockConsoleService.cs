namespace _00_Asset.StompyRobot.SRDebugger.Scripts.Services
{
    public interface IDockConsoleService
    {
        bool IsVisible { get; set; }
        bool IsExpanded { get; set; }
        ConsoleAlignment Alignment { get; set; }
    }
}
