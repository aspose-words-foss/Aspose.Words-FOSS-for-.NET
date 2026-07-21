// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Specifies a reference to a twiddled type library and its extended type library.
    /// </summary>
    /// <dev>
    /// This is an implementation of [MS-OVBA] 2.3.4.2.2.3 REFERENCECONTROL Record.
    /// </dev>
    internal class VbaReferenceControl : VbaReference
    {
        /// <summary>
        /// Initializes a new VbaReferenceControl object with a specified name.
        /// </summary>
        internal VbaReferenceControl(string name) : base(name)
        {
        }

        /// <summary>
        /// Writes the reference into a specified writer.
        /// </summary>
        internal override void Write(VbaRecordWriter writer)
        {
            WriteNames(writer);

            writer.WriteUInt16((ushort)Type);
            WriteTypeLibraries(writer);
        }

        /// <summary>
        /// Writes twiddled type library and its extended type library.
        /// </summary>
        /// <remarks>
        /// See [MS-OVBA] 2.3.4.2.2.3 REFERENCECONTROL Record.
        /// </remarks>
        internal void WriteTypeLibraries(VbaRecordWriter writer)
        {
            byte[] libIdTwiddledbytes = writer.Encoding.GetBytes(LibIdTwiddled);
            int totalSize = libIdTwiddledbytes.Length + 10;
            writer.WriteInt32(totalSize);
            writer.WriteString(LibIdTwiddled);
            writer.WriteInt32(0);
            writer.WriteInt16((short)0); // casting added for Java

            writer.WriteUInt16(VbaRecord.ReferenceReserved);

            byte[] libIdExtendedbytes = writer.Encoding.GetBytes(LibIdExtended);
            totalSize = libIdExtendedbytes.Length + 30;
            writer.WriteInt32(totalSize);
            writer.WriteString(LibIdExtended);
            writer.WriteInt32(0);
            writer.WriteInt16((short)0); // casting added for Java
            writer.WriteBytes(OriginalTypeLib);
            writer.WriteInt32(Cookie);
        }

        /// <summary>
        /// Reads reference data from a specified reader.
        /// </summary>
        internal override void ReadCore(VbaRecordReader reader)
        {
            reader.ReadInt32(); // ignored
            LibIdTwiddled = reader.ReadString();
            reader.ReadInt32(); // reserved1
            reader.ReadInt16(); // reserved2

            if (reader.PeekId() == VbaRecord.ReferenceName)
            {
                NameExtended = reader.ReadStringRecords(VbaRecord.ReferenceName, VbaRecord.ReferenceNameUnicode);
            }

            int id = reader.ReadInt16();
            Debug.Assert(id == VbaRecord.ReferenceReserved);
            reader.ReadInt32(); // ignored
            LibIdExtended = reader.ReadString();
            reader.ReadInt32(); // reserved1
            reader.ReadInt16(); // reserved2
            OriginalTypeLib = reader.ReadBytes(16); // originalTypeLib
            Cookie = reader.ReadInt32(); // cookie
        }

        /// <summary>
        /// Gets <see cref="VbaReferenceType"/> object that indicates the type of reference that a VbaReference object represents.
        /// </summary>
        public override VbaReferenceType Type
        {
            get { return VbaReferenceType.Control; }
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
            get { return LibIdTwiddled; }
        }

        /// <summary>
        /// Specifies the name of the extended type library. This property is optional.
        /// </summary>
        internal string NameExtended { get; private set; }

        /// <summary>
        /// Specifies a twiddled type library’s identifier that MUST conform to the ABNF grammar LibidReference.
        /// </summary>
        internal string LibIdTwiddled { get; private set; }

        /// <summary>
        /// Specifies the extended type library’s identifier.
        /// </summary>
        internal string LibIdExtended { get; private set; }

        /// <summary>
        /// A GUID that specifies the Automation type library the extended type library was generated from.
        /// </summary>
        internal byte[] OriginalTypeLib { get; private set; }

        /// <summary>
        /// An unsigned integer that specifies the extended type library's cookie. Must be unique for each
        /// referenceControl in the project with the same OriginalTypeLib.
        /// </summary>
        private int Cookie { get; set; }
    }
}
