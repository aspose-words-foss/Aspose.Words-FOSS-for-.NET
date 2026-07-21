// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2016 by Konstantin Kornilov, Andrey Noskov

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Aspose.JavaAttributes;

namespace Aspose.Words.Tests.Fonts
{
    [XmlInclude(typeof(MissingFont))]
    [XmlInclude(typeof(DifferentFont))]
    [XmlInclude(typeof(ExtraFont))]
    [JavaDelete("Used only by TestFonts that launched only in .Net by design.")]
    public abstract class ComparsionDifference
    {
        public abstract string GetDiffDetails();

        public string FontFullName;
    }

    [JavaDelete("Used only by TestFonts that launched only in .Net by design.")]
    public class MissingFont : ComparsionDifference
    {
        public override string GetDiffDetails()
        {
            StringBuilder sb = new StringBuilder();
            foreach (FontFile fontFile in StandardFontFiles)
            {
                foreach (FontMetaInfo fontInfo in fontFile.Items)
                {
                    if (sb.Length != 0)
                        sb.AppendLine(string.Empty);

                    sb.AppendFormat("Missed font: {0}, version: {1}, path: {2}",
                        fontInfo.FamilyName, fontInfo.Version, fontFile.Path);
                }
            }
            return sb.ToString();
        }

        internal List<FontFile> StandardFontFiles;
    }

    [JavaDelete("Used only by TestFonts that launched only in .Net by design.")]
    public class DifferentFont : ComparsionDifference
    {
        public override string GetDiffDetails()
        {
            StringBuilder sb = new StringBuilder();
            foreach (FontFile standardFontFile in StandardFontFiles)
            {
                foreach (FontMetaInfo standardFontInfo in standardFontFile.Items)
                {
                    if (sb.Length != 0)
                        sb.AppendLine(string.Empty);

                    sb.AppendFormat("Different font (standard): {0}, version: {1}, path: {2}",
                        standardFontInfo.FamilyName, standardFontInfo.Version, standardFontFile.Path);
                }
            }

            foreach (FontFile localFontFile in LocalFontFiles)
            {
                foreach (FontMetaInfo localFontInfo in localFontFile.Items)
                {
                    if (sb.Length != 0)
                        sb.AppendLine(string.Empty);

                    sb.AppendFormat("Different font (local): {0}, version: {1}, path: {2}",
                        localFontInfo.FamilyName, localFontInfo.Version, localFontFile.Path);
                }
            }

            return sb.ToString();
        }

        internal List<FontFile> StandardFontFiles;
        internal List<FontFile> LocalFontFiles;
    }

    [JavaDelete("Used only by TestFonts that launched only in .Net by design.")]
    public class ExtraFont : ComparsionDifference
    {
        public override string GetDiffDetails()
        {
            StringBuilder sb = new StringBuilder();
            foreach (FontFile fontFile in LocalFontFiles)
            {
                foreach (FontMetaInfo fontInfo in fontFile.Items)
                {
                    if (sb.Length != 0)
                        sb.AppendLine(string.Empty);

                    sb.AppendFormat("Extra font: {0}, version: {1}, path: {2}",
                        fontInfo.FamilyName, fontInfo.Version, fontFile.Path);
                }
            }
            return sb.ToString();
        }

        internal List<FontFile> LocalFontFiles;
    }
}
