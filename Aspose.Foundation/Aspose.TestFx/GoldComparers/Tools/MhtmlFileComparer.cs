// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Compares two MHTML files.
    /// Files are treated as text but base64 image parts are extracted and compared as images.
    /// </summary>
    internal class MhtmlFileComparer : TextFileComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            ExecuteInternal(
                comparerParams.Title,
                comparerParams.FileNameOut,
                comparerParams.FileNameGold,
                comparerParams.FileNameSrc
                );
        }

        /// <summary>
        /// Compares user file with the gold file. Brings up the user interface when there is a difference.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="userFileName">The out file.</param>
        /// <param name="goldFileName">The gold file.</param>
        /// <param name="sourceFileName">The source file name.</param>
        private static void ExecuteInternal(
            string title,
            string userFileName,
            string goldFileName,
            string sourceFileName)
        {
            MhtmlFileComparer comparer = new MhtmlFileComparer();
            comparer.ExecuteCore(title, userFileName, goldFileName, null, sourceFileName);
        }

        /// <summary>
        /// This is a template method that we overide to compare mhtml files.
        /// </summary>
        protected override void DoCompareFiles(string fileName1, string fileName2)
        {
            CompareMhtmlFiles(fileName1, fileName2);
        }

        /// <summary>
        /// Compares everything as text but base64 image parts as images.
        /// </summary>
        private static void CompareMhtmlFiles(string userFileName, string goldFileName)
        {
            if (!File.Exists(userFileName) || !File.Exists(goldFileName))
                throw new GoldDifferenceException(userFileName, goldFileName);

            using (Stream userFileStream = File.OpenRead(userFileName))
            {
                using (Stream goldFileStream = File.OpenRead(goldFileName))
                {
                    MemoryStream userTemplate = new MemoryStream();
                    List<MhtmlImagePart> userImages = new List<MhtmlImagePart>();
                    SplitMhtml(userFileStream, userTemplate, userImages);
                    userTemplate.Position = 0;

                    MemoryStream goldTemplate = new MemoryStream();
                    List<MhtmlImagePart> goldImages = new List<MhtmlImagePart>();
                    SplitMhtml(goldFileStream, goldTemplate, goldImages);
                    goldTemplate.Position = 0;

                    // This compares the readable part of MHTML.
                    CompareTextFiles(userTemplate, goldTemplate, userFileName, goldFileName);

                    // Compare images. Image count should match at this point
                    // since main comparison has succeeded.
                    int imageCount = userImages.Count;
                    Debug.Assert(imageCount == goldImages.Count);

                    for (int i = 0; i < imageCount; ++i)
                    {
                        MhtmlImagePart userImage = userImages[i];
                        MemoryStream userImageStream = new MemoryStream(userImage.ImageData);

                        MhtmlImagePart goldImage = goldImages[i];
                        MemoryStream goldImageStream = new MemoryStream(goldImage.ImageData);

                        // F0SS
                        ImageFileComparer.CompareImageFiles(userImageStream, goldImageStream, userImage.ContentLocation, goldImage.ContentLocation);
                    }
                }
            }
        }

        /// <summary>
        /// Splits MHTML archive into main "template" and image subsidiaries.
        /// </summary>
        /// <param name="fileStream">Source file, either user or gold.</param>
        /// <param name="mainTemplate">Writes Main template here.</param>
        /// <param name="imageParts">Array of image data for every binary part.</param>
        private static void SplitMhtml(Stream fileStream, Stream mainTemplate, IList<MhtmlImagePart> imageParts)
        {
            const string boundary = "--=boundary.Aspose.Words=--";

            // We don't dispose them. Otherwise they will close streams.
            StreamReader sr = new StreamReader(fileStream);
            StreamWriter sw = new StreamWriter(mainTemplate);

            string line = sr.ReadLine();
            while (line != null)
            {
                sw.WriteLine(line);

                if (line == boundary)
                {
                    string contentType = string.Empty;
                    string contentTransferEncoding = string.Empty;
                    string contentLocation = string.Empty;
                    string contentId = string.Empty;

                    // Remember the lines for the case we'll compare this part as text.
                    List<string> linesToFlush = new List<string>();

                    // This will skip empty line and stop before the base64 data
                    while (((line = sr.ReadLine()) != null) && (line.Length != 0))
                    {
                        if (GetValue(line, "Content-Type: ", ref contentType) ||
                            GetValue(line, "Content-Transfer-Encoding: ", ref contentTransferEncoding) ||
                            GetValue(line, "Content-Location: ", ref contentLocation) ||
                            GetValue(line, "Content-ID: ", ref contentId))
                        {
                            // Found
                        }

                        linesToFlush.Add(line);
                    }

                    if (contentTransferEncoding != "base64")
                    {
                        // It is not a binary. Just write as usual.
                        foreach (string line2 in linesToFlush)
                            sw.WriteLine(line2);
                        sw.WriteLine();
                    }
                    else
                    {
                        // It's a binary file. Let's substitute it with placeholder and collect to image parts.
                        if ((contentType.Length != 0) &&
                            (contentLocation.Length != 0 || contentId.Length != 0))
                        {
                            string contentUrl = (contentLocation.Length != 0)
                                ? contentLocation
                                : contentId.Substring(1, contentId.Length - 2); // Remove enclosing angle brackets.
                            byte[] data = ReadBase64Binary(sr);
                            imageParts.Add(new MhtmlImagePart(contentType, contentUrl, data));
                            sw.WriteLine("<<{0}: {1}>>", contentType, contentUrl);
                        }
                        else
                        {
                            throw new InvalidOperationException("Bad subsidiary header.");
                        }
                    }
                }

                // Read next line.
                line = sr.ReadLine();
            }

            sw.Flush();
        }

        private static bool GetValue(string line, string prefix, ref string value)
        {
            bool isFound = line.StartsWith(prefix);
            if (isFound)
                value = line.Substring(prefix.Length);
            return isFound;
        }

        private static byte[] ReadBase64Binary(TextReader tr)
        {
            StringBuilder sb = new StringBuilder(4096);
            string line = tr.ReadLine();
            while (StringUtil.HasChars(line))
            {
                sb.Append(line);
                line = tr.ReadLine();
            }

            line = sb.ToString();
            return Convert.FromBase64String(line);
        }

        /// <summary>
        /// One image part extracted from the MHTML
        /// </summary>
        internal class MhtmlImagePart
        {
            public MhtmlImagePart(string type, string location, byte[] data)
            {
                ContentType = type;
                ContentLocation = location;
                ImageData = data;
            }

            public readonly string ContentType;
            public readonly string ContentLocation;
            public readonly byte[] ImageData;
        }
    }
}
