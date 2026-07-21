// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2009 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies how a column in the external data source shall be mapped to the predefined merge fields within the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/mail-merge-and-reporting/">Mail Merge and Reporting</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Microsoft Word provides some predefined merge field names that it allows to insert into a document as MERGEFIELD or 
    /// use in the ADDRESSBLOCK or GREETINGLINE fields. The information specified in <see cref="OdsoFieldMapData"/>
    /// allows to map one column in the external data source to a single predefined merge field.</para>
    /// 
    /// <seealso cref="OdsoFieldMapDataCollection"/>
    /// <seealso cref="Odso"/>
    /// </remarks>
    public class OdsoFieldMapData
    {
        /// <summary>
        /// Returns a deep clone of this object.
        /// </summary>
        public OdsoFieldMapData Clone()
        {
            return (OdsoFieldMapData)MemberwiseClone();
        }

        /// <summary>
        /// Specifies the zero-based index of the column within an external data source which shall be
        /// mapped to the local name of a specific MERGEFIELD field.
        /// The default value is 0.
        /// </summary>
        public int Column
        {
            get { return mColumn; }
            set
            {
                if (!IsColumnValid(value))
                    throw new ArgumentOutOfRangeException("value");

                mColumn = value;
            }
        }

        internal void SetColumnSafe(int value)
        {
            if (IsColumnValid(value))
                mColumn = value;
        }

        private static bool IsColumnValid(int value)
        {
            return (value >= 0);
        }

        /// <summary>
        /// Specifies that the contents of the AddressBlock MERGEFIELD field shall be dynamically ordered based on the country 
        /// associated with the current record or if the country-invariant version of the address field shall be used in its place.
        /// The default value is <c>false</c>.
        /// </summary>
        internal bool DynamicAddress
        {
            get { return mDynamicAddress; }
            set { mDynamicAddress = value; }
        }

        /// <summary>
        /// Specifies the language ID for the language which was used to generate the merge field name.
        /// The default value is <see cref="Aspose.Language.EnglishUS"/>.
        /// </summary>
        internal Language Language
        {
            get { return mLanguage; }
            set { mLanguage = value; }
        }

        /// <summary>
        /// Specifies the predefined merge field name which shall be mapped to the column number 
        /// specified by the <see cref="Column"/> property within this field mapping.
        /// The default value is an empty string.
        /// </summary>
        public string MappedName
        {
            get { return mMappedName; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mMappedName = value;
            }
        }

        /// <summary>
        /// Specifies the column name within an external data source for the column whose 
        /// index is specified by the <see cref="Column"/> property.
        /// The default value is an empty string.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mName = value;
            }
        }

        /// <summary>
        /// Specifies if a given mail merge field has been mapped to a column in the given external data source or not.
        /// The default value is <see cref="OdsoFieldMappingType.Default"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public OdsoFieldMappingType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        private int mColumn = 0;
        private bool mDynamicAddress = false;
        private Language mLanguage = Language.EnglishUS;
        private string mMappedName = "";
        private string mName = "";
        private OdsoFieldMappingType mType = OdsoFieldMappingType.Default;
    }
}
