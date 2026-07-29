// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2016 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Collections;
using Aspose.Words.Markup;
using Aspose.Words.Saving;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// This class is invoked by <see cref="DocumentValidator"/> to correct any issues with custom XML parts 
    /// including validation of part ID with changing references to them from structured document tags.
    /// </summary>
    internal class CustomXmlValidator
    {
        internal CustomXmlValidator(SaveInfo saveInfo)
        {
            mSaveInfo = saveInfo;
        }

        /// <summary>
        /// Validates ID of custom XML parts. Text values are replaced with GUIDs on saving as ISO/IEC 29500 
        /// OOXML formats. For ECMA-376 a warning is generated only.
        /// </summary>
        internal void VisitDocumentStart(Document doc)
        {
            if (mSaveInfo.SaveOptions is OoxmlSaveOptions)
                ValidateCustomXmlContent(doc);

            if (!mSaveInfo.IsDocxFormat)
                return;

            mIdMapping.Clear();
            List<CustomXmlPart> xmlParts = GetXmlPartsWithNonGuidId(doc);

            foreach (CustomXmlPart xmlPart in xmlParts)
            {
                if (Compliance != OoxmlComplianceCore.Ecma376)
                {
                    string oldId = xmlPart.Id;
                    xmlPart.Id = Guid.NewGuid().ToString("B").ToUpper();

                    mIdMapping.Add(oldId, xmlPart.Id);

                    Warn(WarningType.MinorFormattingLoss, WarningSource.Validator,
                         string.Format(WarningStrings.ChangedXmlPartId, oldId, xmlPart.Id));
                }
                else
                {
                    Warn(WarningType.Hint, WarningSource.Validator,
                         string.Format(WarningStrings.NonGuidXmlPartId, xmlPart.Id));
                }
            }
        }

        /// <summary>
        /// Checks assignment of custom XML parts in data binding of structured document tags after part ID changes.
        /// </summary>
        internal void VisitStructuredDocumentTagStart(StructuredDocumentTag sdt, XmlMappingContext xmlMappingContext)
        {
            if (!mSaveInfo.IsDocxFormat)
                return;

            // Now reference to a custom XML part is stored directly in XmlMapping, but let's keep this check
            // for more care.
            XmlMapping mapping = sdt.XmlMapping;
            if (mapping.IsEmpty)
                return;

            if (mapping.CustomXmlPart == null)
            {
                if (mIdMapping.ContainsKey(mapping.StoreItemId))
                    mapping.StoreItemId = mIdMapping[mapping.StoreItemId];
            }
            else
            {
                // Note, for a moment only SDT of type Date may need to update its content.
                if (sdt.NeedToUpdateContent)
                {
                    mapping.SetValue(sdt.FullDate.ToString(sdt.DateDisplayFormat));
                    // Reset cache to get correctly updated SDT on the validation stage.
                    // See, "TestJira15651" for details.
                    xmlMappingContext.Remove(mapping.StoreItemId);
                }
            }
        }

        /// <summary>
        /// Checks content of the custom part and removes the content, when content is invalid.
        /// </summary>
        private void ValidateCustomXmlContent(Document doc)
        {
            // WORDSNET-9970 Check, that specified XML is well formed (XML, which adheres to the XML standard). otherwise
            // output document can not be opened by the Word.  
            for(int i = doc.CustomXmlParts.Count - 1; i >= 0; --i)
            {
                CustomXmlPart customXmlPart = doc.CustomXmlParts[i];
                if (!customXmlPart.ValidateCustomXmlData())
                {
                    // Remove invalid parts. See "TestJira9970Fopc" for details.
                    doc.CustomXmlParts.RemoveAt(i);
                    Warn(WarningType.DataLossCategory, WarningSource.Validator,
                        string.Format("Custom XML part with id={0} has invalid content and was removed.",
                        customXmlPart.Id));
                }
            }    
        }

        /// <summary>
        /// Returns a list of custom XML parts that have non-GUID value of the Id property.
        /// </summary>
        private static List<CustomXmlPart> GetXmlPartsWithNonGuidId(Document doc)
        {
            List<CustomXmlPart> xmlParts = new List<CustomXmlPart>();

            foreach (CustomXmlPart xmlPart in doc.CustomXmlParts)
            {
                if (StringUtil.HasChars(xmlPart.Id) && !IsValidGuid(xmlPart.Id))
                    xmlParts.Add(xmlPart);
            }

            return xmlParts;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified string is a valid GUID.
        /// </summary>
        private static bool IsValidGuid(string value)
        {
            return gGuidRegex.IsMatch(value);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType type, WarningSource source, string description)
        {
            if (mSaveInfo.Document.WarningCallback != null)
                mSaveInfo.Document.WarningCallback.Warning(new WarningInfo(type, source, description));
        }

        /// <summary>
        /// Specifies the OOXML version for the output document on saving as DOCX formats.
        /// </summary>
        private OoxmlComplianceCore Compliance
        {
            get
            {
                return OoxmlComplianceInfo.GetCompliance(mSaveInfo.Document.ComplianceInfo,
                    mSaveInfo.SaveOptions as OoxmlSaveOptions);
            }
        }

        private readonly SaveInfo mSaveInfo;
        private readonly StringToObjDictionary<string> mIdMapping = new StringToObjDictionary<string>();

        private static readonly Regex gGuidRegex =
            new Regex(@"^[{|(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)|}]?$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
