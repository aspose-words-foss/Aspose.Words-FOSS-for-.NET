// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/04/2019 by Denis Panov

using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Words.RW.Markdown.FormatDetector;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown
{
    /// <summary>
    /// The class for testing markdown feature detectors.
    /// </summary>
    [TestFixture]
    public class TestMarkdownFeatureDetectors
    {
        /// <summary>
        /// Tests FencedCode detector.
        /// </summary>
        [Test]
        public void TestFencedCodeDetector()
        {
            FencedCodeDetector detector = new FencedCodeDetector();

            string[] input = new string[]
            {
                "   ```\n",
                "These lines is",
                "fenced code Block text...\n",
                "```\n",
                "It's simple text\n",
                "   \n",
                "    Indented code is not considered at all\n",
                "``InlineCode spans`` should be detected by EmphasisDetector, so it is not accounted here."
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests Emphasis detector.
        /// </summary>
        [Test]
        public void TestEmphasisDetector()
        {
            EmphasisDetector detector = new EmphasisDetector();
            string[] input = new string[]
            {
                "It's bold text: **type 1**, __type 2__.",
                "It's bold & italic text: ***type 1***, ___type 2___.",
                "It's ~~strikethrough~~ text",
                "``InlineCodeSpan``\n",
                "Mu`lti line\n",
                "code sp`an",
            };
            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(9));
        }
        
        /// <summary>
        /// Tests Heading detector.
        /// </summary>
        [Test]
        public void TestHeadingDetector()
        {
            HeadingDetector detector = new HeadingDetector();
            string[] input = new string[]
            {
                "# Heading 1\n",
                "## Heading 2\n",
                "### Heading 3\n",
                "#### Heading 4\n",
                "##### Heading 5\n",
                "###### Heading 6\n",
                "\n",
                "Heading 7\n",
                "--------\n",
                "Heading 8\n",
                "========\n",
                "\n",
                "> # Heading 9\n",
                "       ######## NotHeading\n",
                "\n",
                "heading1\n",
                "=======\n",
                "\n",
                "=======\n",
                " # heading2\n",
                "======="
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(11));
        }

        /// <summary>
        /// Tests Heading detector inside a Quote.
        /// </summary>
        [Test]
        public void TestHeadingDetectorInQuote()
        {
            HeadingDetector detector = new HeadingDetector();
            string[] input = new string[]
            {
                " > # Heading 1\n", 
                " >> ## Heading 2\n",
                "> ### Heading 3\n",
                "       ######## Not Heading",
                "SetextHeading\n",
                " ----------------"
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests Horizontal rule detector.
        /// </summary>
        [Test]
        public void TestHorizontalRuleDetector()
        {
            HorizontalRuleDetector detector = new HorizontalRuleDetector();
            string[] input = new string[]
            {
                "***\n",
                "---\n",
                "___\n",
                "- - - - - - -\n"
            };
            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(4));
        }


        /// <summary>
        /// Tests Link detector.
        /// </summary>
        [Test]
        public void TestLinkDetector()
        {
            LinkDetector detector = new LinkDetector();
            string[] input = new string[]
            {
                "[www.aspose.com](http://www.aspose.com)",
                "[link text](url)",
                "![image](url)",
                "[not link]  (url)",
                "[reference-style link][1]",
                "[1]:<url>"
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests Link detector for full type links.
        /// </summary>
        [Test]
        public void TestFullLinkDetector()
        {
            LinkDetector detector = new LinkDetector();
            string[] input = new string[]
            {
                "[reference-style link1][1]",
                "[reference-style link2][1]",
                "[reference-style link3][1]",
                "[1]:<url>"
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests this is not a Link detector because there is no link definition for links.
        /// </summary>
        [Test]
        public void TestNotLink()
        {
            LinkDetector detector = new LinkDetector();
            string[] input = new string[]
            {
                "[reference-style link1][1]",
                "[reference-style link2][1]",
                "[reference-style link3][1]"
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(0));
        }


        /// <summary>
        /// Tests List detector.
        /// </summary>
        [Test]
        public void TestListDetector()
        {
            ListDetector detector = new ListDetector();
            string[] input = new string[]
            {
                "List 1:\n",
                "1. Item 1\n",
                "  1. Item 2\n",
                "  9. Item 3\n",
                "List 2:\n",
                "- Item 1\n",
                "  - Item 1\n",
                "List 3:\n",
                "+ Item 1\n",
                "  + Item 1\n",
                "List 4:\n",
                "* Item 1\n",
                "  * Item 1\n"
            };

            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests Quote detector.
        /// </summary>
        [Test]
        public void TestQuoteDetector()
        {
            QuoteDetector detector = new QuoteDetector();
            string[] input = new string[]
            {
                "> Quote 1 line 1\n",
                "> Quote 1 line 2\n",
                "\n",
                ">>> Quote 2\n",
                "\n",
                ">Quote 3\n"
            };
            int count = ExecuteDetector(detector, input);
            Assert.That(count, Is.EqualTo(3));
        }

        /// <summary>
        /// Executes a specified detector over a specified input data.
        /// </summary>
        /// <returns>A number of the detected markdown features.</returns>
        private static int ExecuteDetector(MarkdownDetectorBase detector, string[] inputData)
        {
            int count = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                foreach (string s in inputData)
                    writer.Write(s);

                writer.Flush();
                
                stream.Seek(0, SeekOrigin.Begin);
                
                MarkdownDetectorContext context = new MarkdownDetectorContext(new CustomTextReader(stream));
                while (context.ReadLine())
                {
                    if (detector.DetectAndUpdateContext(context))
                        count += detector.Count;
                }

                writer.Close();
            }

            return count;
        }
    }
}
