// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2009 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Settings;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// A typed collection of the <see cref="OdsoFieldMapData"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/mail-merge-and-reporting/">Mail Merge and Reporting</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="OdsoFieldMapData"/>
    /// <seealso cref="Odso.FieldMapDatas"/>
    public class OdsoFieldMapDataCollection : IEnumerable<OdsoFieldMapData>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets or sets an item in this collection.
        /// </summary>
        public OdsoFieldMapData this[int index]
        {
            get { return mItems[index]; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mItems[index] = value;
            }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<OdsoFieldMapData> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an object to the end of this collection.
        /// </summary>
        /// <param name="value">The object to add. Cannot be <c>null</c>.</param>
        public int Add(OdsoFieldMapData value)
        {
            ArgumentUtil.CheckNotNull(value, "value");
            mItems.Add(value);
            return mItems.Count - 1;
        }

        /// <summary>
        /// Removes all elements from this collection.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// In the DOC file, a valid field map data collection must contain 30 elements and they 
        /// must have predefined merge field names in a specific order. 
        /// This method rearranges this contents of this collection so it becomes valid from the DOC format point of view.
        /// </summary>
        internal void MakeValid()
        {
            if (Count == 0)
                return;

            // We will temporarily place all data into this array.
            OdsoFieldMapData[] validMap = new OdsoFieldMapData[ValidMapLength];

            foreach (OdsoFieldMapData fieldMapData in this)
            {
                // Take every item from the original collection and attempt to find its presrcribed position from its name and place in the temp array.
                if (fieldMapData.Type != OdsoFieldMappingType.Null)
                {
                    PredefinedMergeFieldName predefined = MailMergeEnum.StringToPredefinedMergeFieldName(fieldMapData.MappedName);
                    if (predefined != PredefinedMergeFieldName.Invalid)
                        validMap[(int)predefined] = fieldMapData;
                }
            }

            // Remaining null elements in the temp array should be filled with empty mappings.
            for (int i = 0; i < validMap.Length; i++)
            {
                if (validMap[i] == null)
                    validMap[i] = new OdsoFieldMapData();
            }

            // Now copy the valid temp array into this collection.
            this.Clear();
            foreach (OdsoFieldMapData fieldMapData in validMap)
                this.Add(fieldMapData);
        }

        private readonly List<OdsoFieldMapData> mItems = new List<OdsoFieldMapData>();

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int ValidMapLength = 30;
    }
}
