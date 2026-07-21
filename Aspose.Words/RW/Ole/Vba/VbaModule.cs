// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2019 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.JavaAttributes;
using Aspose.Ss;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Provides access to VBA project module.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-vba-macros/">Working with VBA Macros</a> documentation article.</para>
    /// </summary>
    public class VbaModule
    {
        /// <summary>
        /// Creates an empty module.
        /// </summary>
        public VbaModule()
        {
        }

        /// <summary>
        /// Creates an empty procedural module.
        /// </summary>        
        internal VbaModule(string name)
        {
            mName = name;
            mDocString = string.Empty;

            mType = VbaModuleType.ProceduralModule;
        }

        /// <summary>
        /// Reads module content. Returns null in case of any error.
        /// </summary>
        internal static VbaModule Read(VbaRecordReader reader, MemoryStorage storage)
        {
            VbaModule module = new VbaModule();

            try
            {
                module.Name = reader.ReadStringRecords(VbaRecord.ModuleName, VbaRecord.ModuleNameUnicode);
                string streamName = reader.ReadStringRecords(VbaRecord.StreamName, VbaRecord.StreamNameUnicode);
                module.DocString = reader.ReadStringRecords(VbaRecord.DocString, VbaRecord.DocStringUnicode);

               
                int offset = reader.ReadInt32Record(VbaRecord.Offset);
                module.HelpContext = reader.ReadInt32Record(VbaRecord.ModuleHelpContext);      
                
                // CookieRecord - MUST be ignored on read. MUST be 0xFFFF on write
                reader.ReadInt16Record(VbaRecord.ModuleCookieRecordId);

                while (true)
                {
                    // There are can be TypeRecord, ReadOnlyRecord or PrivateRecord.
                    // Ignore it for a while and look for Terminator record.
                    ushort id = reader.ReadUInt16();
                    reader.ReadInt32();     // Reserved. MUST be 0x00000000. MUST be ignored.

                    if (id == VbaRecord.Terminator)
                        break;
                }

                module.ExtractSourceCode(storage.FetchStorage(VbaRecord.VbaStorageName).FetchStream(streamName), offset, reader.Encoding);
                module.ExtractModuleType(storage.FetchStream(VbaRecord.ProjectStreamName), reader.Encoding);

                // Save additional (currently unparsed) data if exists.
                module.mFormMemoryStorage = storage.GetStorageSafe(module.Name);

                return module;
            }
            catch 
            {
                return null;
            }
        }

        /// <summary>
        /// Performs a copy of the <see cref="VbaModule"/>.
        /// </summary>
        /// <returns>The cloned <see cref="VbaModule"/>.</returns>
        [JavaThrows(false)]
        public VbaModule Clone()
        {
            VbaModule lhs = (VbaModule)MemberwiseClone();
            
            // Set the parent project as null, it will be specified once module added to the project.
            lhs.Project = null;
            if (FormMemoryStorage != null)
            {
                try
                {
                    // Clone memory storage using temporary memory stream.
                    FileSystem fs = new FileSystem(FormMemoryStorage);
                    MemoryStream ms = new MemoryStream();
                    fs.Save(ms);

                    ms.Position = 0;
                    fs = new FileSystem(ms);
                    lhs.mFormMemoryStorage = fs.Root;
                }
                catch
                {
                    return lhs;
                }
            }

            return lhs;
        }

        /// <summary>
        /// Gets information about modules types for "PROJECT" stream.
        /// </summary>
        /// <remarks>
        /// 2.3.4.2.3.2.8 MODULETYPE Record contains either 0x0021 for a procedural module or 0x0022 for all other types.
        /// This method retrieves exact module type.
        /// </remarks>
        private void ExtractModuleType(MemoryStream stream, Encoding encoding)
        {
            string stringProj = encoding.GetString(stream.ToArray());

            VbaModuleType[] allTypes = GetAllTypesOfVbaModule();

            foreach (VbaModuleType type in allTypes)
            {
                string searchString = string.Concat(ModuleTypeToString(type), "=", mName);
                if (stringProj.IndexOf(searchString, StringComparison.Ordinal) >=0)
                    mType = type;
            }
        }

        /// <summary>
        /// Returns the array of all types of VbaModuleType.
        /// </summary>        
        private static VbaModuleType[] GetAllTypesOfVbaModule()
        {
            VbaModuleType[] allTypes = new VbaModuleType[4];
            allTypes[0] = VbaModuleType.DocumentModule;
            allTypes[1] = VbaModuleType.ProceduralModule;
            allTypes[2] = VbaModuleType.ClassModule;
            allTypes[3] = VbaModuleType.DesignerModule;
            return allTypes;
        }

        internal static string ModuleTypeToString(VbaModuleType type)
        {
            string typeString;
            switch (type)
            {
                case VbaModuleType.DocumentModule:
                    typeString = "Document";
                    break;
                case VbaModuleType.ProceduralModule:
                    typeString = "Module";
                    break;
                case VbaModuleType.ClassModule:
                    typeString = "Class";
                    break;
                case VbaModuleType.DesignerModule:
                    typeString = "BaseClass";
                    break;
                default:
                    throw new ArgumentException("Unknown module type");
            }

            return typeString;
        }

        /// <summary>
        /// Extracts source code from the module stream.
        /// </summary>
        private void ExtractSourceCode(MemoryStream moduleStream, int offset, Encoding encoding)
        {
            moduleStream.Position = offset;

            MemoryStream decompressedCode = new MemoryStream();
            RleCompressor.Decompress(moduleStream, decompressedCode);

            mSourceCode = encoding.GetString(decompressedCode.ToArray());
        }

        /// <summary>
        /// Gets or sets VBA project module name.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                if (mProject != null)
                    mProject.MarkModified();
            }
        }

        /// <summary>
        /// Gets or sets VBA project module source code.
        /// </summary>
        public string SourceCode
        {
            get { return mSourceCode; }
            set
            {
                mSourceCode = value;
                if (mProject != null)
                    mProject.MarkModified();
            }
        }

        /// <summary>
        /// Specifies whether the module is a procedural module, document module, class module, or designer module.
        /// </summary>
        public VbaModuleType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        /// <summary>
        /// Parent VBA project.
        /// </summary>
        internal VbaProject Project
        {
            set { mProject = value; }
        }

        /// <summary>
        /// Specifies the description for the module.
        /// </summary>
        internal string DocString
        {
            get { return mDocString; }
            set { mDocString = value; }
        }

        /// <summary>
        /// Specifies the Help topic identifier for the module.
        /// </summary>
        internal int HelpContext
        {
            get { return mHelpContext; }
            set { mHelpContext = value; }
        }

        /// <summary>
        /// Stores extra module data (designer form) as raw memory storage.
        /// </summary>
        internal MemoryStorage FormMemoryStorage
        {
            get { return mFormMemoryStorage; }
        }

        internal void Write(VbaRecordWriter writer)
        {
            writer.WriteStringRecord(VbaRecord.ModuleName, mName);
            writer.WriteStringRecord(VbaRecord.ModuleNameUnicode, Encoding.Unicode, mName);

            writer.WriteStringRecord(VbaRecord.StreamName, mName);
            writer.WriteStringRecord(VbaRecord.StreamNameUnicode, Encoding.Unicode, mName);

            writer.WriteStringRecord(VbaRecord.DocString, mDocString);
            writer.WriteStringRecord(VbaRecord.DocStringUnicode, Encoding.Unicode, mDocString);

            writer.WriteInt32Record(VbaRecord.Offset, 0);
            writer.WriteInt32Record(VbaRecord.ModuleHelpContext, mHelpContext);

            // CookieRecord - MUST be ignored on read. MUST be 0xFFFF on write
            writer.WriteUInt16(VbaRecord.ModuleCookieRecordId);
            // Write size of ushort. 
            writer.WriteInt32(2);            
            writer.WriteUInt16(VbaRecord.CookieRecord);

            int moduleType = (mType == VbaModuleType.ProceduralModule) ? VbaRecord.ProceduralModule : VbaRecord.NonProceduralModule;
            writer.WriteUInt16((ushort)moduleType);
            writer.WriteInt32(0);

            writer.WriteUInt16(VbaRecord.Terminator);
            writer.WriteInt32(0);
        }

        private string mName;
        private string mSourceCode;
        private string mDocString;
        private int mHelpContext;
        private VbaModuleType mType;
        private MemoryStorage mFormMemoryStorage;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private VbaProject mProject;
    }
}
