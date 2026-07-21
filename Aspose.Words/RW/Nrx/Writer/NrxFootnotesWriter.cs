// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2017 by Alexander Zhiltsov

using Aspose.JavaAttributes;
using Aspose.Words.Notes;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Base class of WML and DOCX footnote writers.
    /// </summary>
    internal abstract class NrxFootnotesWriter
    {
        protected NrxFootnotesWriter(FootnoteType footnoteType, bool hasFootnotes, INrxWriterContext writer)
        {
            mFootnoteType = footnoteType;
            mHasFootnotes = hasFootnotes;
            mWriter = writer;
        }

        /// <summary>
        /// Checks if footnote/endnote options should be written and writes them as footnotePr/endnotePr element.
        /// </summary>
        /// <param name="attrs">The properties to write.</param>
        /// <param name="writeSeparators">
        /// Set to true to write separator references in the "Settings" builder.
        /// and separator definitions in "Footnote"/"Endnote" part.
        /// </param>
        internal void WriteFootnotePr(AttrCollection attrs, bool writeSeparators)
        {
            PrepareOptionKeys();
            PrepareSeparators(writeSeparators);

            if (HasOptionsToWrite(attrs))
                WriteFootnotePrElement(attrs);
        }

        /// <summary>
        /// Writes the "w:footnotePr" or "w:endnotePr" element to the current builder.
        /// </summary>
        private void WriteFootnotePrElement(AttrCollection attrs)
        {
            // Generate start element
            NrxXmlBuilder builder = mWriter.Builder;
            builder.StartElement(IsEndnote ? "w:endnotePr" : "w:footnotePr");

            // Write options if needed
            if (!attrs.IsDefaultValue(mPosKey))
            {
                builder.WriteVal("w:pos", GetPositionValueForXml(attrs));
            }
            if (!attrs.IsDefaultValue(mNumFmtKey))
            {
                builder.WriteVal("w:numFmt", NumberStyleToXml((NumberStyle)attrs.GetDirectAttr(mNumFmtKey)));
            }
            if (!attrs.IsDefaultValue(mNumStartKey))
            {
                builder.WriteVal("w:numStart", (int)attrs.GetDirectAttr(mNumStartKey));
            }
            if (!attrs.IsDefaultValue(mNumRestartKey))
            {
                builder.WriteVal("w:numRestart", NrxSectEnum.NoteNumberingRuleToXml(
                    (FootnoteNumberingRule)attrs.GetDirectAttr(mNumRestartKey), mWriter.IsDocx));
            }

            // Write separators
            WriteSeparator(mSeparator);
            WriteSeparator(mContinuation);
            WriteSeparator(mContinuationNotice);

            // Write end element
            builder.EndElement();
        }

        /// <summary>
        /// Gets string value of footnote/endnote position to write to XML.
        /// </summary>
        private string GetPositionValueForXml(AttrCollection attrs)
        {
            return IsEndnote
                ? NrxSectEnum.EndnotePositionToXml((EndnotePosition)attrs.GetDirectAttr(mPosKey), mWriter.IsDocx)
                : NrxSectEnum.FootnotePositionToXml((FootnotePosition)attrs.GetDirectAttr(mPosKey), mWriter.IsDocx);
        }

        /// <summary>
        /// Returns <c>true</c> if there exist options to write.
        /// </summary>
        private bool HasOptionsToWrite(AttrCollection attrs)
        {
            return (mSeparator != null) ||
                   (mContinuation != null) ||
                   (mContinuationNotice != null) ||
                   (!attrs.IsDefaultValue(mPosKey) && StringUtil.HasChars(GetPositionValueForXml(attrs))) ||
                   !attrs.IsDefaultValue(mNumFmtKey) ||
                   !attrs.IsDefaultValue(mNumStartKey) ||
                   !attrs.IsDefaultValue(mNumRestartKey);
        }

        /// <summary>
        /// Initializes values for option key fields depending on footnote type.
        /// </summary>
        private void PrepareOptionKeys()
        {
            int keyDelta = IsEndnote ? SectAttr.EndnoteKeyDelta : 0;
            mPosKey = SectAttr.FootnoteLocation + keyDelta;
            mNumFmtKey = SectAttr.FootnoteNumberStyle + keyDelta;
            mNumStartKey = SectAttr.FootnoteStartNumber + keyDelta;
            mNumRestartKey = SectAttr.FootnoteNumberingRule + keyDelta;
        }

        /// <summary>
        /// Initializes values for the separator fields depending on current document and footnote type.
        /// </summary>
        private void PrepareSeparators(bool writeSeparators)
        {
            mSeparator = null;
            mContinuation = null;
            mContinuationNotice = null;
            if (writeSeparators && mHasFootnotes)
            {
                FootnoteSeparatorCollection separators = mWriter.Document.FootnoteSeparators;

                mSeparator = separators[IsEndnote ? FootnoteSeparatorType.EndnoteSeparator 
                    : FootnoteSeparatorType.FootnoteSeparator];
                mContinuation = separators[IsEndnote ? FootnoteSeparatorType.EndnoteContinuationSeparator
                    : FootnoteSeparatorType.FootnoteContinuationSeparator];
                mContinuationNotice = separators[IsEndnote ? FootnoteSeparatorType.EndnoteContinuationNotice
                    : FootnoteSeparatorType.FootnoteContinuationNotice];
            }
        }

        /// <summary>
        /// Writes definition of a footnote separator.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void WriteSeparator(FootnoteSeparator separator);

        /// <summary>
        /// Converts the specified number style to a string value to write it to document XML.
        /// </summary>
        protected abstract string NumberStyleToXml(NumberStyle value);

        /// <summary>
        /// Returns <c>true</c> if this object is writing endnotes.
        /// </summary>
        protected bool IsEndnote
        {
            get { return mFootnoteType == FootnoteType.Endnote; }
        }

        /// <summary>
        /// Returns <c>true</c> if the document contains footnotes of <see cref="mFootnoteType"/> type.
        /// </summary>
        protected bool HasFootnotes
        {
            get { return mHasFootnotes; }
        }

        // Fields that are temporarily used by the WriteFootnotePr method.
        private int mPosKey;
        private int mNumFmtKey;
        private int mNumStartKey;
        private int mNumRestartKey;
        private FootnoteSeparator mSeparator;
        private FootnoteSeparator mContinuation;
        private FootnoteSeparator mContinuationNotice;

        private readonly FootnoteType mFootnoteType;
        private readonly bool mHasFootnotes;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly INrxWriterContext mWriter;
    }
}
