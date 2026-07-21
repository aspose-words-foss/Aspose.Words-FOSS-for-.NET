// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the INCLUDE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts all or part of the text and graphics contained in another document.
    /// </remarks>
    public class FieldInclude : Field, IFieldCodeTokenInfoProvider, IFieldIncludeTextCode
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return FieldIncludeTextUpdater.Update(this);
        }

        /// <summary>
        /// Gets or sets the location of the document.
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

        string IFieldIncludeTextCode.NamespaceMappings
        {
            get { return null; }
        }

        string IFieldIncludeTextCode.XPath
        {
            get { return null; }
        }

        string IFieldIncludeTextCode.XslTransformation
        {
            get { return null; }
        }

        int IFieldIncludeTextCode.SourceFullNameArgumentIndex
        {
            get { return SourceFullNameArgumentIndex; }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case LockFieldsSwitch:
                    return FieldSwitchType.Flag;
                case TextConverterSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const int SourceFullNameArgumentIndex = 0;
        private const int BookmarkNameIndex = 1;

        private const string LockFieldsSwitch = "\\!";
        private const string TextConverterSwitch = "\\c";
    }
}
