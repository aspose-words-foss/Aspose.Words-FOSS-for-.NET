// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2011 by Dmitry Vorobyev

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Aspose.Collections;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a typed collection of general formats.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class GeneralFormatCollection : IEnumerable<GeneralFormat>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="fieldCode"></param>
        internal GeneralFormatCollection(FieldCode fieldCode)
        {
            mFieldCode = fieldCode;
        }

        /// <summary>
        /// Adds a general format to the collection.
        /// </summary>
        /// <param name="item">A general format.</param>
        public void Add(GeneralFormat item)
        {
            mFieldCode.SetSwitch(FieldFormat.GeneralFormatSwitch, GeneralFormatUtil.GeneralFormatToString(item), Count);
        }

        /// <summary>
        /// Removes all occurrences of the specified general format from the collection.
        /// </summary>
        /// <param name="item">A general format.</param>
        public void Remove(GeneralFormat item)
        {
            IntList indices = new IntList();

            for (int i = 0; i < Count; i++)
            {
                if (this[i] == item)
                    indices.Add(i);
            }

            for (int i = indices.Count - 1; i >= 0; i--)
                RemoveAt(indices[i]);
        }

        /// <summary>
        /// Removes a general format occurrence at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            mFieldCode.SetSwitch(FieldFormat.GeneralFormatSwitch, null, index);
        }

        /// <summary>
        /// Gets the total number of the items in the collection.
        /// </summary>
        public int Count
        {
            [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "format", Justification = "Foreach is used for compatibility with java autoporter.")]
            get
            {
                int count = 0;

                foreach (GeneralFormat format in this)
                    count++;

                return count;
            }
        }

        /// <summary>
        /// Gets a general format at the specified index.
        /// </summary>
        /// <param name="index">The index of a general format.</param>
        /// <returns>A general format.</returns>
        public GeneralFormat this[int index]
        {
            get
            {
                int switchIndex = 0;
                foreach (FieldSwitch fieldSwitch in mFieldCode.Switches)
                {
                    if (IsValidSwitch(fieldSwitch))
                    {
                        if (switchIndex == index)
                            return GeneralFormatUtil.StringToGeneralFormat(fieldSwitch.Argument.GetNormalizedText());

                        switchIndex++;
                    }
                }

                return GeneralFormat.None;
            }
        }

        internal bool HasFormat(GeneralFormat format)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == format)
                    return true;
            }

            return false;
        }

        internal void AddOrRemove(GeneralFormat format, bool isAdd)
        {
            if (HasFormat(format) == isAdd)
                return;

            if (isAdd)
                Add(format);
            else
                Remove(format);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<GeneralFormat> GetEnumerator()
        {
            return new GeneralFormatEnumerator(mFieldCode.Switches.GetEnumerator());
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("C++ doesn't support untyped collection interfaces")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static bool IsValidSwitch(FieldSwitch fieldSwitch)
        {
            return fieldSwitch.HasName(FieldFormat.GeneralFormatSwitch) && fieldSwitch.HasArgument;
        }

        /// <summary>
        /// Returns the first numeric format encountered.
        /// </summary>
        /// <returns></returns>
        internal GeneralFormat GetNumericFormat()
        {
            foreach (GeneralFormat format in this)
            {
                if (GeneralFormatUtil.IsNumericFormat(format))
                    return format;
            }

            return GeneralFormat.None;
        }

        /// <summary>
        /// Returns string representation of the first numeric format encountered.
        /// </summary>
        /// <returns></returns>
        internal string GetNumericFormatString()
        {
            return GeneralFormatUtil.GeneralFormatToString(GetNumericFormat());
        }

        private sealed class GeneralFormatEnumerator : IEnumerator<GeneralFormat>
        {
            public GeneralFormatEnumerator(IEnumerator<FieldSwitch> switchEnumerator)
            {
                mSwitchEnumerator = new EnumeratorWrapperPalGeneric<FieldSwitch>(switchEnumerator);
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public bool MoveNext()
            {
                while (mSwitchEnumerator.MoveNext())
                {
                    FieldSwitch currentSwitch = mSwitchEnumerator.Current;
                    if (IsValidSwitch(currentSwitch))
                        return true;
                }

                return false;
            }

            public void Reset()
            {
                mSwitchEnumerator.Reset();
            }

            [JavaConvertCheckedExceptions]
            public GeneralFormat Current
            {
                get
                {
                    FieldSwitch fieldSwitch = mSwitchEnumerator.Current;
                    return GeneralFormatUtil.StringToGeneralFormat(fieldSwitch.Argument.GetNormalizedText());
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly EnumeratorWrapperPalGeneric<FieldSwitch> mSwitchEnumerator;
        }

        private readonly FieldCode mFieldCode;
    }
}
