using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using TEditor.Common;

namespace TEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            GlobalCache.Args = e.Args;
            ConsoleManager.AttachConsole(ConsoleManager.ATTACH_PARENT_PROCESS);

            base.OnStartup(e);
        }


    }
}
