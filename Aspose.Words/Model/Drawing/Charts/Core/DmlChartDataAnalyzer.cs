// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2019 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Contains tools to obtain specific information from chart data stored in <see cref="DmlChartValueCollection"/>.
    /// </summary>
    /// <dev>
    /// For performance reasons, collected information is cached in instances of this class and returned without
    /// processing data on the next call if the data has not been changed. The fact that the data is changed is checked
    /// using the <see cref="DmlChartValueCollection.ValueChangeCount"/> property.
    /// </dev>
    internal class DmlChartDataAnalyzer
    {
        /// <summary>
        /// Calculates range of numeric values stored in the collection.
        /// </summary>
        internal ValueRange GetValueRange(DmlChartValueCollection collection)
        {
            Debug.Assert((collection.ValueType == DmlChartValueType.Numeric) ||
                (collection.ValueType == DmlChartValueType.MultiLvlNumeric));

            if (!collection.HasNonEmptyValues)
                return null;

            if ((mCachedValueRange != null) && mCachedValueRange.IsActual(collection))
                return mCachedValueRange.Range;

            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (DmlChartValue value in collection)
            {
                if (collection.ValueType == DmlChartValueType.MultiLvlNumeric)
                {
                    DmlChartMultiLvlValue multiLevelValue = (DmlChartMultiLvlValue)value;

                    foreach (object levelValue in multiLevelValue.Levels)
                    {
                        if (levelValue == null)
                            continue;

                        double numericValue = (double)levelValue;

                        if (numericValue < min)
                            min = numericValue;

                        if (numericValue > max)
                            max = numericValue;
                    }
                }
                else
                {
                    double numericValue = value.Value;

                    if (numericValue < min)
                        min = numericValue;

                    if (numericValue > max)
                        max = numericValue;
                }
            }

            ValueRange range = new ValueRange(min, max);

            mCachedValueRange = new CachedValueRange(range, collection);

            return range;
        }

        /// <summary>
        /// Calculates number of unique categories in the collection.
        /// </summary>
        internal int GetUniqueCategoryCount(DmlChartValueCollection collection, bool calcEmptyCategories)
        {
            Debug.Assert(collection.ValueType == DmlChartValueType.MultiLvlString,
                "The method is used for this point type only");

            if ((mCachedUniqueCategoryCount != null) &&
                mCachedUniqueCategoryCount.IsActual(collection, calcEmptyCategories))
            {
                return mCachedUniqueCategoryCount.Count;
            }

            // Sort values by index.
            List<DmlChartMultiLvlValue> values = GetSortedMultiLevelValues(collection);

            IntToObjDictionary<StringList> uniqueValuesByLevels = new IntToObjDictionary<StringList>();
            int lastIndex = -1;
            bool addEmptyCategories = false;

            foreach (DmlChartMultiLvlValue multiLevelValue in values)
            {
                // If there is a missing point, it is intended that it has an empty category.
                addEmptyCategories = addEmptyCategories ||
                    (calcEmptyCategories && (multiLevelValue.Index != lastIndex + 1));
                lastIndex = multiLevelValue.Index;

                for (int levelIndex = 0; levelIndex < multiLevelValue.LevelsCount; levelIndex++)
                {
                    string level = NullToEmptyString((string)multiLevelValue.Levels[levelIndex]);

                    if (string.IsNullOrEmpty(level) && !calcEmptyCategories)
                        continue;

                    StringList list = uniqueValuesByLevels[levelIndex];
                    if (list == null)
                    {
                        list = new StringList();
                        uniqueValuesByLevels.Add(levelIndex, list);
                    }

                    AddIfNotExists(list, level.ToUpper());
                }
            }

            if (addEmptyCategories)
            {
                foreach (StringList list in uniqueValuesByLevels.Values)
                    AddIfNotExists(list, "");
            }

            int count = GetTotalItemCount(uniqueValuesByLevels);
            mCachedUniqueCategoryCount = new CachedUniqueCategoryCount(count, collection, calcEmptyCategories);

            return count;
        }

        /// <summary>
        /// Gets aggregated category info of the Box and Whisker chart data value of the specified X multi-level
        /// string data collections.
        /// </summary>
        internal AggregatedCategory GetBoxWhiskerCategoryInfo(DmlChartValueCollection xValues,
            DmlChartValueCollection yValues, int valueIndex)
        {
            // The method is used for the multi-level point type only.
            Debug.Assert(xValues.ValueType == DmlChartValueType.MultiLvlString);

            DmlChartMultiLvlStrValue xValue = (DmlChartMultiLvlStrValue)xValues[valueIndex];
            string targetCategoryName = (xValue != null)
                ? NullToEmptyString((string)xValue.Levels[0])
                : string.Empty;

            List<Category> categories = new List<Category>();
            // A label for an empty Y value in a Box&Whisker chart has the largest index: use double.MaxValue here.
            DmlChartValue yValue = yValues[valueIndex];
            double valueY = (yValue != null) ? yValue.Value : double.MaxValue;

            AggregatedCategory targetCategory = new AggregatedCategory();
            targetCategory.CategoryIndex = int.MaxValue;
            targetCategory.IsSingleValueCategory = true;
            targetCategory.LastOrderedValueIndex = -1;

            // Calculate the category index and chart data value index assuming that the chart values are grouped
            // into categories by their X value with sorting chart data values within a category by their Y value.
            // The categories are arranged in the order they appear in the data.

            for (int i = 0; i <= System.Math.Max(valueIndex, xValues.LastNonEmptyValueIndex); i++)
            {
                DmlChartMultiLvlStrValue value = (DmlChartMultiLvlStrValue)xValues[i];

                // If there is a missing point, it is intended that it has an empty category.
                string categoryName = (value != null) ? NullToEmptyString((string)value.Levels[0]) : string.Empty;

                double y = (yValues[i] != null) ? yValues[i].Value : double.MaxValue;
                int categoryIndex = FindOrCreateCategory(categories, categoryName, 0, i);

                if (categoryName != targetCategoryName)
                {
                    if (categoryIndex < targetCategory.CategoryIndex)
                    {
                        // The processing value has a point/label before than a point/label of the data value with
                        // index valueIndex.
                        targetCategory.OrderedValueIndex++;
                        targetCategory.LastOrderedValueIndex++;
                    }

                    continue;
                }

                // If the processing value has the same category, and its Y value is less than Y value of the data value
                // with the index valueIndex, its point/label is located before the value valueIndex.
                if ((y < valueY) ||
                    ((i < valueIndex) && MathUtil.AreEqual(y, valueY)))
                {
                    targetCategory.OrderedValueIndex++;
                }

                targetCategory.LastOrderedValueIndex++;

                targetCategory.CategoryIndex = categoryIndex;
                if (i != valueIndex)
                    targetCategory.IsSingleValueCategory = false;
            }

            targetCategory.CategoryCount = categories.Count;

            return targetCategory;
        }

        /// <summary>
        /// Gets aggregated category info of the Pareto chart data value of the specified X multi-level
        /// string data collections.
        /// </summary>
        internal AggregatedCategory GetParetoCategoryInfo(DmlChartValueCollection xValues,
            DmlChartValueCollection yValues, int valueIndex)
        {
            // The method is used for the multi-level point type only.
            Debug.Assert(xValues.ValueType == DmlChartValueType.MultiLvlString);

            int xValueLevel = xValues.LevelProperties.Count - 1;

            DmlChartMultiLvlStrValue xValue = (DmlChartMultiLvlStrValue)xValues[valueIndex];
            if (xValue == null)
                return null; // No point for this value.

            string targetCategoryName = NullToEmptyString((string)xValue.Levels[xValueLevel]);
            if (targetCategoryName == string.Empty)
                return null; // No point for this value.

            List<Category> categories = new List<Category>();

            AggregatedCategory targetCategory = new AggregatedCategory();
            targetCategory.IsSingleValueCategory = true;

            // Calculate the category index assuming that the chart values are grouped into categories by their X value
            // with sorting the categories by total Y value.

            for (int i = 0; i <= System.Math.Max(valueIndex, xValues.LastNonEmptyValueIndex); i++)
            {
                DmlChartMultiLvlStrValue value = (DmlChartMultiLvlStrValue)xValues[i];
                if (value == null)
                    continue;

                string categoryName = NullToEmptyString((string)value.Levels[xValueLevel]);
                if (categoryName == string.Empty)
                    continue;

                int categoryIndex = FindOrCreateCategory(categories, categoryName, xValueLevel, i);
                if (yValues[i] != null)
                    categories[categoryIndex].TotalY += yValues[i].Value;

                if ((i != valueIndex) && (categoryName == targetCategoryName))
                    targetCategory.IsSingleValueCategory = false;
            }

            categories.Sort(new CategoryComparerByTotalYDesc());
            targetCategory.CategoryIndex = FindCategory(categories, targetCategoryName);
            targetCategory.CategoryCount = categories.Count;

            return targetCategory;
        }

        /// <summary>
        /// Get category infos of multi-level data of Sunburst or Treemap chart series.
        /// </summary>
        /// <remarks>
        /// Here, a category means a branch, stem, or leaf of a Sunburst or Treemap chart. A branch is the highest level
        /// of data, a stem is a middle level and a leaf is the lowest level.
        /// A level has its own category if the level or any of the higher levels is different than the corresponding
        /// level of the previous chart data value. And all lowest levels have own categories.
        /// In the difference with behaviour of the <see cref="GetUniqueCategoryCount"/> method, order of category values
        /// is important. The 'category1' items in the example below are treated as two different categories by this method:
        /// 'category1', 'category2', 'category1'.
        /// </remarks>
        internal List<Category> GetCategories(DmlChartValueCollection collection)
        {
            Debug.Assert(collection.ValueType == DmlChartValueType.MultiLvlString,
                "The method is intended for this point type only");

            List<Category> categories = new List<Category>();
            if ((mCachedCategories != null) && mCachedCategories.IsActual(collection))
            {
                categories.AddRange(mCachedCategories.Categories);
                return categories;
            }

            // Sort values by index.
            List<DmlChartMultiLvlValue> values = GetSortedMultiLevelValues(collection);

            IntToObjDictionary<string> previousValueLevels = new IntToObjDictionary<string>();

            foreach (DmlChartMultiLvlValue multiLevelValue in values)
            {
                bool isHigherLevelDifferent = false;
                // Need return categories from higher level to lower, levels of DmlChartMultiLvlValue are in reverse order.
                for (int i = multiLevelValue.LevelsCount - 1; i >= 0; i--)
                {
                    string level = (string)multiLevelValue.Levels[i];
                    if (string.IsNullOrEmpty(level))
                        continue;

                    // A level has its own category if the level or any of the higher levels is different than the
                    // corresponding level of the previous chart data value. And all lowest levels have own categories.
                    if (isHigherLevelDifferent || (level != previousValueLevels[i]) || (i == 0))
                    {
                        categories.Add(new Category(level, categories.Count, i, multiLevelValue.Index));
                        previousValueLevels[i] = level;
                        isHigherLevelDifferent = true;
                    }
                }
            }

            mCachedCategories = new CachedCategories(categories.ToArray(), collection);

            return categories;
        }

        /// <summary>
        /// Gets plot infos of a Box and Whisker chart.
        /// </summary>
        internal List<BoxAndWhiskerPlot> GetBoxAndWhiskerPlots(DmlChartValueCollection categories,
            DmlChartValueCollection values, QuartileMethod quartileMethod)
        {
            Debug.Assert(
                (categories.ValueType == DmlChartValueType.MultiLvlString) &&
                    (values.ValueType == DmlChartValueType.MultiLvlNumeric),
                "The method is intended for this point type only");

            List<BoxAndWhiskerPlot> list = new List<BoxAndWhiskerPlot>();
            if ((mCachedBoxAndWhiskerPlots != null) &&
                mCachedBoxAndWhiskerPlots.IsActual(categories, values, quartileMethod))
            {
                list.AddRange(mCachedBoxAndWhiskerPlots.Plots);
                return list;
            }

            Dictionary<int, string> categoryByIndex = new Dictionary<int, string>();
            foreach (DmlChartValue value in categories)
            {
                DmlChartMultiLvlValue multiLevelValue = (DmlChartMultiLvlValue)value;
                Debug.Assert(multiLevelValue.LevelsCount == 1);
                categoryByIndex.Add(multiLevelValue.Index, multiLevelValue.StringValue);
            }

            StringToObjDictionary<DoubleList> valuesByCategory = new StringToObjDictionary<DoubleList>();

            foreach (DmlChartValue value in values)
            {
                DmlChartMultiLvlValue multiLevelValue = (DmlChartMultiLvlValue)value;
                Debug.Assert(multiLevelValue.LevelsCount == 1);

                // Categories are case sensitive. It seems nulls are possible.
                string category = NullToEmptyString(categoryByIndex.GetValueOrNull(multiLevelValue.Index));

                DoubleList categoryValues = valuesByCategory[category];
                if (categoryValues == null)
                {
                    categoryValues = new DoubleList();
                    valuesByCategory.Add(category, categoryValues);
                }

                categoryValues.Add(multiLevelValue.Value);
            }

            StringToObjDictionary<DoubleList>.Enumerator enumerator = valuesByCategory.GetEnumerator();
            while (enumerator.MoveNext())
                list.Add(CalculateBoxAndWhiskerPlot(enumerator.CurrentKey, enumerator.CurrentValue, quartileMethod));

            mCachedBoxAndWhiskerPlots = new CachedBoxAndWhiskerPlots(list.ToArray(), categories, values, quartileMethod);

            return list;
        }

        /// <summary>
        /// Searches for a category with the specified name in the list. If the category is not found, creates it and
        /// adds to the list. Returns index of the category.
        /// </summary>
        private static int FindOrCreateCategory(List<Category> categories, string name, int levelIndex, int valueIndex)
        {
            int index = FindCategory(categories, name);
            if (index >= 0)
                return index;

            Category category = new Category(name, categories.Count, levelIndex, valueIndex);
            categories.Add(category);
            return category.Index;
        }

        private static int FindCategory(List<Category> categories, string name)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Name == name)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the total number of items in the string lists contained in the specified dictionary.
        /// </summary>
        private static int GetTotalItemCount(IntToObjDictionary<StringList> uniqueValuesByLevels)
        {
            int count = 0;

            foreach (StringList list in uniqueValuesByLevels.Values)
                count += list.Count;

            return count;
        }

        /// <summary>
        /// Adds the value to the list if it is not already in it.
        /// </summary>
        private static void AddIfNotExists(StringList sortedList, string value)
        {
            int index = sortedList.BinarySearch(value);
            if (index < 0)
                sortedList.Insert(~index, value);
        }

        private static string NullToEmptyString(string value)
        {
            return (value != null) ? value : string.Empty;
        }

        /// <summary>
        /// Calculates plot parameters of Box and Whisker category. Mimics MS Word behavior.
        /// </summary>
        private static BoxAndWhiskerPlot CalculateBoxAndWhiskerPlot(string category, DoubleList values,
            QuartileMethod quartileMethod)
        {
            Debug.Assert(values.Count > 0);

            if (values.Count == 1)
            {
                double value = values[0];
                return new BoxAndWhiskerPlot(category, value, value, value, value, value, value, 0, 0, values.ToArray());
            }

            bool includeMedian = quartileMethod == QuartileMethod.InclusiveMedian;

            values.Sort();
            int midpoint = values.Count / 2;
            double median = (values.Count % 2 == 0)
                ? (values[midpoint] + values[midpoint - 1]) / 2
                : values[midpoint];

            double sum = 0;
            for (int i = 0; i < values.Count; i++)
                sum += values[i];
            double mean = sum / values.Count;

            if (values.Count <= 3)
            {
                int lastIndex = values.Count - 1;
                double lowerQuartile = includeMedian ? (values[0] + median) / 2 : values[0];
                double upperQuartile = includeMedian ? (values[lastIndex] + median) / 2 : values[lastIndex];

                return new BoxAndWhiskerPlot(category, values[0], lowerQuartile, median, upperQuartile,
                    values[lastIndex], mean, 0, 0, values.ToArray());
            }

            int quarter = (midpoint + (includeMedian ? 1 : 0)) / 2;
            int quarter3 = values.Count - 1 - quarter;
            double q1;
            double q3;

            switch ((values.Count + (includeMedian ? 2 : 0)) % 4)
            {
                case 0:
                    q1 = (values[quarter] + 3 * values[quarter - 1]) / 4;
                    q3 = (values[quarter3] + 3 * values[quarter3 + 1]) / 4;
                    break;
                case 1:
                    q1 = (values[quarter] + values[quarter - 1]) / 2;
                    q3 = (values[quarter3] + values[quarter3 + 1]) / 2;
                    break;
                case 2:
                    q1 = (3 * values[quarter] + values[quarter - 1]) / 4;
                    q3 = (3 * values[quarter3] + values[quarter3 + 1]) / 4;
                    break;
                case 3:
                default:
                    q1 = values[quarter];
                    q3 = values[quarter3];
                    break;
            }

            double lowerExtreme = q1 - 1.5 * (q3 - q1);
            int lowerOutlierCount = 0;
            while (values[lowerOutlierCount] < lowerExtreme)
                lowerOutlierCount++;
            lowerExtreme = values[lowerOutlierCount];

            double upperExtreme = q3 + 1.5 * (q3 - q1);
            int index = values.Count - 1;
            while (values[index] > upperExtreme)
                index--;
            upperExtreme = values[index];
            int upperOutlierCount = values.Count - index - 1;

            return new BoxAndWhiskerPlot(category, lowerExtreme, q1, median, q3, upperExtreme, mean,
                lowerOutlierCount, upperOutlierCount, values.ToArray());
        }

        /// <summary>
        /// Returns a list of chart data values of the specified collection sorted by the index.
        /// </summary>
        private static List<DmlChartMultiLvlValue> GetSortedMultiLevelValues(DmlChartValueCollection collection)
        {
            Debug.Assert((collection.ValueType == DmlChartValueType.MultiLvlString) ||
                (collection.ValueType == DmlChartValueType.MultiLvlNumeric));

            List<DmlChartMultiLvlValue> values = new List<DmlChartMultiLvlValue>(collection.NonEmptyValueCount);

            foreach (DmlChartValue multiLevelValue in collection)
                values.Add((DmlChartMultiLvlValue)multiLevelValue);

            values.Sort(new DmlChartMultiLvlValueComparer());

            return values;
        }

        private class DmlChartMultiLvlValueComparer : IComparer<DmlChartMultiLvlValue>
        {
            int IComparer<DmlChartMultiLvlValue>.Compare(DmlChartMultiLvlValue value1, DmlChartMultiLvlValue value2)
            {
                return value1.Index - value2.Index;
            }
        }

        private CachedValueRange mCachedValueRange;
        private CachedUniqueCategoryCount mCachedUniqueCategoryCount;
        private CachedCategories mCachedCategories;
        private CachedBoxAndWhiskerPlots mCachedBoxAndWhiskerPlots;

        internal class ValueRange
        {
            internal ValueRange(double start, double end)
            {
                Start = start;
                End = end;
            }

            internal double Start { get; private set; }
            internal double End { get; private set; }
        }

        internal class Category
        {
            internal Category(string name, int index, int levelIndex, int firstRelatedValueIndex)
            {
                Name = name;
                Index = index;
                LevelIndex = levelIndex;
                FirstRelatedValueIndex = firstRelatedValueIndex;
            }

            internal string Name { get; private set; }
            internal int Index { get; private set; }
            internal int LevelIndex { get; private set; }
            internal int FirstRelatedValueIndex { get; private set; }
            internal double TotalY { get; set; }
        }

        private class CategoryComparerByTotalYDesc : IComparer<Category>
        {
            int IComparer<Category>.Compare(Category value1, Category value2)
            {
                if (value1.TotalY > value2.TotalY)
                    return -1;

                if (value1.TotalY < value2.TotalY)
                    return 1;

                return 0;
            }
        }

        /// <summary>
        /// Represents an info of a displayed X value related to a particular chart data value in charts, in which
        /// chart data values are grouped to categories by string X value. Used for Box and Whisker and Pareto charts.
        /// </summary>
        internal class AggregatedCategory
        {
            /// <summary>
            /// The index of the category in the order in which the categories appear in the chart.
            /// </summary>
            internal int CategoryIndex { get; set; }

            /// <summary>
            /// Total number of categories in the series.
            /// </summary>
            internal int CategoryCount { get; set; }

            /// <summary>
            /// The index of the chart data value, for which this category info is retrieved, when the values are sorted
            /// by category in the category appearance order, and then by Y. This is not <see cref="DmlChartValue.Index"/>.
            /// </summary>
            internal int OrderedValueIndex { get; set; }

            /// <summary>
            /// The index of the last chart data value of the category when the values are sorted by category in the
            /// category appearance order, and then by Y.
            /// </summary>
            internal int LastOrderedValueIndex { get; set; }

            /// <summary>
            /// A flag indicating whether the category consists of only one chart data value.
            /// </summary>
            internal bool IsSingleValueCategory { get; set; }
        }

        private class CachedValueBase
        {
            internal CachedValueBase(DmlChartValueCollection collection)
            {
                mCollection = collection;
                mValueChangeCount = collection.ValueChangeCount;
            }

            /// <summary>
            /// Returns <c>true</c> if cached value is actual for the specified data collection.
            /// </summary>
            internal bool IsActual(DmlChartValueCollection collection)
            {
                return (mCollection == collection) && (mValueChangeCount == collection.ValueChangeCount);
            }

            private readonly DmlChartValueCollection mCollection;
            private readonly int mValueChangeCount;
        }

        private class CachedValueRange : CachedValueBase
        {
            internal CachedValueRange(ValueRange range, DmlChartValueCollection collection)
                : base (collection)
            {
                Range = range;
            }

            internal ValueRange Range { get; private set; }
        }

        private class CachedUniqueCategoryCount : CachedValueBase
        {
            internal CachedUniqueCategoryCount(int count, DmlChartValueCollection collection,
                bool isEmptyCategoriesIncluded)
                : base(collection)
            {
                Count = count;
                mIsEmptyCategoriesIncluded = isEmptyCategoriesIncluded;
            }

            /// <summary>
            /// Returns <c>true</c> if cached value is actual for the specified data collections.
            /// </summary>
            internal bool IsActual(DmlChartValueCollection collection, bool isEmptyCategoriesIncluded)
            {
                return base.IsActual(collection) && mIsEmptyCategoriesIncluded == isEmptyCategoriesIncluded;
            }

            internal int Count { get; private set; }

            private readonly bool mIsEmptyCategoriesIncluded;
        }

        private class CachedCategories : CachedValueBase
        {
            internal CachedCategories(Category[] categories, DmlChartValueCollection collection)
                : base(collection)
            {
                Categories = categories;
            }

            internal Category[] Categories { get; private set; }
        }

        private class CachedBoxAndWhiskerPlots
        {
            internal CachedBoxAndWhiskerPlots(BoxAndWhiskerPlot[] plots, DmlChartValueCollection dataCategories,
                DmlChartValueCollection dataValues, QuartileMethod quartileMethod)
            {
                Plots = plots;
                mDataCategories = dataCategories;
                mCategoryChangeCount = dataCategories.ValueChangeCount;
                mDataValues = dataValues;
                mValueChangeCount = dataValues.ValueChangeCount;
                mQuartileMethod = quartileMethod;
            }

            /// <summary>
            /// Returns <c>true</c> if cached value is actual for the specified data collections.
            /// </summary>
            internal bool IsActual(DmlChartValueCollection dataCategories, DmlChartValueCollection dataValues,
                QuartileMethod quartileMethod)
            {
                return
                    (mDataCategories == dataCategories) && (mCategoryChangeCount == dataCategories.ValueChangeCount) &&
                    (mDataValues == dataValues) && (mValueChangeCount == dataValues.ValueChangeCount) &&
                    (mQuartileMethod == quartileMethod);
            }

            internal BoxAndWhiskerPlot[] Plots { get; private set; }

            private readonly DmlChartValueCollection mDataCategories;
            private readonly int mCategoryChangeCount;
            private readonly DmlChartValueCollection mDataValues;
            private readonly int mValueChangeCount;
            private readonly QuartileMethod mQuartileMethod;
        }
    }
}
