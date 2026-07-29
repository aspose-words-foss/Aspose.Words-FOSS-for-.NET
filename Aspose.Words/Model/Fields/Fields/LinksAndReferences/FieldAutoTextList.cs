// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the AUTOTEXTLIST field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Creates a shortcut menu based on AutoText entries in the active template.
    /// </remarks>
    public class FieldAutoTextList : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Gets or sets the name of the AutoText entry.
        /// </summary>
        public string EntryName
        {
            get { return FieldCodeCache.GetArgumentAsString(EntryNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(EntryNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the name of the style on which the list to contain entries is based.
        /// </summary>
        public string ListStyle
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ListStyleSwitch); }
            set { FieldCodeCache.SetSwitch(ListStyleSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the text of the ScreenTip to show.
        /// </summary>
        public string ScreenTip
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ScreenTipSwitch); }
            set { FieldCodeCache.SetSwitch(ScreenTipSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case ListStyleSwitch:
                case ScreenTipSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const int EntryNameArgumentIndex = 0;

        private const string ListStyleSwitch = "\\s";
        private const string ScreenTipSwitch = "\\t";
    }
}
