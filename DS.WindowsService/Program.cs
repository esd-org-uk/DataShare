using System.ServiceProcess;
using System.Windows.Forms;
using DS.WindowsService.Debugger;

namespace DS.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

#if DEBUG
                Application.Run(new ServiceRunner(new DSService()));
#else 
                ServiceBase.Run(new DSService());
#endif
        }
    }
}
