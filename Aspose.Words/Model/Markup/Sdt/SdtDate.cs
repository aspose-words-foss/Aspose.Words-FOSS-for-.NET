// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2010 by Denis Darkin

using System;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies that the Sdt shall be a date picker when displayed in the document.
    /// </summary>
    internal class SdtDate : SdtControlProperties
    {
        internal override SdtType Type
        {
            get { return SdtType.Date; }
        }

        /// <summary>
        /// <para>This element specifies the language ID which shall be used for displaying a calendar for the current date picker 
        /// structured document tag, if a user interface is present for the sdt.</para>
        /// <para>
        /// If this element is omitted, then the language ID shall be the language ID of the run contents of the parent
        /// structured document tag.</para>
        /// </summary>
        internal int Lid
        {
            get { return mLid; }
            set
            {

                if (mLid != value)
                    NeedToUpdateContent = true;
                mLid = value;
            }
        }

        /// <summary>
        /// The element specifies the display format which shall be used to format any date entered into the parent 
        /// structured document tag in full DateTime format [Example: Through a user interface (a date picker), or through 
        /// custom XML data associated with this structured document tag via the <see cref="XmlMapping"/> element  end 
        /// example] before displaying it in the structured document tag's run content.
        /// </summary>
        /// <remarks>If this element is omitted, then the date shall be formatted using the standard date display mask for the
        /// language ID specified on the <see cref="Lid"/> element if present, or the language ID of the run contents 
        /// otherwise.</remarks>
        internal string DateFormat
        {
            get { return mDateFormat; }
            set
            {
                if (mDateFormat != value)
                    NeedToUpdateContent = true;
                mDateFormat = value;
            }
        }

        /// <summary>
        /// Date Picker Calendar Type, specifies the calendar which shall be displayed for the current date picker structured document
        /// tag, if a user interface is present for the structured document tag.
        /// </summary>
        internal SdtCalendarType CalendarType
        {
            get { return mCalendarType; }
            set { mCalendarType = value; }
        }

        /// <summary>
        /// Specifies the full date and time last entered into the parent structured document tag 
        /// using the standard XML Schema DateTime syntax.
        /// </summary>
        internal DateTime FullDate
        {
            get { return mFullDate; }
            set
            {
                if (mFullDate != value)
                    NeedToUpdateContent = true;
                mFullDate = value;
            }
        }

        /// <summary>
        /// Custom XML Data Date Storage Format. This element specifies the translation which 
        /// shall be performed on the displayed date in a date picker structured document tag when 
        /// the current contents are saved into the associated custom XML data via the <see cref="XmlMapping"/> element.
        /// </summary>
        internal SdtDateStorageFormat StoreMappedDataAs
        {
            get { return mStoreMappedDataAs; }
            set { mStoreMappedDataAs = value; }
        }

        internal bool NeedToUpdateContent
        {
            get { return mNeedToUpdateContent; }
            set { mNeedToUpdateContent = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int EmptyLid = -1;

        private string mDateFormat = "";
        private DateTime mFullDate = DateTime.MinValue; // Init for Java.
        private SdtCalendarType mCalendarType = SdtCalendarType.Default;
        
        private int mLid = EmptyLid;

        private SdtDateStorageFormat mStoreMappedDataAs = SdtDateStorageFormat.Default;
        private bool mNeedToUpdateContent;
    }
}
