// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/04/2016 by Andrey Noskov

using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fonts
{
    [TestFixture]
    [JavaDelete("Used only on .Net by design.")]
    public class TestFonts
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Generate font listing for standard system.
        /// Run this test after system font changes on standard system only.
        /// The result of running this test is StandardFontsListing.xml file which contains listing of all fonts installed 
        /// inside standard system.  
        /// Please read "What to do if unit tests are failing?" article in the wiki to get more information:
        /// https://auckland.dynabic.com/wiki/pages/viewpage.action?pageId=787655
        /// </summary>
        [Test, Ignore("TestFonts")]
        public void GenerateStandardFontListing()
        {
            FontLister lister = new FontLister();
            FontListing localListing = lister.GetListing();

            string standardListingPath = TestUtil.GetInTestGoldPath(StandardFontsListing);
            using (FileStream file = File.Create(standardListingPath))
            {
                localListing.Serialize(file);
            }
        }

#if !NETSTANDARD 
        // Serialization failed on .NET Standard. There was an error generating the XML document. 
        // System.InvalidOperationException : Instance validation error: '3' is not a valid value for System.Drawing.FontStyle.
        [Test]
        public void CompareStandardAndLocalFonts()
        {
            FontLister lister = new FontLister();
            FontListing localListing = lister.GetListing();

            string outPath = TestUtil.GetInTestOutPath(LocalFontsListing);
            using (FileStream file = File.Create(outPath))
            {
                localListing.Serialize(file);
            }

            string standardListingPath = TestUtil.GetInTestGoldPath(StandardFontsListing);
            if (!File.Exists(standardListingPath))
            {
                Debug.WriteLine("WARNING: StandardFontsListing.xml not found. No font comparison is conducted.");
                return;
            }

            FontListing standardListing;
            using (FileStream file = File.OpenRead(standardListingPath))
            {
                standardListing = FontListing.Deserialize(file);
            }

            FontComparer comparer = new FontComparer();
            ComparsionResult result = comparer.CompareListing(localListing, standardListing);

            outPath = TestUtil.GetInTestOutPath(ComparsionFontsResult);
            using (FileStream file = File.Create(outPath))
            {
                result.Serialize(file);
            }

            result.Differences.Sort(new ComparsionResultComparer());
            foreach (ComparsionDifference diff in result.Differences)
            {
                Debug.WriteLine(diff.GetDiffDetails());
            }
        }
#endif

        private const string StandardFontsListing = "StandardFontsListing.xml";
        private const string LocalFontsListing = "LocalFontsListing.xml";
        private const string ComparsionFontsResult = "ComparsionFontsResult.xml";
    }

    internal class ComparsionResultComparer : IComparer<ComparsionDifference>
    {
        public int Compare(ComparsionDifference x, ComparsionDifference y)
        {
            int xWeight = GetDifferenceWeight(x);
            int yWeight = GetDifferenceWeight(y);
            return (xWeight != yWeight)
                ? xWeight.CompareTo(yWeight)
                : string.CompareOrdinal(x.FontFullName, y.FontFullName);
        }

        private static int GetDifferenceWeight(ComparsionDifference value)
        {
            if (value is MissingFont)
                return -3;
            if (value is DifferentFont)
                return -2;
            if (value is ExtraFont)
                return -1;

            Debug.Assert(false);
            return 0;
        }
    }
}
