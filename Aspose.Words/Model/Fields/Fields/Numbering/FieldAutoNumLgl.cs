// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the AUTONUMLGL field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts an automatic number in legal format.
    /// </remarks>
    public class FieldAutoNumLgl : Field, IFieldCodeTokenInfoProvider
    {
        internal override NodeRange GetFakeResult()
        {
            return FieldNumUtil.GetFakeResultNodeRange(this);
        }

        /// <summary>
        /// Gets or sets whether to display the number without a trailing period.
        /// </summary>
        public bool RemoveTrailingPeriod
        {
            get { return FieldCodeCache.HasSwitch(RemoveTrailingPeriodSwitch); }
            set { FieldCodeCache.SetSwitch(RemoveTrailingPeriodSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the separator character to be used.
        /// </summary>
        public string SeparatorCharacter
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SeparatorCharacterSwitch); }
            set { FieldCodeCache.SetSwitch(SeparatorCharacterSwitch, value); }
        }

        /// <summary>
        /// Gets a separator character by the separator string provided by the field's code.
        /// </summary>
        internal char SeparatorCharacterCore
        {
            get { return FieldNumUtil.GetSeparatorCharacterCore(SeparatorCharacter); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case RemoveTrailingPeriodSwitch:
                    return FieldSwitchType.Flag;
                case SeparatorCharacterSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private const string RemoveTrailingPeriodSwitch = "\\e";
        private const string SeparatorCharacterSwitch = "\\s";
    }
}
