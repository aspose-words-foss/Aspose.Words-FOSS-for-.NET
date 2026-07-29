// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/08/2012 by Dmitry Bormashov

using System;
using System.IO;
using Aspose.Collections;
using Aspose.Images;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Xml;

namespace Aspose.Words
{
    /// <summary>
    /// Provides access to page border art information.
    /// </summary>
    internal class PageBorderArtRepository
    {
        [JavaConvertCheckedExceptions]
        internal static PageBorderArtInfo GetPageBorderArtInfo(PageBorderArt pageBorderArt)
        {
            // Only ECMA376 border art graphics information is available.
            pageBorderArt = GetEcma376BorderArt(pageBorderArt);

            PageBorderArtInfo info = PageBorderArtInfoCache[(int)pageBorderArt];
            if (info != null)
                return info;

            throw new InvalidOperationException("Unknown border art.");
        }

        /// <summary>
        /// Gets the ECMA376 compliant border art corresponding to specified ISO29500 border art.
        /// </summary>
        /// <remarks>
        /// Experiments with MS Word 2016 show that there's a mutual correspondence
        /// between Tribal* border art styles and ISO29500 border art styles,
        /// in range from Triangle1 to Shapes2.
        /// </remarks>
        internal static PageBorderArt GetEcma376BorderArt(PageBorderArt pageBorderArt)
        {
            switch (pageBorderArt)
            {
                case PageBorderArt.Earth3:
                    // Experiments show that this value is not accessible via MS Word GUI, 
                    // and no border art is rendered for this value if specified directly in document.
                    // Let's assign it Earth2 value, according to the legacy code in Iso29500ComplianceEnforcer.
                    return PageBorderArt.Earth2;
                case PageBorderArt.Triangle1:
                    return PageBorderArt.Tribal1;
                case PageBorderArt.Triangle2:
                    return PageBorderArt.Tribal2;
                case PageBorderArt.TriangleCircle1:
                    return PageBorderArt.Tribal3;
                case PageBorderArt.TriangleCircle2:
                    return PageBorderArt.Tribal4;
                case PageBorderArt.Shapes1:
                    return PageBorderArt.Tribal5;
                case PageBorderArt.Shapes2:
                    return PageBorderArt.Tribal6;
                case PageBorderArt.Custom:
                    // MS-OI29500/[MS-OI29500].pdf / 2.1.528 Part 1 Section 17.18.2, ST_Border (Border Styles):
                    // The standard states that custom is a valid enumeration value, however in MS Word, 
                    // if the enumeration value of an ST_Border simple type is custom, then the file is considered corrupt.
                    break;
                default:
                    break;
            }

            return pageBorderArt;
        }

        /// <summary>
        /// Gets the ISO29500 compliant border art corresponding to specified ISO29500 border art.
        /// </summary>
        /// <remarks>
        /// Experiments with MS Word 2016 show that there's a mutual correspondence
        /// between Tribal* border art styles and ISO29500 border art styles,
        /// in range from Triangle1 to Shapes2.
        /// </remarks>
        internal static PageBorderArt GetIso29500BorderArt(PageBorderArt pageBorderArt)
        {
            // Experiments with MS Word 2016 show that there's a mutual correspondence
            // between Tribal* border art styles and ISO29500 border art styles,
            // in range from Triangle1 to Shapes2.
            switch (pageBorderArt)
            {
                case PageBorderArt.Tribal1:
                    return PageBorderArt.Triangle1;
                case PageBorderArt.Tribal2:
                    return PageBorderArt.Triangle2;                    
                case PageBorderArt.Tribal3:
                    return PageBorderArt.TriangleCircle1;
                case PageBorderArt.Tribal4:
                    return PageBorderArt.TriangleCircle2;
                case PageBorderArt.Tribal5:
                    return PageBorderArt.Shapes1;
                case PageBorderArt.Tribal6:
                    return PageBorderArt.Shapes2;
                default:
                    return pageBorderArt;
            }
        }

