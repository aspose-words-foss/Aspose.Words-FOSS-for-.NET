// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2004 by Roman Korchagin

using System;
using System.IO;
using System.Text.RegularExpressions;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.Images.Pal;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the MERGEFIELD field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Retrieves the name of a data field within the merge characters in a mail merge main document.
    /// When the main document is merged with the selected data source, information from the specified
    /// data field is inserted in place of the merge field.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public class FieldMergeField : Field, IFieldCodeTokenInfoProvider
    {
        internal FieldMergeField()
            : this(false)
        {
        }

        private FieldMergeField(bool isSurrogate)
        {
            mIsSurrogate = isSurrogate;
        }

        /// <summary>
        /// Used to create a merge field to replace during mail merge.
        /// The field created is not actually a merge field because its code is still the surrogate's
        /// code. So actually it stays, for example, a MACROBUTTON field, but behaving like a merge field.
        /// </summary>
        internal static FieldMergeField CreateFromSurrogate(IMergeFieldSurrogate surrogate)
        {
            FieldMergeField field = new FieldMergeField(true);
            field.Initialize(surrogate.Start, surrogate.Separator, surrogate.End);
            field.FieldNameNoPrefix = surrogate.GetMergeFieldName();

            if (field.GetFieldTypeFromCode() == FieldType.FieldMergeBarcode)
                field.mMergeFieldType = MergeFieldType.Barcode;

            return field;
        }

        /// <summary>
        /// Checks whether the specified field can be treated as MERGEFIELD surrogate. If it can, returns
        /// the corresponding <see cref="IMergeFieldSurrogate"/> instance. Otherwise, returns <c>null</c>.
        /// </summary>
        internal static IMergeFieldSurrogate GetMergeFieldSurrogate(Field field)
        {
            IMergeFieldSurrogate surrogate = field as IMergeFieldSurrogate;
            if ((surrogate != null) && surrogate.CanWorkAsMergeField())
                return surrogate;

            return null;
        }

        /// <summary>
        /// Two stage construction.
        /// </summary>
        internal override void Initialize(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            base.Initialize(start, separator, end);
            ParseFieldCode();
        }

        internal override void ParseFieldCode()
        {
            base.ParseFieldCode();

            // Only parse the code if the field is a real merge field, not based on a surrogate.
            if (!mIsSurrogate)
                ParseFieldCode(FieldCodeCache);
        }

        /// <summary>
        /// Parses field text various parts of the field. Public for unit testing.
        ///    Note that instead of MERGEFIELD there can be basically anything as Word French version
        ///    using CHAMPFUSION as field code.
        ///
        /// Accepts:
        ///    MERGEFIELD FirstName
        ///    MERGEFIELD "First Name"
        ///    MERGEFIELD  FirstName
        ///    MERGEFIELD "Table Start:First Name"
        ///    MERGEFIELD Image:FirstName
        ///    MERGEFIELD FirstName \b sometext
        ///    MERGEFIELD FirstName \b "some text" \@ "date format"
        ///    MERGEFIELD FirstName \b beforetext \f aftertext \# "$#,##0.00;($#,##0.00);Zero"
        ///
        /// </summary>
        private void ParseFieldCode(FieldCode fieldCode)
        {
            FieldMergeFieldParamBag result = ParseFieldCodeToParamBag(fieldCode);
            if (result != null)
            {
                Prefix = result.Prefix;
                FieldNameNoPrefix = result.FieldName;
                mMergeFieldType = result.MergeFieldType;
                mImageWidth = result.ImageWidth;
                mImageHeight = result.ImageHeight;
            }
        }

        /// <summary>
        /// Parses a full merge field name into an optional prefix and the field name.
        /// For example TableStart:MyTable.
        /// Understands ":" or "_" as the separator between the prefix and the field name.
        /// But note that "_" still can be used in the field name, for example "TableStart:Table_1".
        /// </summary>
        internal static FieldMergeFieldParamBag ParseFieldCodeToParamBag(FieldCode fieldCode)
        {
            string fullFieldName = fieldCode.GetArgumentAsString(0);
            if (fullFieldName == null)
                return null;

            // Fill related member variables as for a common MERGEFIELD.
            FieldMergeFieldParamBag result = new FieldMergeFieldParamBag();
            result.Prefix = string.Empty;
            result.FieldName = fullFieldName;
            result.MergeFieldType = MergeFieldType.Common;
            result.ImageWidth = null;
            result.ImageHeight = null;

            // Try to find a prefix separator. If it is not found, the field is surely common, simply return.
            int prefixSeparatorIndex = fullFieldName.IndexOfAny(gNamePrefixSeparators);
            if (prefixSeparatorIndex == -1)
                return result;

            // The field is suspected to have a special meaning, get its prefix for further analysis.
            string prefix = fullFieldName.Substring(0, prefixSeparatorIndex);

            if (ProcessPossibleImageField(prefix, result))
            {
                result.MergeFieldType = MergeFieldType.Image;
            }
            else
            {
                // The field is common, simply return.
                return result;
            }

            // The field is not common, set related member variables.
            result.Prefix = prefix;
            result.FieldName = fullFieldName.Substring(prefixSeparatorIndex + 1);
            return result;
        }

        /// <summary>
        /// Parses a full merge field name into an optional prefix and the field name.
        /// For example TableStart:MyTable.
        /// Understands ":" or "_" as the separator between the prefix and the field name.
        /// But note that "_" still can be used in the field name, for example "TableStart:Table_1".
        /// </summary>
        internal static FieldMergeFieldParamBag ParseFieldCodeToParamBag(string fieldCode)
        {
            return ParseFieldCodeToParamBag(new FieldCode(fieldCode, new FieldMergeField()));
        }

        private static bool ProcessPossibleImageField(string prefix, FieldMergeFieldParamBag param)
        {
            Match match = gImageSizeRegex.Match(prefix);
            if (!match.Success)
                return false;

            if (!StringUtil.HasChars(match.Groups[ImageSizeGroupIndex].Value))
                return true;

            MergeFieldImageDimension imageWidth = MergeFieldImageDimension.TryParse(
                match.Groups[ImageWidthValueGroupIndex].Value,
                match.Groups[ImageWidthUnitGroupIndex].Value);
            if (imageWidth == null)
                return true;

            MergeFieldImageDimension imageHeight = MergeFieldImageDimension.TryParse(
                match.Groups[ImageHeightValueGroupIndex].Value,
                match.Groups[ImageHeightUnitGroupIndex].Value);
            if (imageHeight == null)
                return true;

            // Set image dimensions only if they are both successfully parsed.
            // Otherwise, the original image size is to be used.
            param.ImageWidth = imageWidth;
            param.ImageHeight = imageHeight;

            return true;
        }

        /// <summary>
        /// Gets the Microsoft Word field type.
        /// </summary>
        public override FieldType Type
        {
            get
            {
                return FieldType.FieldMergeField;
            }
        }

        internal override FieldUpdateAction UpdateCore()
        {
            // The update was launched directly. Simply set the result to the field name enclosed into
            // French double quotes as Word does.
            string result = string.Format(
                "{0}«{1}»{2}",
                TextBefore,
                FieldCodeCache.GetArgumentAsString(0),
                TextAfter);

            return new FieldUpdateActionApplyResult(this, result, false);
        }

        internal override void EndUpdate()
        {
            // WORDSNET-11981 Implement MERGEBARCODE
            if (GetFieldTypeFromCode() == FieldType.FieldMergeBarcode)
                FieldBarcodeUtil.ReplaceMergeBarcode(this);

            base.EndUpdate();
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsMappedSwitch:
                case IsVerticalFormattingSwitch:
                    return FieldSwitchType.Flag;
                case TextBeforeSwitch:
                case TextAfterSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Returns the image width which has been achieved through the field code parsing.
        /// </summary>
        /// <returns></returns>
        internal MergeFieldImageDimension GetImageWidthCopy()
        {
            return GetImageDimensionCopy(mImageWidth);
        }

        /// <summary>
        /// Returns the image height which has been achieved through the field code parsing.
        /// </summary>
        /// <returns></returns>
        internal MergeFieldImageDimension GetImageHeightCopy()
        {
            return GetImageDimensionCopy(mImageHeight);
        }

        private static MergeFieldImageDimension GetImageDimensionCopy(MergeFieldImageDimension source)
        {
            return (source == null) ? new MergeFieldImageDimension() : source.Clone();
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="string"/> contains any characters
        /// reserved to separate a merge field's name from its prefix.
        /// </summary>
        internal static bool ContainsNamePrefixSeparator(string value)
        {
            Debug.Assert(value != null);

            return (value.IndexOfAny(gNamePrefixSeparators) != -1);
        }

        internal static string GetMergeFieldName(string prefix, string fieldName)
        {
            return string.IsNullOrEmpty(prefix)
                ? fieldName
                : string.Format("{0}:{1}", prefix, fieldName);
        }

        /// <summary>
        /// Gets whether this field is a mail merge region start mark.
        /// </summary>
        internal bool IsRegionStartMark
        {
            get { return (mMergeFieldType == MergeFieldType.RegionStart); }
        }

        /// <summary>
        /// Gets whether this field is a mail merge region start mark.
        /// </summary>
        internal bool IsRegionEndMark
        {
            get { return (mMergeFieldType == MergeFieldType.RegionEnd); }
        }

        /// <summary>
        /// Returns a value indicating whether this field is used to insert an image into the document.
        /// </summary>
        internal bool IsImageField
        {
            get { return (mMergeFieldType == MergeFieldType.Image); }
        }

        /// <summary>
        /// Extended merge field names could have a prefix such as TableStart, TableEnd or Image.
        /// This is a way for me to support extended fields such a mail merge image, or
        /// formatted text in the future.
        /// </summary>
        internal string Prefix { get; private set; }

        /// <summary>
        /// Returns just the name of the data field. Any prefix is stripped to the prefix property.
        /// </summary>
        public string FieldNameNoPrefix { get; private set; }

        /// <summary>
        /// Gets or sets the name of a data field.
        /// </summary>
        public string FieldName
        {
            get { return FieldCodeCache.GetArgumentAsString(FieldNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(FieldNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the text to be inserted before the field if the field is not blank.
        /// </summary>
        public string TextBefore
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TextBeforeSwitch); }
            set { FieldCodeCache.SetSwitch(TextBeforeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the text to be inserted after the field if the field is not blank.
        /// </summary>
        public string TextAfter
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TextAfterSwitch); }
            set { FieldCodeCache.SetSwitch(TextAfterSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether this field is a mapped field.
        /// </summary>
        public bool IsMapped
        {
            get { return FieldCodeCache.HasSwitch(IsMappedSwitch); }
            set { FieldCodeCache.SetSwitch(IsMappedSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to enable character conversion for vertical formatting.
        /// </summary>
        public bool IsVerticalFormatting
        {
            get { return FieldCodeCache.HasSwitch(IsVerticalFormattingSwitch); }
            set { FieldCodeCache.SetSwitch(IsVerticalFormattingSwitch, value); }
        }

        /// <summary>
        /// Specifies how a particular merge field should be treated.
        /// </summary>
        internal enum MergeFieldType
        {
            Common,
            Image,
            Barcode,
            RegionStart,
            RegionEnd
        }

        private readonly bool mIsSurrogate;

        private MergeFieldType mMergeFieldType;

        private MergeFieldImageDimension mImageWidth;
        private MergeFieldImageDimension mImageHeight;

        private const int ImageSizeGroupIndex = 3;
        private const int ImageWidthValueGroupIndex = 4;
        private const int ImageWidthUnitGroupIndex = 5;
        private const int ImageHeightValueGroupIndex = 6;
        private const int ImageHeightUnitGroupIndex = 7;

        /// <summary>
        /// Represents the regex to parse a MERGEFIELD image size. All possible MERGEFIELD image dimension units
        /// should be listed in its pattern.
        /// </summary>
        private static readonly Regex gImageSizeRegex = new Regex(
            @"\Aimage(\s*\(\s*(((.+?)\s*(pt|%|)\s*;\s*(.+?)\s*(pt|%|))|.*?)\s*\)\s*)??\z",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly char[] gNamePrefixSeparators = { ':', '_' };
        private const int FieldNameArgumentIndex = 0;

        private const string TextBeforeSwitch = "\\b";
        private const string TextAfterSwitch = "\\f";
        private const string IsMappedSwitch = "\\m";
        private const string IsVerticalFormattingSwitch = "\\v";
    }
}
