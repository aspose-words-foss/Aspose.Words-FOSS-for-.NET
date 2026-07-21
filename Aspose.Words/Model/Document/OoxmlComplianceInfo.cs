// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2013 by Denis Darkin

using Aspose.Common;
using Aspose.Words.Saving;
using Aspose.Words.Validation;

namespace Aspose.Words
{
    /// <summary>
    /// Stores OOXML compliance info about the document.
    /// Used for automated identification of spec features during document reading.
    /// Once all the features are identified we can do a better saving job, e.g. set correct <see cref="OoxmlCompliance"/> upon saving.
    /// </summary>
    /// <remarks>
    /// DD: One very important TODO item here is to accurately support updates to this class after DOM updates. 
    /// For example users imports a DrawingML node with Drawing Extensions, or an SdtCheckbox (DOCX extensions), 
    /// but AW writes this document using old ComplianceInfo resulting in wrong behavior.
    /// Presently I've added code to <see cref="DocumentValidator.VisitStructuredDocumentTagStart"/> 
    /// for checking SdtCheckbox, all other elements and even properties will have to be checked there as well.
    /// </remarks>
    internal class OoxmlComplianceInfo
    {
        internal static OoxmlComplianceCore GetCompliance(OoxmlComplianceInfo cInfo, OoxmlSaveOptions options)
        {
            // Note! ISO Strict format may be returned only if it is set in the options until the support of 
            // ISO Strict is finished.
            if (options == null)
            {
                return (cInfo == null) ? OoxmlComplianceCore.Ecma376 : cInfo.CompatibleCompliance;
            }
            else
            {
                OoxmlComplianceCore result = 
                    ((options.UserSetCompliance) || (cInfo == null)) ? 
                        options.ComplianceCore : cInfo.CompatibleCompliance;
                if (cInfo != null)
                {
                    // DD: It seems having these elements in the model, we always have to write transitional namespaces,
                    // otherwise MS Word fails to open such files. Extra research required.
                    // TODO: need to think what to do when user wants to save as ECMA386
                    if ((cInfo.IsDrawingExtensions || cInfo.IsDocxExtensions) &&
                        (result < OoxmlComplianceCore.IsoTransitional))
                        result = OoxmlComplianceCore.IsoTransitional;
                }
                return result;
            }
        }

        internal OoxmlComplianceInfo Clone()
        {
            return (OoxmlComplianceInfo) MemberwiseClone();
        }

        /// <summary>
        /// Marks the document that it conforms to ISO/IEC 29500 Transitional specification.
        /// </summary>
        internal void MarkAsIsoTransitional()
        {
            // We can set isoTransitional if we have not yet determined isoStrict.
            if (mCompliance < OoxmlComplianceCore.IsoTransitional)
                mCompliance = OoxmlComplianceCore.IsoTransitional;
        }

        /// <summary>
        /// Marks the document that it conforms to ISO/IEC 29500 Strict specification.
        /// </summary>
        internal void MarkAsIsoStrict()
        {
            // We can set IsoStrict if we have not yet determined higher version.
            if (mCompliance < OoxmlComplianceCore.IsoStrict)
                mCompliance = OoxmlComplianceCore.IsoStrict;
        }

        /// <summary>
        /// Marks the document that it conforms to [MS-DOCX]: Word Extensions to the Office Open XML (.docx) File.
        /// </summary>
        internal static void MarkAsHasDocxExtensions(Document document)
        {
            if (document.ComplianceInfo == null)
                document.ComplianceInfo = new OoxmlComplianceInfo();
            document.ComplianceInfo.IsDocxExtensions = true;
        }

        /// <summary>
        /// Marks the document as containing Extensions introduced in specified version of MS Word.
        /// </summary>
        internal static void MarkAsHasDocxExtensionsOf(Document document, MsWordVersionCore version)
        {
            if (document.ComplianceInfo == null)
                document.ComplianceInfo = new OoxmlComplianceInfo();

            document.ComplianceInfo.MarkAsHasDocxExtensionsOf(version);
        }

        /// <summary>
        /// Marks the document as containing Extensions introduced in specified version of MS Word.
        /// </summary>
        internal void MarkAsHasDocxExtensionsOf(MsWordVersionCore version)
        {
            if (mMsWordExtensionsVersion >= version)
                return;
            
            mMsWordExtensionsVersion = version;

            if (IsDocxExtensions)
                MarkAsIsoTransitional();
        }

        /// <summary>
        /// Marks the document that it conforms to [MS-ODRAWXML]: Office Drawing Extensions to Office Open XML Structure.
        /// </summary>
        internal static void MarkAsHasDrawingExtensions(Document document)
        {
            if (document.ComplianceInfo == null)
                document.ComplianceInfo = new OoxmlComplianceInfo();
            document.ComplianceInfo.IsDrawingExtensions = true;
        }

