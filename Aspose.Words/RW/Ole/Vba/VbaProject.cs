// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2019 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Ss;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Provides access to VBA project information.
    /// A VBA project inside the document is defined as a collection of VBA modules.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-vba-macros/">Working with VBA Macros</a> documentation article.</para>
    /// </summary>
    public class VbaProject
    {
        /// <summary>
        /// Creates a blank <see cref="VbaProject"/>.
        /// </summary>
        public VbaProject()
        {
            mName = "Project";
            mSysKind = 1;
            mCodePage = 1251;
            mLcid = 0x00000409;
            mLcidInvoke = 0x00000409;
            mVersionMajor = 1;
            mVersionMinor = 1;
            mDescription = "";
            mVbaStorage = null;

            VbaModule module = new VbaModule("ThisDocument");
            module.Type = VbaModuleType.DocumentModule;
            Modules.Add(module);
        }

        /// <summary>
        /// Creates a <see cref="VbaProject"/> from the <paramref name="vbaStorage"/>.
        /// </summary>
        internal VbaProject(MemoryStorage vbaStorage)
        {
            mVbaStorage = vbaStorage;

            try
            {
                MemoryStream compressedDir = vbaStorage.FetchStorage(VbaRecord.VbaStorageName).FetchStream(VbaRecord.DirStreamName);
                MemoryStream dir = new MemoryStream();

                RleCompressor.Decompress(compressedDir, dir);
                dir.Position = 0;

                mReader = new VbaRecordReader(new BinaryReader(dir));

                // PROJECTINFORMATION
                mSysKind = mReader.ReadInt32Record(VbaRecord.SysKind);           // PROJECTSYSKIND
                mLcid = mReader.ReadInt32Record(VbaRecord.Lcid);              // PROJECTLCID
                mLcidInvoke = mReader.ReadInt32Record(VbaRecord.LcidInvoke);        // PROJECTLCIDINVOKE
                mCodePage = mReader.ReadInt16Record(VbaRecord.CodePage);          // PROJECTCODEPAGE
                mReader.Encoding = Encoding.GetEncoding(mCodePage);

                mName = mReader.ReadStringRecord(VbaRecord.Name);             // PROJECTNAME

                // PROJECTDOCSTRING
                mDescription = mReader.ReadStringRecords(VbaRecord.Description, VbaRecord.DescriptionUnicode);
                // <PROJECTHELPFILEPATH> must be defined in PROJECT Stream, if SizeOfHelpFile1 > 0.
                mHelpFilePath1 = mReader.ReadStringRecord(VbaRecord.HelpFilePath1);

                // [MS-OVBA] 2009 was using ID 0x49 (VbaRecord.HelpFilePath2Obsolete).
                // [MS-OVBA] 2019 is using ID 0x3d (VbaRecord.HelpFilePath2).
                // Let's make optional read here to be able to import old documents as well.
                mHelpFilePath2 = (mReader.PeekId() == VbaRecord.HelpFilePath2Obsolete) ?
                    mReader.ReadStringRecord(VbaRecord.HelpFilePath2Obsolete) :
                    mReader.ReadStringRecord(VbaRecord.HelpFilePath2);

                mHelpContext = mReader.ReadInt32Record(VbaRecord.ProjectHelpContext);       // PROJECTHELPCONTEXT
                mLibFlags = mReader.ReadInt32Record(VbaRecord.LibFlags);          // PROJECTLIBFLAGS

                // PROJECTVERSION
                mReader.ReadVersionRecord(VbaRecord.ProjectVersion);
                mVersionMajor = mReader.ReadUInt32();    // major
                mVersionMinor = mReader.ReadUInt16();    // minor

                // PROJECTCONSTANTS
                if (mReader.PeekId() == VbaRecord.Constants)
                    mConstants = mReader.ReadStringRecords(VbaRecord.Constants, VbaRecord.ConstantsUnicode);

                // PROJECTREFERENCES
                while (true)
                {
                    if (mReader.PeekId() == VbaRecord.ModulesCount)
                    {
                        // Start of PROJECTMODULES
                        break;
                    }

                    References.Add(VbaReference.Read(mReader));
                }

                int modulesCount = mReader.ReadInt16Record(VbaRecord.ModulesCount);
                mReader.ReadBytes(8);           // PROJECTCOOKIE

                // Array of Module record.
                for (int i = 0; i < modulesCount; i++)
                {
                    VbaModule module = VbaModule.Read(mReader, vbaStorage);
                    if (module != null)
                        Modules.AddInternal(module);
                }

                ParseProjectStream();
            }
            catch
            {
                Debug.Assert(false, "Unexpected VbaProject content");
            }
        }

        /// <summary>
        /// Gets or sets VBA project name.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                MarkModified();
            }
        }

        /// <summary>
        /// Returns collection of VBA project modules.
        /// </summary>
        public VbaModuleCollection Modules
        {
            get
            {
                if (mModulesCollection == null)
                    mModulesCollection = new VbaModuleCollection(this);
                return mModulesCollection;
            }
        }

        /// <summary>
        /// Gets or sets the VBA project’s code page.
        /// </summary>
        /// <remarks>
        /// Please note that VBA is pre-Unicode feature and you have to explicitly set appropriate code page
        /// to preserve regional character sets.
        /// </remarks>
        public int CodePage
        {
            get { return mCodePage; }
            set { mCodePage = (short)value; }
        }

        /// <summary>
        /// Performs a copy of the <see cref="VbaProject"/>.
        /// </summary>
        /// <returns>The cloned <see cref="VbaProject"/>.</returns>
        public VbaProject Clone()
        {
            VbaProject lhs = (VbaProject)MemberwiseClone();

            lhs.mModulesCollection = mModulesCollection.Clone(lhs);
            lhs.mReferences = References.Clone();

            if (mMacroNames != null)
            {
                lhs.mMacroNames = new List<string>();
                foreach (string macroName in mMacroNames)
                    lhs.AddMacroName(macroName);
            }

            return lhs;
        }

        /// <summary>
        /// Shows whether the <see cref="VbaProject"/> is signed or not.
        /// </summary>
        public bool IsSigned
        {
            get { return mSignature != null; }
        }

        /// <summary>
        /// Shows whether the <see cref="VbaProject"/> is password protected.
        /// </summary>
        /// <dev>
        /// Actually we need to parse ProtectionState property but according to spec
        /// protected project ID MUST be {00000000-0000-0000-0000-000000000000}.
        /// Lets do this simple way first.
        /// </dev>
        public bool IsProtected
        {
            get { return mId == "{00000000-0000-0000-0000-000000000000}"; }
        }

        /// <summary>
        /// Gets a collection of VBA project references.
        /// </summary>
        public VbaReferenceCollection References
        {
            get
            {
                if (mReferences == null)
                    mReferences = new VbaReferenceCollection(this);

                return mReferences;
            }
        }

        /// <summary>
        /// Rebuilds macro names list.
        /// </summary>
        internal void RebuildMacroNames()
        {
            if(mMacroNames != null)
                mMacroNames.Clear();

            foreach (VbaModule module in Modules)
            {
                if (!string.IsNullOrEmpty(module.SourceCode))
                {
                    string[] lines =
                        module.SourceCode.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    // Do naive scan for subroutines names.
                    foreach (string line in lines)
                    {
                        Regex regex = new Regex("Sub\\s*([a-z, A-Z, 0-9]*)\\(\\)");
                        Match match = regex.Match(line);

                        if (!string.IsNullOrEmpty(match.Value))
                            AddMacroName(string.Format("{0}.{1}.{2}", Name, module.Name, match.Groups[1].Value));
                    }
                }
            }
        }

        /// <summary>
        /// Vba signature of the project.
        /// </summary>
        internal byte[] Signature
        {
            get { return mSignature; }
            set { mSignature = value; }
        }

        /// <summary>
        /// Contains strings that are macro names.
        /// Can be null.
        /// </summary>
        internal IList<string> MacroNames
        {
            get { return mMacroNames; }
            set { mMacroNames = value; }
        }

        /// <summary>
        /// Adds a macro name into the <see cref="MacroNames"/> collection.
        /// </summary>
        internal void AddMacroName(string macroName)
        {
            if (mMacroNames == null)
                mMacroNames = new List<string>();

            mMacroNames.Add(macroName);
        }

        /// <summary>
        /// Specifies the platform for which the VBA project is created.
        /// </summary>
        internal int SysKind
        {
            get { return mSysKind; }
        }

        /// <summary>
        /// Language code identifier
        /// </summary>
        internal int Lcid
        {
            get { return mLcid; }
        }

        /// <summary>
        /// Specifies an LCID value used for Invoke calls on an Automation server as specified in [MS-OAUT] section 3.1.4.4.
        /// </summary>
        internal int LcidInvoke
        {
            get { return mLcidInvoke; }
        }

        /// <summary>
        /// Specifies the description for the VBA project.(2.3.4.2.1.6 PROJECTDOCSTRING Record)
        /// </summary>
        internal string Description
        {
            get { return mDescription; }
        }

        /// <summary>
        /// Specifies the path to the Help file for the VBA project.
        /// </summary>
        internal string HelpFilePath1
        {
            get { return mHelpFilePath1; }
        }

        /// <summary>
        /// Specifies the path to the Help file for the VBA project.
        /// </summary>
        internal string HelpFilePath2
        {
            get { return mHelpFilePath2; }
        }

        /// <summary>
        /// Specifies the Help topic identifier for the VBA project.
        /// </summary>
        internal int HelpContext
        {
            get { return mHelpContext; }
        }

        /// <summary>
        /// Specifies the LIBFLAGS for the VBA project’s Automation type library as specified in [MS-OAUT] section 2.2.20.
        /// </summary>
        internal int LibFlags
        {
            get { return mLibFlags; }
        }

        /// <summary>
        /// Specifies the major version of the VBA project.
        /// </summary>
        internal uint VersionMajor
        {
            get { return mVersionMajor; }
        }

        /// <summary>
        /// Specifies the minor version of the VBA project.
        /// </summary>
        internal ushort VersionMinor
        {
            get { return mVersionMinor; }
        }

        /// <summary>
        /// Specifies the compilation constants for the VBA project.
        /// </summary>
        internal string Constants
        {
            get { return mConstants; }
        }

        internal void MarkModified()
        {
            mVbaStorage = null;
            mSignature = null;

            RebuildMacroNames();
        }

        /// <summary>
        /// Returns VBA storage for a project.
        /// </summary>
        internal MemoryStorage Storage
        {
            get
            {
                // While not modified return original VBA storage.
                if (mVbaStorage != null)
                    return mVbaStorage;

                mVbaStorage = new MemoryStorage();

                mVbaStorage.Add(VbaRecord.VbaStorageName, new MemoryStorage());
                mVbaStorage.Add(VbaRecord.ProjectStreamName, StreamPROJECT());

                MemoryStorage vbaStorage = mVbaStorage.FetchStorage(VbaRecord.VbaStorageName);

                MemoryStream dir = new MemoryStream();
                WriteDir(dir);
                dir.Position = 0;
                vbaStorage.Add(VbaRecord.DirStreamName, dir);

                vbaStorage.Add(VbaRecord.CacheStreamName, Stream_VBA_PROJECT());

                // Write module source code streams.
                foreach(VbaModule module in Modules)
                {
                    // Add designer form data to VBA project.
                    if(module.FormMemoryStorage != null)
                        mVbaStorage.Add(module.Name, module.FormMemoryStorage);

                    MemoryStream newModuleStream = new MemoryStream();
                    MemoryStream inputStream = StringUtil.HasChars(module.SourceCode)?
                        new MemoryStream(Encoding.GetEncoding(mCodePage).GetBytes(module.SourceCode)):
                        new MemoryStream();
                    RleCompressor.Compress(inputStream, newModuleStream);

                    vbaStorage.Add(module.Name, newModuleStream);
                }

                return mVbaStorage;
            }
        }

        /// <summary>
        /// Parses a PROJECT stream.
        /// </summary>
        private void ParseProjectStream()
        {
            MemoryStream stream = mVbaStorage.FetchStream(VbaRecord.ProjectStreamName);

            string stringProj = mReader.Encoding.GetString(stream.ToArray());

            mId = GetParamValueByName(stringProj, VbaRecord.Id);
            mProtectionState = GetParamValueByName(stringProj, VbaRecord.ProtectionState);
            mPassword = GetParamValueByName(stringProj, VbaRecord.Password);
            mVisibilityState = GetParamValueByName(stringProj, VbaRecord.VisibilityState);
        }

        /// <summary>
        /// Gets param value enclosed into quotes from a string.
        /// </summary>
        private static string GetParamValueByName(string stringProj, string paramName)
        {
            // Concat '="' to avoid finding paramName in other places.
            string param = string.Concat(paramName, "=\"");

            int i = stringProj.IndexOf(param, StringComparison.Ordinal);
            if (i == -1)
                return string.Empty;

            int first = stringProj.IndexOf('"', i) + 1;

            int last = stringProj.IndexOf('"', first);

            if ((first > 0) && (last > 0))
                return stringProj.Substring(first, last - first);

            return string.Empty;
        }

        /// <summary>
        /// Creates a PROJECT stream for a VbaMacros.
        /// </summary>
        private MemoryStream StreamPROJECT()
        {
            // This list of properties makes the output document readable for MSW.
            // This list is just a part of all properties, feel free to add properties.

            StringBuilder stringProj = new StringBuilder();
            WriteParam(stringProj, VbaRecord.Id, mId, true);

            foreach (VbaModule module in Modules)
                WriteParam(stringProj, VbaModule.ModuleTypeToString(module.Type), module.Name, false);

            WriteParam(stringProj, "Name", mName, true);
            WriteParam(stringProj, "VersionCompatible32", "393222000", true);

            WriteParam(stringProj, VbaRecord.ProtectionState, mProtectionState, true);
            WriteParam(stringProj, VbaRecord.Password, mPassword, true);
            WriteParam(stringProj, VbaRecord.VisibilityState, mVisibilityState, true);

            byte[] bytes = Encoding.GetEncoding(mCodePage).GetBytes(stringProj.ToString());
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// Writes a parameter in a form paramName="paramValue".
        /// </summary>
        private static void WriteParam(StringBuilder sb, string paramName, string paramValue, bool useQuotes)
        {
            if (!StringUtil.HasChars(paramValue))
                return;

            sb.Append(paramName);
            sb.Append('=');
            if (useQuotes)
                sb.Append('"');
            sb.Append(paramValue);
            if (useQuotes)
                sb.Append('"');
            sb.Append("\r\n");
        }

        /// <summary>
        /// Creates a cached stream without the performance cache.
        /// </summary>
        private static MemoryStream Stream_VBA_PROJECT()
        {
            MemoryStream newCacheStream = new MemoryStream();
            newCacheStream.WriteByte(0xcc);
            newCacheStream.WriteByte(0x61);
            newCacheStream.WriteByte(0xff);
            newCacheStream.WriteByte(0xff);
            newCacheStream.WriteByte(0x00);
            newCacheStream.WriteByte(0x00);
            newCacheStream.WriteByte(0x00);
            return newCacheStream;
        }

        /// <summary>
        /// Writes "dir" stream.
        /// </summary>
        private void WriteDir(MemoryStream compressedDir)
        {
            MemoryStream dir = new MemoryStream();
            VbaRecordWriter writer = new VbaRecordWriter(new BinaryWriter(dir), Encoding.GetEncoding(mCodePage));

            // PROJECTINFORMATION
            writer.WriteInt32Record(VbaRecord.SysKind, mSysKind);           // PROJECTSYSKIND
            writer.WriteInt32Record(VbaRecord.Lcid, mLcid);              // PROJECTLCID
            writer.WriteInt32Record(VbaRecord.LcidInvoke, mLcidInvoke);              // PROJECTLCIDINVOKE
            writer.WriteInt16Record(VbaRecord.CodePage, mCodePage);               // PROJECTCODEPAGE

            writer.WriteStringRecord(VbaRecord.Name, mName);

            // PROJECTDOCSTRING
            writer.WriteStringRecord(VbaRecord.Description, mDescription);
            writer.WriteStringRecord(VbaRecord.DescriptionUnicode, Encoding.Unicode, mDescription);

            // <PROJECTHELPFILEPATH> must be defined in PROJECT Stream, if SizeOfHelpFile1 > 0.
            writer.WriteStringRecord(VbaRecord.HelpFilePath1, mHelpFilePath1);

            // According to specs: size of mHelpFilePath2 must be equal to size of SizeOfHelpFile1.
            // TODO check MSW behavior for the case, when sizes are not equal.
            writer.WriteStringRecord(VbaRecord.HelpFilePath2, mHelpFilePath2);

            writer.WriteInt32Record(VbaRecord.ProjectHelpContext, mHelpContext);
            writer.WriteInt32Record(VbaRecord.LibFlags, 0);

            // PROJECTVERSION
            writer.WriteUInt16(VbaRecord.ProjectVersion);
            // Write size of int32.
            writer.WriteInt32(4);
            writer.WriteUInt32(mVersionMajor);
            writer.WriteUInt16(mVersionMinor);

            // PROJECTCONSTANTS
            writer.WriteStringRecord(VbaRecord.Constants, mConstants);
            writer.WriteStringRecord(VbaRecord.ConstantsUnicode, Encoding.Unicode, mConstants);

            // PROJECTREFERENCES
            foreach (VbaReference reference in References)
                reference.Write(writer);

            writer.WriteInt16Record(VbaRecord.ModulesCount, (short)Modules.Count);

            // PROJECTCOOKIE
            writer.WriteUInt16(VbaRecord.ProjectCookieRecordId);
            // Write size of ushort.
            writer.WriteInt32(2);
            writer.WriteUInt16(VbaRecord.CookieRecord);

            // Array of Module record.
            foreach (VbaModule module in Modules)
                module.Write(writer);

            // The end of dir stream
            writer.WriteUInt16(VbaRecord.DirTerminator);
            writer.WriteInt32(0);

            dir.Position = 0;
            RleCompressor.Compress(dir, compressedDir);
        }

        private readonly VbaRecordReader mReader;

        private short mCodePage;
        private string mName;
        private VbaModuleCollection mModulesCollection;
        private VbaReferenceCollection mReferences;

        private readonly int mSysKind;
        private readonly int mLcid;
        private readonly int mLcidInvoke;

        private readonly string mDescription;
        private readonly string mHelpFilePath1;
        private readonly string mHelpFilePath2;

        private readonly int mHelpContext;
        private readonly int mLibFlags;

        private readonly uint mVersionMajor;
        private readonly ushort mVersionMinor;

        private readonly string mConstants;

        private MemoryStorage mVbaStorage;
        private byte[] mSignature;

        private IList<string> mMacroNames;

        private string mProtectionState;
        private string mPassword;
        private string mVisibilityState;
        private string mId;
    }
}
