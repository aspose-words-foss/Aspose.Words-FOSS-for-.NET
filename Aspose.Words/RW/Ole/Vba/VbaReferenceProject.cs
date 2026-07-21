// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Specifies a reference to an external VBA project.
    /// </summary>
    /// <dev>
    /// This is an implementation of [MS-OVBA] 2.3.4.2.2.6 REFERENCEPROJECT Record.
    /// </dev>
    internal class VbaReferenceProject : VbaReference
    {
        /// <summary>
        /// Initializes a new VbaReferenceProject object with a specified name.
        /// </summary>
        internal VbaReferenceProject(string name) : base(name)
        {
        }

        /// <summary>
        /// Writes the reference into a specified writer.
        /// </summary>
        internal override void Write(VbaRecordWriter writer)
        {
            WriteNames(writer);

            writer.WriteUInt16((ushort)Type);

            byte[] libIdAbsolutebytes = writer.Encoding.GetBytes(LibIdAbsolute);
            byte[] libIdRelativebytes = writer.Encoding.GetBytes(LibIdRelative);
            int totalSize = libIdAbsolutebytes.Length + libIdRelativebytes.Length + 14;
            writer.WriteInt32(totalSize);
            writer.WriteString(LibIdAbsolute);
            writer.WriteString(LibIdRelative);
            writer.WriteUInt32(MajorVersion);
            writer.WriteUInt16(MinorVersion);
        }

        /// <summary>
        /// Reads reference data from a specified reader.
        /// </summary>
        internal override void ReadCore(VbaRecordReader reader)
        {
            reader.ReadInt32(); // ignored
            LibIdAbsolute = reader.ReadString();
            LibIdRelative = reader.ReadString();
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt16();
        }

        /// <summary>
        /// Gets <see cref="VbaReferenceType"/> object that indicates the type of reference that a VbaReference object represents.
        /// </summary>
        public override VbaReferenceType Type
        {
            get { return VbaReferenceType.Project; }
        }

        /// <summary>
        /// Gets a string value containing the identifier of an Automation type library.
        /// </summary>
        /// <remarks>
        /// Depending on reference type, the value of this property can be:
        /// <list type="bullet">
        /// <item>a LibidReference specified at 2.1.1.8 LibidReference of [MS-OVBA]:
        /// https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-ovba/3737ef6e-d819-4186-a5f2-6e258ddf66a5</item>
        /// <item>a ProjectReference specified at 2.1.1.12 ProjectReference of [MS-OVBA]:
        /// https://docs.microsoft.com/en-us/openspecs/office_file_formats/ms-ovba/9a45ac1a-f1ff-4ebd-958e-537701aa8131</item>
        /// </list>
        ///  </remarks>
        public override string LibId
        {
            get { return LibIdAbsolute; }
        }

        /// <summary>
        /// Specifies the referenced VBA project’s identifier with an absolute path.
        /// </summary>
        internal string LibIdAbsolute { get; private set; }

        /// <summary>
        /// Specifies the referenced VBA project’s identifier with a relative path, that is relative to the current VBA project.
        /// </summary>
        internal string LibIdRelative { get; private set; }

        /// <summary>
        /// Specifies the major version of the referenced VBA project.
        /// </summary>
        internal uint MajorVersion { get; private set; }

        /// <summary>
        /// Specifies the minor version of the external VBA project.
        /// </summary>
        internal ushort MinorVersion { get; private set; }
    }
}
