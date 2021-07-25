using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace hb.tests
{
    [TestClass]
    public class GeneralUnitTest
    {
        [TestMethod]
        public void ProcessExecTest()
        {
            XProcess.OutputAction = (str) =>
            {
                Console.WriteLine(str);
            };
            XProcess.Execute("dir");
            bool isFinished = false;
            XTask.Delay(ref isFinished);
        }

        [TestMethod]
        public void GuidTest()
        {
            var g1 = XGuid.Next();
            var g2 = XGuid.NextB();
            var g3 = XGuid.NextN();
            var g4 = XGuid.NextP();
            var g5 = XGuid.NextX();
            Console.WriteLine(g1);
            Console.WriteLine(g2);
            Console.WriteLine(g3);
            Console.WriteLine(g4);
            Console.WriteLine(g5);
        }

        [TestMethod]
        public void STest()
        {
            var user = new User
            {
                Name = "张三",
                Age = 18,
                Weight = 180
            };
            byte[] bytes = BytesSerialize.Serialize(user);
            User obj = BytesSerialize.Deserialize<User>(bytes);
            Assert.AreEqual(obj.Name, user.Name);
        }

        [TestMethod]
        public void FileWatcher()
        {
            string path = "D:";
            FileWatcher fw = new FileWatcher();
            fw.AddPath(path);
            fw.WatcherEvent += (obj, e) =>
            {
                Console.WriteLine(e.FullPath);
            };
            fw.Start();
            bool isFinished = false;
            XTask.Delay(ref isFinished, 1000 * 10);
            fw.Stop();
        }

        [TestMethod]
        public void CompressTest()
        {
            string test = "adklf;adfja;dklfja;skdfja;lskdjf;laksdjl;askxz;,cvxzcvnzx,cmvlfdjasd'kjfa;sdlkjfa;lskdfjwqoeiruq[pwoeri[p;dfskj;lkvjklasdfalsdfja;sdlkfjxc;z.,vmlxckvjkldsjfglkjfgpow[ipoya;lsdkja;klsdj,vaslkjdfl;asdfgj;lkgfjf';oihosdifgjoidfsh;kvcxcznvkjahsfkjasdg;ltjho0trhjoilrethlkfd;jgs;ldfgja;lpkdgja;lsdkfjals;df";
            byte[] bytes = Encoding.UTF8.GetBytes(test);
            File.WriteAllBytes("d:\\t1.txt",bytes);
            var compressbytes = ZipCompression.Compress(bytes);
            File.WriteAllBytes("d:\\t2.txt", compressbytes);
            byte[] newBytes = ZipCompression.Decompress(compressbytes);
            Assert.AreEqual(Convert.ToBase64String(bytes), Convert.ToBase64String(newBytes));
        }

        [TestMethod]
        public void CompressFileTest()
        {
            //说明：先创建空文件d:/a.txt
            string file = "d:\\a.txt";
            string destFile = "d:\\b.gz";
            string newFile = "d:\\c.txt";
            ZipCompression.Compress(file, destFile);
            ZipCompression.Decompress(destFile, newFile);
        }
    }

    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
    }
}