        /// <summary>
        /// Gets/sets the OOXML version determined from document contents.
        /// </summary>
        internal OoxmlComplianceCore Compliance
        {
            get { return mCompliance; }
            set { mCompliance = value; }
        }

        /// <summary>
        /// Returns the version in which the all MS Word Extensions contained in the document are supported.
        /// </summary>
        internal MsWordVersionCore MsWordExtensionsVersion
        {
            get { return mMsWordExtensionsVersion; }
        }

        /// <summary>
        /// Gets the OOXML version determined from document contents and that can be used on saving the document.
        /// If Aspose.Words does not support saving as OOXML format that is stored in <see cref="mCompliance"/>,
        /// this property returns nearest supported format or <see cref="mCompliance"/> otherwise.
        /// </summary>
        private OoxmlComplianceCore CompatibleCompliance
        {
            get
            {
                return mCompliance;
            }
        }

        /// <summary>
        /// Gets flag signaling that the document conforms to ECMA-376 specification.
        /// </summary>
        internal bool IsEcma376
        {
            get { return mCompliance == OoxmlComplianceCore.Ecma376; }
        }

        /// <summary>
        /// Gets flag signaling that the document conforms to ISO/IEC 29500 Transitional specification.
        /// </summary>
        internal bool IsIsoTransitional
        {
            get { return mCompliance == OoxmlComplianceCore.IsoTransitional; }
        }

        /// <summary>
        /// Gets flag signaling that the document conforms to ISO/IEC 29500 Strict specification.
        /// </summary>
        internal bool IsIsoStrict
        {
            get { return mCompliance == OoxmlComplianceCore.IsoStrict; }
        }

        /// <summary>
        /// Gets/sets flag signaling that the document conforms to 
        /// [MS-ODRAWXML]: Office Drawing Extensions to Office Open XML Structure
        /// </summary>
        internal bool IsDrawingExtensions
        {
            set
            {
                mIsDrawingExtensions = value;
                if (mIsDrawingExtensions)
                    MarkAsIsoTransitional(); // at least iso29500 transitional level
            }
            get { return mIsDrawingExtensions; }
        }

        /// <summary>
        /// Gets/sets flag signaling that the document conforms to 
        /// [MS-DOCX]: Word Extensions to the Office Open XML (.docx) File
        /// </summary>
        internal bool IsDocxExtensions
        {
            set
            {
                if (value)
                {
                    if (mMsWordExtensionsVersion < MsWordVersionCore.Word2010)
                        mMsWordExtensionsVersion = MsWordVersionCore.Word2010;

                    MarkAsIsoTransitional(); // at least iso29500 transitional level
                }
                else
                {
                    if (mMsWordExtensionsVersion >= MsWordVersionCore.Word2010)
                        mMsWordExtensionsVersion = MsWordVersionCore.Word2007;
                }
            }
            get { return mMsWordExtensionsVersion >= MsWordVersionCore.Word2010; }
        }

        /// <summary>
        /// Gets or sets the ODT UseFormerTextWrapping setting.
        /// </summary>
        /// <dev>
        /// This property is separated from IsIsoStrict to prevent conflict with DOCX format.
        /// It was a fix for WORDSNET-9652, WORDSNET-9832
        /// </dev>
        internal bool IsOdtFormerTextWrapping
        {
            get { return mIsOdtFormerTextWrapping; }
            set { mIsOdtFormerTextWrapping = value; }
        }

        internal static MsWordVersionCore MsWordVersionFromCompatibilityMode(string compatibilityModeValue)
        {
            MsWordVersionCore result;

            int version = FormatterPal.XmlToInt(compatibilityModeValue);
            switch (version)
            {
                case 11: // least version of MSW known to produce DOCX DD: Actually it is 12, but I've seen 11 in the customer docs.
                case 12:
                case 14:
                case 15: // latest version of MSW known to date. Add more when found.
                    result = (MsWordVersionCore) version;
                    break;
                default:
                    Debug.Assert(version <= 15); // DD: We would like to know whenever document created with a new MS Word versions is found. 
                    result = MsWordVersionCore.Unspecified; // DD: Lets return unspecified for all unknown stuff.
                    break;
            }
            return result;
        }

        // By default DOCX should conform to the common base spec Ecma386
        private OoxmlComplianceCore mCompliance = OoxmlComplianceCore.Ecma376; 

        private bool mIsDrawingExtensions;
        private MsWordVersionCore mMsWordExtensionsVersion;
        private bool mIsOdtFormerTextWrapping;
    }
}

