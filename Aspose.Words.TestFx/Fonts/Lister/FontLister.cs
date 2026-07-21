// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Konstantin Kornilov, Andrey Noskov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;

namespace Aspose.Words.Tests.Fonts
{
    public class FontLister
    {
        public FontListing GetListing()
        {
            SortedStringListGeneric<FontFile> items = new SortedStringListGeneric<FontFile>(false);
            GetFontsFormSystemFolder(items);
            GetFontsFromRegistry(items);

            FontListing result = new FontListing();
            foreach (FontFile item in items.Values)
                result.Items.Add(item);
            return result;
        }

        private void GetFontsFromRegistry(SortedStringListGeneric<FontFile> items)
        {
            StringToObjDictionary<string> fileNames = new StringToObjDictionary<string>();
            SystemPal.GetFontFileNamesFromRegistry(fileNames);

            foreach (string file in fileNames.Values)
            {
                if (items.ContainsKey(file))
                {
                    items[file].IsInRegistry = true;
                    continue;
                }

                FontFile item = ParseFile(file);
                if(item == null) continue;
                item.IsInRegistry = true;
                items.Add(item.Path, item);
            }
        }

        private void GetFontsFormSystemFolder(SortedStringListGeneric<FontFile> items)
        {
            string folder = SystemPal.GetWindowsFontsFolder();
            foreach (string file in Directory.GetFiles(folder))
            {
                FontFile item = ParseFile(file);
                if (item == null) continue;

                item.IsInFontsFolder = true;
                items.Add(item.Path, item);
            }
        }

        private FontFile ParseFile(string file)
        {
            TTFontFiler filer = new TTFontFiler();
            IList<FontSearchInfo> searchInfo = new List<FontSearchInfo>();
            filer.ExtractFontSearchInfo(searchInfo, new FileFontData(file), 0);

            if (searchInfo.Count == 0)
                return null;

            FontFile result = searchInfo[0].IsTtc
                ? ParseCollection(searchInfo)
                : ParseOpenType((FontSearchInfo)searchInfo[0]);

            if (result == null)
                return null;

            result.Path = file;
            result.Size = GetFileSize(result.Path);
            result.Md5 = GetFileMd5(result.Path);
            return result;
        }

        private static long GetFileSize(string path)
        {
            FileInfo info = new FileInfo(path);
            return info.Length;
        }

        internal static string GetFileMd5(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] fileHash = HashUtil.ComputeHash(DigestAlgorithm.MD5, stream);
                return BitConverter.ToString(fileHash).Replace("-", "").ToUpper();
            }
        }

        private static OpenTypeFont ParseOpenType(FontSearchInfo info)
        {
            OpenTypeFont openTypeFont = new OpenTypeFont();
            openTypeFont.Items.Add(ParseFont(info));
            return openTypeFont;
        }

        private static FontFile ParseCollection(IList<FontSearchInfo> searchInfo)
        {
            TrueTypeCollection collection = new TrueTypeCollection();

            foreach (FontSearchInfo info in searchInfo)
                collection.Items.Add(ParseFont(info));

            return collection;
        }

        private static FontMetaInfo ParseFont(FontSearchInfo info)
        {
            FontMetaInfo fontMetaInfo = new FontMetaInfo();
            fontMetaInfo.FamilyName = info.FontFamilyName;
            fontMetaInfo.FullName = info.FontFullName;
            fontMetaInfo.Style = info.Style;
            fontMetaInfo.Version = info.Version;

            return fontMetaInfo;
        }
    }
}
