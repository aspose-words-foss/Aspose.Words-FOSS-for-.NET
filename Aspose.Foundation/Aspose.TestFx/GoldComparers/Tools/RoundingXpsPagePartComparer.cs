// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2018 by Michael Morozoff

using System;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Common;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Implements comparing of Xps page parts with rounded points
    /// </summary>
    public class RoundingXpsPagePartComparer : IPackagePartComparer
    {
        public static readonly RoundingXpsPagePartComparer Instance = new RoundingXpsPagePartComparer();

        public bool CompareBuffers(string partName, byte[] buffer1, byte[] buffer2, bool isPreferXmlDiff)
        {
            if (StringUtil.HasChars(partName) && partName.EndsWith(".fpage") && buffer1 != null && buffer2 != null)
            {
                // Since we do not accept rounded golds always I'd rather round them all the time
#if !JAVA
                buffer1 = RoundPoints(buffer1);
                buffer2 = RoundPoints(buffer2);
#endif
            }

            return DefaultPackagePartComparer.Instance.CompareBuffers(partName, buffer1, buffer2, isPreferXmlDiff);
        }

        // This is not implemented in Java yet
        [JavaAttributes.JavaDelete]
        private static byte[] RoundPoints(byte[] buffer)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new MemoryStream(buffer));

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("a", "http://schemas.microsoft.com/xps/2005/06");

            XmlNodeList nodes = doc.SelectNodes("//a:Glyphs", ns);
            foreach (XmlNode node in nodes)
            {
                RoundAttribute(node.Attributes["OriginX"]);
                RoundAttribute(node.Attributes["OriginY"]);

                XmlAttribute a = node.Attributes["Indices"];
                if (a != null && StringUtil.HasChars(a.Value))
                {
                    string[] vs = a.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < vs.Length; i++)
                    {
                        string[] p = vs[i].Split(',');
                        p[0] = RoundDouble(p[0]);
                        p[1] = RoundDouble(p[1]);
                        vs[i] = string.Format("{0},{1}", p[0], p[1]);
                    }

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < vs.Length; i++)
                    {
                        if (i != 0)
                            sb.Append(';');
                        sb.Append(vs[i]);
                    }
                    a.Value = sb.ToString();
                }
            }

            MemoryStream o = new MemoryStream();
            doc.Save(o);
            buffer = new byte[o.Length];
            o.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        private static void RoundAttribute(XmlAttribute a)
        {
            if (a != null)
            {
                string v = RoundDouble(a.Value);
                if (v != null)
                    a.Value = v;
            }
        }

        private static string RoundDouble(string v)
        {
            if (!StringUtil.HasChars(v))
                return v;

            // Number of decimal digits after rounding, 3 should be enough, thats 1/10000th of a point, ot 1/10th of Li in layout.
            const int NUM_DIGITS = 4;
            double d = FormatterPal.ParseDouble(v);
            return Math.Round(d, NUM_DIGITS).ToString();
        }
    }
}
