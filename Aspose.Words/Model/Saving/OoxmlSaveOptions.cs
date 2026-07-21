// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/07/2010 by Roman Korchagin

using System;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Can be used to specify additional options when saving a document into the <see cref="Words.SaveFormat.Docx"/>,
    /// <see cref="Words.SaveFormat.Docm"/>, <see cref="Words.SaveFormat.Dotx"/>, <see cref="Words.SaveFormat.Dotm"/> or
    /// <see cref="Words.SaveFormat.FlatOpc"/> format.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-save-options/">Specify Save Options</a> documentation article.</para>
    /// </summary>
    public class OoxmlSaveOptions : SaveOptions
    {
        /// <summary>
        /// Initializes a new instance of this class that can be used to save a document in the <see cref="Words.SaveFormat.Docx"/> format.
        /// </summary>
        public OoxmlSaveOptions()
            : this(SaveFormat.Docx)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class that can be used to save a document in the <see cref="Words.SaveFormat.Docx"/>,
        /// <see cref="Words.SaveFormat.Docm"/>, <see cref="Words.SaveFormat.Dotx"/>, <see cref="Words.SaveFormat.Dotm"/> or
        /// <see cref="Words.SaveFormat.FlatOpc"/> format.
        /// </summary>
        /// <param name="saveFormat">Can be <see cref="Words.SaveFormat.Docx"/>, <see cref="Words.SaveFormat.Docm"/>,
        /// <see cref="Words.SaveFormat.Dotx"/>, <see cref="Words.SaveFormat.Dotm"/> or <see cref="Words.SaveFormat.FlatOpc"/>.
        /// </param>
        public OoxmlSaveOptions(SaveFormat saveFormat)
        {
            WriteClrSchemeMapping = true;
            WriteWordCountOption = true;
            WriteDoNotTrackMovesCorrectly = true;
            WriteExtendedIds = true;
            Write2019LatentStyle = true;
            WriteLatentStyles = true;
            CompressionLevel = CompressionLevel.Normal;
            Zip64Mode = Zip64Mode.Never;

            SetSaveFormatCore(saveFormat);
        }

        /// <summary>
        /// Returns options to save to the DOCX format with specifying document compliance.
        /// </summary>
        /// <dev>
        /// We can't use ctor OoxmlSaveOptions(OoxmlComplianceCore) here since it is name conglict with
        /// OoxmlSaveOptions(SaveFormat) ctor after autoporting:
        /// Both SaveFormat and OoxmlComplianceCore enums are autoported as ints so we get two
        /// OoxmlSaveOptions(int) ctors in Java.
        /// </dev>
        internal static OoxmlSaveOptions DocxWithCompliance(OoxmlComplianceCore compliance)
        {
            OoxmlSaveOptions options = new OoxmlSaveOptions(SaveFormat.Docx) { ComplianceCore = compliance };
            return options;
        }

        /// <summary>
        /// Sets options we use for unit testing.
        /// </summary>
        internal override void SetTestMode()
        {
            base.SetTestMode();

            // We need it to avoid accepting lot of golds.
            WriteClrSchemeMapping = false;
            WriteWordCountOption = false;
            WriteDoNotTrackMovesCorrectly = false;
            WriteExtendedIds = false;
            Write2019LatentStyle = false;
        }

        /// <summary>
        /// Specifies the format in which the document will be saved if this save options object is used.
        /// Can be <see cref="Words.SaveFormat.Docx"/>, <see cref="Words.SaveFormat.Docm"/>,
        /// <see cref="Words.SaveFormat.Dotx"/>, <see cref="Words.SaveFormat.Dotm"/> or <see cref="Words.SaveFormat.FlatOpc"/>.
        /// </summary>
        public override SaveFormat SaveFormat
        {
            get { return mSaveFormat; }
            set { SetSaveFormatCore(value); }
        }

        private void SetSaveFormatCore(SaveFormat saveFormat)
        {
            switch (saveFormat)
            {
                case SaveFormat.Docx:
                case SaveFormat.Docm:
                case SaveFormat.Dotx:
                case SaveFormat.Dotm:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    mSaveFormat = saveFormat;
                    break;
                default:
                    throw new ArgumentException("An invalid SaveFormat for this options type was chosen.");
            }
        }

        /// <summary>
        /// Gets/sets a password to encrypt document using ECMA376 Standard encryption algorithm.
        /// </summary>
        /// <remarks>
        /// <para>In order to save document without encryption this property should be <c>null</c> or empty string.</para>
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        /// Specifies the OOXML version for the output document.
        /// The default value is <see cref="OoxmlCompliance.Ecma376_2006"/>.
        /// </summary>
        public OoxmlCompliance Compliance
        {
            get
            {
                switch (ComplianceCore)
                {
                    case OoxmlComplianceCore.Ecma376:
                        return OoxmlCompliance.Ecma376_2006;
                    case OoxmlComplianceCore.IsoTransitional:
                        return OoxmlCompliance.Iso29500_2008_Transitional;
                    case OoxmlComplianceCore.IsoStrict:
                        return OoxmlCompliance.Iso29500_2008_Strict;
                    default:
                        throw new InvalidOperationException("Unknown OOXML version value.");
                }
            }
            set
            {
                switch (value)
                {
                    case OoxmlCompliance.Ecma376_2006:
                        ComplianceCore = OoxmlComplianceCore.Ecma376;
                        break;
                    case OoxmlCompliance.Iso29500_2008_Transitional:
                        ComplianceCore = OoxmlComplianceCore.IsoTransitional;
                        break;
                    case OoxmlCompliance.Iso29500_2008_Strict:
                        ComplianceCore = OoxmlComplianceCore.IsoStrict;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown OOXML version value.");
                }
            }
        }

        /// <summary>
        /// Keeps original representation of legacy control characters.
        /// </summary>
        public bool KeepLegacyControlChars { get; set; }

        /// <summary>
        /// Specifies the compression level used to save document.
        /// The default value is <see cref="Aspose.Words.Saving.CompressionLevel.Normal"/>.
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; }

        /// <summary>
        /// Specifies whether or not to use ZIP64 format extensions for the output document.
        /// The default value is <see cref="Saving.Zip64Mode.Never"/>.
        /// </summary>
        /// <seealso cref="Zip64Mode"/>
        public Zip64Mode Zip64Mode { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Aspose.Words.Saving.DigitalSignatureDetails"/> object used to sign a document.
        /// </summary>
        public DigitalSignatureDetails DigitalSignatureDetails { get; set;}

        /// <summary>
        /// Specifies the OOXML version for the output document. This is internal version of
        /// <see cref="Compliance"/> that allows setting officially not supported document format.
        /// Result OOXML version that is used on saving a document is a combination with
        /// <see cref="OoxmlComplianceInfo.Compliance"/>.
        /// </summary>
        internal OoxmlComplianceCore ComplianceCore
        {
            get { return mComplianceCore; }
            set
            {
                mComplianceCore = value;
                UserSetCompliance = true;
            }
        }

        /// <summary>
        /// Used for unit testing only, not really needed for the users I think.
        /// </summary>
        internal bool ConvertDmlPictureToVml { get; set; }

        /// <summary>
        /// andrnosk: This property is FALSE for test mode and TRUE for Release.
        /// We need it to avoid accepting lot of golds.
        /// </summary>
        internal bool WriteClrSchemeMapping { get; set; }

        /// <summary>
        /// Refers to WORDSNET-6575 Used to avoid accept lot of golds.
        /// </summary>
        internal bool WriteWordCountOption { get; set; }

        /// <summary>
        /// Refers to WORDSNET-9070 Used to avoid accept lot of golds.
        /// </summary>
        internal bool WriteDoNotTrackMovesCorrectly { get; set; }

        /// <summary>
        /// Refers to WORDSNET-14123 Used to avoid accepting lot of golds.
        /// </summary>
        internal bool WriteExtendedIds { get; set; }

        /// <summary>
        /// Refers to WORDSNET-22367 Used to avoid accepting lot of golds.
        /// </summary>
        internal bool Write2019LatentStyle { get; set; }

        /// <summary>
        /// Returns true if user has explicitly set <see cref="OoxmlCompliance"/>.
        /// AW can identify compliance by analyzing source DOCX files upon loading, but if user overrides compliance,
        /// AW shall not use its derived information during saving, but shall use user-provided compliance info.
        /// </summary>
        internal bool UserSetCompliance { get; private set; }

        /// <summary>
        /// Controls whether LatentStyles collection is written to output document file.
        /// </summary>
        /// <remarks>
        /// Relates to WORDSNET-24602
        /// </remarks>
        internal bool WriteLatentStyles { get; set; }

        private SaveFormat mSaveFormat;
        private OoxmlComplianceCore mComplianceCore = OoxmlComplianceCore.Ecma376;
    }
}
