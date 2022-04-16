using SharpSvn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateAssemblyInfo
{
    internal class Svn
    {
        const string templateFile = "GlobalAssemblyInfo.cs.template";
        const string globalAssemblyInfo = "GlobalAssemblyInfo.cs";
        //public static int Main(string[] args)
        //{
        //    try
        //    {
        //        string exeDir = Path.GetDirectoryName(typeof(Svn).Assembly.Location);
        //        bool createdNew;
        //        using (Mutex mutex = new Mutex(true, "Sunia" + exeDir.GetHashCode(), out createdNew))
        //        {
        //            if (!createdNew)
        //            {
        //                // multiple calls in parallel?
        //                // it's sufficient to let one call run, so just wait for the other call to finish
        //                try
        //                {
        //                    mutex.WaitOne(10000);
        //                }
        //                catch (AbandonedMutexException)
        //                {
        //                }
        //                return 0;
        //            }
        //            if (!File.Exists("Sunia.HWREngine.sln"))
        //            {
        //                string mainDir = Path.GetFullPath(Path.Combine(exeDir, "../../../../.."));
        //                if (File.Exists(mainDir + "\\Sunia.HWREngine.sln"))
        //                {
        //                    Directory.SetCurrentDirectory(mainDir);
        //                }
        //            }
        //            if (!File.Exists("Sunia.HWREngine.sln"))
        //            {
        //                Console.WriteLine("Working directory must be Sunia.HWREngine!");
        //                return 2;
        //            }
        //            RetrieveRevisionNumber();
        //            string versionNumber = GetMajorVersion() + "." + revisionNumber;
        //            UpdateStartup();
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return 3;
        //    }
        //}


        static void UpdateStartup()
        {
            string content;
            using (StreamReader r = new StreamReader(templateFile))
            {
                content = r.ReadToEnd();
            }
            content = content.Replace("$INSERTREVISION$", revisionNumber);
            content = content.Replace("$INSERTDATE$", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (File.Exists(globalAssemblyInfo))
            {
                using (StreamReader r = new StreamReader(globalAssemblyInfo))
                {
                    if (r.ReadToEnd() == content)
                    {
                        // nothing changed, do not overwrite file to prevent recompilation
                        // every time.
                        return;
                    }
                }
            }
            using (StreamWriter w = new StreamWriter(globalAssemblyInfo, false, Encoding.UTF8))
            {
                w.Write(content);
            }
        }

        static string GetMajorVersion()
        {
            string version = "?";
            // Get main version from startup
            using (StreamReader r = new StreamReader(templateFile))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string search = "string Major = \"";
                    int pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        version = line.Substring(pos + search.Length, e - pos - search.Length);
                    }
                    search = "string Minor = \"";
                    pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        version = version + "." + line.Substring(pos + search.Length, e - pos - search.Length);
                    }
                    search = "string Build = \"";
                    pos = line.IndexOf(search);
                    if (pos >= 0)
                    {
                        int e = line.IndexOf('"', pos + search.Length + 1);
                        version = version + "." + line.Substring(pos + search.Length, e - pos - search.Length);
                    }
                }
            }
            return version;
        }

        #region Retrieve Revision Number
        static string revisionNumber = "0";

        static void RetrieveRevisionNumber()
        {
            try
            {
                using (SvnClient client = new SvnClient())
                {
                    client.Info(
                        Environment.CurrentDirectory,
                        (sender, info) =>
                        {
                            revisionNumber = info.Revision.ToString(CultureInfo.InvariantCulture);
                        });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Reading revision number with SharpSvn failed: " + e.ToString());
            }
            if (revisionNumber == null || revisionNumber.Length == 0 || revisionNumber == "0")
            {
                throw new ApplicationException("Error reading revision number");
            }
        }
        #endregion
    }
}
