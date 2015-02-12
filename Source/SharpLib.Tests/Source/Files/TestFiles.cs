using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace SharpLib.Tests
{
    [TestFixture]
    public class TestFiles
    {
        /// <summary>
        /// Провверка MD5
        /// </summary>
        [Test]
        public void TestCopyDir()
        {
            var temp = Files.GetTempDirectory();
            var newTemp = temp + "newTemp";
            var dir1 = Files.CreateDirectory(temp, "1");
            var dir2  = Files.CreateDirectory(temp, "2");
            Files.WriteText(Path.Combine(dir1, "1.txt"), "1");
            Files.WriteText(Path.Combine(dir2, "2.txt"), "2");

            Files.CopyDirectory(newTemp, temp);

            var listDirs = Directory.GetDirectories(newTemp, "*", SearchOption.AllDirectories).ToList();
            var listFiles = Directory.GetFiles(newTemp, "*.*", SearchOption.AllDirectories).ToList();

            Assert.AreEqual(listDirs.Count, 2);
            Assert.AreEqual(listFiles.Count, 2);
        }
    }
}
