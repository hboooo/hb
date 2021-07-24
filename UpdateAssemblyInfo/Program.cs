using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace UpdateAssemblyInfo
{
    class Program
    {
        const int BaseCommitRev = 0;
        static string gitBranchName;
        static TemplateFile templateFile = null;
        static Params pa = null;
        static int Main(string[] args)
        {
            try
            {
                var ret = BuildEnvironment(args);
                if (ret != 0) return ret;
                ret = BuildTemplateFiles();
                if (ret != 0) return ret;

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
                    UpdateFiles();
                    SaveRevision();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 99;
            }
        }

        static int BuildWorkDirectory()
        {
            string exeDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            string slnPath = GetProjectSlnFile(exeDir);
            if (string.IsNullOrEmpty(slnPath))
            {
                Console.WriteLine($".sln file can not find, please run in project folder");
                return 11;
            }

            string workDir = Path.GetDirectoryName(slnPath);
            if (string.IsNullOrEmpty(workDir))
            {
                if (!File.Exists(slnPath))
                {
                    Console.WriteLine($"pro param .sln file [{slnPath}] can not find");
                    return 2;
                }
            }
            Directory.SetCurrentDirectory(workDir);
            return 0;
        }

        static int BuildTemplateFiles()
        {
            string template = pa[Word.tmp];
            if (string.IsNullOrEmpty(template))
            {
                template = "GlobalAssemblyInfo.cs.template";
            }
            if (!File.Exists(template))
            {
                Console.WriteLine($".template file can not find, please run in project folder");
                return 12;
            }
            templateFile = new TemplateFile()
            {
                Input = template,
                Output = template.Replace(".template", "")
            };
            return 0;
        }

        static void UpdateFiles()
        {
            string content;
            using (StreamReader r = new StreamReader(templateFile.Input))
            {
                content = r.ReadToEnd();
            }
            content = content.Replace("$INSERTVERSION$", fullVersionNumber);
            content = content.Replace("$INSERTMAJORVERSION$", majorVersionNumber);
            content = content.Replace("$INSERTREVISION$", revisionNumber);
            content = content.Replace("$INSERTCOMMITHASH$", gitCommitHash);
            content = content.Replace("$INSERTSHORTCOMMITHASH$", gitCommitHash.Substring(0, 8));
            content = content.Replace("$INSERTDATE$", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            content = content.Replace("$INSERTYEAR$", DateTime.Now.Year.ToString());
            content = content.Replace("$INSERTBRANCHNAME$", gitBranchName);
            bool isDefaultBranch = string.IsNullOrEmpty(gitBranchName) || gitBranchName == "master";
            content = content.Replace("$INSERTBRANCHPOSTFIX$", isDefaultBranch ? "" : ("-" + gitBranchName));

            content = content.Replace("$INSERTVERSIONNAME$", versionName ?? "");
            content = content.Replace("$INSERTVERSIONNAMEPOSTFIX$", string.IsNullOrEmpty(versionName) ? "" : "-" + versionName);
            content = content.Replace("$MODULENAME$", pa[Word.mod]);

            if (File.Exists(templateFile.Output))
            {
                using (StreamReader r = new StreamReader(templateFile.Output))
                {
                    if (r.ReadToEnd() == content)
                    {
                        return;
                    }
                }
            }

            string outputFile;
            if (!string.IsNullOrEmpty(pa[Word.@out]))
                outputFile = pa[Word.@out];
            else
                outputFile = templateFile.Output;
            using (StreamWriter w = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                w.Write(content);
            }

        }

        static void GetMajorVersion()
        {
            majorVersionNumber = "?";
            fullVersionNumber = "?";
            versionName = null;
            using (StreamReader r = new StreamReader(templateFile.Input))
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

        static string revisionNumber;
        static string majorVersionNumber;
        static string fullVersionNumber;
        static string versionName;
        static string gitCommitHash;

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
            if (!string.IsNullOrEmpty(pa[Word.bra]))
            {
                gitBranchName = pa[Word.bra];
            }
            GetMajorVersion();
        }

        static int ReadRevisionNumberFromGit()
        {
            string folder = string.IsNullOrEmpty(pa[Word.dir]) ? "" : pa[Word.dir];
            ProcessStartInfo info = new ProcessStartInfo("cmd", string.Format("/c git log --pretty=\"%H\" {0}", folder));
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
            return 0;
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

        static void SaveRevision()
        {
            if (pa.Contains(Word.ver))
            {
                var doc = new XDocument(new XElement(
                            "versionInfo",
                            new XElement("version", fullVersionNumber),
                            new XElement("revision", revisionNumber),
                            new XElement("commitHash", gitCommitHash),
                            new XElement("branchName", gitBranchName),
                            new XElement("versionName", versionName)
                        ));
                doc.Save("REVISION");
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
                    if (!Directory.Exists(previousPath)) return null;
                }
                return files[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        static int BuildEnvironment(string[] args)
        {
            pa = new Params();
            for (int i = 0; i < args.Length; i++)
            {
                string value = null;
                if (i + 1 < args.Length)
                {
                    if (!args[i + 1].StartsWith("--"))
                        value = args[i + 1];
                }
                pa[args[i].Replace("--", "")] = value;
            }

            var ret = BuildWorkDirectory();
            if (ret != 0) return ret;

            if (pa.Contains(Word.tmp) && !File.Exists(pa[Word.tmp]))
            {
                Console.WriteLine($"tmp param .template file [{pa[Word.tmp]}] can not find");
                return 3;
            }
            if (pa.Contains(Word.dir) && !Directory.Exists(pa[Word.dir]))
            {
                Console.WriteLine($"dir param path [{pa[Word.dir]}] does not exists");
                return 4;
            }
            if (pa.Contains(Word.@out) && string.IsNullOrEmpty(pa[Word.@out]))
            {
                Console.WriteLine($"out param [{pa[Word.@out]}] cannot be empty");
                return 4;
            }
            if (pa.Contains(Word.mod) && string.IsNullOrEmpty(pa[Word.mod]))
            {
                Console.WriteLine($"mod param [{pa[Word.mod]}] cannot be empty");
                return 5;
            }
            if (pa.Contains(Word.bra) && string.IsNullOrEmpty(pa[Word.bra]))
            {
                Console.WriteLine($"bra param [{pa[Word.bra]}] cannot be empty");
                return 6;
            }

            return 0;
        }


        class TemplateFile
        {
            public string Input, Output;
        }

        class Params
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();

            HashSet<string> _words = new HashSet<string>();
            public Params()
            {
                InitWords();
            }

            private void InitWords()
            {
                foreach (var item in Enum.GetValues(typeof(Word)))
                {
                    _words.Add(item.ToString());
                }
            }

            public bool Contains(Word w)
            {
                return _params.ContainsKey(w.ToString());
            }

            public string this[string w]
            {
                set
                {
                    if (_words.Contains(w))
                        _params[w] = value;
                }
                get
                {
                    if (_params.ContainsKey(w))
                        return _params[w];
                    return null;
                }
            }

            public string this[Word w]
            {
                set
                {
                    if (_words.Contains(w.ToString()))
                        _params[w.ToString()] = value;
                }
                get
                {
                    if (_params.ContainsKey(w.ToString()))
                        return _params[w.ToString()];
                    return null;
                }
            }
        }

        enum Word
        {
            //.template file
            tmp,
            //git folder
            dir,
            //output assemblyInfo.cs file dir
            @out,
            //module name
            mod,
            //branch name
            bra,
            //is output revision file
            ver
        }
    }
}
