// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Dmitry Vorobyev

using System.Text;
using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the GREETINGLINE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts a mail merge greeting line.
    /// </remarks>
    public class FieldGreetingLine : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return MergeFieldUtil.ProcessUnusedField(this, "«GreetingLine»");
        }

        /// <summary>
        /// Gets or sets the text to include in the field if the name is blank.
        /// </summary>
        public string AlternateText
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(AlternateTextSwitch); }
            set { FieldCodeCache.SetSwitch(AlternateTextSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the format of the name included in the field.
        /// </summary>
        public string NameFormat
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(NameFormatSwitch); }
            set { FieldCodeCache.SetSwitch(NameFormatSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the language id used to format the name.
        /// </summary>
        public string LanguageId
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LanguageIdSwitch); }
            set { FieldCodeCache.SetSwitch(LanguageIdSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case AlternateTextSwitch:
                case NameFormatSwitch:
                case LanguageIdSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        static FieldGreetingLine()
        {
            // Found at http://msdn.microsoft.com/en-us/library/ff532312(v=office.12).aspx.

            gPlaceholdersToFieldsMap.Add("TITLE0", "Courtesy Title");
            gPlaceholdersToFieldsMap.Add("NICK0", "Nickname");
            gPlaceholdersToFieldsMap.Add("FIRST0", "First Name");
            gPlaceholdersToFieldsMap.Add("LAST0", "Last Name");
            gPlaceholdersToFieldsMap.Add("SUFFIX0", "Suffix");
            gPlaceholdersToFieldsMap.Add("TITLE1", "Spouse Courtesy Title");
            gPlaceholdersToFieldsMap.Add("NICK1", "Spouse Nickname");
            gPlaceholdersToFieldsMap.Add("FIRST1", "Spouse First Name");
            gPlaceholdersToFieldsMap.Add("LAST1", "Spouse Last Name");
        }

        private static readonly StringToObjDictionary<string> gPlaceholdersToFieldsMap =
            new StringToObjDictionary<string>(false);

        private static readonly string[] gValidFormats =
        {
            "<<_TITLE0_>><<_LAST0_>>",
            "<<_TITLE0_>><<and _TITLE1_>><<_LAST0_>>",
            "<<_TITLE0_>><<_FIRST0_>><<_LAST0_>><<_SUFFIX0_>>",
            "<<_TITLE0_>><<_NICK0_>><<_LAST0_>><<_SUFFIX0_>>",
            "<<_FIRST0_>><<_LAST0_>><<_SUFFIX0_>>",
            "<<_NICK0_>><<_LAST0_>><<_SUFFIX0_>>",
            "<<_FIRST0_>><<and _FIRST1_>><<_LAST0_>>",
            "<<_NICK0_>><<and _FIRST1_>><<_LAST0_>>",
            "<<_FIRST0_>>",
            "<<_NICK0_>>",
            "<<_FIRST0_>><<and _FIRST1_>>",
            "<<_NICK0_>><<and _FIRST1_>>",
            "<<Mr. and Mrs. _LAST0_>>"
        };

        private const string AlternateTextSwitch = "\\e";
        private const string NameFormatSwitch = "\\f";
        private const string LanguageIdSwitch = "\\l";
    }
}
