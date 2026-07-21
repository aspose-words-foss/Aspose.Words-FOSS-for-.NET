// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2012 by Alexey Noskov

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    internal class DmlChartValueCollection : DmlExtensionListSource, IEnumerable<DmlChartValue>
    {
        internal DmlChartValueCollection(DmlChartValueType valueType)
        {
            mValueType = valueType;
        }

        /// <summary>
        /// Adds the specified chart data value to the collection.
        /// </summary>
        internal void Add(DmlChartValue value)
        {
            if (value == null)
                return;
            
            mValues[value.Index] = value;

            if (value.Index > LastNonEmptyValueIndex)
                LastNonEmptyValueIndex = value.Index;

            MarkAsChanged();
            value.Collection = this;
        }

        /// <summary>
        /// Adds the specified chart data value to the collection.
        /// </summary>
        /// <remarks>
        /// If the value is <b>null</b>, the existing value with the specified index is removed from the collection.
        /// If the value is a multi-level value, missing levels are added to the value or to all existing values in
        /// the collection.
        /// </remarks>
        internal void AddNullAware(DmlChartValue value, int index)
        {
            if (value != null)
            {
                AddMissingLevelsToCollectionOrValue(value);
                Add(value);
            }
            else
            {
                mValues.Remove(index);

                if (mLastNonEmptyValueIndex == index)
                    CalculateLastNonEmptyValueIndex();

                MarkAsChanged();
            }
        }

        /// <summary>
        /// Inserts the specified chart data value to the collection at the specified index.
        /// </summary>
        /// <remarks>
        /// The indexes of existing values that have an index equal to or greater than the specified one are
        /// incremented by 1.
        /// If a multi-level value is inserted, missing levels are added to the value or to all existing values in
        /// the collection.
        /// The value can be <b>null</b>.
        /// </remarks>
        internal void Insert(int index, DmlChartValue value)
        {
            IntToObjDictionary<DmlChartValue> values = new IntToObjDictionary<DmlChartValue>(mValues.Count + 1);

            foreach (DmlChartValue currentValue in mValues.Values)
            {
                if (currentValue.Index >= index)
                    currentValue.Index++;

                values[currentValue.Index] = currentValue;
            }

            mValues = values;

            if (value != null)
            {
                AddMissingLevelsToCollectionOrValue(value);
                values.Add(index, value);
                value.Collection = this;
                mLastNonEmptyValueIndex = System.Math.Max(mLastNonEmptyValueIndex + 1, index);
            }
            else
            {
                if (index < mLastNonEmptyValueIndex)
                    mLastNonEmptyValueIndex++;
            }

            ValueCount++;

            MarkAsChanged();
        }

        /// <summary>
        /// Removes the specified chart data value from the collection at the specified index.
        /// </summary>
        /// <remarks>
        /// The indexes of existing values after the specified one are decremented by 1.
        /// </remarks>
        internal void Remove(int index)
        {
            if (ValueCount > index)
                ValueCount--;

            if (index > mLastNonEmptyValueIndex)
                return;

            int lastIndex = -1;
            IntToObjDictionary<DmlChartValue> values = new IntToObjDictionary<DmlChartValue>(mValues.Count);

            foreach (DmlChartValue value in mValues.Values)
            {
                if (value.Index == index)
                    continue;

                if (value.Index > index)
                    value.Index--;

                values[value.Index] = value;

                if (lastIndex < value.Index)
                    lastIndex = value.Index;
            }

            mValues = values;
            mLastNonEmptyValueIndex = lastIndex;

            MarkAsChanged();
        }

        /// <summary>
        /// Removes the all values from the collection.
        /// </summary>
        internal void Clear()
        {
            // Remove the all level properties except the last one.
            while ((mLevelProperties != null) && (mLevelProperties.Count > 1))
                mLevelProperties.RemoveAt(0);

            mLastNonEmptyValueIndex = -1;
            ValueCount = 0;
            mValues.Clear();

            MarkAsChanged();
        }

        internal DmlChartValueCollection Clone()
        {
            DmlChartValueCollection lhs = (DmlChartValueCollection)MemberwiseClone();
            lhs.mValues = new IntToObjDictionary<DmlChartValue>();

            if (mLevelProperties != null)
            {
                lhs.mLevelProperties = new List<DmlChartDataLevelProperties>(mLevelProperties.Count);
                foreach (DmlChartDataLevelProperties properties in mLevelProperties)
                    lhs.mLevelProperties.Add(properties.Clone());
            }

            foreach (DmlChartValue value in mValues.Values)
            {
                DmlChartValue clonedValue = value.Clone();
                clonedValue.Collection = lhs;

                DmlChartMultiLvlNumValue numValue = clonedValue as DmlChartMultiLvlNumValue;
                if (numValue != null)
                    numValue.SetLevelProperties(lhs.mLevelProperties);

                lhs.Add(clonedValue);
            }

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Adds the specified properties to the level properties list.
        /// </summary>
        internal void AddLevelProperties(DmlChartDataLevelProperties levelProperties)
        {
            if (mLevelProperties == null)
                mLevelProperties = new List<DmlChartDataLevelProperties>();

            mLevelProperties.Add(levelProperties);
        }

        /// <summary>
        /// Sets the <see cref="DmlChartDataLevelProperties.ValueCount"/> level property to the specified value for all
        /// levels of the collection.
        /// </summary>
        internal void SetValueCountForAllLevels(int valueCount, int levelCount)
        {
            for (int i = 0; i < levelCount; i++)
            {
                if (i >= mLevelProperties.Count)
                    mLevelProperties.Add(new DmlChartDataLevelProperties());

                mLevelProperties[i].ValueCount = valueCount;
            }
        }

        /// <summary>
        /// Fills this <see cref="DmlChartValueCollection"/> with "dummy" values. The collection is expected to be
        /// empty with defined <see cref="ValueCount"/> property.
        /// </summary>
        internal void FillWithDummyValues()
        {
            Debug.Assert(mValues.Count == 0);

            for (int i = 0; i < ValueCount; i++)
                Add(new DmlChartDummyValue(i, (i + 1)));
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<DmlChartValue> GetEnumerator()
        {
            return mValues.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Marks the collection as modified by incrementing <see cref="ValueChangeCount"/>.
        /// </summary>
        internal void MarkAsChanged()
        {
            mValueChangeCount++;
        }

        /// <summary>
        /// Returns the first non-empty value in this collection.
        /// </summary>
        internal DmlChartValue GetFirstNotNullValue()
        {
            for (int i = 0; i <= LastNonEmptyValueIndex; i++)
            {
                if (!DmlChartValue.IsNullOrNaN(this[i]))
                    return this[i];
            }

            return null;
        }

        /// <summary>
        /// If the collection contains multilevel values, and the number of levels in the existing values differs from
        /// the number of levels in the specified value, the missing levels are added to either the existing values or
        /// the specified value.
        /// </summary>
        private void AddMissingLevelsToCollectionOrValue(DmlChartValue value)
        {
            DmlChartMultiLvlValue multiLevelValue = value as DmlChartMultiLvlValue;
            if (multiLevelValue == null)
                return;

            while (mLevelProperties.Count > multiLevelValue.LevelsCount)
                multiLevelValue.Levels.Insert(0, null);

            // Make number of levels the same in the inserting and existing values.
            int levelCount = -1;
            foreach (DmlChartValue currentValue in mValues.Values)
            {
                DmlChartMultiLvlValue currentMultilevelValue = currentValue as DmlChartMultiLvlValue;
                if (currentMultilevelValue == null)
                    continue;

                if (levelCount < 0)
                    levelCount = currentMultilevelValue.LevelsCount;
                else
                    Debug.Assert(currentMultilevelValue.LevelsCount == levelCount);

                while (currentMultilevelValue.LevelsCount < multiLevelValue.LevelsCount)
                    currentMultilevelValue.Levels.Insert(0, null);
            }

            while (multiLevelValue.LevelsCount < levelCount)
                multiLevelValue.Levels.Insert(0, null);

            // Create LastLevelProperties if it does not exist yet.
            DmlChartDataLevelProperties lastLevelProperties = LastLevelProperties;
            while (mLevelProperties.Count < multiLevelValue.LevelsCount)
                mLevelProperties.Add(lastLevelProperties.Clone());
        }

        /// <summary>
        /// Calculates and sets the value of the <see cref="LastNonEmptyValueIndex"/> property.
        /// </summary>
        private void CalculateLastNonEmptyValueIndex()
        {
            int lastIndex = -1;
            foreach (DmlChartValue currentValue in mValues.Values)
            {
                if (lastIndex < currentValue.Index)
                    lastIndex = currentValue.Index;
            }

            LastNonEmptyValueIndex = lastIndex;
        }

        /// <summary>
        /// Returns a data value by an index.
        /// </summary>
        internal DmlChartValue this[int index]
        {
            get { return mValues[index]; }
        }

        /// <summary>
        /// Returns number of data values of a chart dimension including empty data values.
        /// </summary>
        internal int ValueCount
        {
            get
            {
                // It seems the last level always contains real data. The other levels may have some intermediate or
                // calculated data, for example, the 'size' dimension of Model\Charts\Word2016Charts\BoxWhisker.docx
                return LastLevelProperties.ValueCount;
            }
            set
            {
                if (ValueCount == value)
                    return;
                
                MarkAsChanged();

                // It seems the last level always contains real data. The other levels may have some intermediate or
                // calculated data, for example, the 'size' dimension of Model\Charts\Word2016Charts\BoxWhisker.docx
                // Let's keep those unknown levels for now, just update their value count.
                foreach (DmlChartDataLevelProperties levelProperties in mLevelProperties)
                {
                    if (levelProperties.ValueCount >= LastLevelProperties.ValueCount)
                        levelProperties.ValueCount = value;
                }
            }
        }

        /// <summary>
        /// Returns number of non-empty values of a chart dimension.
        /// </summary>
        internal int NonEmptyValueCount
        {
            get { return mValues.Count; }
        }

        /// <summary>
        /// Indicates whether the collection contains non-empty values.
        /// </summary>
        internal bool HasNonEmptyValues
        {
            get { return NonEmptyValueCount > 0; }
        }

        /// <summary>
        /// Gets the index of the last non-empty value in this collection.
        /// </summary>
        internal int LastNonEmptyValueIndex
        {
            get { return mLastNonEmptyValueIndex; }
            private set
            {
                mLastNonEmptyValueIndex = value;

                if (ValueCount <= mLastNonEmptyValueIndex)
                    ValueCount = mLastNonEmptyValueIndex + 1;
            }
        }

        /// <summary>
        /// Gets or sets a format code used to convert data values to their string representation.
        /// </summary>
        internal string FormatCode
        {
            get { return LastLevelProperties.FormatCode; }
            set
            {
                if (FormatCode == value)
                    return;
                
                MarkAsChanged();

                // All levels have the same data type, let's use the same format.
                foreach (DmlChartDataLevelProperties levelProperties in mLevelProperties)
                    levelProperties.FormatCode = value;
            }
        }

        /// <summary>
        /// Gets value type of items of this collection.
        /// </summary>
        internal DmlChartValueType ValueType
        {
            get { return mValueType; }
            set
            {
                if (mValueType == value)
                    return;
                
                mValueType = value;
                MarkAsChanged();
            }
        }

        /// <summary>
        /// Allows to check whether values are changed since the last access.
        /// </summary>
        internal int ValueChangeCount
        {
            get { return mValueChangeCount; }
        }

        /// <summary>
        /// Indicates whether the collection stores date or time values.
        /// </summary>
        internal bool IsDate
        {
            get { return DmlChartFormatCodeValidator.IsDateFormat(FormatCode); }
        }

        /// <summary>
        /// Gets list of dimension level properties.
        /// </summary>
        internal IList<DmlChartDataLevelProperties> LevelProperties
        {
            get { return mLevelProperties; }
        }

        /// <summary>
        /// Gets the properties of the last level.
        /// </summary>
        private DmlChartDataLevelProperties LastLevelProperties
        {
            get
            {
                if (mLevelProperties.Count == 0)
                    mLevelProperties.Add(new DmlChartDataLevelProperties());

                return mLevelProperties[mLevelProperties.Count - 1];
            }
        }

        private int mLastNonEmptyValueIndex = -1;
        private IntToObjDictionary<DmlChartValue> mValues = new IntToObjDictionary<DmlChartValue>();
        private DmlChartValueType mValueType;
        private List<DmlChartDataLevelProperties> mLevelProperties =
            new List<DmlChartDataLevelProperties>(MaxChartExLevelCount);
        private int mValueChangeCount;

        private const int MaxChartExLevelCount = 3;
    }
}
