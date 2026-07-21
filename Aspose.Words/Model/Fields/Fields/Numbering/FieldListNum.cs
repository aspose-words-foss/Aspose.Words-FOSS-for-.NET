// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Revisions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the LISTNUM field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public class FieldListNum : Field, IFieldCodeTokenInfoProvider
    {
        internal override NodeRange GetFakeResult()
        {
            return FieldNumUtil.GetFakeResultNodeRange(this);
        }

        public override Node Remove()
        {
            EnsureFieldNumberingRevisions();

            return base.Remove();
        }

        private void EnsureFieldNumberingRevisions()
        {
            if (!Document.IsTrackRevisionsEnabled)
                return;

            List<Field> fields = FieldExtractor.ExtractToCollection(Document, false, FieldType.FieldListNum);
            for (int index = fields.Count - 1; index >= 0; index--)
            {
                Field field = fields[index];
                if (field.End.NumberRevision != null)
                    fields.RemoveAt(index);
            }

            if (fields.Count == 0)
                return;

            Document document = FetchDocument();

            document.UpdateListLabels();

            EditSession editSession = document.EditSession;

            foreach (Field field in fields)
            {
                string result = field.DisplayResult;
                if (string.IsNullOrEmpty(result))
                    continue;

                field.End.NumberRevision = new FieldNumberRevision(
                    editSession.Author,
                    editSession.DateTime,
                    result);
            }
        }

        /// <summary>
        /// Gets or sets the name of the abstract numbering definition used for the numbering.
        /// </summary>
        public string ListName
        {
            get { return FieldCodeCache.GetArgumentAsString(ListNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(ListNameArgumentIndex, value); }
        }

        /// <summary>
        /// Returns a value indicating whether the name of an abstract numbering definition
        /// is provided by the field's code.
        /// </summary>
        public bool HasListName
        {
            get { return FieldCodeCache.HasArgument(ListNameArgumentIndex); }
        }

        /// <summary>
        /// Gets or sets the level in the list, overriding the default behavior of the field.
        /// </summary>
        public string ListLevel //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ListLevelSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(ListLevelSwitch, value); }
        }

        internal int ListLevelAsInt32
        {
            get { return ParseIntSwitchValue(ListLevel); }
        }

        /// <summary>
        /// Gets normalized <see cref="ListLevel"/> value. A negative value means error or absence.
        /// </summary>
        internal int ListLevelCore
        {
            get
            {
                int value = ListLevelAsInt32;

                // Negative means error or absence. Zero means one (i.e. zero for list levels). MS Word behavior.
                if (value <= 0)
                    return value;

                // List levels are 0-based and LISTNUM's ones are 1-based.
                return value - 1;
            }
        }

        /// <summary>
        /// Gets or sets the starting value for this field.
        /// </summary>
        public string StartingNumber //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(StartingNumberSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(StartingNumberSwitch, value); }
        }

        internal int StartingNumberAsInt32
        {
            get { return ParseIntSwitchValue(StartingNumber); }
        }

        /// <summary>
        /// Gets normalized <see cref="StartingNumber"/> value. A negative value means error or absence.
        /// </summary>
        internal int StartingNumberCore
        {
            get
            {
                int value = StartingNumberAsInt32;

                // MS Word considers only four first digits of the value.
                while (value > 9999)
                    value /= 10;

                // Negative means error or absence. Everything else is acceptable.
                return value;
            }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case ListLevelSwitch:
                case StartingNumberSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private static int ParseIntSwitchValue(string switchValue)
        {
            string value = switchValue;
            if (string.IsNullOrEmpty(value) || !char.IsDigit(value[0]))
                return -1;

            // WORDSNET-13387 MS Word allows trailing non-digits symbols, but does not allow leading.
            value = StringUtil.TrimEndNonDigits(value);

            int listLevel = FormatterPal.TryParseInt(value);
            return listLevel != int.MinValue ? listLevel : -1;
        }

        private const int ListNameArgumentIndex = 0;

        private const string ListLevelSwitch = "\\l";
        private const string StartingNumberSwitch = "\\s";
    }
}
