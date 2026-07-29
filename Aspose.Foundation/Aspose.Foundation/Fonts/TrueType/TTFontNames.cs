// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/09/2009 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Collection of names of true type font stored in true type font Naming Table.
    /// Collection contains only FamilyName, SubFamilyName, FullFontName and PostSciptName.
    /// </summary>
    internal class TTFontNames
    {
        public static TTFontNames Read(BigEndianBinaryReader reader)
        {
            long tableStart = reader.BaseStream.Position;
            ushort format = reader.ReadUInt16();
            if (format != 0)
                throw new InvalidOperationException("Unsupported Name table format.");

            ushort count = reader.ReadUInt16();
            // Offset to start of string storage (from start of table).
            ushort stringStorageOffset = reader.ReadUInt16();

            // Read NameRecords.
            TTFontNames result = new TTFontNames();
            List<TTFontNameRecord> familyNames = new List<TTFontNameRecord>();
            List<TTFontNameRecord> subfamilyNames = new List<TTFontNameRecord>();
            List<TTFontNameRecord> fullNames = new List<TTFontNameRecord>();
            List<TTFontNameRecord> postScriptNames = new List<TTFontNameRecord>();
            List<TTFontNameRecord> versions = new List<TTFontNameRecord>();
            result.mFontSpecificNames = new List<TTFontNameRecord>();

            for (int i = 0; i < count; i++)
            {
                TTFontNameRecord record = TTFontNameRecord.Read(reader, tableStart, stringStorageOffset);
                if(record == null)
                    continue;

                switch (record.NameId)
                {
                    case NameString.FamilyName:
                        familyNames.Add(record);
                        break;
                    case NameString.SubFamilyName:
                        subfamilyNames.Add(record);
                        break;
                    case NameString.FullFontName:
                        fullNames.Add(record);
                        break;
                    case NameString.PostSciptName:
                        postScriptNames.Add(record);
                        break;
                    case NameString.Version:
                        versions.Add(record);
                        break;
                    default:
                        break;
                }

                if (record.IsFontSpecificName)
                    result.mFontSpecificNames.Add(record);
            }

            result.FamilyName = SelectSingleName(familyNames);
            result.SubFamilyName = SelectSingleName(subfamilyNames);
            result.FullFontName = SelectSingleName(fullNames);
            result.PostScriptName = SelectSingleName(postScriptNames);
            result.VersionString = SelectSingleName(versions);
            result.mFamilyNamesAllLanguages = SelectNamesList(familyNames);
            result.mFullFontNamesAllLanguages = SelectNamesList(fullNames);

            if (result.FamilyName == null || result.SubFamilyName == null || result.FullFontName == null)
                throw new InvalidOperationException("Font do not have required names.");

            return result;
        }

        private static List<string> SelectNamesList(List<TTFontNameRecord> names)
        {
            // WORDSNET-10040 Experiments shows that MW prefers Windows platform and ignores other platform entries if it is available.
            // Follow the MW logic and use only single platform names in following order: MS, Mac, Unicode.
            List<string> result = GetPlatformNames(names, Cmap.PlatformIdMicrosoft);
            if (result.Count == 0)
                result = GetPlatformNames(names, Cmap.PlatformIdMacintosh);
            if (result.Count == 0)
                result = GetPlatformNames(names, Cmap.PlatformIdUnicode);

            return result;
        }

        private static List<string> GetPlatformNames(List<TTFontNameRecord> names, int platform)
        {
            List<string> result = new List<string>();
            foreach (TTFontNameRecord record in names)
                if (record.PlatformId == platform)
                    result.Add(record.Name);

            return result;
        }

        private static string SelectSingleName(List<TTFontNameRecord> names)
        {
            // First priority is platform: MS->Mac->Unicode.
            string result = SelectSingleNamePlatform(names, Cmap.PlatformIdMicrosoft);
            result = result ?? SelectSingleNamePlatform(names, Cmap.PlatformIdMacintosh);
            result = result ?? SelectSingleNamePlatform(names, Cmap.PlatformIdUnicode);
            return result;
        }

        private static string SelectSingleNamePlatform(List<TTFontNameRecord> names, int platform)
        {
            List<string> platformName = GetPlatformNames(names, platform);
            if (platformName.Count == 0)
                return null;

            // Second priority is encoding. First try filer partially supported encodings.
            List<TTFontNameRecord> filteredNames = FilterPartiallySupportedEncodings(names);

            // If there are no fully supported encoding, use all other.
            if (filteredNames.Count == 0)
                filteredNames = names;

            return SelectSingleNameLanguage(filteredNames, platform);
        }

        private static List<TTFontNameRecord> FilterPartiallySupportedEncodings(List<TTFontNameRecord> names)
        {
            List<TTFontNameRecord> result = new List<TTFontNameRecord>();
            foreach (TTFontNameRecord rec in names)
                if (IsEncodingFullySupported(rec.PlatformId, rec.EncodingId))
                    result.Add(rec);

            return result;
        }

        private static bool IsEncodingFullySupported(int platform, int encoding)
        {
            // Currently only MS PRC encoding may return unreliable result. Other encoding are properly supported.
            return !(platform == Cmap.PlatformIdMicrosoft && encoding == Cmap.MicrosoftEncodingIdPrc);
        }

        private static string SelectSingleNameLanguage(List<TTFontNameRecord> names, int platform)
        {
            if (names.Count == 0)
                return null;

            // Third priority is language. First try to get English name.
            string result = null;
            if (platform == Cmap.PlatformIdMicrosoft)
                result = GetNameSpecificLanguage(names, Cmap.MicrosoftLanguageIdEnglishUs);
            else if (platform == Cmap.PlatformIdMacintosh)
                result = GetNameSpecificLanguage(names, Cmap.MacintoshLanguageIdEnglish);

            // If there are no English name then use any name.
            if (result == null)
                result = names[0].Name;

            return result;
        }

        private static string GetNameSpecificLanguage(List<TTFontNameRecord> names, int language)
        {
            foreach (TTFontNameRecord rec in names)
                if (rec.LanguageId == language)
                    return rec.Name;

            return null;
        }

        public string GetFontSpecificName(int nameId)
        {
            if (mFontSpecificNames.Count == 0 || !TTFontNameRecord.IsFontSpecificNameId(nameId))
                return null;

            TTFontNameRecord record = SelectFontSpecificRecord(
                nameId,
                Cmap.PlatformIdMicrosoft,
                Cmap.MicrosoftLanguageIdEnglishUs);
            if (record != null)
                return record.Name;
            record = SelectFontSpecificRecord(
                nameId,
                Cmap.PlatformIdMacintosh,
                Cmap.MacintoshLanguageIdEnglish);
            if (record != null)
                return record.Name;
            record = SelectFontSpecificRecord(nameId);
            return record != null
                ? record.Name
                : null;
        }

        private TTFontNameRecord SelectFontSpecificRecord(int nameId, int platform, int language)
        {
            foreach (TTFontNameRecord record in FontSpecificNames)
                if ((int)record.NameId == nameId && record.PlatformId == platform && record.LanguageId == language)
                    return record;
            return null;
        }

        private TTFontNameRecord SelectFontSpecificRecord(int nameId)
        {
            foreach (TTFontNameRecord record in FontSpecificNames)
                if ((int)record.NameId == nameId)
                    return record;
            return null;
        }

        /// <summary>
        /// Font version string.
        /// </summary>
        public string VersionString { get; private set; }

        public string FamilyName { get; private set; }

        public string SubFamilyName { get; private set; }

        public string FullFontName { get; private set; }

        public string PostScriptName { get; private set; }

        public IList<string> FamilyNamesAllLanguages
        {
            get { return mFamilyNamesAllLanguages; }
        }

        public IList<string> FullFontNamesAllLanguages
        {
            get { return mFullFontNamesAllLanguages; }
        }

        public IList<TTFontNameRecord> FontSpecificNames
        {
            get { return mFontSpecificNames; }
        }

        private List<string> mFamilyNamesAllLanguages;
        private List<string> mFullFontNamesAllLanguages;
        private List<TTFontNameRecord> mFontSpecificNames;
    }
}