        private static IntToObjDictionary<PageBorderArtInfo> LoadPageBorderArtXml()
        {
            const string resourceName = "Aspose.Words.Resources.PageBorderArt.Definitions.xml";

            IntToObjDictionary<PageBorderArtInfo> result = new IntToObjDictionary<PageBorderArtInfo>();

            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                AnyXmlReader xr = new AnyXmlReader(stream);
                
                while (xr.ReadChild("BorderArtDefinitions"))
                {
                    PageBorderArtInfo pageBorderArtInfo = new PageBorderArtInfo(
                        Convert.ToInt32(xr.ReadAttribute("id","-1")),
                        Convert.ToInt32(xr.ReadAttribute("contraction", "0")),
                        Convert.ToInt32(xr.ReadAttribute("hexpansion", "0")),
                        Convert.ToInt32(xr.ReadAttribute("vexpansion", "0")));


                    pageBorderArtInfo.SetElement(BorderType.Top, PageBorderArtElementPosition.First,
                                                 GetElementBytes(xr.ReadAttribute("tl", "")));
                    pageBorderArtInfo.SetElement(BorderType.Top, PageBorderArtElementPosition.Middle,
                                                 GetElementBytes(xr.ReadAttribute("t", "")));
                    pageBorderArtInfo.SetElement(BorderType.Top, PageBorderArtElementPosition.Last,
                                                 GetElementBytes(xr.ReadAttribute("tr", "")));

                    pageBorderArtInfo.SetElement(BorderType.Left, PageBorderArtElementPosition.Middle,
                                                 GetElementBytes(xr.ReadAttribute("l", "")));
                    pageBorderArtInfo.SetElement(BorderType.Right, PageBorderArtElementPosition.Middle,
                                                 GetElementBytes(xr.ReadAttribute("r", "")));

                    pageBorderArtInfo.SetElement(BorderType.Bottom, PageBorderArtElementPosition.First,
                                                 GetElementBytes(xr.ReadAttribute("bl", "")));
                    pageBorderArtInfo.SetElement(BorderType.Bottom, PageBorderArtElementPosition.Middle,
                                                 GetElementBytes(xr.ReadAttribute("b", "")));
                    pageBorderArtInfo.SetElement(BorderType.Bottom, PageBorderArtElementPosition.Last,
                                                 GetElementBytes(xr.ReadAttribute("br", "")));
                   
                    result.Add(pageBorderArtInfo.Id, pageBorderArtInfo);
                }
            }
            return result;
        }

        private static IntToObjDictionary<PageBorderArtInfo> PageBorderArtInfoCache
        {
            get
            {
                // double-checked locking pattern.
                if (gPageBorderArtInfoCache == null)
                {
                    lock (gCacheSyncRoot)
                    {
                        if (gPageBorderArtInfoCache == null)
                            gPageBorderArtInfoCache = LoadPageBorderArtXml();
                    }
                }

                return gPageBorderArtInfoCache;
            }
        }

        [JavaThrows(false)]
        private static byte[] GetElementBytes(string elementName)
        {
            try
            {
                using (Stream stream =
                    ResourceUtil.FetchResourceStream(String.Format("Aspose.Words.Resources.PageBorderArt.{0}", elementName)))
                {
                    // Some images for border art don't appear correctly if they don't have
                    // placeable header. To solve this problem we add the code below.
                    // In this case, width and height in imageSizeCore can have any values,
                    // but they should be greater than zero.
                    ImageSizeCore imageSizeCore = ImageSizeCore.CreateWithResolution(
                        1, 1, ImageConstants.StandardResolution);

                    return ImageUtil.PrependWmfPlaceableHeader(StreamUtil.CopyStreamToByteArray(stream), imageSizeCore);
                }
            }
            catch
            {
                return new byte[0];
            }
        }

        private static readonly object gCacheSyncRoot = new object();
        //JAVA: volatile modifier is added to static field with purpose to double-check pattern work in java.
        private static volatile IntToObjDictionary<PageBorderArtInfo> gPageBorderArtInfoCache;
    }
}
