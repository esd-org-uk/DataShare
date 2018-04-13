using System.ServiceProcess;

namespace DS.WindowsService.Debugger
{
    public class DebuggableService : ServiceBase, IDebuggableService
    {
        public void Start(string[] args)
        {
            OnStart(args);
        }

        public void StopService()
        {
            OnStop();
        }

        public string DebugInfo(string value)
        {
            return value;
        }
    }
}