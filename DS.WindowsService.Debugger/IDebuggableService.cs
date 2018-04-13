namespace DS.WindowsService.Debugger
{
    public interface IDebuggableService
    {
        void Start(string[] args);
        void StopService();
        string DebugInfo(string value);
    }
}