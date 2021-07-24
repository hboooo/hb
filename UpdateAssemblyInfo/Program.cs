using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UpdateAssemblyInfo
{
    class Program
    {
        const int BaseCommitRev = 0;
        static string slnFile = "";

        const string globalAssemblyInfoTemplateFile = "GlobalAssemblyInfo.cs.template";

        static readonly TemplateFile[] templateFiles =
        {
            new TemplateFile
            {
                Input = globalAssemblyInfoTemplateFile,
                Output = globalAssemblyInfoTemplateFile.Replace(".template","")
            }
        };

        class TemplateFile
        {
            public string Input, Output;
        }

        static int Main(string[] args)
        {
            try
            {
                BuildEnvironment(args);

                string exeDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                string tempProject = GetProjectSlnFile(exeDir);
                if (!string.IsNullOrEmpty(tempProject))
                {
                    slnFile = tempProject;
                }
                if (!File.Exists(slnFile))
                {
                    Console.WriteLine($"sln file {slnFile} can not find");
                    return 2;
                }
                string workDir = Path.GetDirectoryName(slnFile);
                Directory.SetCurrentDirectory(workDir);

                bool createNew;
                using (Mutex mutex = new Mutex(true, "UpdateAssemblyInfo", out createNew))
                {
                    if (!createNew)
                    {
                        try
                        {
                            mutex.WaitOne(10000);
                        }
                        catch (AbandonedMutexException ex)
                        {
                            Console.WriteLine(ex);
                        }
                        return 0;
                    }

                    RetrieveRevisionNumber();
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "--branchname" && i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]))
                            gitBranchName = args[i + 1];
                    }
                    UpdateFiles();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 99;
            }
        }

        static void UpdateFiles()
        {
            TemplateFile[] updateTemplateFiles = templateFiles;
            foreach (var file in updateTemplateFiles)
            {
                string content;
                using (StreamReader r = new StreamReader(file.Input))
                {
                    content = r.ReadToEnd();
                }
                content = content.Replace("$INSERTVERSION$", fullVersionNumber);
                content = content.Replace("$INSERTMAJORVERSION$", majorVersionNumber);
                content = content.Replace("$INSERTREVISION$", revisionNumber);
                content = content.Replace("$INSERTCOMMITHASH$", gitCommitHash);
                content = content.Replace("$INSERTSHORTCOMMITHASH$", gitCommitHash.Substring(0, 8));
                content = content.Replace("$INSERTDATE$", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture));
                content = content.Replace("$INSERTYEAR$", DateTime.Now.Year.ToString());
                content = content.Replace("$INSERTBRANCHNAME$", gitBranchName);
                bool isDefaultBranch = string.IsNullOrEmpty(gitBranchName) || gitBranchName == "master";
                content = content.Replace("$INSERTBRANCHPOSTFIX$", isDefaultBranch ? "" : ("-" + gitBranchName));

                content = content.Replace("$INSERTVERSIONNAME$", versionName ?? "");
                content = content.Replace("$INSERTVERSIONNAMEPOSTFIX$", string.IsNullOrEmpty(versionName) ? "" : "-" + versionName);
                content = content.Replace("$MODULENAME$", moduleName);

                if (File.Exists(file.Output))
                {
                    using (StreamReader r = new StreamReader(file.Output))
                    {
                        if (r.ReadToEnd() == content)
                        {
                            continue;
                        }
                    }
                }
                using (StreamWriter w = new StreamWriter(file.Output, false, Encoding.UTF8))
                {
                    w.Write(content);
                }
            }
        }


        static void GetMajorVersion()
        {
            majorVersionNumber = "?";
            fullVersionNumber = "?";
            versionName = null;
            using (StreamReader r = new StreamReader(globalAssemblyInfoTemplateFile))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string search = "string Major = \"";
                    int pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        majorVersionNumber = line.Substring(pos + search.Length, e - pos - search.Length);
                    }
                    search = "string Minor = \"";
                    pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        majorVersionNumber = majorVersionNumber + "." + line.Substring(pos + search.Length, e - pos - search.Length);
                    }
                    search = "string Build = \"";
                    pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        fullVersionNumber = majorVersionNumber + "." + line.Substring(pos + search.Length, e - pos - search.Length) + "." + revisionNumber;
                    }
                    search = "string VersionName = \"";
                    pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        versionName = line.Substring(pos + search.Length, e - pos - search.Length);
                    }
                }
            }
        }

        static void SetVersionInfo(string fileName, Regex regex, string replacement)
        {
            string content;
            using (StreamReader inFile = new StreamReader(fileName))
            {
                content = inFile.ReadToEnd();
            }
            string newContent = regex.Replace(content, replacement);
            if (newContent == content)
                return;
            using (StreamWriter outFile = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                outFile.Write(newContent);
            }
        }

        #region Retrieve Revision Number


        static string revisionNumber;
        static string majorVersionNumber;
        static string fullVersionNumber;
        static string versionName;
        static string gitCommitHash;
        static string gitBranchName;
        static string gitFolder;
        static string moduleName;
        public static string outAssemblyName;

        static void RetrieveRevisionNumber()
        {
            if (revisionNumber == null)
            {
                if (Directory.Exists(".git"))
                {
                    try
                    {
                        ReadRevisionNumberFromGit();
                        ReadBranchNameFromGit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("There's no git working copy in " + Path.GetFullPath("."));
                }
            }

            if (revisionNumber == null)
            {
                ReadRevisionFromFile();
            }
            GetMajorVersion();
        }

        static void ReadRevisionNumberFromGit()
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd", string.Format("/c git log --pretty=\"%H\" {0}", string.IsNullOrEmpty(gitFolder) ? "" : gitFolder));
            string path = Environment.GetEnvironmentVariable("PATH");
            path += ";" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "git\\bin");
            info.EnvironmentVariables["PATH"] = path;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            using (Process p = Process.Start(info))
            {
                string line;
                int revNum = BaseCommitRev;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    if (gitCommitHash == null)
                    {
                        gitCommitHash = line;
                    }
                    revNum++;
                }
                p.WaitForExit();
                if (p.ExitCode != 0)
                    throw new Exception("git-rev-list exit code was " + p.ExitCode);
                revisionNumber = revNum.ToString();
            }
        }

        static void ReadBranchNameFromGit()
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd", "/c git branch --no-color");
            string path = Environment.GetEnvironmentVariable("PATH");
            path += ";" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "git\\bin");
            info.EnvironmentVariables["PATH"] = path;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            using (Process p = Process.Start(info))
            {
                string line;
                gitBranchName = "(no branch)";
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    if (line.StartsWith("* ", StringComparison.Ordinal))
                    {
                        gitBranchName = line.Substring(2);
                    }
                }
                p.WaitForExit();
                if (p.ExitCode != 0)
                    throw new Exception("git-branch exit code was " + p.ExitCode);
            }
        }

        static void ReadRevisionFromFile()
        {
            try
            {
                XDocument doc = XDocument.Load("REVISION");
                revisionNumber = (string)doc.Root.Element("revision");
                gitCommitHash = (string)doc.Root.Element("commitHash");
                gitBranchName = (string)doc.Root.Element("branchName");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("The revision number of the project version being compiled could not be retrieved.");
                Console.WriteLine();
                Console.WriteLine("Build continues with revision number '0'...");

                revisionNumber = null;
            }
            if (revisionNumber == null || revisionNumber.Length == 0)
            {
                revisionNumber = "0";
                gitCommitHash = "0000000000000000000000000000000000000000";
            }
        }

        static string GetProjectSlnFile(string workDir)
        {
            try
            {
                string previous = "";
                string[] files = Directory.GetFiles(workDir, "*.sln");
                while (files.Length == 0)
                {
                    previous += "../";
                    string previousPath = Path.GetFullPath(Path.Combine(workDir, previous));
                    files = Directory.GetFiles(previousPath, "*.sln");
                    if (Directory.Exists(previousPath)) break;
                }
                return files[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        static void BuildEnvironment(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--project" && i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]))
                    gitFolder = args[i + 1];
                if (args[i] == "--folder" && i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]))
                    gitFolder = args[i + 1];
                if (args[i] == "--assName" && i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]))
                    outAssemblyName = args[i + 1];
                if (args[i] == "--output" && i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]))
                    outputFolder = args[i + 1];
                if (args[i] == "--moduleName" && i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]))
                    moduleName = args[i + 1];
            }
        }

        #endregion

    }
}
