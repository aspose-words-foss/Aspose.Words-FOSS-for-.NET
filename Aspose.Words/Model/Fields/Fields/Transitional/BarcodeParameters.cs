// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2015 by Vadim Polienko

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Container class for barcode parameters to pass-through to BarcodeGenerator.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// The set of parameters are according to DISPLAYBARCODE field options.
    /// See the exact list at <a href="https://msdn.microsoft.com/en-us/library/hh745901(v=office.12).aspx"/>
    /// </remarks>
    public class BarcodeParameters
    {
        /// <summary>
        /// Bar code type.
        /// </summary>
        public string BarcodeType { get; set; }

        /// <summary>
        /// Data to be encoded.
        /// </summary>
        public string BarcodeValue { get; set; }

        /// <summary>
        /// Bar code image height (in twips - 1/1440 inches)
        /// </summary>
        public string SymbolHeight { get; set; }

        /// <summary>
        /// Bar code foreground color (0x000000 - 0xFFFFFF)
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// Bar code background color (0x000000 - 0xFFFFFF)
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Rotation of the barcode symbol. Valid values are [0, 3].
        /// </summary>
        public string SymbolRotation { get; set; }

        /// <summary>
        /// Scaling factor for the symbol. The value is in whole percentage points and the valid values are [10, 1000].
        /// </summary>
        public string ScalingFactor { get; set; }

        /// <summary>
        /// Style of a Point of Sale barcode (barcode types UPCA|UPCE|EAN13|EAN8). The valid values (case insensitive) are [STD|SUP2|SUP5|CASE].
        /// </summary>
        public string PosCodeStyle { get; set; }

        /// <summary>
        /// Style of a Case Code for barcode type ITF14. The valid values are [STD|EXT|ADD]
        /// </summary>
        public string CaseCodeStyle { get; set; }

        /// <summary>
        /// Error correction level of QR Code. Valid values are [0, 3].
        /// </summary>
        public string ErrorCorrectionLevel { get; set; }

        /// <summary>
        /// Whether to display barcode data (text) along with image.
        /// </summary>
        public bool DisplayText { get; set; }

        /// <summary>
        /// Whether to add Start/Stop characters for barcode types NW7 and CODE39.
        /// </summary>
        public bool AddStartStopChar { get; set; }

        /// <summary>
        /// Whether to fix the check digit if it’s invalid.
        /// </summary>
        public bool FixCheckDigit { get; set; }

        // Below are parameters for old-fashioned Barcode field.

        /// <summary>
        /// Barcode postal address.
        /// </summary>
        public string PostalAddress { get; set; }

        /// <summary>
        /// Whether <see cref="PostalAddress"/> is the name of a bookmark.
        /// </summary>
        public bool IsBookmark { get; set; }

        /// <summary>
        /// Type of a Facing Identification Mark (FIM).
        /// </summary>
        public string FacingIdentificationMark { get; set; }

        /// <summary>
        /// Whether <see cref="PostalAddress"/> is a U.S. postal address.
        /// </summary>
        public bool IsUSPostalAddress { get; set; }
    }
}
