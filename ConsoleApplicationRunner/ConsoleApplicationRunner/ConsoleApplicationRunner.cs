using System;
using System.Collections.Generic;
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

        public string[] RunApplicationWithInputs(string[] applicationInputLines)
        {
            string applicationExecutablePath = GetApplicationExecutablePath(_applicationType, _applicationExecutableName);

            Process applicationProcess = CreateApplicationProcess(applicationExecutablePath);
            applicationProcess.Start();

            WriteApplicationInputs(applicationProcess, applicationInputLines);
            return ReadApplicationOutputs(applicationProcess);
        }

        private static string[] ReadApplicationOutputs(Process applicationProcess)
        {
            StreamReader outputReader = applicationProcess.StandardOutput;
            applicationProcess.WaitForExit();
            string totalApplicationOutput = ReplaceLineEndings(outputReader.ReadToEnd());
            
            return totalApplicationOutput.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string ReplaceLineEndings(string originalString)
        {
            return Regex.Replace(originalString, "\r\n", "\n");
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
