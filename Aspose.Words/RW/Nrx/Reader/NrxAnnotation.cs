// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2007 by Vladimir Averkin

using System;
using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// This class is used as a temporary storage of annotation properties read from 
    /// attributes of annotation elements, like bookmarks, revisions, etc.
    /// </summary>
    internal class NrxAnnotation
    {
        internal NrxAnnotation(NrxXmlReader xmlReader)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                    {
                        // andrnosk: WORDSNET-7990 Abs is used to properly handle negative Ids and Ids 
                        // which is greater then Int32 maximum value (in case when this value will be trimmed and became negative).
                        // MS Word also does not preserve negative values upon open/save.
                        int value = xmlReader.ValueAsInt;
                        if (value < 0)
                        {
                            xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Nrx,
                                "Negative Ids are not supported by AW, Id was corrected to positive value");

                            value = System.Math.Abs(value);
                        }

                        mId = value;
                        break;
                    }
                    case "date":
                    case "createdate":  // WML
                        // WORDSNET-23812 Word ignores TimeZone in timestamp of revisions.
                        mDate = FormatterPal.XmlToDateTimeIgnoreTimeZone(xmlReader.Value);
                        break;
                    case "name":
                        mName = xmlReader.Value;
                        break;
                    case "author":
                        mAuthor = xmlReader.Value;
                        break;
                    case "initials":
                        mInitials = xmlReader.Value;
                        break;
                    case "original":
                        mOriginal = xmlReader.Value;
                        break;

                    case "colFirst":
                    case "col-first":   // WML
                        mColFirst = xmlReader.ValueAsInt;
                        break;
                    case "colLast":
                    case "col-last":    // WML
                        mColLast = xmlReader.ValueAsInt;
                        break;
                    case "type":        // WML
                        // w:type
                        mType = xmlReader.Value;
                        break;
                    case "edGrp":
                        mEdGrp = DocxDopEnum.DocxToEditorType(xmlReader.Value);
                        break;
                    case "ed":        
                        mEd = xmlReader.Value;
                        break;
                    case "displacedByCustomXml":
                        mDisplacedByType = DocxDopEnum.DocxToDisplacedByType(xmlReader.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        internal int Id
        {
            get { return mId; }
        }

        internal DateTime Date
        {
            get { return mDate; }
        }

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal string Author
        {
            get { return mAuthor; }
        }

        internal string Initials
        {
            get { return mInitials; }
        }

        internal string Original
        {
            get { return mOriginal; }
        }

        internal int ColFirst
        {
            get { return mColFirst; }
        }

        internal int ColLast
        {
            get { return mColLast; }
        }

        /// <summary>
        /// WML only.
        /// </summary>
        internal string Type
        {
            get { return mType; }
        }

        /// <summary>
        /// Single user for editable range.
        /// </summary>
        internal string SingleUser
        {
            get { return mEd; }
        }

        /// <summary>
        /// Editor group for editable range.
        /// </summary>
        internal EditorType EditorGroup
        {
            get { return mEdGrp; }
        }

        /// <summary>
        /// Specifies that the parent annotation's placement shall be directly linked with the location 
        /// of the physical presentation of a custom XML element in the document. This element 
        /// only has an effect when the custom XML element is block-level (i.e. surrounds an entire 
        /// paragraph), as in this scenario the logical and physical placement of the annotation and 
        /// custom XML element can differ.
        /// </summary>
        internal DisplacedByType DisplacedBy
        {
            get { return mDisplacedByType; }
        }

        /// <summary>
        /// Checks if annotation element has id defined in 'w:id' attribute.
        /// </summary>
        internal bool HasId
        {
            get { return mId >= 0; }
        }

        /// <summary>
        /// WML only.
        /// Checks if 'aml:annotation' element has 'Word.Comment' type and id defined in 'aml:id'.
        /// </summary>
        internal bool IsComment
        {
            get { return (mType == "Word.Comment") && (mId >= 0); }
        }

        private readonly int mId = -1;
        private readonly DateTime mDate = DateTime.MinValue; // Init for Java.
        private string mName = "";
        private readonly string mAuthor = "";
        private readonly string mInitials = "";
        private readonly string mOriginal = "";
        private readonly int mColFirst = -1;
        private readonly int mColLast = -1;
        private readonly string mType = "";
        private readonly EditorType mEdGrp = EditorType.Unspecified;
        private readonly string mEd = "";
        private readonly DisplacedByType mDisplacedByType = DisplacedByType.Unspecified;
    }
}
