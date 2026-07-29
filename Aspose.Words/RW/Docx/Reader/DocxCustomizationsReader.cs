// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2010 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads the Word 97-2003 toolbar and keyboard command customizations.
    /// More info http://www.documentinteropinitiative.org/additionalinfo/sect1.aspx
    /// </summary>
    internal class DocxCustomizationsReader
    {
        /// <summary>
        /// The static method that is easy to invoke.
        /// </summary>
        internal static void Read(DocxDocumentReader docReader)
        {
            DocxCustomizationsReader customizationsReader = new DocxCustomizationsReader();
            customizationsReader.ReadCore(docReader);
        }

        /// <summary>
        /// Reads 1.2.1.1.12. tcg (Keyboard and Toolbar Customizations) and all related info.
        /// </summary>
        private void ReadCore(DocxDocumentReader reader)
        {
            // If there is no customization part there is nothing to read.
            OpcPackagePart customizationsPart = reader.GetPartByRelationshipType(reader.DocumentPart, reader.RelTypes.Customizations);
            if (customizationsPart == null)
                return;

            mDocReader = reader;
            mXmlReader = new DocxXmlReader(customizationsPart, mDocReader.ComplianceInfo);
            while (mXmlReader.ReadChild("tcg"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "keymaps":
                    case "keymapsBad":
                        // RK We read both of these elements into one keymap collection for the time being.
                        // I do not really understand the difference between them well and have never seen keymapsBad.
                        ReadKeymaps();
                        break;
                    case "toolbars":
                        ReadToolbars();
                        break;
                    case "acds":
                        ReadAcds();
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }

            BuildAllocatedCommands();
            ResolveKeyMapsWithAcdNames();
        }

        /// <summary>
        /// String reads 1.2.1.1.9. keymaps (Key Map Customizations) or 1.2.1.1.10. keymapsBad (Key Map Customizations for Mismatched Keyboards).
        /// </summary>
        private void ReadKeymaps()
        {
            // Check for null because can be invoked twice.
            if (mDocReader.Document.KeyMaps == null)
                mDocReader.Document.KeyMaps = new List<KeyMap>();

            string parentName = mXmlReader.LocalName;
            while (mXmlReader.ReadChild(parentName))
            {
                switch (mXmlReader.LocalName)
                {
                    case "keymap":
                        ReadKeymap();
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 1.2.1.1.8. keymap (Key Map Entry).
        /// </summary>
        private void ReadKeymap()
        {
            KeyMap keyMap = new KeyMap();
            mDocReader.Document.KeyMaps.Add(keyMap);

            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "chmPrimary":
                        keyMap.ChmPrimary = NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    case "chmSecondary":
                        keyMap.ChmSecondary = NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    case "kcmPrimary":
                        keyMap.KcmPrimary = NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    case "kcmSecondary":
                        keyMap.KcmSecondary = NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    case "mask":
                        if (mXmlReader.ValueAsBool)
                            keyMap.KeymapType = KeyMapType.Mask;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // Read elements.
            while (mXmlReader.ReadChild("keymap"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "acd":
                        // A keymap with an acdName requires resolution into acd index, store it in the temporary list.
                        keyMap.KeymapType = KeyMapType.AllocatedCommand;
                        mKeyMapsWithAcd[mXmlReader.ReadAttribute("acdName", "")] = keyMap;
                        break;
                    case "fci":
                        ReadKeyMapFci(keyMap);
                        break;
                    case "macro":
                        keyMap.KeymapType = KeyMapType.Macro;
                        keyMap.MacroName = mXmlReader.ReadAttribute("macroName", "");
                        break;
                    case "wch":
                        keyMap.KeymapType = KeyMapType.InsertCharacter;
                        keyMap.CharacterCode = NrxXmlUtil.HexToInt(mXmlReader.ReadVal());   // This is an element with the "val" attribute.
                        break;
                    case "wll":
                        keyMap.KeymapType = KeyMapType.LegacyMacro;
                        keyMap.MacroName = mXmlReader.ReadAttribute("macroName", "");
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 1.2.1.1.7. fci (Fixed Command Keyboard Customization Action).
        /// </summary>
        private void ReadKeyMapFci(KeyMap keyMap)
        {
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "fciIndex":
                        keyMap.KeymapType = KeyMapType.FixedCommand;
                        keyMap.FixedCommandIdentifier = (FixedCommandIdentifier)NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    case "fciName":
                        keyMap.KeymapType = KeyMapType.FixedCommand;
                        keyMap.FixedCommandIdentifier = DocxCustomizationsEnum.DocxToFixedCommandIdentifier(mXmlReader.Value);
                        break;
                    case "swArg":
                        keyMap.FixedCommandArgument = NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 1.2.1.1.14. toolbars (Legacy Toolbar Customizations).
        /// </summary>
        private void ReadToolbars()
        {
            while (mXmlReader.ReadChild("toolbars"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "acdManifest":
                        ReadAcdManifest();
                        break;
                    case "toolbarData":
                        ReadToolbarData();
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 1.2.1.1.4. acdManifest (Allocated Command Manifest).
        /// </summary>
        private void ReadAcdManifest()
        {
            while (mXmlReader.ReadChild("acdManifest"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "acdEntry":
                        ReadAcdEntry();
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 1.2.1.1.3. acdEntry (Allocated Command Manifest Entry).
        /// </summary>
        private void ReadAcdEntry()
        {
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "acdName":
                        // Add the acdName to our internal list. We build the data for the model from this later on.
                        mAcdManifest.Add(mXmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the attachedToolbars.bin into the model.
        /// </summary>
        private void ReadToolbarData()
        {
            string relId = mXmlReader.ReadId();
            string target = mXmlReader.Part.GetRelationshipTarget(relId);
            OpcPackagePart attachedToolbarsPart = mDocReader.FetchPartByName(target);
            mDocReader.Document.AttachedToolbars = StreamUtil.CopyStreamToByteArray(attachedToolbarsPart.Stream);
        }

        /// <summary>
        /// Reads 1.2.1.1.5. acds (Allocated Commands)
        /// </summary>
        private void ReadAcds()
        {
            while (mXmlReader.ReadChild("acds"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "acd":
                        ReadAcd();
                        break;
                    default:
                        mXmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 1.2.1.1.1. acd (Allocated Command).
        /// </summary>
        private void ReadAcd()
        {
            string acdName = null;
            AllocatedCommand acd = new AllocatedCommand();

            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "acdName":
                        acdName = mXmlReader.Value;
                        break;
                    case "argValue":
                        acd.ArgValue = Convert.FromBase64String(mXmlReader.Value);
                        break;
                    case "fciBasedOn":
                        acd.FciBasedOn = DocxCustomizationsEnum.DocxToFixedCommandIdentifier(mXmlReader.Value);
                        break;
                    case "fciIndexBasedOn":
                        acd.FciBasedOn = (FixedCommandIdentifier)NrxXmlUtil.HexToInt(mXmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // Store the acd in the temporary list. We build the collection of allocated commands for the model later on.
            if (StringUtil.HasChars(acdName))
                mAcds[acdName] = acd;
        }

        /// <summary>
        /// Builds the proper <see cref="Document.AllocatedCommands"/> collection out of the temporary collections that we have read from DOCX.
        /// </summary>
        private void BuildAllocatedCommands()
        {
            if ((mAcdManifest.Count == 0) || (mAcds.Count == 0))
                return;

            Document doc = mDocReader.Document;
            doc.AllocatedCommands = new List<AllocatedCommand>();

            // The entries in the acdManifest have the correct order for the binary data. Our allocated commands in the model
            // match the binary data too. So loop through the acdManifest and fetch the corresponding acd from the acds collection
            // to build the collection for the model.
            foreach (string acdName in mAcdManifest)
            {
                AllocatedCommand acd = mAcds.GetValueOrNull(acdName);

                // If we cannot find an acd for some reason we should use the "unused" command. These can occur in binary.
                // This code is not really tested as I could not find any documents with unused commands.
                if (acd == null)
                    acd = new AllocatedCommand();

                doc.AllocatedCommands.Add(acd);
            }
        }

        /// <summary>
        /// Key mappings that invoke allocated commands (acd) are linked to their acd using an index in the model.
        /// This resolves the acd names read from the DOCX into indexes for the model.
        /// </summary>
        private void ResolveKeyMapsWithAcdNames()
        {
            foreach (KeyValuePair<string, KeyMap> entry in mKeyMapsWithAcd)
            {
                string acdName = entry.Key;
                KeyMap keyMap = entry.Value;
                keyMap.AllocatedCommandIndex = FindAcdIndexInManifest(acdName);
            }
        }

        private int FindAcdIndexInManifest(string acdName)
        {
            for (int i = 0; i < mAcdManifest.Count; i++)
            {
                if (mAcdManifest[i] == acdName)
                    return i;
            }
            return -1;
        }

        private DocxDocumentReader mDocReader;
        private DocxXmlReader mXmlReader;
        /// <summary>
        /// Contains string acdName values. Created when loading wne:acdManifest.
        /// </summary>
        private readonly List<string> mAcdManifest = new List<string>();
        /// <summary>
        /// The key is a string acdName, the value is <see cref="AllocatedCommand"/>. 
        /// Created during loading of wne:acds.
        /// Used together with <see cref="mAcdManifest"/> to build <see cref="Document.AllocatedCommands"/>.
        /// </summary>
        private readonly Dictionary<string, AllocatedCommand> mAcds = new Dictionary<string, AllocatedCommand>();
        /// <summary>
        /// The key is a string acdName, the value is <see cref="KeyMap"/>.
        /// Created during loading of wne:keymaps for wne:keymap elements that contain wne:acd in them.
        /// Used to resolve <see cref="KeyMap.AllocatedCommandIndex"/> values.
        /// </summary>
        private readonly Dictionary<string, KeyMap> mKeyMapsWithAcd = new Dictionary<string, KeyMap>();
    }
}
