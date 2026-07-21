// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/11/2008 by Roman Korchagin

using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Utility methods for writing common DrawingML elements.
    /// </summary>
    internal static class DmlBasicsWriter
    {
        internal static void WriteColor(string elemName, DrColor color, NrxXmlBuilder builder, 
            OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement(elemName);

            builder.StartElement("a:srgbClr");

            builder.WriteAttribute("val", FormatterPal.IntToStrX8(color.ToArgb()).Substring(2));

            const int AlphaMaxValue = 0xff;
            if (color.A != AlphaMaxValue)
            {
                builder.StartElement("a:alpha");
                builder.WriteAttribute("val", 
                    DmlPercentageUtil.ToPercentOrDmlPercent((double)color.A / AlphaMaxValue, isIsoStrict));
                builder.EndElement();
            }

            builder.EndElement();   //srgbClr

            builder.EndElement();   // elemName
        }

        internal static void WriteXY(string elemName, int x, int y, NrxXmlBuilder builder)
        {
            builder.StartElement(elemName);
            builder.WriteAttribute("x", x);
            builder.WriteAttribute("y", y);
            builder.EndElement();
        }

        internal static void WriteCxCy(string elemName, int cx, int cy, NrxXmlBuilder builder)
        {
            builder.StartElement(elemName);
            builder.WriteAttribute("cx", cx);
            builder.WriteAttribute("cy", cy);
            builder.EndElement();
        }

        internal static void WriteXYZ(string elemName, double x, double y, double z, NrxXmlBuilder builder)
        {
            builder.StartElement(elemName);
            builder.WriteAttribute("x", x);
            builder.WriteAttribute("y", y);
            builder.WriteAttribute("z", z);
            builder.EndElement();
        }

        internal static void WriteDxDyDz(string elemName, double dx, double dy, double dz, NrxXmlBuilder builder)
        {
            builder.StartElement(elemName);
            builder.WriteAttribute("dx", dx);
            builder.WriteAttribute("dy", dy);
            builder.WriteAttribute("dz", dz);
            builder.EndElement();
        }
    }
}
