// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2009 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Represents information about a single record within an external data source that is to be excluded from the mail merge.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/mail-merge-and-reporting/">Mail Merge and Reporting</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>If a record shall be merged into a merged document, then no information is needed about that record. 
    /// However, if a given record shall not be merged into a merged document, then the value of the unique key 
    /// for that record shall be stored in the <see cref="UniqueTag"/> property of this object to indicate this exclusion.</para>
    /// </remarks>
    public class OdsoRecipientData
    {
        /// <summary>
        /// Returns a deep clone of this object.
        /// </summary>
        public OdsoRecipientData Clone()
        {
            return (OdsoRecipientData)MemberwiseClone();    
        }

        /// <summary>
        /// Specifies whether the record from the data source shall be imported into a document when the mail merge is performed.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool Active
        {
            get { return mActive;  }
            set { mActive = value; }
        }

        /// <summary>
        /// Specifies the column within the data source that contains unique data for the current record.
        /// The default value is 0.
        /// </summary>
        public int Column
        {
            get { return mColumn; }
            set { mColumn = value; }
        }

        /// <summary>
        /// Specifies the contents of a given record in the column containing unique data.
        /// The default value is <c>null</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public byte[] UniqueTag
        {
            get { return mUniqueTag; }
            set { mUniqueTag = value; }
        }

        /// <summary>
        /// Represents the hash code for this record. 
        /// Sometimes Microsoft Word uses <see cref="Hash"/> of a whole record instead of a <see cref="UniqueTag"/> value.
        /// The default value is 0.
        /// </summary>
        public int Hash
        {
            get { return mHash; }
            set { mHash = value; }
        }

        private bool mActive = true;
        private int mColumn = 0;
        private byte[] mUniqueTag = null;
        private int mHash;
    }
}
