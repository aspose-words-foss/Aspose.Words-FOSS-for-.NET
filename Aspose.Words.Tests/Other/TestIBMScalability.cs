// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2012 by Konstantin Sidorenko
using System;
using System.IO;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Other
{
    ///<summary>
    /// J-510 ibm performance issue. Should be launched explicitly from Release configuration.
    ///</summary>
    [JavaManual ("Contains too many system-specific calls.")]
    public class TestIBMScalability
    {
        ///<summary>
        /// Should be launched explicitly from Release configuration.
        ///</summary>
        [Test, Ignore("Should be launched explicitly from Release configuration.")]
        public void TestScalability()
        {
            PrintHeader();

            Run(1000, true);
            Run(2500, true);
            Run(5000, true);
            Run(10000, true);

            Run(1000, false);
            Run(2500, false);
            Run(5000, false);
            Run(10000, false);
        }

        private void Run(int records, bool sections)
        {
            string unique = Guid.NewGuid().ToString().Replace(' ', '_');
            string pdfpath = mBaseFolder + "PerformanceTest." + unique + "." + records + ".doc";

            //start
            long startTime = DateTime.Now.Ticks;

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Title;
            builder.Writeln("Test Scalability");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder.InsertParagraph();

            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            builder.InsertParagraph();

            int i = 0;
            while (i++ < records)
            {
                if (i % 200 == 0)
                {
                    if (sections)
                        builder.InsertSection(SectionStart.NewPage);
                    builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
                    builder.Writeln("Heading " + i);
                }
                else
                {
                    builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
                    builder.Writeln("Paragraph " + i);
                }

                builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
                builder.Writeln(LongText);

                builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
                builder.Writeln(LongText);

                builder.InsertParagraph();
            }
            Console.Out.Write("\r\n" + records + " records" + (sections ? " with sections:\t" : " w/o sections:\t"));
            long last = PrintTimeStamp(startTime, startTime);

            doc.UpdateFields();
            last = PrintTimeStamp(startTime, last);

            doc.Save(pdfpath);
            last = PrintTimeStamp(startTime, last);
        }

        private static void PrintHeader()
        {
            Console.Out.WriteLine("test\t\t\t\t\t\tbuild\tupdate\tsave\t(all)");
        }

        private static long PrintTimeStamp(long startTime, long lastTime)
        {
            long current = DateTime.Now.Ticks;
            long allTime = (current - startTime) / 10000000;
            long blockTime = (current - lastTime) / 10000000;

            Console.Out.Write(blockTime + " (" + allTime + ")\t");
            return current;
        }

        private readonly string mBaseFolder = Path.GetTempPath();
        private const string LongText =
            "This year, the CDL Performance Engineering Community will share a series of good articles we've found useful to you each month.  " +
            "Every articles we shared has been carefully selected and studied.  For the following two months, we will start from DB2 and Agile Performance Engineering topics.  " +
            "We have also created a forum topic for each article for you to share your impressions and questions, so feel free to click on the relevant forum link and " +
            "join the discussion!   Wish you all Happy reading!";
    }
}
