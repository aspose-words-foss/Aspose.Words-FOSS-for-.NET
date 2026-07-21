// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2019 by Alexander Sevidov

using System;
using System.Text;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Implements a reference to an Automation type library or VBA project.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-vba-macros/">Working with VBA Macros</a> documentation article.</para>
    /// </summary>
    public abstract class VbaReference
    {
        /// <summary>
        /// Initializes a new VbaReference object with a specified name.
        /// </summary>
        internal VbaReference(string name)
        {
            mName = name;
        }

        /// <summary>
        /// Performs a copy of the <see cref="VbaReference"/>.
        /// </summary>
        /// <returns>The cloned <see cref="VbaReference"/>.</returns>
        internal VbaReference Clone()
        {
            return (VbaReference)MemberwiseClone();
        }

        /// <summary>
        /// Reads VbaReference from a specified reader.
        /// </summary>
        internal static VbaReference Read(VbaRecordReader reader)
        {
            string name = reader.ReadStringRecords(VbaRecord.ReferenceName, VbaRecord.ReferenceNameUnicode);
            VbaReferenceType type = (VbaReferenceType)reader.ReadUInt16();

            // Create concrete empty reference in accordance with read type.
            VbaReference vbaReference = Create(type, name);

            // Populate reference with a related data from the reader.
            vbaReference.ReadCore(reader);

            return vbaReference;
        }

        /// <summary>
        /// Writes the reference into a specified writer.
        /// </summary>
        [JavaAttributes.JavaThrows(true)]
        internal abstract void Write(VbaRecordWriter writer);

        /// <summary>
        ///  Writes <see cref="VbaRecord.Name"/> and <see cref="VbaRecord.ReferenceNameUnicode"/> to a specified writer.
        /// </summary>
        internal void WriteNames(VbaRecordWriter writer)
        {
            writer.WriteStringRecord(VbaRecord.ReferenceName, Name);
            writer.WriteStringRecord(VbaRecord.ReferenceNameUnicode, Encoding.Unicode, Name);
        }

        /// <summary>
        /// Reads reference data from a specified reader.
        /// </summary>
        [JavaAttributes.JavaThrows(true)]
        internal abstract void ReadCore(VbaRecordReader reader);

        /// <summary>
        /// Creates VbaReference instance with a specified name and type.
        /// </summary>
        private static VbaReference Create(VbaReferenceType type, string name)
        {
            switch (type)
            {
                case VbaReferenceType.Registered:
                    return new VbaReferenceRegistered(name);
                case VbaReferenceType.Project:
                    return new VbaReferenceProject(name);
                case VbaReferenceType.Original:
                    return new VbaReferenceOriginal(name);
                case VbaReferenceType.Control:
                    return new VbaReferenceControl(name);
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected VBA reference type: {0}", type));
            }
        }

#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, LibId);
        }
#endif

        /// <summary>
        /// Gets <see cref="VbaReferenceType"/> object that indicates the type of reference that a <see cref="VbaReference"/> object represents.
        /// </summary>
        public abstract VbaReferenceType Type { get; }

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
        public abstract string LibId { get; }

        /// <summary>
        /// Specifies the name of a referenced VBA project or Automation type library.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }

        private readonly string mName;
    }
}
