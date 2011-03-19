using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Principal;

namespace CortexCommandModManager.Startup
{
    /// <summary>Class able to elevate the current program instance to Administrator if needed.</summary>
    public class ProgramElevator
    {
        private FileInfo CurrentAssemblyLocation { get { return new FileInfo(Assembly.GetExecutingAssembly().Location); } }

        /// <summary>Returns true if the current program is running in an environment that requires administrator privileges.</summary>
        public bool ExecutingProgramRequiresElevation
        {
            get
            {
                return IsVistaOr7() && !IsRunningAsAdmin() && LocalProgramIsInProgramFiles();
            }
        }

        public bool DependentFolderRequiresElevation(string folder)
        {
            return folder.Contains("Program Files");
        }

        /// <summary>Immediately terminates the current process and starts a new one with elevated privileges.</summary>
        public void Elevate()
        {
            var CCMMFile = CurrentAssemblyLocation.FullName;
            var process = new Process
            {
                StartInfo = 
                {
                    FileName = CCMMFile,
                    WorkingDirectory = new DirectoryInfo(CCMMFile).Parent.FullName,
                    UseShellExecute = true,
                    Verb = "runas"
                }
            };
            process.Start();

            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.Kill();

            throw new InvalidOperationException("Current process terminated.");
        }

        /// <summary>Checks whether the current assembly is running in program files.</summary>
        private bool LocalProgramIsInProgramFiles()
        {
            return CurrentAssemblyLocation.FullName.Contains("Program Files");
        }

        /// <summary>Checks whether the current environment is running in windows vista or windows 7, each of which need elevation in program files.</summary>
        private bool IsVistaOr7()
        {
            return Environment.OSVersion.Version.Major == 6;
        }

        /// <summary>Checks whether the current instance of the program is running with administrator privileges.</summary>
        private bool IsRunningAsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
