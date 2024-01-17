using Eon.Services.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eon.Services.Launching
{
    public class Inject
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId
        );

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            int dwSize,
            uint flAllocationType,
            uint flProtect
        );

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);


        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        public static void InjectDll(int pid, string dllPath)
        {
            IntPtr process = OpenProcess(0x43A, false, pid);
            IntPtr processAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (processAddress == IntPtr.Zero)
            {
                Loggers.Log($"Cannot get process address for pid {pid}");
                throw new Exception($"Please Check Logs -> Failed To Grab PID");
            }

            byte[] dllBytes = System.Text.Encoding.ASCII.GetBytes(dllPath + "\0");
            IntPtr dllAddress = VirtualAllocEx(process, IntPtr.Zero, dllBytes.Length, 0x3000, 0x4);

            bool writeMemoryResult = WriteProcessMemory(process, dllAddress, dllBytes, dllBytes.Length, IntPtr.Zero);
            if (!writeMemoryResult)
            {
                throw new Exception("Memory write failed");
            }

            IntPtr createThreadResult = CreateRemoteThread(process, IntPtr.Zero, 0U, processAddress, dllAddress, 0U, IntPtr.Zero);
            if (createThreadResult == IntPtr.Zero)
            {
                throw new Exception("Thread creation failed");
            }

            bool closeResult = CloseHandle(process);
            if (!closeResult)
            {
                throw new Exception("Cannot close handle");
            }

            Loggers.Log("INJECTED DLL!"); 

           // MessageBox.Show("INJECTED");
        }
    }
}
