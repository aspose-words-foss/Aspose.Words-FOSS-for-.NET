// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Specifies the identifier of the Automation type library the containing
    /// reference control twiddled type library was generated from.
    /// </summary>
    /// <dev>
    /// This is an implementation of [MS-OVBA] 2.3.4.2.2.4 REFERENCEORIGINAL Record.
    /// </dev>
    internal class VbaReferenceOriginal : VbaReferenceControl
    {

        /// <summary>
        /// Initializes a new VbaReferenceOriginal object with a specified name.
        /// </summary>
        internal VbaReferenceOriginal(string name) : base(name)
        {
        }

        /// <summary>
        /// Writes the reference into a specified writer.
        /// </summary>
        internal override void Write(VbaRecordWriter writer)
        {
            WriteNames(writer);
            writer.WriteUInt16((ushort)Type);
            writer.WriteString(LibIdOriginal);

             // WORDSNET-22807 Write reference to a twiddled type library and its extended type library.
            if (StringUtil.HasChars(NameExtended))
            {
                writer.WriteUInt16(VbaRecord.ReferenceControlId);
                WriteTypeLibraries(writer);
            }
        }

        /// <summary>
        /// Reads reference data from a specified reader.
        /// </summary>
        internal override void ReadCore(VbaRecordReader reader)
        {
            LibIdOriginal = reader.ReadString();
            if (reader.PeekId() == VbaRecord.ReferenceControlId)
            {
                reader.ReadUInt16();
                base.ReadCore(reader);
            }
        }

        /// <summary>
        /// Gets <see cref="VbaReferenceType"/> object that indicates the type of reference that a VbaReference object represents.
        /// </summary>
        public override VbaReferenceType Type
        {
            get { return VbaReferenceType.Original; }
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
            get { return LibIdOriginal; }
        }

        /// <summary>
        /// Specifies the identifier of the Automation type library a REFERENCECONTROL was generated from.
        /// </summary>
        internal string LibIdOriginal { get; private set; }
    }
}
