// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Konstantin Kornilov, Andrey Noskov

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests.Fonts
{
    [JavaDelete("Used only by TestFonts that launched only in .Net by design.")]
    public class FontComparer
    {
        public ComparsionResult CompareListing(FontListing local, FontListing standard)
        {
            Dictionary<string, List<FontFile>> localIndex = BuildFontNameIndex(local);
            Dictionary<string, List<FontFile>> standardIndex = BuildFontNameIndex(standard);

            ComparsionResult result = new ComparsionResult();
            GetMissingFonts(localIndex, standardIndex, result);
            GetExtraFonts(localIndex, standardIndex, result);
            GetDifferentFonts(localIndex, standardIndex, result);
            return result;
        }

        private void GetDifferentFonts(Dictionary<string, List<FontFile>> localIndex, Dictionary<string, List<FontFile>> standardIndex, ComparsionResult result)
        {
            foreach (string localKey in localIndex.Keys)
            {
                if (!standardIndex.ContainsKey(localKey))
                    continue;

                if (!CompareFiles(localIndex[localKey], standardIndex[localKey]))
                    result.Differences.Add(
                            new DifferentFont
                            {
                                FontFullName = localKey,
                                LocalFontFiles = localIndex[localKey],
                                StandardFontFiles = standardIndex[localKey]
                            });
            }
        }

        private bool CompareFiles(List<FontFile> localFiles, List<FontFile> standardFiles)
        {
            if (localFiles.Count != standardFiles.Count)
                return false;

            bool result = true;

            for (int i = 0; i < localFiles.Count; i++)
            {
                bool intermadiateResult = false;
                for (int j = 0; j < standardFiles.Count; j++)
                {
                    if (localFiles[i].Md5 == standardFiles[j].Md5)
                    {
                        intermadiateResult = true;
                        break;
                    }
                }

                result &= intermadiateResult;

                if (!result)
                    break;
            }

            return result;
        }

        private void GetExtraFonts(Dictionary<string, List<FontFile>> localIndex, Dictionary<string, List<FontFile>> standardIndex, ComparsionResult result)
        {
            foreach (string localKey in localIndex.Keys)
            {
                if (!standardIndex.ContainsKey(localKey))
                    result.Differences.Add(
                        new ExtraFont
                        {
                            FontFullName = localKey,
                            LocalFontFiles = localIndex[localKey]
                        });
            }
        }

        private void GetMissingFonts(Dictionary<string, List<FontFile>> localIndex,
                                     Dictionary<string, List<FontFile>> standardIndex, ComparsionResult result)
        {
            foreach (string standardKey in standardIndex.Keys)
            {
                if (!localIndex.ContainsKey(standardKey))
                    result.Differences.Add(
                        new MissingFont
                        {
                            FontFullName = standardKey,
                            StandardFontFiles = standardIndex[standardKey]
                        });
            }
        }


        private Dictionary<string, List<FontFile>> BuildFontNameIndex(FontListing listing)
        {
            Dictionary<string, List<FontFile>> index = new Dictionary<string, List<FontFile>>(StringComparer.OrdinalIgnoreCase);
            foreach (FontFile item in listing.Items)
            {
                OpenTypeFont font = item as OpenTypeFont;
                if (font != null)
                {
                    AddFontToIndex(font, index);
                    continue;
                }

                TrueTypeCollection collection = item as TrueTypeCollection;
                if (collection != null)
                    AddCollectionToIndex(collection, index);
            }
            return index;
        }

        private void AddCollectionToIndex(TrueTypeCollection collection, Dictionary<string, List<FontFile>> index)
        {
            foreach (FontMetaInfo info in collection.Items)
                AddEntryToIndex(info, collection, index);
        }

        private void AddFontToIndex(OpenTypeFont font, Dictionary<string, List<FontFile>> index)
        {
            AddEntryToIndex(font.FontInfo, font, index);
        }

        private void AddEntryToIndex(FontMetaInfo fontInfo, FontFile item, Dictionary<string, List<FontFile>> index)
        {
            string fontName = fontInfo.FullName;
            if (!index.ContainsKey(fontName))
                index.Add(fontName, new List<FontFile>());

            index[fontName].Add(item);
        }
    }
}
