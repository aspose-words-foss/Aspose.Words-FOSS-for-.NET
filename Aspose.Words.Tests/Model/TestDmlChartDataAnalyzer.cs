// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2019 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests methods of the <see cref="DmlChartDataAnalyzer"/> class.
    /// </summary>
    [TestFixture]
    public class TestDmlChartDataAnalyzer
    {
        /// <summary>
        /// Tests the <see cref="DmlChartDataAnalyzer.GetValueRange"/> method.
        /// </summary>
        [Test]
        public void TestValueRange()
        {
            DmlChartValueCollection collection = new DmlChartValueCollection(DmlChartValueType.MultiLvlNumeric);
            FillMultiLvlNumValues(collection, 10, -6, 1, 34, 22, -12, -28, -3);

            DmlChartDataAnalyzer analyzer = new DmlChartDataAnalyzer();
            DmlChartDataAnalyzer.ValueRange range = analyzer.GetValueRange(collection);

            Assert.That(range.Start, Is.EqualTo(-28));
            Assert.That(range.End, Is.EqualTo(34));
        }

        /// <summary>
        /// Tests the <see cref="DmlChartDataAnalyzer.GetBoxAndWhiskerPlots"/> method.
        /// </summary>
        [TestCase(new double[] { 34 }, false, 34, 34, 34, 34, 34, 34, 0, 0)]
        [TestCase(new double[] { 34, 10 }, false, 10, 10, 22, 34, 34, 22, 0, 0)]
        [TestCase(new double[] { 34, 10, -6 }, false, -6, -6, 10, 34, 34, 12.6667, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1 }, false, -6, -4.25, 5.5, 28, 34, 9.75, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22 }, false, -6, -2.5, 10, 28, 34, 12.2, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12 }, false, -12, -7.5, 5.5, 25, 34, 8.1667, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3 }, false, -12, -6, 1, 22, 34, 6.5714, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28 }, false, -28, -10.5, -1, 19, 34, 2.25, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48 }, false, -28, -9, 1, 28, 48, 7.3333, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6 }, false, -28, -7.5, 3.5, 25, 48, 7.2, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31 }, false, -28, -6, 6, 31, 48, 9.3636, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31, 3 }, false, -28, -5.25, 4.5, 28.75, 48,
            8.8333, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31, 3, 12 }, false, -28, -4.5, 6, 26.5, 48,
            9.0769, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31, 3, 12, -12 }, false, -28, -7.5, 4.5,
            24.25, 48, 7.5714, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31, 3, 12, -12, -13 }, false, -28, -12, 3,
            22, 48, 6.2, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31, 3, 12, -12, -13, 6 }, false, -28, -10.5, 4.5,
            19.5, 48, 6.1875, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6, 31, 3, 12, -12, -13, 6, 15 }, false, 
            -28, -9, 6, 18.5, 48, 6.7059, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 128 }, false, -28, -9, 1, 28, 34, 16.2222, 0, 1)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, -120 }, false, -28, -20, -3, 16, 34, -11.3333, 1, 0)]
        [TestCase(new double[] { 34 }, true, 34, 34, 34, 34, 34, 34, 0, 0)]
        [TestCase(new double[] { 34, 10 }, true, 10, 16, 22, 28, 34, 22, 0, 0)]
        [TestCase(new double[] { 34, 10, -6 }, true, -6, 2, 10, 22, 34, 12.6667, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1 }, true, -6, -0.75, 5.5, 16, 34, 9.75, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22 }, true, -6, 1, 10, 22, 34, 12.2, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12 }, true, -12, -4.25, 5.5, 19, 34, 8.1667, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3 }, true, -12, -4.5, 1, 16, 34, 6.5714, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28 }, true, -28, -7.5, -1, 13, 34, 2.25, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48 }, true, -28, -6, 1, 22, 48, 7.3333, 0, 0)]
        [TestCase(new double[] { 34, 10, -6, 1, 22, -12, -3, -28, 48, 6 }, true, -28, -5.25, 3.5, 19, 48, 7.2, 0, 0)]
        public void TestBoxAndWhisker(double[] data, bool isMedianInclusive, double expectedLowerExtreme, 
            double expectedLowerQuartile, double expectedMedian, double expectedUpperQuartile, double expectedUpperExtreme,
            double expectedMean, int expectedLowerOutlierCount, int expectedUpperOutlierCount)
        {
            DmlChartValueCollection categories = new DmlChartValueCollection(DmlChartValueType.MultiLvlString);
            DmlChartValueCollection values = new DmlChartValueCollection(DmlChartValueType.MultiLvlNumeric);

            for (int i = 0; i < data.Length; i++)
            {
                DmlChartMultiLvlStrValue category = new DmlChartMultiLvlStrValue(i);
                category.AddLevelValue("Category", 0);
                categories.Add(category);

                DmlChartMultiLvlNumValue value = new DmlChartMultiLvlNumValue(i, null);
                value.AddLevelValue(data[i], 0);
                values.Add(value);
            }

            DmlChartDataAnalyzer analyzer = new DmlChartDataAnalyzer();
            QuartileMethod method = isMedianInclusive ? QuartileMethod.InclusiveMedian : QuartileMethod.ExclusiveMedian;
            List<BoxAndWhiskerPlot> list = analyzer.GetBoxAndWhiskerPlots(categories, values, method);
            BoxAndWhiskerPlot result = list[0];

            Assert.That(result.LowerExtreme, Is.EqualTo(expectedLowerExtreme));
            Assert.That(result.LowerQuartile, Is.EqualTo(expectedLowerQuartile));
            Assert.That(result.Median, Is.EqualTo(expectedMedian));
            Assert.That(result.UpperQuartile, Is.EqualTo(expectedUpperQuartile));
            Assert.That(result.UpperExtreme, Is.EqualTo(expectedUpperExtreme));
            Assert.That(result.Mean, Is.EqualTo(expectedMean).Within(0.0001));

            Assert.That(result.LowerOutlierCount, Is.EqualTo(expectedLowerOutlierCount));
            Assert.That(result.UpperOutlierCount, Is.EqualTo(expectedUpperOutlierCount));
        }

        /// <summary>
        /// Tests the <see cref="DmlChartDataAnalyzer.GetUniqueCategoryCount"/> method.
        /// </summary>
        [Test]
        public void TestUniqueCategoryCount()
        {
            DmlChartValueCollection collection = new DmlChartValueCollection(DmlChartValueType.MultiLvlString);
            FillMultiLvlStrValues(collection, new string[] { "category1", "category2", "Category1", "CATEGORY1" }, null);

            DmlChartDataAnalyzer analyzer = new DmlChartDataAnalyzer();
            int count = analyzer.GetUniqueCategoryCount(collection, false);

            Assert.That(count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests the <see cref="DmlChartDataAnalyzer.GetCategories"/> method.
        /// </summary>
        [Test]
        [JavaAttributes.JavaGenericArguments("ArrayList<DmlChartDataAnalyzer.Category>")]
        public void TestCategories()
        {
            DmlChartValueCollection collection = new DmlChartValueCollection(DmlChartValueType.MultiLvlString);
            FillMultiLvlStrValues(collection,
                new string[] { "branch1", "stem1", "category1", "category2", "Category1", "CATEGORY1", "stem2" },
                new int[] { 0, 1, 2, 2, 2, 2, 1 });

            DmlChartDataAnalyzer analyzer = new DmlChartDataAnalyzer();
            List<DmlChartDataAnalyzer.Category> list = analyzer.GetCategories(collection);

            Assert.That(list.Count, Is.EqualTo(7));
            CheckCategory(list[0], "branch1", 0);
            CheckCategory(list[1], "stem1", 1);
            CheckCategory(list[2], "category1", 2);
            CheckCategory(list[3], "category2", 2);
            CheckCategory(list[4], "Category1", 2);
            CheckCategory(list[5], "CATEGORY1", 2);
            CheckCategory(list[6], "stem2", 1);
        }

        /// <summary>
        /// Checks properties of the <see cref="DmlChartDataAnalyzer.Category"/> object.
        /// </summary>
        private void CheckCategory(DmlChartDataAnalyzer.Category category, string expectedName, int expectedLevel)
        {
            Assert.That(category.Name, Is.EqualTo(expectedName));
            Assert.That(category.LevelIndex, Is.EqualTo(expectedLevel));
        }

        /// <summary>
        /// Fills a data value collection of <see cref="DmlChartMultiLvlNumValue"/> type with test data.
        /// </summary>
        private static void FillMultiLvlNumValues(DmlChartValueCollection collection, params double[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                DmlChartMultiLvlNumValue value = new DmlChartMultiLvlNumValue(i, null);
                value.AddLevelValue(data[i], 0);
                collection.Add(value);
            }
        }

        /// <summary>
        /// Fills a data value collection of <see cref="DmlChartMultiLvlStrValue"/> type with test data.
        /// </summary>
        private static void FillMultiLvlStrValues(DmlChartValueCollection collection, string[] data, int[] levels)
        {
            for (int i = 0; i < data.Length; i++)
            {
                DmlChartMultiLvlStrValue value = new DmlChartMultiLvlStrValue(i);
                value.AddLevelValue(data[i], (levels != null) ? levels[i] : 0);
                collection.Add(value);
            }
        }
    }
}
