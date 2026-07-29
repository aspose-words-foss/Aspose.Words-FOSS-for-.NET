// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2006 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies all of the mail merge information for a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/mail-merge-and-reporting/">Mail Merge and Reporting</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You can use this object to specify a mail merge data source for a document and this information
    /// (along with the available data fields) will appear in Microsoft Word when the user opens this document.
    /// Or you can use this object to query mail merge settings that the user has specified in Microsoft Word
    /// for this document.</para>
    ///
    /// <para>You do not normally need to create objects of this class directly because Mail merge settings
    /// of a document are always available via the <see cref="Document.MailMergeSettings"/> property.</para>
    ///
    /// <para>To detect whether this document is a mail merge main document, check the value of the
    /// <see cref="MainDocumentType"/> property.</para>
    ///
    /// <para>To remove mail merge settings and data source information from a document you can use the
    /// <see cref="Clear"/> method. Aspose.Words will not write mail merge settings to a document if
    /// the <see cref="MainDocumentType"/> property is set to <see cref="MailMergeMainDocumentType.NotAMergeDocument"/>
    /// or the <see cref="DataType"/> property is set to <see cref="MailMergeDataType.None"/>.</para>
    ///
    /// <para>The best way to learn how to use the properties of this object is to create a document with a desired
    /// data source manually in Microsoft Word and then open that document using Aspose.Words and examine the properties
    /// of the <see cref="Document.MailMergeSettings"/> and <see cref="MailMergeSettings.Odso"/> objects. This is
    /// a good approach to take if you want to learn how to programmatically configure a data source, for example.</para>
    ///
    /// </remarks>
    public class MailMergeSettings
    {
        /// <summary>
        /// Returns true if the mail merge settings should not be written to the document when saving.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return
                    (MainDocumentType == MailMergeMainDocumentType.NotAMergeDocument) ||
                    (DataType == MailMergeDataType.None);
            }
        }

        /// <summary>
        /// Clears the mail merge settings in such a way that when the document is saved,
        /// no mail merge settings will be saved and it will become a normal document.
        /// </summary>
        public void Clear()
        {
            MainDocumentType = MailMergeMainDocumentType.NotAMergeDocument;
            DataType = MailMergeDataType.None;
        }

        /// <summary>
        /// Returns a deep clone of this object.
        /// </summary>
        public MailMergeSettings Clone()
        {
            MailMergeSettings rhs = (MailMergeSettings)MemberwiseClone();
            rhs.mOdso = mOdso.Clone();
            return rhs;
        }

        /// <summary>
        /// Specifies the one-based index of the record from the data source which shall be displayed in Microsoft Word. The default value is 1.
        /// </summary>
        public int ActiveRecord
        {
            get { return mActiveRecord; }
            set { mActiveRecord = value; }
        }

        /// <summary>
        /// Specifies the column within the data source that contains e-mail addresses. The default value is an empty string.
        /// </summary>
        public string AddressFieldName
        {
            get { return mAddressFieldName; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mAddressFieldName = value;
            }
        }

        /// <summary>
        /// Specifies the type of error reporting which shall be conducted by Microsoft Word when performing a mail merge.
        /// The default value is <see cref="MailMergeCheckErrors.Default"/>.
        /// </summary>
        public MailMergeCheckErrors CheckErrors
        {
            get { return mCheckErrors; }
            set { mCheckErrors = value; }
        }

        /// <summary>
        /// Specifies the connection string used to connect to an external data source. The default value is an empty string.
        /// </summary>
        public string ConnectString
        {
            get { return mConnectString; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mConnectString = value;
            }
        }

        /// <summary>
        /// Specifies the path to the mail-merge data source. The default value is an empty string.
        /// </summary>
        public string DataSource
        {
            get { return mDataSource; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mDataSource = value;
            }
        }

        /// <summary>
        /// Specifies the type of the mail-merge data source and the method of data access.
        /// The default value is <see cref="MailMergeDataType.Default"/>.
        /// </summary>
        public MailMergeDataType DataType
        {
            get { return mDataType; }
            set { mDataType = value; }
        }

        /// <summary>
        /// Specifies how Microsoft Word will output the results of a mail merge.
        /// The default value is <see cref="MailMergeDestination.Default"/>.
        /// </summary>
        public MailMergeDestination Destination
        {
            get { return mDestination; }
            set { mDestination = value; }
        }

        /// <summary>
        /// Specifies how an application performing the mail merge shall handle blank lines in the merged documents resulting from the mail merge.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool DoNotSupressBlankLines
        {
            get { return mDoNotSupressBlankLines; }
            set { mDoNotSupressBlankLines = value; }
        }

        /// <summary>
        /// Specifies the path to the mail-merge header source.
        /// The default value is an empty string.
        /// </summary>
        public string HeaderSource
        {
            get { return mHeaderSource; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mHeaderSource = value;
            }
        }

        /// <summary>
        /// Not sure about this one.
        /// The Microsoft Word Automation Reference suggests that this specifies that the query is executed every time the document
        /// is opened in Microsoft Word. But the OOXML specification suggests that this specifies that the query contains a reference
        /// to an external query file which contains the actual query.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool LinkToQuery
        {
            get { return mLinkToQuery; }
            set { mLinkToQuery = value; }
        }

        /// <summary>
        /// Specifies that the documents produced during a mail merge operation should be emailed as an attachment rather
        /// than the body of the actual e-mail. The default value is <c>false</c>.
        /// </summary>
        public bool MailAsAttachment
        {
            get { return mMailAsAttachment; }
            set { mMailAsAttachment = value; }
        }

        /// <summary>
        /// Specifies the text which shall appear in the subject line of the e-mails or faxes produced during mail merge.
        /// The default value is an empty string.
        /// </summary>
        public string MailSubject
        {
            get { return mMailSubject; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mMailSubject = value;
            }
        }

        /// <summary>
        /// Specifies the mail-merge main document type.
        /// The default value is <see cref="MailMergeMainDocumentType.Default"/>.
        /// </summary>
        /// <remarks>
        /// <para>The main document is the document that contains information that is the same for each version of the merged document.</para>
        /// </remarks>
        public MailMergeMainDocumentType MainDocumentType
        {
            get { return mMainDocumentType; }
            set { mMainDocumentType = value; }
        }

        /// <summary>
        /// Gets or sets the object that specifies the Office Data Source Object (ODSO) settings.
        /// </summary>
        /// <remarks>
        /// <para>This object is never <c>null</c>.</para>
        /// </remarks>
        public Odso Odso
        {
            get { return mOdso; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mOdso = value;
            }
        }

        /// <summary>
        /// Contains the Structured Query Language string that shall be run against the specified external data source to
        /// return the set of records which shall be imported into the document when the mail merge operation is performed.
        /// The default value is an empty string.
        /// </summary>
        public string Query
        {
            get { return mQuery; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mQuery = value;
            }
        }

        /// <summary>
        /// Specifies that Microsoft Word shall display the data from the specified external data source where merge fields
        /// have been inserted (e.g. preview merged data). The default value is <c>false</c>.
        /// </summary>
        public bool ViewMergedData
        {
            get { return mViewMergedData; }
            set { mViewMergedData = value; }
        }

        /// <summary>
        /// A DOC only property, specifies the token to separate fields in the data file.
        /// The default value is <see cref="MailMergeLegacySeparator.Tab"/>.
        /// </summary>
        internal MailMergeLegacySeparator LegacyDataFieldSeparator
        {
            get { return mLegacyDataFieldSeparator; }
            set { mLegacyDataFieldSeparator = value; }
        }

        /// <summary>
        /// A DOC only property, specifies the token to separate records in the data file.
        /// The default value is <see cref="MailMergeLegacySeparator.Enter"/>.
        /// </summary>
        internal MailMergeLegacySeparator LegacyDataRowSeparator
        {
            get { return mLegacyDataRowSeparator; }
            set { mLegacyDataRowSeparator = value; }
        }

        /// <summary>
        /// A DOC only property, specifies the token to separate fields in the header file.
        /// The default value is <see cref="MailMergeLegacySeparator.Tab"/>.
        /// </summary>
        internal MailMergeLegacySeparator LegacyHeaderFieldSeparator
        {
            get { return mLegacyHeaderFieldSeparator; }
            set { mLegacyHeaderFieldSeparator = value; }
        }

        /// <summary>
        /// A DOC only property, specifies the token to separate records in the header file.
        /// The default value is <see cref="MailMergeLegacySeparator.Enter"/>.
        /// </summary>
        internal MailMergeLegacySeparator LegacyHeaderRowSeparator
        {
            get { return mLegacyHeaderRowSeparator; }
            set { mLegacyHeaderRowSeparator = value; }
        }


        private int mActiveRecord = 1;
        private string mAddressFieldName = "";
        private MailMergeCheckErrors mCheckErrors = MailMergeCheckErrors.Default;
        private string mConnectString = "";
        private string mDataSource = "";
        private MailMergeDataType mDataType = MailMergeDataType.Default;
        private MailMergeDestination mDestination = MailMergeDestination.Default;
        private bool mDoNotSupressBlankLines = false;
        private string mHeaderSource = "";
        private bool mLinkToQuery = false;
        private bool mMailAsAttachment = false;
        private string mMailSubject = "";
        private MailMergeMainDocumentType mMainDocumentType = MailMergeMainDocumentType.Default;
        private Odso mOdso = new Odso();
        private string mQuery = "";
        private bool mViewMergedData = false;
        // WORDSNET-7853 LegacyDataFiledSeparator should be semicolon otherwise Word for Mac fails mail merge.
        private MailMergeLegacySeparator mLegacyDataFieldSeparator = MailMergeLegacySeparator.Semicolon;
        private MailMergeLegacySeparator mLegacyDataRowSeparator = MailMergeLegacySeparator.Enter;
        private MailMergeLegacySeparator mLegacyHeaderFieldSeparator = MailMergeLegacySeparator.Tab;
        private MailMergeLegacySeparator mLegacyHeaderRowSeparator = MailMergeLegacySeparator.Enter;
    }
}
