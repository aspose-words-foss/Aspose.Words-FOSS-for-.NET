// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2010 by Dmitry Vorobyev

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the INCLUDETEXT field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts all or part of the text and graphics contained in another document.
    /// </remarks>
    public class FieldIncludeText : Field, IFieldCodeTokenInfoProvider, IFieldIncludeTextCode
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return FieldIncludeTextUpdater.Update(this);
        }

        /// <summary>
        /// Gets or sets the location of the document using an IRI.
        /// </summary>
        public string SourceFullName
        {
            get { return FieldCodeCache.GetArgumentAsString(SourceFullNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(SourceFullNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark in the document to include.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetArgumentAsString(BookmarkNameIndex); }
            set { FieldCodeCache.SetArgument(BookmarkNameIndex, value); }
        }

        /// <summary>
        /// Gets or sets whether to prevent fields in the included document from being updated.
        /// </summary>
        public bool LockFields
        {
            get { return FieldCodeCache.HasSwitch(LockFieldsSwitch); }
            set { FieldCodeCache.SetSwitch(LockFieldsSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of the text converter for the format of the included file.
        /// </summary>
        public string TextConverter
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TextConverterSwitch); }
            set { FieldCodeCache.SetSwitch(TextConverterSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the namespace mappings for XPath queries.
        /// </summary>
        public string NamespaceMappings
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(NamespaceMappingsSwitch); }
            set { FieldCodeCache.SetSwitch(NamespaceMappingsSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the location of XSL Transformation to format XML data.
        /// </summary>
        public string XslTransformation
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(XslTransformationSwitch); }
            set { FieldCodeCache.SetSwitch(XslTransformationSwitch, value); }
        }

        /// <summary>
        /// Gets or sets XPath for the desired portion of the XML file.
        /// </summary>
        public string XPath
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(XPathSwitch); }
            set { FieldCodeCache.SetSwitch(XPathSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the encoding applied to the data within the referenced file.
        /// </summary>
        public string Encoding
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EncodingSwitch); }
            set { FieldCodeCache.SetSwitch(EncodingSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the MIME type of the referenced file.
        /// </summary>
        public string MimeType
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(MimeTypeSwitch); }
            set { FieldCodeCache.SetSwitch(MimeTypeSwitch, value); }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        string IFieldIncludeTextCode.SourceFullName
        {
            get { return SourceFullName; }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        string IFieldIncludeTextCode.BookmarkName
        {
            get { return BookmarkName; }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        bool IFieldIncludeTextCode.LockFields
        {
            get { return LockFields; }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        string IFieldIncludeTextCode.NamespaceMappings
        {
            get { return NamespaceMappings; }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        string IFieldIncludeTextCode.XPath
        {
            get { return XPath; }
        }

        int IFieldIncludeTextCode.SourceFullNameArgumentIndex
        {
            get { return SourceFullNameArgumentIndex; }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            // DV It is interesting that ECMA has two more switches specified, \e and \m, but they seem to be not recognized
            // in Word and are never mentioned anywhere else.
            switch (switchName)
            {
                case LockFieldsSwitch:
                    {
                        return FieldSwitchType.Flag;
                    }
                case TextConverterSwitch:
                case EncodingSwitch:
                case MimeTypeSwitch:
                case NamespaceMappingsSwitch:
                case XslTransformationSwitch:
                case XPathSwitch:
                    {
                        return FieldSwitchType.HasArgument;
                    }
                default:
                    {
                        return FieldSwitchType.Unknown;
                    }
            }
        }

        private const int SourceFullNameArgumentIndex = 0;
        private const int BookmarkNameIndex = 1;

        private const string LockFieldsSwitch = "\\!";
        private const string TextConverterSwitch = "\\c";
        private const string EncodingSwitch = "\\e";
        private const string MimeTypeSwitch = "\\m";
        private const string NamespaceMappingsSwitch = "\\n";
        private const string XslTransformationSwitch = "\\t";
        private const string XPathSwitch = "\\x";
    }
}
