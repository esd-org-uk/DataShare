using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;

namespace DS.WindowsService.Debugger
{
    public partial class ServiceRunner : Form
    {
        private readonly IDebuggableService _theService;

        public ServiceRunner(IDebuggableService service)
        {
            InitializeComponent();
            _theService = service;
            var winService = _theService as ServiceBase;
            if (winService != null)
            {
                
            } 
            Show();
            Win32.AllocConsole();
        }

        protected void DebugInfoAdded(object sender, EventArgs e)  
        {  
        
        }  

        private void StartButtonClick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Started";
            _theService.Start(new string[] {});
        }

        public class Win32
        {
            /// <summary>
            /// Allocates a new console for current process.
            /// </summary>
            [DllImport("kernel32.dll")]
            public static extern Boolean AllocConsole();

            /// <summary>
            /// Frees the console.
            /// </summary>
            [DllImport("kernel32.dll")]
            public static extern Boolean FreeConsole();
        }
    }
}