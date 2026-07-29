// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2015 by Vadim Polienko

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the MERGEBARCODE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Mail merge a barcode.
    /// </remarks>
    public class FieldMergeBarcode : Field, IFieldCodeTokenInfoProvider, IMergeFieldSurrogate
    {
        /// <summary>
        /// Gets or sets the barcode value.
        /// </summary>
        public string BarcodeValue
        {
            get { return FieldCodeCache.GetArgumentAsString(FieldBarcodeUtil.BarcodeValueArgumentIndex); }
            set { FieldCodeCache.SetArgument(FieldBarcodeUtil.BarcodeValueArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the barcode type (QR, etc.)
        /// </summary>
        public string BarcodeType
        {
            get { return FieldCodeCache.GetArgumentAsString(FieldBarcodeUtil.BarcodeTypeArgumentIndex); }
            set { FieldCodeCache.SetArgument(FieldBarcodeUtil.BarcodeTypeArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the height of the symbol. The units are in TWIPS (1/1440 inch).
        /// </summary>
        public string SymbolHeight
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.SymbolHeightSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.SymbolHeightSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the rotation of the barcode symbol. Valid values are [0, 3]
        /// </summary>
        public string SymbolRotation
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.SymbolRotationSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.SymbolRotationSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a scaling factor for the symbol. The value is in whole percentage points and the valid values are [10, 1000]
        /// </summary>
        public string ScalingFactor
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.ScalingFactorSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.ScalingFactorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the foreground color of the barcode symbol. Valid values are in the range [0, 0xFFFFFF]
        /// </summary>
        public string ForegroundColor
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.ForegroundColorSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.ForegroundColorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the background color of the barcode symbol. Valid values are in the range [0, 0xFFFFFF]
        /// </summary>
        public string BackgroundColor
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.BackgroundColorSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.BackgroundColorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the style of a Point of Sale barcode (barcode types UPCA|UPCE|EAN13|EAN8). The valid values (case insensitive) are [STD|SUP2|SUP5|CASE].
        /// </summary>
        public string PosCodeStyle
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.PosCodeStyleSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.PosCodeStyleSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the style of a Case Code for barcode type ITF14. The valid values are [STD|EXT|ADD]
        /// </summary>
        public string CaseCodeStyle
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.CaseCodeStyleSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.CaseCodeStyleSwitch, value); }
        }

        /// <summary>
        /// Gets or sets an error correction level of QR Code. Valid values are [0, 3].
        /// </summary>
        public string ErrorCorrectionLevel
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(FieldBarcodeUtil.ErrorCorrectionLevelSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.ErrorCorrectionLevelSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to display barcode data (text) along with image.
        /// </summary>
        public bool DisplayText
        {
            get { return FieldCodeCache.HasSwitch(FieldBarcodeUtil.DisplayTextSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.DisplayTextSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to add Start/Stop characters for barcode types NW7 and CODE39.
        /// </summary>
        public bool AddStartStopChar
        {
            get { return FieldCodeCache.HasSwitch(FieldBarcodeUtil.AddStartStopCharSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.AddStartStopCharSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to fix the check digit if it’s invalid.
        /// </summary>
        public bool FixCheckDigit
        {
            get { return FieldCodeCache.HasSwitch(FieldBarcodeUtil.FixCheckDigitSwitch); }
            set { FieldCodeCache.SetSwitch(FieldBarcodeUtil.FixCheckDigitSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            return FieldBarcodeUtil.GetSwitchType(switchName);
        }

        string IMergeFieldSurrogate.GetMergeFieldName()
        {
            return BarcodeValue;
        }

        bool IMergeFieldSurrogate.CanWorkAsMergeField()
        {
            return true;
        }

        bool IMergeFieldSurrogate.IsMergeValueRequired()
        {
            return true;
        }
    }
}
