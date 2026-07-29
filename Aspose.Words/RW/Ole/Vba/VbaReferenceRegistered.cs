// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Specifies a reference to an Automation type library.
    /// </summary>
    /// <dev>
    /// This is an implementation of [MS-OVBA] 2.3.4.2.2.5 REFERENCEREGISTERED Record.
    /// </dev>
    internal class VbaReferenceRegistered : VbaReference
    {
        /// <summary>
        /// Initializes a new VbaReferenceRegistered object with a specified name.
        /// </summary>
        internal VbaReferenceRegistered(string name) : base(name)
        {
        }

        /// <summary>
        /// Writes the reference into a specified writer.
        /// </summary>
        internal override void Write(VbaRecordWriter writer)
        {
            WriteNames(writer);

            writer.WriteUInt16((ushort)Type);

            byte[] libIdbytes = writer.Encoding.GetBytes(LibId);
            int totalSize = libIdbytes.Length + 10;
            writer.WriteInt32(totalSize);
            writer.WriteString(LibId);
            writer.WriteInt32(0);
            writer.WriteInt16((short)0); // casting added for Java
        }

        /// <summary>
        /// Reads reference data from a specified reader.
        /// </summary>
        internal override void ReadCore(VbaRecordReader reader)
        {
            reader.ReadInt32(); // ignored
            mLibId = reader.ReadString(); // libId
            reader.ReadInt32(); // reserved1
            reader.ReadInt16(); // reserved2
        }

        /// <summary>
        /// Gets <see cref="VbaReferenceType"/> object that indicates the type of reference that a VbaReference object represents.
        /// </summary>
        public override VbaReferenceType Type
        {
            get { return VbaReferenceType.Registered; }
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
            get { return mLibId; }
        }

        private string mLibId;
    }
}
