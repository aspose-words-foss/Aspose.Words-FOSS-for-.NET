// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2016 by Andrey Noskov

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Known uri of dml extensions. 
    /// Each extension has an Uri attribute, which serves as an identifier to indicate information about the extension.
    /// Some of them are described in MS-ODRAWXML.pdf - Office Drawing Extensions to Office Open XML 
    /// Structure Specification (v20110608). The other were detected during testing.
    /// </summary>
    internal static class DmlExtensionUri
    {
        /// <summary>
        /// The cNvPr extension adds non-visual drawing properties to points in the diagram. This enables 
        /// adding hyperlinks and alternative text for use by assistive technologies or applications that do not 
        /// display the diagram shapes.
        /// </summary>
        internal const string NonVisualPr = "{E40237B7-FDA0-4F09-8148-C483321AD2D9}";

        /// <summary>
        /// Extension that specifies the properties for displaying an online video.
        /// </summary>
        internal const string VideoPr = "{C809E66F-F1BF-436E-b5F7-EEA9579F0CBA}";

        /// <summary>
        /// Specifies the shape identifier of a legacy drawing object.
        /// </summary>
        internal const string CompatExt = "{63B3BB69-23CF-44e3-9099-C40C66FF867C}";

        /// <summary>
        /// Specifies a flag indicating that the local BLIP compression setting 
        /// overrides the document default compression setting.
        /// a14:useLocalDpi 
        /// </summary>
        internal const string UseLocalDpi = "{28A0092B-C50C-407E-A947-70E740481C1C}";

        /// <summary>
        /// Stores the fill information of an object when the shape fill has been set to invisible. 
        /// If shape fill has been set to visible this element is ignored.
        /// a14:hiddenFill 
        /// </summary>
        internal const string HiddenFill = "{909E8E84-426E-40DD-AFC4-6F175D3DCCD1}";

        /// <summary>
        /// Stores the line information of an object when the line fill has been set to invisible. 
        /// If line fill has been set to visible this element is ignored.
        /// a14:hiddenLine
        /// </summary>
        internal const string HiddenLine = "{91240B29-F687-4F45-9708-019B960494DF}";

        /// <summary>
        /// Stores the effect information of an object when the effects have been set to invisible. 
        /// If effects have been set to visible this element is ignored.
        /// a14:hiddenEffects
        /// </summary>
        internal const string HiddenEffects = "{AF507438-7753-43E0-B8FC-AC1667EBCBE1}";

        /// <summary>
        /// Specifies properties that produce the embedded picture in the containing BLIP.
        /// a14:imgProps
        /// </summary>
        internal const string ImgProps = "{BEBA8EAE-BF5A-486C-A8C5-ECC9F3942E4B}";

        /// <summary>
        /// Specifies the pivot controls that appear on the chart.
        /// c14:pivotOptions
        /// </summary>
        internal const string PivotOptions = "{781A3756-C4B2-4CAC-9D66-4F8BD8637D16}";

        /// <summary>
        /// Specifies the source pivot table for a pivot chart. 
        /// MUST exist only if the pivot table associated with the chart is a Non-WorkSheet PivotTable.
        /// </summary>
        internal const string PivotSource = "{723BEF56-08C2-4564-9609-F4CBC75E7E54}";
        
        /// <summary>
        /// Specifies whether a shadow is obscured by a shape with no fill.
        /// a14:shadowObscured
        /// </summary>
        internal const string ShadowObscured = "{53640926-AAD7-44D8-BBD7-CCE9431645EC}";

        /// <summary>
        /// Specifies the data about the applied theme. 
        /// </summary>
        internal const string ThemeFamily = "{05A4C25C-085E-4340-85A3-A5531E510DB2}";

        /// <summary>
        /// Specifies the properties of the background of the document.
        /// a15:backgroundPr
        /// </summary>
        internal const string BackgroundPr = "{A998136B-4AC2-44c3-8CCF-79AB77ABDD1D}";

        /// <summary>
        /// Specifies the color of the negative data points of the chart series.
        /// </summary>
        internal const string InvertSolidFillFmt = "{6F2FDCE9-48DA-4B69-8628-5D25D57E5C99}";

        /// <summary>
        /// Specifies number formatting for elements of type CT_CatAx, CT_DateAx, CT_SerAx and CT_ValAx. 
        /// MUST NOT exist if the CT_ExtensionList element of the parent CT_ChartSpace element does not have a child 
        /// CT_PivotSource element.
        /// </summary>
        internal const string NumFmt = "{F40574EE-89B7-4290-83BB-5DA773EAF853}";

        /// <summary>
        /// Specifies that images in the diagram are to be rendered as duotone, and thus rendered with the color specified by 
        /// the fillCrlLst of the color transform on the diagram.
        /// </summary>
        internal const string RecolorImg = "{C62137D5-CB1D-491b-B009-E17868A290BF}";

        internal const string Filtering = "{02D57815-91ED-43cb-92C2-25804820EDAC}";

        /// <summary>
        /// Set of data labels properties.
        /// </summary>
        internal const string DataLabels = "{CE6537A1-D6FC-4f65-9D91-7224C49458BB}";

        /// <summary>
        /// Specifies that a picture, as defined by the pic element is a camera object.
        /// </summary>
        internal const string CameraTool = "{84589F7E-364E-4c9e-8A38-B11213B215E9}";

        internal const string ObjectPr = "{837473B0-CC2E-450a-ABE3-18F120FF3D37}";

        /// <summary>
        /// Specifies a signature line. 
        /// A signature line provides a visual representation of a signature that is digitally signed.
        /// </summary>
        internal const string SignatureLine = "{F385189D-CB6C-4498-A905-10932F83BE7A}";

        /// <summary>
        /// Element which is used to distinguish a column uniquely.
        /// </summary>
        internal const string A16colId = "{9D8B030D-6E8A-4147-A177-3AD203B41FA5}";

        /// <summary>
        /// Element which is used to distinguish a row uniquely.
        /// </summary>
        internal const string A16rowId = "{0D108BD9-81ED-4DB2-BD59-A6C34878D82A}";

        /// <summary>
        /// Specifies non-visual properties of a group of shapes.
        /// </summary>
        internal const string LegacyGroupProp = "{F59B8463-F414-42e2-B3A4-FFEF48DC7170}";

        /// <summary>
        /// Specifies a graphic element in Scalable Vector Graphics (SVG) format.
        /// </summary>
        internal const string SvgBlip = "{96DAC541-7B7A-43D3-8B79-37D633B846F1}";

        /// <summary>
        /// Specifies unique ID of a chart data label.
        /// </summary>
        internal const string UniqueId = "{C3380CC4-5D6E-409C-BE32-E72D297353CC}";

        /// <summary>
        /// Defines the explicit part location of the Diagram Drawing and the minimum application version required to layout this diagram.
        /// dsp:dataModelExt
        /// </summary>
        internal const string DataModelExt = "http://schemas.microsoft.com/office/drawing/2008/diagram";

        /// <summary>
        /// Specifies the flag indicating that the current shape is decorative.
        /// </summary>
        internal const string Decorative = "{C183D7F6-B498-43B3-948B-1728B52AA6E4}";
    }
}
