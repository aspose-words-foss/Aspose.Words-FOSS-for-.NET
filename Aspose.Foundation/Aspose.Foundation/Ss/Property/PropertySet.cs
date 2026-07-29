// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2005 by Roman Korchagin
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aspose.Ss.Property
{
    /// <summary>
    /// Property set in a structured storage.
    /// </summary>
    public class PropertySet
    {
        public PropertySet()
        {
        }

        public PropertySet(Stream stream) : this(stream, 2)
        {
        }

        public PropertySet(Stream stream, int maxSections)
        {
            stream.Position = 0;
            BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

            //Read property set header
            reader.ReadUInt16();    //byte order
            //I support format version 0 only at the moment.
            //http://msdn2.microsoft.com/en-us/library/aa379124.aspx
            ushort format = reader.ReadUInt16();
            if (format != 0)
                throw new NotSupportedException("Cannot read property set in this format.");
            reader.ReadUInt32();    //os version
            mClsid = new Guid(reader.ReadBytes(16));
            int sectionCount = reader.ReadInt32();    //this is specified as reserved in MSDN.

            // WORDSNET-7686 Writers of property sets should set this value to 1; readers of property sets should ensure that this value is at least 1.
            // An exception to this guideline is that, for The DocumentSummaryInformation and UserDefined Property Sets, this value may be 2.
            // Any other value means that property set is corrupted.
            if((sectionCount < 0) || (sectionCount > maxSections))
            {
                // WARN.
                return;
            }

            //Read Format Identifier/Offset Pairs of the sections.
            Guid[] fmtIds = new Guid[sectionCount];
            int[] sectionOffsets = new int[sectionCount];
            for (int i = 0; i < sectionCount; i++)
            {
                fmtIds[i] = new Guid(reader.ReadBytes(16));
                sectionOffsets[i] = reader.ReadInt32();
            }

            //Read the sections themselves.
            for (int i = 0; i < sectionCount; i++)
            {
                reader.BaseStream.Position = sectionOffsets[i];
                mSections.Add(new PropertySetSection(fmtIds[i], reader));
            }
        }

        public void Save(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);

            //Write header
            writer.Write((UInt16)0xfffe);        //byte order
            writer.Write((UInt16)0);            //format
            writer.Write((UInt32)0x00020105);    //os version, this is what my WinXP writes.
            writer.Write(mClsid.ToByteArray());
            writer.Write((Int32)mSections.Count);    //marked as reserved in MSDN

            //First section will start after the header and after the Format Identifier/Offset Pairs.
            int firstSectionOffset = (int)stream.Position + FmtIdOffsetPairLength * mSections.Count;

            //Write Format Identifier/Offset Pairs of the sections.
            //And at the same time write sections into a memory buffer first so we know
            //their length and can arrange offsets.
            MemoryStream sectionsStream = new MemoryStream();
            for (int i = 0; i < mSections.Count; i++)
            {
                PropertySetSection section = (PropertySetSection)mSections[i];
                writer.Write(section.FmtId.ToByteArray());
                int sectionOffset = firstSectionOffset + (int)sectionsStream.Length;
                writer.Write(sectionOffset);
                //Write the section to the buffer. The next section will start right after this, thus
                //the length of the buffer can be used to determine the next section offset.
                byte[] sectionData = section.ToByteArray();
                sectionsStream.Write(sectionData, 0, sectionData.Length);
            }

            //Write the sections
            stream.Write(sectionsStream.GetBuffer(), 0, (int)sectionsStream.Length);
        }

        /// <summary>
        /// Contains <see cref="PropertySetSection"/> objects.
        /// </summary>
        public IList<PropertySetSection> Sections
        {
            get { return mSections; }
        }

        /// <summary>
        /// Sometimes this is zero.
        ///
        /// MSDN: The class identifier is the CLSID of a class that can display and/or provide programmatic
        /// access to the property values. If there is no such class, it is recommended that the Format ID be
        /// used (see below), though a value of all zeros is also acceptable; the former simply allows for
        /// greater future extensibility.
        /// </summary>
        private readonly Guid mClsid = Guid.Empty;    // Must be initialized for Java to work.
        private readonly List<PropertySetSection> mSections = new List<PropertySetSection>();

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int FmtIdOffsetPairLength = 16 + 4;
    }
}
