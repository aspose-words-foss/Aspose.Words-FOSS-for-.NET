// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Crypto;
using Aspose.IO;
using NUnit.Framework; 

namespace Aspose.Words.Tests.Other
{
    [TestFixture]
    public class TestGoogleProof
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test, Ignore("GoogleProof")]
        public void TestGoogle()
        {
            TestGoogleDirectory(@"E:\GoogleProof\Doc", LoadFormat.Doc);
        }

        [Test, Ignore("GoogleProof")]
        public void TestDeleteRepeatingFiles()
        {
            Deleter d = new Deleter();
            d.Execute(@"F:\GoogleProof\Doc\8441");
        }

        private class Deleter
        {
            private List<string> mAllFiles;

            internal void Execute(string dir)
            {
                mAllFiles = new List<string>();
                CollectAllFiles(dir);

                Dictionary<BytesHash, string> guids = new Dictionary<BytesHash, string>();

                foreach (string fileName in mAllFiles)
                {
                    byte[] bytes;
                    using (Stream stream = File.OpenRead(fileName))
                        bytes = StreamUtil.CopyStreamToByteArray(stream);

                    BytesHash guid = HashUtil.GetSHA512Hash(bytes);
                    if (guids.ContainsKey(guid))
                    {
                        Debug.WriteLine("Deleting " + fileName);
                        File.Delete(fileName);
                    }
                    else
                    {
                        guids.Add(guid, fileName);
                    }
                }
            }

            private void CollectAllFiles(string dir)
            {
                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
                    mAllFiles.Add(file);

                string[] subDirs = Directory.GetDirectories(dir);
                foreach (string subDir in subDirs)
                    CollectAllFiles(subDir);
            }
        }

        private static void TestGoogleDirectory(string dir, LoadFormat loadFormat)
        {
            Debug.WriteLine("");
            Debug.WriteLine("Reading directory {0}.", dir);

            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
                TestGoogleFile(file, loadFormat);

            string[] subDirs = Directory.GetDirectories(dir);
            foreach (string subDir in subDirs)
                TestGoogleDirectory(subDir, loadFormat);
        }

        private static void TestGoogleFile(string fileName, LoadFormat loadFormat)
        {
            try
            {
                Debug.Write(fileName);
                DateTime start = DateTime.Now;
                
                Document doc = new Document(fileName);

                if (doc.OriginalLoadFormat != loadFormat)
                {
                    string dstDir = @"F:\GoogleProof\111_" + FileFormatUtil.LoadFormatToExtension(doc.OriginalLoadFormat).Substring(1);
                    if (!Directory.Exists(dstDir))
                        Directory.CreateDirectory(dstDir);

                    MoveFile(
                        fileName, 
                        dstDir, 
                        FileFormatUtil.LoadFormatToExtension(doc.OriginalLoadFormat), 
                        string.Format("Not an {0} file.", FileFormatUtil.LoadFormatToExtension(loadFormat)));
                }
                else
                {
                    TimeSpan elapsed = DateTime.Now - start;
                    Debug.WriteLine(" " + elapsed.TotalSeconds.ToString("F2") + "s.");
                    if (elapsed.TotalSeconds > 3)
                        Debug.WriteLine("*** SLOW ***");
                }
            }
            catch (UnsupportedFileFormatException e)
            {
                MoveFile(fileName, @"F:\GoogleProof\111_Unsupported", null, e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("");
                Debug.WriteLine("File '" + fileName + "'. " + e.ToString());
            }
        }

        private static void MoveFile(string fileName, string newDir, string newExtension, string reason)
        {
            string dstFileName = Path.Combine(newDir, Path.GetFileName(fileName));

            if (StringUtil.HasChars(newExtension))
                dstFileName = Path.ChangeExtension(dstFileName, newExtension);

            if (File.Exists(dstFileName))
                dstFileName = dstFileName + Guid.NewGuid().ToString() + Path.GetExtension(dstFileName);

            Debug.WriteLine("");
            Debug.WriteLine(string.Format("Moving file {0} to {1}. {2}", fileName, dstFileName, reason));
            File.Move(fileName, dstFileName);
        }

        [Test, Ignore("GoogleProof")]
        public void TesXXX()
        {
            TestUtil.Open(@"E:\GoogleProof\Doc\mazda\39.doc");
        }
    }
}
