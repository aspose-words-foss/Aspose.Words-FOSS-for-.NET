// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2014 by Alexey Morozov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Windows Registry substitute. Holds OLE related information.
    /// </summary>
    internal class OleRegistryInfo
    {
        private OleRegistryInfo(string fileExt, string clsId, string progId, string userType, string iconName, OleEmbeddingType embeddingType)
        {
            ProgId = progId;
            ClsId = clsId;
            UserType = userType;
            FileExt = fileExt;
            IconName = iconName;
            EmbeddingType = embeddingType;
        }

        /// <summary>
        /// Returns OLE information by given ProgId value. If no information found returns OLE Packager record.
        /// </summary>
        internal static OleRegistryInfo GetByProgId(string progId)
        {
            OleRegistryInfo oleInfo = gMapByProgId[progId];
            if (oleInfo == null)
                oleInfo = GetOlePackage();

            return oleInfo;
        }

        /// <summary>
        /// Returns OLE information by given file extension. If no information found returns OLE Packager record.
        /// </summary>
        internal static OleRegistryInfo GetByFileExt(string fileExt)
        {
            OleRegistryInfo oleInfo = gMapByFileExt[fileExt];
            if (oleInfo == null)
                oleInfo = GetOlePackage();

            return oleInfo;
        }

        /// <summary>
        /// Returns OLE information by given ClsId. If no information found returns OLE Packager record.
        /// </summary>
        internal static OleRegistryInfo GetByClsId(Guid clsId)
        {
            string formattedClsId = string.Format("{{{0}}}", clsId.ToString().ToUpperInvariant());
            OleRegistryInfo oleInfo = gMapByClsId[formattedClsId];

            return (oleInfo == null)
                ? GetOlePackage()
                : oleInfo;
        }

        internal static OleRegistryInfo GetOlePackage()
        {
            return gPackage;
        }

        /// <summary>
        /// Gets ClsId for embedded document type (ProgId). Returns string of empty Guid if given progId is not found.
        /// </summary>
        /// <remarks>
        /// This is used during document roundtrip and does not return OLE Packager record.
        /// Instead it returns empty ClsId which is written if information about OLE object is missed in our database.
        /// </remarks>
        internal static string GetClsId(string progId)
        {
            OleRegistryInfo oleInfo = gMapByProgId[progId];

            return (oleInfo != null) ? oleInfo.ClsId : Guid.Empty.ToString();
        }

        /// <summary>
        /// Gets default mapped extension by ClsID.
        /// </summary>
        internal static string GetExtensionByClsId(Guid guid)
        {
            string clsId = String.Format("{{{0}}}", guid.ToString().ToLower());

            if (gMapExtByClsId.ContainsKey(clsId))
                return (gMapExtByClsId[clsId]);

            return ".bin";
        }

        static OleRegistryInfo()
        {
            gOleInfos = new List<OleRegistryInfo>();

            // When searching through clsIDs when no ProgID provided only the first record will be chosen.
            // So the order of added pairs "extension/{clsid} for duplicating clsIDs is important.

            // Word.12
            Add(".docx", "{F4754C9B-64F5-4B40-8AF4-679732AC0607}", "Word.Document.12", "Microsoft Word Document", "docx", OleEmbeddingType.Ole2StreamPackage);
            Add(".dotx", "{912ABC52-36E2-4714-8E62-A8B73CA5E390}", "Word.Template.12", "Microsoft Word Template", "dotx", OleEmbeddingType.Ole2StreamPackage);
            Add(".docm", "{18A06B6B-2F3F-4E2B-A611-52BE631B2D22}", "Word.DocumentMacroEnabled.12", "Microsoft Word Macro-Enabled Document", "docm", OleEmbeddingType.Ole2StreamPackage);
            Add(".dotm", "{8A624388-AA27-43E0-89F8-2A12BFF7BCCD}", "Word.TemplateMacroEnabled.12", "Microsoft Word Macro-Enabled Template", "dotm", OleEmbeddingType.Ole2StreamPackage);

            // Word.8
            Add(".doc", "{00020906-0000-0000-C000-000000000046}", "Word.Document.8", "Microsoft Word 97 - 2003 Document", "doc", OleEmbeddingType.Ole2Storage);
            Add(".dot", "{00020906-0000-0000-C000-000000000046}", "Word.Template.8", "Microsoft Word 97 - 2003 Template", "dot", OleEmbeddingType.Ole2Storage);

            // Excel.12
            Add(".xlsx", "{00020830-0000-0000-C000-000000000046}", "Excel.Sheet.12", "Microsoft Excel Worksheet", "xlsx", OleEmbeddingType.Ole2StreamPackage);
            Add(".xlsb", "{00020833-0000-0000-C000-000000000046}", "Excel.SheetBinaryMacroEnabled.12", "Microsoft Excel Binary Worksheet", "xlsb", OleEmbeddingType.Ole2StreamPackage);
            Add(".xlsm", "{00020832-0000-0000-C000-000000000046}", "Excel.SheetMacroEnabled.12", "Microsoft Excel Macro-Enabled Worksheet", "xlsm", OleEmbeddingType.Ole2StreamPackage);
            Add(".xltx", "{00020830-0000-0000-C000-000000000046}", "Excel.Template", "Microsoft Excel Template", "xltx", OleEmbeddingType.Ole2StreamPackage);
            Add(".xltm", "{00020832-0000-0000-C000-000000000046}", "Excel.TemplateMacroEnabled", "Microsoft Excel Macro-Enabled Template", "xltm", OleEmbeddingType.Ole2StreamPackage);

            // Excel.8
            Add(".xls", "{00020820-0000-0000-C000-000000000046}", "Excel.Sheet.8", "Microsoft Excel 97-2003 Worksheet", "xls", OleEmbeddingType.Ole2Storage);
            Add(".xlt", "{00020820-0000-0000-C000-000000000046}", "Excel.Template.8", "Microsoft Excel Template", "xlt", OleEmbeddingType.Ole2Storage);

            // OpenDocument
            Add(".odp", "{C282417B-2662-44B8-8A94-3BFF61C50900}", "PowerPoint.OpenDocumentPresentation.12", "OpenDocument Presentation", "odp", OleEmbeddingType.Ole2StreamEmbeddedOdf);
            Add(".ods", "{EABCECDB-CC1C-4A6F-B4E3-7F888A5ADFC8}", "Excel.OpenDocumentSpreadsheet.12", "OpenDocument Spreadsheet", "ods", OleEmbeddingType.Ole2StreamEmbeddedOdf);
            Add(".odt", "{1B261B22-AC6A-4E68-A870-AB5080E8687B}", "Word.OpenDocumentText.12", "OpenDocument Text", "odt", OleEmbeddingType.Ole2StreamEmbeddedOdf);

            // PowerPoint.12
            Add(".potx", "{75D01070-1234-44E9-82F6-DB5B39A47C13}", "PowerPoint.Template.12", "Microsoft PowerPoint Template", "potx", OleEmbeddingType.Ole2StreamPackage);
            Add(".potm", "{AA14F9C9-62B5-4637-8AC4-8F25BF29D5A7}", "PowerPoint.TemplateMacroEnabled.12", "Microsoft PowerPoint Macro-Enabled Design Template", "potm",
                OleEmbeddingType.Ole2StreamPackage);
            Add(".pptx", "{CF4F55F4-8F87-4D47-80BB-5808164BB3F8}", "PowerPoint.Show.12", "Microsoft PowerPoint Presentation", "pptx", OleEmbeddingType.Ole2StreamPackage);
            Add(".pptm", "{DC020317-E6E2-4A62-B9FA-B3EFE16626F4}", "PowerPoint.ShowMacroEnabled.12", "Microsoft PowerPoint Macro-Enabled Presentation", "pptm", OleEmbeddingType.Ole2StreamPackage);
            Add(".ppsx", "{CF4F55F4-8F87-4D47-80BB-5808164BB3F8}", "PowerPoint.SlideShow.12", "Microsoft PowerPoint Slide Show", "ppsx", OleEmbeddingType.Ole2StreamPackage);
            Add(".ppsm", "{DC020317-E6E2-4A62-B9FA-B3EFE16626F4}", "PowerPoint.SlideShowMacroEnabled.12", "Microsoft PowerPoint Macro-Enabled Slide Show", "ppsm", OleEmbeddingType.Ole2StreamPackage);

            // AM Couldn't find how to get this file so use OLE Packager embedding mode for a while.
            Add(".sldx", "{048EB43E-2059-422F-95E0-557DA96038AF}", "PowerPoint.Slide.12", "Microsoft PowerPoint Slide", "sldx", OleEmbeddingType.Ole1Package);
            Add(".sldm", "{3C18EAE4-BC25-4134-B7DF-1ECA1337DDDC}", "PowerPoint.SlideMacroEnabled.12", "Microsoft PowerPoint Macro-Enabled Slide", "sldx", OleEmbeddingType.Ole1Package);

            // PowerPoint.8
            Add(".ppt", "{64818D10-4F9B-11CF-86EA-00AA00B929E8}", "PowerPoint.Show.8", "Microsoft PowerPoint 97-2003 Presentation", "ppt", OleEmbeddingType.Ole2Storage);
            Add(".pot", "{64818D11-4F9B-11CF-86EA-00AA00B929E8}", "PowerPoint.Template.8", "Microsoft PowerPoint 97-2003 Template", "pot", OleEmbeddingType.Ole2Storage);
            Add(".pps", "{64818D10-4F9B-11CF-86EA-00AA00B929E8}", "PowerPoint.SlideShow.8", "Microsoft PowerPoint 97-2003 Slide Show", "pps", OleEmbeddingType.Ole2Storage);

            // PDF
            Add(".pdf", "{B801CA65-A1FC-11D0-85AD-444553540000}", "AcroExch.Document.7", "Adobe Acrobat Document", "pdf", OleEmbeddingType.Ole2StreamContents);

            //AutoCad
            Add(".dwg", "{6a221957-2d85-42a7-8e19-be33950d1deb}", "AutoCAD.Drawing.19", "Autodesk AutoCAD Document", null, OleEmbeddingType.Ole2StreamContents);

            // BMP
            Add(".bmp", "{0003000A-0000-0000-C000-000000000046}", "PBrush", "Bitmap Image", "bmp", OleEmbeddingType.Ole1NativeStream);

            // Visio
            Add(".vsd", "{00021A14-0000-0000-C000-000000000046}", "Visio.Drawing.11", "Microsoft Visio Document", "vsd", OleEmbeddingType.Ole2Storage);
            Add(".vsdx", "{00021a15-0000-0000-c000-000000000046}", "Visio.Drawing.15", "Microsoft Visio Document", "vsd", OleEmbeddingType.Ole2StreamPackage);

            // MS Project
            Add(".mpp", "{74b78f3a-c8c8-11d1-be11-00c04fb6faf1}", "MSProject.Project.9", "Microsoft Project 9.0", "mpp", OleEmbeddingType.Ole2Storage);

            Add(".html", "{25336920-03F9-11CF-8FD0-00AA00686F13}", "htmlfile", "HTML Document", "html", OleEmbeddingType.Ole1Package);

            // Equation.3
            Add(null, "{0002ce02-0000-0000-c000-000000000046}", "Equation.3", "Microsoft Equation 3.0", null, OleEmbeddingType.Ole2StreamEquationNative);

            Add(null, "{e4c18d40-1cd5-101c-b325-00aa001f3168}", "OrgPlusWOPX.4", "Organization Chart Add-in for Microsoft Office", "xls", OleEmbeddingType.Ole2Storage);

            // Below object either can't be saved to file or suspicious to be inserted from file.
            // For example I think we can't distinguish between Excel.Sheet.5 and Excel.Sheet.8, because they both have .xls file extension.
            // Still they can be inserted from stream using progId.
            Add(null, "{64818D11-4F9B-11CF-86EA-00AA00B929E8}", "PowerPoint.Slide.8", "Microsoft PowerPoint 97-2003 Slide", "ppt", OleEmbeddingType.Ole2Storage);
            Add("{00030007-0000-0000-C000-000000000046}", "MSDraw", "Microsoft Word Picture");
            Add("{0003000B-0000-0000-C000-000000000046}", "Equation", "Microsoft Equation 2.0");
            Add("{0003000D-0000-0000-C000-000000000046}", "SoundRec", "");
            Add("{0003000E-0000-0000-C000-000000000046}", "MPlayer", "");
            Add("{0004c8d8-0000-0000-c000-000000000046}", "WPGraphic21", "");
            Add("{EA7BAE70-FB3B-11CD-A903-00AA00510EA3}", "PowerPoint.Show.7", "");
            Add("{EA7BAE71-FB3B-11CD-A903-00AA00510EA3}", "PowerPoint.Slide.7", "");
            Add("{64818D10-4F9B-11CF-86EA-00AA00B929E8}", "PowerPoint.Wizard.8", "");
            Add("{00020900-0000-0000-C000-000000000046}", "Word.Document.6", "");
            Add("{00020901-0000-0000-C000-000000000046}", "Word.Picture.6", "");
            Add(null, "{00020907-0000-0000-C000-000000000046}", "Word.Picture.8", "Image Microsoft Office Word", null, OleEmbeddingType.Ole2Storage);
            Add("{00020906-0000-0000-C000-000000000046}", "Word.RTF.8", "");
            Add("{00024500-0000-0000-C000-000000000046}", "Excel.Application.12", "");
            Add("{00020811-0000-0000-C000-000000000046}", "Excel.Chart.5", "");
            Add(null, "{00020821-0000-0000-C000-000000000046}", "Excel.Chart.8", "Microsoft Excel Chart", "xls", OleEmbeddingType.Ole2Storage);
            Add("{00020832-0000-0000-C000-000000000046}", "Excel.CSV", "");
            Add("{00020810-0000-0000-C000-000000000046}", "Excel.Sheet.5", "");
            Add("{D3E34B21-9D75-101A-8C3D-00AA001A1652}", "Paint.Picture", "");
            Add("{448BB771-CFE2-47C4-BCDF-1FBF378E202C}", "opendocument.DrawDocument.1", "");
            Add("{73fddc80-aea9-101a-98a7-00aa00374959}", "WordPad.Document.1", "Document WordPad");

            // Special record. It returned if OLE object is unknown.
            gPackage = new OleRegistryInfo(null, PackageClsId, "Package", "OLE Package", OleUtil.Packager, OleEmbeddingType.Ole1Package);
            Add(gPackage.FileExt, gPackage.ClsId, gPackage.ProgId, gPackage.UserType, gPackage.IconName, OleEmbeddingType.Ole1Package);
        }

        private static void Add(string clsId, string progId, string userType)
        {
            Add(null, clsId, progId, userType, null, OleEmbeddingType.Ole1Package);
        }

        [JavaThrows(false)]
        private static void Add(string fileExt, string clsId, string progId, string userType, string iconName, OleEmbeddingType type)
        {
            Debug.Assert(progId != null);
            Debug.Assert(clsId != null);
            Debug.Assert((!StringUtil.HasChars(iconName) || OleUtil.IcoFileLibrary.ContainsKey(iconName)),
                "The 'iconName' argument should be empty or already registered with OleUtil.IcoFileLibrary.");

            OleRegistryInfo oleInfo = new OleRegistryInfo(fileExt, clsId, progId, userType, iconName, type);
            gOleInfos.Add(oleInfo);

            // File extension can be null for certain entries.
            // This means that the OLE object can't be saved to file so we should not map them to file extension.
            if (fileExt != null)
            {
                gMapByFileExt.Add(fileExt, oleInfo);

                // One ClsId can be used for different extensions so we use only the first one.
                string id = clsId.ToLower();
                if (!gMapExtByClsId.ContainsKey(id))
                    gMapExtByClsId.Add(id, fileExt);
            }

            gMapByProgId.Add(progId, oleInfo);
            if (!gMapByClsId.ContainsKey(clsId))
                gMapByClsId.Add(clsId, oleInfo);
        }

        internal readonly string ProgId;
        internal readonly string ClsId;
        internal readonly string UserType;
        internal readonly string FileExt;
        internal readonly string IconName;
        internal readonly OleEmbeddingType EmbeddingType;

        internal const string PackageClsId = "0003000c-0000-0000-c000-000000000046";

        private static readonly List<OleRegistryInfo> gOleInfos;

        private static readonly StringToObjDictionary<OleRegistryInfo> gMapByFileExt =
            new StringToObjDictionary<OleRegistryInfo>();
        private static readonly StringToObjDictionary<OleRegistryInfo> gMapByProgId =
            new StringToObjDictionary<OleRegistryInfo>();
        private static readonly StringToObjDictionary<OleRegistryInfo> gMapByClsId =
            new StringToObjDictionary<OleRegistryInfo>();
        private static readonly Dictionary<string,string> gMapExtByClsId =
            new Dictionary<string,string>();

        private static readonly OleRegistryInfo gPackage;
    }
}
