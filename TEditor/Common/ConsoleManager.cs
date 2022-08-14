using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TEditor.Common
{
    internal static class ConsoleManager
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        public const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
    }
}
