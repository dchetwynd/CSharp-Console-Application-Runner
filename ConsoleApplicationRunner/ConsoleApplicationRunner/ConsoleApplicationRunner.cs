using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConsoleApplicationRunner
{
    public class ConsoleApplicationRunner
    {
        private Type _applicationType;
        private string _applicationExecutableName;

        public ConsoleApplicationRunner(Type applicationType, string applicationExecutableName)
        {
            _applicationType = applicationType;
            _applicationExecutableName = applicationExecutableName;
        }

        public string RunApplicationWithInputs(string[] applicationInputLines)
        {
            string applicationExecutablePath = GetApplicationExecutablePath(_applicationType, _applicationExecutableName);

            Process applicationProcess = CreateApplicationProcess(applicationExecutablePath);
            applicationProcess.Start();

            WriteApplicationInputs(applicationProcess, applicationInputLines);
            string applicationOutputs = ReadApplicationOutputs(applicationProcess);

            return applicationOutputs;
        }

        private static string ReadApplicationOutputs(Process applicationProcess)
        {
            StreamReader outputReader = applicationProcess.StandardOutput;
            applicationProcess.WaitForExit();
            return outputReader.ReadToEnd();
        }

        private static void WriteApplicationInputs(Process applicationProcess, string[] applicationInputLines)
        {
            foreach (string inputLine in applicationInputLines)
                WriteApplicationInput(applicationProcess, inputLine);
        }

        private static void WriteApplicationInput(Process applicationProcess, string applicationInput)
        {
            StreamWriter inputWriter = applicationProcess.StandardInput;
            inputWriter.WriteLine(applicationInput);
        }

        private static Process CreateApplicationProcess(string applicationExecutablePath)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = applicationExecutablePath,
                ErrorDialog = false,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            Process process = new Process();
            process.StartInfo = processStartInfo;
            return process;
        }

        private static string GetApplicationExecutablePath(Type executableClass, string applicationExecutableName)
        {
            string applicationExecutableDirectoryWithFileScheme =
                Path.GetDirectoryName(Assembly.GetAssembly(executableClass).CodeBase);
            string applicationExecutableDirectory = Regex.Replace(applicationExecutableDirectoryWithFileScheme,
                                                                  @"^file:\\", "");
            string applicationExecutablePath = applicationExecutableDirectory + "\\" + applicationExecutableName;
            return applicationExecutablePath;
        }
    }
}
