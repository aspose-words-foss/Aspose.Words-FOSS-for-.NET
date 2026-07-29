// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2010 by Denis Darkin

using Aspose.Common;
using Aspose.Words.Saving;
using Aspose.Words.Settings;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// This class enforces standard compliance for ISO29500 capable AW model.
    /// Particularly it:
    /// - Deals with unsupported ISO29500-specific features of AW model when saving to pre-ISO29500 formats.
    /// - When saving AW model to ISO29500 OOXML, it makes sure all non-compliant stuff is handled.
    /// </summary>
    /// <remarks>
    /// ISO29500 introduces some new data types that are neither present in the older DOCX (ECMA-376) nor
    /// in other save formats supported by AW. To complicate matter ISO29500 does not support 
    /// some little things supported by ECMA-376. to comply to older/newer formats we have to alter
    /// AW document contents. This is the dedicated place for this type of code.
    /// Called exclusively from <see cref="DocumentValidator"/>.</remarks>
    internal class Iso29500ComplianceEnforcer
    {
        internal Iso29500ComplianceEnforcer(SaveOptions saveOptions, IWarningCallback warningCallback, OoxmlComplianceInfo cinfo)
        {
            OoxmlSaveOptions ooxmlSaveOptions = saveOptions as OoxmlSaveOptions;
            mCompliance = OoxmlComplianceInfo.GetCompliance(cinfo, ooxmlSaveOptions);
            mIsComplianceSetByUser = (ooxmlSaveOptions != null) && ooxmlSaveOptions.UserSetCompliance;
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// MS Word 2010 up shows CompatibilityMode for any OOXML that does not contain a set of these CompatibilityOptions,
        /// so we have to add these properties for any iso29500 OOXML document.
        /// </summary>
        /// <remarks>Strictly speaking it seems only "compatibilityMode" is required for MS Word 2010, others are for 2012</remarks>
        internal void UpdateCompatibilityOptions(CompatibilityOptions co)
        {
            // Update the compatibility options to prevent showing Compatibility Mode info in MS Word 2013 
            // on saving as Strict OOXML. Strict OOXML is supported since MS Word 2013.
            bool upgradeForIsoStrict = mIsComplianceSetByUser && (co.MswVersion < MsWordVersionCore.Word2013) &&
                (mCompliance == OoxmlComplianceCore.IsoStrict);
            if (upgradeForIsoStrict)
                co.MswVersion = MsWordVersionCore.Word2013;

            const string schemaUri = @"http://schemas.microsoft.com/office/word";

            // WORDSNET-4777 Add compatibilityMode in order to suppress "Compatibility Mode" ribbon in MS Word.
            if (co.MswVersion >= MsWordVersionCore.Word2003)
                VerifyExistenceOrSetProperty(co, "compatibilityMode", schemaUri, 
                    FormatterPal.IntToStr((int)co.MswVersion), upgradeForIsoStrict);

            if (IsIso29500 || (co.MswVersion >= MsWordVersionCore.Word2013))
            {
                // Without these MS Word 2013 turns compatibility mode on the document.

                // WORDSNET-13797 Do not force this setting to be 1 for Word2007 files unless we explicitly upgrade to ISO Strict.
                string overrideTableStyleFontSizeAndJustification = GetTargetCompatibilityOptionValue(upgradeForIsoStrict, co.MswVersion);

                VerifyExistenceOrSetProperty(co, "overrideTableStyleFontSizeAndJustification", schemaUri, 
                    overrideTableStyleFontSizeAndJustification, upgradeForIsoStrict);

                VerifyExistenceOrSetProperty(co, "enableOpenTypeFeatures", schemaUri, "1", upgradeForIsoStrict);
                VerifyExistenceOrSetProperty(co, "doNotFlipMirrorIndents", schemaUri, "1", upgradeForIsoStrict);
                VerifyExistenceOrSetProperty(co, "differentiateMultirowTableHeaders", schemaUri, "1", upgradeForIsoStrict);
            }
        }

        /// <summary>
        /// Returns value for compatibility option depending on document version and compliance chosen.
        /// </summary>
        private static string GetTargetCompatibilityOptionValue(bool upgradeForIsoStrict, MsWordVersionCore mswVersion)
        {
            // Force value to be 1 when upgrade for IsoStrict.
            if (upgradeForIsoStrict)
                return "1";

            // For certain document versions write 0 if value is unspecified.
            if ((mswVersion == MsWordVersionCore.Word2007) || 
                (mswVersion == MsWordVersionCore.Word2010) || 
                (mswVersion == MsWordVersionCore.Unspecified))
                return "0";
            
            // Write 1 if value is unspecified.
            return "1";
        }

        private static void VerifyExistenceOrSetProperty(CompatibilityOptions co, string propertyName, string uri,
            string value, bool setProperty)
        {

            CustomCompatibilitySetting setting = co.CustomCompatibilitySettings[propertyName];

            if (setting == null)
                co.CustomCompatibilitySettings.Add(new CustomCompatibilitySetting(propertyName, uri, value));
            else if (setProperty)
                setting.Value = value;
        }

        /// <summary>
        /// Verify section compliance and modify attributes if needed.
        /// </summary>
        internal void EnforceSectionCompliance(Section sections)
        {
            EnforceCompliantBorderLineStyle(sections.SectPr.BorderTop);
            EnforceCompliantBorderLineStyle(sections.SectPr.BorderBottom);
            EnforceCompliantBorderLineStyle(sections.SectPr.BorderLeft);
            EnforceCompliantBorderLineStyle(sections.SectPr.BorderRight);
        }

        /// <summary>
        /// Validate LineStyle of a single border.
        /// </summary>
        private void EnforceCompliantBorderLineStyle(Border border)
        {
            if (border.IsPageBorderArt)
                border.LineStyle = GetCompliantLineStyle(border.LineStyle);
        }

        /// <summary>
        /// In Iso29500 LineStyle enum has some items added and removed. We have to map them when
        /// saving.
        /// </summary>
        private LineStyle GetCompliantLineStyle(LineStyle lineStyle)
        {
            LineStyle result = lineStyle;
            if (IsIso29500)
            {
                // The following enumeration values were removed from the ST_Border simple type (§17.18.2): 
                // tribal1, tribal2, tribal3, tribal4, tribal5, tribal6
                // so we need to map them to something when saving to ISO29500
                result = (LineStyle)PageBorderArtRepository.GetIso29500BorderArt((PageBorderArt)lineStyle);
            }
            else if ((PageBorderArt)lineStyle >= PageBorderArt.Earth3)
            {
                // The following enumeration values were added to the ST_Border simple type (§17.18.2): 
                // earth3, triangle1, triangle2, triangleCircle1, triangleCircle2, shapes1, shapes2, custom
                // so we need to map them to something when saving to pre-ISO29500 formats.
                switch((PageBorderArt)lineStyle)
                {
                    case PageBorderArt.Earth3:                        
                        result = (LineStyle)PageBorderArt.Earth2;
                        break;
                    case PageBorderArt.Custom:                        
                        result = (LineStyle)PageBorderArt.TwistedLines1;
                        break;
                    default:
                        result = (LineStyle)PageBorderArtRepository.GetEcma376BorderArt((PageBorderArt)lineStyle);                        
                        break;
                }
            }

            if (result != lineStyle)
                Warn(WarningType.MinorFormattingLoss, WarningStrings.Iso29500CompliancePageBorderArtChanged);

            return result;
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(warningType, WarningSource.Docx, description));
        }
        
        /// <summary>
        /// True when saving to ISO29500. False when saving to any other format.
        /// </summary>
        private bool IsIso29500
        {
            get { return mCompliance != OoxmlComplianceCore.Ecma376; }
        }

        private readonly OoxmlComplianceCore mCompliance;
        private readonly bool mIsComplianceSetByUser;
        private readonly IWarningCallback mWarningCallback;
    }
}
