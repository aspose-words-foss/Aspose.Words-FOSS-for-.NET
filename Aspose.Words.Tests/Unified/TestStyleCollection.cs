// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/09/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx.Pal;
using Aspose.Words.Markup;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Tests that operate on and mostly involve the style collection in the document.
    /// </summary>
    [TestFixture]
    public class TestStyleCollection
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Test that empty doc has some default styles and test some basic operations on the Styles collection.
        /// </summary>
        [Test]
        public void TestEmptyDoc()
        {
            Document doc = new Document();

            StyleCollection styles = doc.Styles;
            Assert.That(styles.Count, Is.EqualTo(4)); //Default Paragraph Font, No List, Normal, TableNormal
            Assert.That(styles.GetNextFreeIstd(), Is.EqualTo(15));

            //Styles are sorted by istd so the first is Normal
            Style style = styles[0];
            Assert.That(style.Name, Is.EqualTo("Normal"));
            Assert.That(style.Istd, Is.EqualTo(StyleIndex.Normal));

            //Test get by name
            Assert.That(styles["No List"], IsNot.Null());
            Assert.That(styles["XXX"], Is.Null);

            //Test get by istd
            Assert.That(styles.GetByIstd(StyleIndex.DefaultParagraphFont, false), IsNot.Null());
            Assert.That(styles.GetByIstd(StyleIndex.Heading1, false), Is.Null);
        }

        /// <summary>
        /// Test adding a user defined style works.
        /// </summary>
        [Test]
        public void TestAddUserStyle()
        {
            Document doc = new Document();

            Style style = doc.Styles.Add(StyleType.Paragraph, "AAA");

            Assert.That(doc.Styles["AAA"], IsNot.Null());
            Assert.That(style.Name, Is.EqualTo("AAA"));
            Assert.That(style.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
            Assert.That(style.BaseStyleName, Is.EqualTo(""));
            Assert.That(style.NextParagraphStyleName, Is.EqualTo("AAA"));
            Assert.That(style.BuiltIn, Is.EqualTo(false));
            Assert.That(style.Font.Size, Is.EqualTo(10d));
            Assert.That(style.ParagraphFormat.SpaceBefore, Is.EqualTo(0d));

            //Normal in Word2003 has font size 12pt and it will be inherited.
            style.BaseStyleName = "Normal";
            Assert.That(style.Font.Size, Is.EqualTo(12d));
        }

        /// <summary>
        /// Test that built in styles are created on demand in the document.
        /// </summary>
        [Test]
        public void TestCreateBuiltinOnDemand()
        {
            Document doc = new Document();

            //By the default only 4 styles in the collection.
            Assert.That(doc.Styles.Count, Is.EqualTo(4));

            //Requesting a built in style by name that is not in a collection, creates it.
            Style style = doc.Styles["Heading 1"];
            Assert.That(style, IsNot.Null());
            Assert.That(style.Istd, Is.EqualTo(StyleIndex.Heading1));
            // RK FOSS The default font size changed from 16 to 14 when I upgraded from Blank.doc to Blank.docx. Recheck later.
            Assert.That(style.Font.Size, Is.EqualTo(14d));
            Assert.That(doc.Styles.Count, Is.EqualTo(5));

            //Requesting a built in style by identifier also creates it.
            style = doc.Styles[StyleIdentifier.Heading2];
            Assert.That(style, IsNot.Null());
            Assert.That(doc.Styles.Count, Is.EqualTo(6));

            //Requesting a built in style with a non fixed istd, check istd is changed during import.
            style = doc.Styles["Message Header"];
            Assert.That(style, IsNot.Null());
            Assert.That(doc.Styles.Count, Is.EqualTo(7));
            //Istd and NextIstd will be 15 because its the first non fixed istd in our document.
            Assert.That(style.Istd, Is.EqualTo(15));
            Assert.That(style.NextIstd, Is.EqualTo(15));

            //Requesting a built in style indirectly also creates.
            style.NextParagraphStyleName = "Heading 3";
            Assert.That(doc.Styles.Count, Is.EqualTo(8));
        }

        /// <summary>
        /// This test checks that the embedded resource AllStyles.docx contains what we expect it to contain.
        /// </summary>
        [JavaThrows(true)]
        [Test]
        public void TestAllBuiltInStyles2007AreInResource()
        {
            Document doc = new Document();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2007);

            // This loads the document from the embedded resource.
            StyleCollection styleCollection = new StyleCollection(doc);

            StyleCollection styles = styleCollection.BuiltInStyles;

            // We expect only built-in styles here.
            foreach (Style style in styles)
            {
                // The only exception we can allow is for character styles linked to built-in paragraph styles.
                if (!style.BuiltIn)
                {
                    switch (style.Name)
                    {
                        case "Intense Quote Char":
                        case "Quote Char":
                        case "Note Heading Char":
                            break;
                        default:
                            Assert.Fail("A user defined style found: " + style.Name);
                            break;
                    }
                }
            }

            Type styleIdentifierType = typeof (StyleIdentifier);
            int[] styleIdentifiers = TestUtilPal.GetEnumValues(styleIdentifierType);

            // This is to allow porting to Java.
            foreach (int stiInt in styleIdentifiers)
            {
                StyleIdentifier sti = (StyleIdentifier) stiInt;

                if ((sti == StyleIdentifier.User) || (sti == StyleIdentifier.Nil))
                    continue;

                if (styles.GetBySti(sti, false) == null)
                {
                    switch (sti)
                    {
                        case StyleIdentifier.HtmlTopOfForm:
                        case StyleIdentifier.HtmlBottomOfForm:
                            // These are obscure or rare styles. If the user requests to create such style he will not be able
                            // to do so because the resource document does not have them and its okay.
                            break;

                        case StyleIdentifier.OutlineList1:
                        case StyleIdentifier.OutlineList2:
                        case StyleIdentifier.OutlineList3:
                            // These are list styles for outline formatting. I don't understand why MS Word does not create them in the this document.
                            // But it is okay because I think we have list style creation hardcoded somewhere in the lists code.
                            break;

                        default:
                            if (sti >= StyleIdentifier.PlainTable1 &&
                                sti <= StyleIdentifier.ListTable7ColorfulAccent6)
                                // These are list styles for tables in word 2013.
                                // They aren't exist in word 2007.
                                break;

                            Debug.WriteLine("Cannot find style in the embedded resource: " + sti.ToString());
                            break;
                    }
                }
            }

            Assert.That(styleIdentifiers.Length, Is.EqualTo(378)); // This is how many we have defined in the enum.
            Assert.That(styles.Count, Is.EqualTo(265)); // This is how many we have in the document.
            // The difference is made up of:
            // MINUS
            // 1. User
            // 2. Nil
            // 3. HtmlTopOfForm
            // 4. HtmlBottomOfForm
            // 5. OutlineList1
            // 6. OutlineList2
            // 7. OutlineList3
            // 8. from StyleIdentifier.PlainTable1 to StyleIdentifier.ListTable7ColorfulAccent6, it is 104 styles
            //
            // PLUS
            // 1. Intense Quote Char
            // 2. Quote Char
            // 3. Note Heading Char
            //
            // TOTAL DIFF = 108
        }

        /// <summary>
        /// This test checks that the embedded resource AllStyles2013.docx contains what we expect it to contain.
        /// </summary>
        [JavaThrows(true)]
        [Test]
        public void TestAllBuiltInStyles2013AreInResource()
        {
            Document doc = new Document();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2013);

            // This loads the document from the embedded resource.
            StyleCollection styleCollection = new StyleCollection(doc);

            StyleCollection styles = styleCollection.BuiltInStyles;

            // We expect only built-in styles here.
            foreach (Style style in styles)
            {
                // The only exception we can allow is for character styles linked to built-in paragraph styles.
                if (!style.BuiltIn)
                {
                    switch (style.Name)
                    {
                        case "Balloon Text Char":
                        case "Body Text 2 Char":
                        case "Body Text 3 Char":
                        case "Body Text Char":
                        case "Body Text First Indent 2 Char":
                        case "Body Text First Indent Char":
                        case "Body Text Indent 2 Char":
                        case "Body Text Indent 3 Char":
                        case "Body Text Indent Char":
                        case "Closing Char":
                        case "Comment Subject Char":
                        case "Comment Text Char":
                        case "Date Char":
                        case "Document Map Char":
                        case "E-mail Signature Char":
                        case "Endnote Text Char":
                        case "Footer Char":
                        case "Footnote Text Char":
                        case "HTML Address Char":
                        case "HTML Preformatted Char":
                        case "Header Char":
                        case "Heading 1 Char":
                        case "Heading 2 Char":
                        case "Heading 3 Char":
                        case "Heading 4 Char":
                        case "Heading 5 Char":
                        case "Heading 6 Char":
                        case "Heading 7 Char":
                        case "Heading 8 Char":
                        case "Heading 9 Char":
                        case "Intense Quote Char":
                        case "Macro Text Char":
                        case "Message Header Char":
                        case "Note Heading Char":
                        case "Plain Text Char":
                        case "Quote Char":
                        case "Salutation Char":
                        case "Signature Char":
                        case "Subtitle Char":
                        case "Title Char":
                            break;
                        default:
                            Assert.Fail("A user defined style found: " + style.Name);
                            break;
                    }
                }
            }

            Type styleIdentifierType = typeof (StyleIdentifier);
            int[] styleIdentifiers = TestUtilPal.GetEnumValues(styleIdentifierType);

            // This is to allow porting to Java.
            foreach (int stiInt in styleIdentifiers)
            {
                StyleIdentifier sti = (StyleIdentifier) stiInt;

                if ((sti == StyleIdentifier.User) || (sti == StyleIdentifier.Nil))
                    continue;

                if (styles.GetBySti(sti, false) == null)
                {
                    switch (sti)
                    {
                        case StyleIdentifier.HtmlTopOfForm:
                        case StyleIdentifier.HtmlBottomOfForm:
                            // These are obscure or rare styles. If the user requests to create such style he will not be able
                            // to do so because the resource document does not have them and its okay.
                            break;

                        case StyleIdentifier.OutlineList1:
                        case StyleIdentifier.OutlineList2:
                        case StyleIdentifier.OutlineList3:
                            // These are list styles for outline formatting. I don't understand why MS Word does not create them in the this document.
                            // But it is okay because I think we have list style creation hardcoded somewhere in the lists code.
                            break;

                        default:
                            if (sti >= StyleIdentifier.TableSimple1 &&
                                sti <= StyleIdentifier.ColorfulGridAccent6)
                                // These are list styles for tables in word 2007.
                                // They aren't exist in word 2013.
                                break;

                            Debug.WriteLine("Cannot find style in the embedded resource: " + sti.ToString());
                            break;
                    }
                }
            }

            Assert.That(styleIdentifiers.Length, Is.EqualTo(378)); // This is how many we have defined in the enum.
            Assert.That(styles.Count, Is.EqualTo(264)); // This is how many we have in the document.
            // The difference is made up of:
            // MINUS
            // 1. User
            // 2. Nil
            // 3. HtmlTopOfForm
            // 4. HtmlBottomOfForm
            // 5. OutlineList1
            // 6. OutlineList2
            // 7. OutlineList3
            // 8. from StyleIdentifier.TableSimple1 to StyleIdentifier.ColorfulGridAccent6, it is 142 styles
            //
            // PLUS 40 Linked Char Styles
            //
            // TOTAL DIFF = 109
        }

        /// <summary>
        /// WORDSNET-8813 UseDestinationStyles is not working with Heading styles.
        /// There is DOCM format, we have to get BuildIn StyleCollection depending on load format.
        /// DOCM/DOTM/DOTX etc have been added to switch.
        /// </summary>
        [Test]
        public void TestJira8813()
        {
            StyleCollection styles = StyleCollection.GetBuiltInStyles(LoadFormat.Docm);

            Assert.That(styles.Count, Is.EqualTo(265));

            //Requesting a built in style by name.
            Style style = styles["Heading 1"];
            Assert.That(style, IsNot.Null());

            Assert.That(style.Font.Color, Is.EqualTo(System.Drawing.Color.FromArgb(255, 54, 95, 145)));
        }




        [Test]
        public void TestStyleCollectionEnumerator()
        {
            Document document = TestUtil.Open(@"Model\Style\TestStyleCollectionEnumerator.docx");
            StyleCollection styles = document.Styles;

            int count = 0;
            IEnumerator<Style> enumerator = styles.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                Assert.That(current, IsNot.Null());
                count++;
            }

            Assert.That(count, Is.EqualTo(styles.Count));
        }




        private static bool ToBeDeleted(Style style)
        {
            List<StyleIdentifier> stylesToKeep = new List<StyleIdentifier>
                    {
                        StyleIdentifier.DefaultParagraphFont,
                        StyleIdentifier.Normal,
                        StyleIdentifier.NoList,
                        StyleIdentifier.TableNormal
                    };

            if (stylesToKeep.Contains(style.StyleIdentifier) || style.Name.Equals("1Bullets"))
                return false;

            return true;
        }

        private static void DeleteStyle(Style style)
        {
            Style replaceWith;
            switch (style.Type)
            {
                case StyleType.Paragraph:
                    replaceWith = style.Styles[StyleIdentifier.Normal];
                    break;
                case StyleType.Character:
                    replaceWith = style.Styles[StyleIdentifier.DefaultParagraphFont];
                    break;
                case StyleType.Table:
                    replaceWith = style.Styles[StyleIdentifier.TableNormal];
                    break;
                case StyleType.List:
                    replaceWith = style.Styles[StyleIdentifier.NoList];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string normalName = replaceWith.Name;
            replaceWith.Name = style.Name;
            replaceWith.Name = normalName;
        }
    }
}
