// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2014 by Alexey Morozov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Implement reading of 'shapeDefaults' and 'hdrShapeDefaults' elements.
    /// </summary>
    internal static class NrxShapeDefaultsReader
    {
        internal static void Read(NrxXmlReader xmlReader, IDictionary<string, ConnectorRule> connectors)
        {
            string tagName = xmlReader.LocalName;

            while (xmlReader.ReadChild(tagName))
            {
                if (xmlReader.LocalName == "shapelayout")
                    ReadShapeLayout(xmlReader, connectors);
                else
                    xmlReader.IgnoreElement();
            }
        }

        private static void ReadShapeLayout(NrxXmlReader xmlReader, IDictionary<string, ConnectorRule> connectors)
        {
            while (xmlReader.ReadChild("shapelayout"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rules":
                        ReadRules(xmlReader, connectors);
                        break;

                    case "idmap":
                        break;

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadRules(NrxXmlReader xmlReader, IDictionary<string, ConnectorRule> connectors)
        {
            while (xmlReader.ReadChild("rules"))
            {
                if (xmlReader.LocalName == "r")
                    ReadRule(xmlReader, connectors);
                else
                    xmlReader.IgnoreElement();
            }
        }

        private static void ReadRule(NrxXmlReader xmlReader, IDictionary<string, ConnectorRule> connectors)
        {
            string id = "";
            ConnectorRule rule = new ConnectorRule();
            while (xmlReader.MoveToNextAttribute())
            {
                if (xmlReader.LocalName == "idref")
                    // Get unparsed as we have mShapeNamesMap already that maps shapes by name.
                    id = xmlReader.Value;
            }

            while (xmlReader.ReadChild("r"))
            {
                if (xmlReader.LocalName == "proxy")
                    ReadProxy(xmlReader, rule);
                else
                    xmlReader.IgnoreElement();
            }

            if (StringUtil.HasChars(rule.ShapeAIdRaw) && StringUtil.HasChars(rule.ShapeBIdRaw))
            {
                // WORDSNET-11580 Ignore duplicated connector rules.
                if (!connectors.ContainsKey(id))
                    connectors.Add(id, rule);
            }
        }

        private static void ReadProxy(NrxXmlReader xmlReader, ConnectorRule rule)
        {
            bool isStart = false;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "start":
                        isStart = true;
                        break;

                    case "end":
                        isStart = false;
                        break;

                    case "idref":
                        if (isStart)
                            rule.ShapeAIdRaw = xmlReader.Value;
                        else
                            rule.ShapeBIdRaw = xmlReader.Value;
                        break;

                    case "connectloc":
                        if (isStart)
                            rule.ShapeASite = xmlReader.ValueAsInt;
                        else
                            rule.ShapeBSite = xmlReader.ValueAsInt;
                        break;

                    default:
                        WarnNotSupported(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private static void WarnNotSupported(NrxXmlReader reader)
        {
            reader.Warn(
                WarningType.MinorFormattingLoss,
                WarningSource.DrawingML,
                string.Format(WarningStrings.NotSupportedTag, reader.LocalName));
        }
    }
}
