// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// This element specifies a single list item within a parent <see cref="SdtType.ComboBox"/> or <see cref="SdtType.DropDownList"/> structured document tag.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    public class SdtListItem
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public SdtListItem(string displayText, string value)
        {
            mDisplayText = displayText;

            ArgumentUtil.CheckHasChars(value, "value");
            mValue = value;
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public SdtListItem(string value)
            : this(value, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <remarks>
        /// WORDSNET-24793 Allows to create list item with empty value string.
        /// Don't want to make this public for a while, used for roundtrip only.
        /// </remarks>
        internal SdtListItem()
        {
        }

        internal SdtListItem Clone()
        {
            return (SdtListItem)MemberwiseClone();
        }

        /// <summary>
        /// Gets the text to display in the run content in place of the <see cref="Value"/> attribute contents for this list item.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c> and cannot be an empty string.</para>
        /// </remarks>
        /// <dev>
        /// WORDSNET-24793 In conjunction with parameterless ctor internal setter allows to create empty value item.
        /// </dev>
        public string DisplayText
        {
            get { return mDisplayText; }
            internal set { mDisplayText = value; }
        }

        /// <summary>
        /// Gets the value of this list item.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c> and cannot be an empty string.</para>
        /// </remarks>
        public string Value
        {
            get { return mValue; }
        }

        private readonly string mValue;
        private string mDisplayText;
    }
}
