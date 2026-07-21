// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the DDEAUTO field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// For information copied from another application, this field links that information to its original source file using DDE
    /// and is updated automatically.
    /// </remarks>
    public class FieldDdeAuto : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Gets or sets the application type of the link information.
        /// </summary>
        public string ProgId
        {
            get { return FieldCodeCache.GetArgumentAsString(ProgIdArgumentIndex); }
            set { FieldCodeCache.SetArgument(ProgIdArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the name and location of the source file.
        /// </summary>
        public string SourceFullName
        {
            get { return FieldCodeCache.GetArgumentAsString(SourceFullNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(SourceFullNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the portion of the source file that's being linked.
        /// </summary>
        public string SourceItem
        {
            get { return FieldCodeCache.GetArgumentAsString(SourceItemArgumentIndex); }
            set { FieldCodeCache.SetArgument(SourceItemArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the linked object as a bitmap.
        /// </summary>
        public bool InsertAsBitmap
        {
            get { return FieldCodeCache.HasSwitch(InsertAsBitmapSwitch); }
            set { FieldCodeCache.SetSwitch(InsertAsBitmapSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to reduce the file size by not storing graphics data with the document.
        /// </summary>
        public bool IsLinked
        {
            get { return FieldCodeCache.HasSwitch(IsLinkedSwitch); }
            set { FieldCodeCache.SetSwitch(IsLinkedSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the linked object as HTML format text.
        /// </summary>
        public bool InsertAsHtml
        {
            get { return FieldCodeCache.HasSwitch(InsertAsHtmlSwitch); }
            set { FieldCodeCache.SetSwitch(InsertAsHtmlSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the linked object as a picture.
        /// </summary>
        public bool InsertAsPicture
        {
            get { return FieldCodeCache.HasSwitch(InsertAsPictureSwitch); }
            set { FieldCodeCache.SetSwitch(InsertAsPictureSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the linked object in rich-text format (RTF).
        /// </summary>
        public bool InsertAsRtf
        {
            get { return FieldCodeCache.HasSwitch(InsertAsRtfSwitch); }
            set { FieldCodeCache.SetSwitch(InsertAsRtfSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the linked object in text-only format.
        /// </summary>
        public bool InsertAsText
        {
            get { return FieldCodeCache.HasSwitch(InsertAsTextSwitch); }
            set { FieldCodeCache.SetSwitch(InsertAsTextSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the linked object as Unicode text.
        /// </summary>
        public bool InsertAsUnicode
        {
            get { return FieldCodeCache.HasSwitch(InsertAsUnicodeSwitch); }
            set { FieldCodeCache.SetSwitch(InsertAsUnicodeSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case InsertAsBitmapSwitch:
                case IsLinkedSwitch:
                case InsertAsHtmlSwitch:
                case InsertAsPictureSwitch:
                case InsertAsRtfSwitch:
                case InsertAsTextSwitch:
                case InsertAsUnicodeSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const int ProgIdArgumentIndex = 0;
        private const int SourceFullNameArgumentIndex = 1;
        private const int SourceItemArgumentIndex = 2;

        private const string InsertAsBitmapSwitch = "\\b";
        private const string IsLinkedSwitch = "\\d";
        private const string InsertAsHtmlSwitch = "\\h";
        private const string InsertAsPictureSwitch = "\\p";
        private const string InsertAsRtfSwitch = "\\r";
        private const string InsertAsTextSwitch = "\\t";
        private const string InsertAsUnicodeSwitch = "\\u";
    }
}
