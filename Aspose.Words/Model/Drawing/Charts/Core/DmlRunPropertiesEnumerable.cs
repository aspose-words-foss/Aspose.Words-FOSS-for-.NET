// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2022 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents an enumerable over <see cref="DmlRunProperties"/> of all text elements of a paragraph
    /// list including <see cref="DmlParagraph.EndParagraphRunProperties"/> (optionally) and
    /// <see cref="DmlParagraphProperties.DefaultRunProperties"/>.
    /// </summary>
    internal class DmlRunPropertiesEnumerable : IEnumerable<DmlRunProperties>
    {
        internal DmlRunPropertiesEnumerable(IList<DmlParagraph> paragraphs, bool includeParagraphBreakProperties)
        {
            mParagraphs = paragraphs;
            mIncludeParagraphBreakProperties = includeParagraphBreakProperties;
        }

        public IEnumerator<DmlRunProperties> GetEnumerator()
        {
            return new DmlRunPropertiesEnumerator(mParagraphs, mIncludeParagraphBreakProperties);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IList<DmlParagraph> mParagraphs;
        private readonly bool mIncludeParagraphBreakProperties;

        /// <summary>
        /// Represents an enumerator over <see cref="DmlRunProperties"/> of all text elements of a paragraph
        /// list including <see cref="DmlParagraph.EndParagraphRunProperties"/> (optionally) and
        /// <see cref="DmlParagraphProperties.DefaultRunProperties"/>.
        /// </summary>
        private sealed class DmlRunPropertiesEnumerator : IEnumerator<DmlRunProperties>
        {
            internal DmlRunPropertiesEnumerator(IList<DmlParagraph> paragraphs, bool includeParagraphBreakProperties)
            {
                mParagraphs = paragraphs;
                mIncludeParagraphBreakProperties = includeParagraphBreakProperties;
            }

            public bool MoveNext()
            {
                mCurrent = null;
                if (mParagraphs == null)
                    return false;

                int paragraphCount = mParagraphs.Count;
                if ((mParagraphIndex >= paragraphCount) || (paragraphCount == 0))
                    return false;

                if (mParagraphIndex < 0)
                {
                    mParagraphIndex = 0;
                    mElementIndex = -1;
                }

                DmlParagraph paragraph = mParagraphs[mParagraphIndex];
                int elementCount = paragraph.Elements.Count;

                if (!mHasFieldParagraphProperties)
                    mElementIndex++;

                // Number of DmlRunProperties instances of a paragraph itself to be included in the enumeration.
                int paragraphRunPropertiesCount = mIncludeParagraphBreakProperties ? 2 : 1;
                if (mElementIndex >= elementCount + paragraphRunPropertiesCount)
                {
                    mParagraphIndex++;
                    mElementIndex = 0;
                    if (mParagraphIndex >= paragraphCount)
                        return false;
                }

                // mElementIndex == elementCount is intended for paragraph.Properties.DefaultRunProperties
                if (mElementIndex == elementCount)
                {
                    mCurrent = paragraph.Properties.DefaultRunProperties;
                }
                // mElementIndex == elementCount + 1 is intended for paragraph.EndParagraphRunProperties
                else if (mElementIndex == elementCount + 1)
                {
                    mCurrent = paragraph.EndParagraphRunProperties;
                }
                // DmlTextField.ParagraphProperties.DefaultRunProperties
                else if (mHasFieldParagraphProperties)
                {
                    DmlTextField field = (DmlTextField)paragraph.Elements[mElementIndex];
                    mCurrent = field.ParagraphProperties.DefaultRunProperties;
                    mHasFieldParagraphProperties = false;
                }
                // DmlParagraphTextElementBase.RunProperties
                else
                {
                    DmlParagraphTextElementBase element = paragraph.Elements[mElementIndex];
                    mCurrent = element.RunProperties;

                    DmlTextField field = element as DmlTextField;
                    mHasFieldParagraphProperties =
                        (field != null) &&
                        (field.ParagraphProperties != null) &&
                        !field.ParagraphProperties.IsEmpty &&
                        field.ParagraphProperties.HasDefaultRunProperties;
                }

                return true;
            }

            public void Reset()
            {
                mParagraphIndex = -1;
                mElementIndex = -1;
                mHasFieldParagraphProperties = false;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public DmlRunProperties Current
            {
                get { return mCurrent; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly IList<DmlParagraph> mParagraphs;
            private readonly bool mIncludeParagraphBreakProperties;
            private int mParagraphIndex = -1;
            private int mElementIndex = -1;
            private bool mHasFieldParagraphProperties;
            private DmlRunProperties mCurrent;
        }
    }
}
