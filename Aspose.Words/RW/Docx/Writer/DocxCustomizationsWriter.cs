// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2010 by Roman Korchagin

using System;
using Aspose.Common;
using Aspose.OpcPackaging;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes the customizations.xml and attachedToolbars.bin parts.
    /// See http://www.documentinteropinitiative.org/additionalinfo/sect1.aspx for details.
    /// </summary>
    internal class DocxCustomizationsWriter
    {
        internal static void Write(DocxDocumentWriter writer)
        {
            if (!writer.Document.HasCustomizations)
                return;

            DocxBuilder builder = writer.CreateChildPartAndBuilder(
                "customizations.xml", DocxContentType.Customizations, writer.RelTypes.Customizations);

            builder.StartDocument("wne:tcg");
            builder.WriteAttributeString("xmlns:r", writer.DocxNamespaces.Relationships);
            builder.WriteAttributeString("xmlns:wne", writer.DocxNamespaces.WmlBeta);

            WriteKeyMaps(writer, builder);
            WriteToolbars(writer, builder);
            WriteAcds(writer, builder);

            builder.EndDocument(); 
        }

        private static void WriteKeyMaps(DocxDocumentWriter writer, DocxBuilder builder)
        {
            Debug.Assert(builder != null);

            Document doc = writer.Document;
            if (!doc.HasKeyMaps)
                return;

            builder.StartElement("wne:keymaps");

            foreach (KeyMap keyMap in doc.KeyMaps)
            {
                if (keyMap.KeymapType == KeyMapType.None)
                    continue;

                builder.StartElement("wne:keymap");    

                if (keyMap.KeymapType == KeyMapType.Mask)
                    builder.WriteAttribute("wne:mask", 1);

                WriteKeyMapKey(builder, "wne:chmPrimary", keyMap.ChmPrimary);
                WriteKeyMapKey(builder, "wne:chmSecondary", keyMap.ChmSecondary);
                WriteKeyMapKey(builder, "wne:kcmPrimary", keyMap.KcmPrimary);
                WriteKeyMapKey(builder, "wne:kcmSecondary", keyMap.KcmSecondary);

                switch (keyMap.KeymapType)
                {
                    case KeyMapType.None:
                    case KeyMapType.Mask:
                        // Do nothing, handled above.
                        break;
                    case KeyMapType.AllocatedCommand:
                    {
                        builder.StartElement("wne:acd");
                        builder.WriteAttribute("wne:acdName", FormatAcdName(keyMap.AllocatedCommandIndex));
                        builder.EndElement();   // wne:acd
                        break;
                    }
                    case KeyMapType.FixedCommand:
                    {
                        builder.StartElement("wne:fci");
                        int fciIndex = (int)keyMap.FixedCommandIdentifier;
                        builder.WriteAttribute("wne:fciIndex", FormatterPal.IntToStrX4(fciIndex));
                        builder.WriteAttribute("wne:swArg", FormatterPal.IntToStrX4(keyMap.FixedCommandArgument));
                        builder.EndElement();   // wne:fci
                        break;
                    }
                    case KeyMapType.Macro:
                    {
                        builder.StartElement("wne:macro");
                        builder.WriteAttribute("wne:macroName", keyMap.MacroName);
                        builder.EndElement();   // wne:macro
                        break;
                    }
                    case KeyMapType.InsertCharacter:
                    {
                        builder.StartElement("wne:wch");
                        builder.WriteAttribute("wne:val", FormatterPal.IntToStrX8(keyMap.CharacterCode));
                        builder.EndElement();   // wne:wch
                        break;
                    }
                    case KeyMapType.LegacyMacro:
                    {
                        builder.StartElement("wne:wll");
                        builder.WriteAttribute("wne:macroName", keyMap.MacroName);
                        builder.EndElement();   // wne:wll
                        break;
                    }
                    default:
                        throw new InvalidOperationException("Unknown keymap type.");
                }

                builder.EndElement();// wne:keymap
            }

            builder.EndElement();   // wne:keymaps
        }

        private static void WriteKeyMapKey(NrxXmlBuilder xmlBuilder, string name, int value)
        {
            if (value != 0)
                xmlBuilder.WriteAttribute(name, FormatterPal.IntToStrX4(value));
        }

        private static string FormatAcdName(int acdIndex)
        {
            return string.Format("acd{0}", acdIndex);
        }

        private static void WriteToolbars(DocxDocumentWriter writer, DocxBuilder builder)
        {
            Debug.Assert(builder != null);

            Document doc = writer.Document;
            if (!doc.HasAllocatedCommands && !doc.HasAttachedToolbars)
                return;

            builder.StartElement("wne:toolbars");

            if (doc.HasAllocatedCommands)
            {
                builder.StartElement("wne:acdManifest");
                for (int i = 0; i < doc.AllocatedCommands.Count; i++)
                {
                    builder.StartElement("wne:acdEntry");
                    builder.WriteAttribute("wne:acdName", FormatAcdName(i));
                    builder.EndElement();   // wne:acdEntry
                }
                builder.EndElement();   // wne:acdManifest
            }

            if (doc.HasAttachedToolbars)
            {
                string relId;
                OpcPackagePart attachedToolbarsPart = writer.Package.CreateChildPart(
                    builder.Part, "attachedToolbars.bin", DocxContentType.AttachedToolbars, writer.RelTypes.AttachedToolbars, out relId);
                attachedToolbarsPart.Stream.Write(doc.AttachedToolbars, 0, doc.AttachedToolbars.Length);
                builder.WriteRelationshipId("wne:toolbarData", relId);
            }

            builder.EndElement();   // wne:toolbars
        }

        private static void WriteAcds(DocxDocumentWriter writer, NrxXmlBuilder builder)
        {
            Document doc = writer.Document;
            if (!doc.HasAllocatedCommands)
                return;

            builder.StartElement("wne:acds");
            for (int i = 0; i < doc.AllocatedCommands.Count; i++)
            {
                AllocatedCommand acd = (AllocatedCommand)doc.AllocatedCommands[i];
                if (acd.FciBasedOn != FixedCommandIdentifier.None)
                {
                    builder.StartElement("wne:acd");
                    builder.WriteAttribute("wne:argValue", acd.ArgValue);
                    builder.WriteAttribute("wne:acdName", FormatAcdName(i));
                    int fciIndexBasedOn = (int)acd.FciBasedOn;
                    builder.WriteAttribute("wne:fciIndexBasedOn", FormatterPal.IntToStrX4(fciIndexBasedOn));
                    builder.EndElement();   // wne:acd
                }
            }
            builder.EndElement();   
        }
    }
}
