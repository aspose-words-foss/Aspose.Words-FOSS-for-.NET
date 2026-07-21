// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2005 by Roman Korchagin
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Aspose.IO;

namespace Aspose.Ss.Property
{
    /// <summary>
    /// Represents a section in a property set in a structured storage.
    /// This is the actual container for properties.
    /// 
    /// http://msdn.microsoft.com/archive/default.asp?url=/archive/en-us/dnarolegen/html/msdn_propset.asp
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public class PropertySetSection
    {
        /// <summary>
        /// Common private ctor code.
        /// </summary>
        private PropertySetSection(Guid fmtId)
        {
            mFmtId = fmtId;
            mProperties = new PropertyCollection();
        }

        /// <summary>
        /// Ctor. creates a property set section. Properties will be written to OLE in the specified code page.
        /// </summary>
        public PropertySetSection(Guid fmtId, int codePage) : this(fmtId)
        {
            if (codePage <= 0)
                throw new ArgumentOutOfRangeException("codePage");

            mCodePage = codePage;
        }

        /// <summary>
        /// Ctor, reads a section of properties.
        /// </summary>
        public PropertySetSection(Guid fmtId, BinaryReader reader) : this(fmtId)
        {
            long sectionStart = reader.BaseStream.Position;

            // Read property section header
            // Ignore size.
            reader.ReadInt32();
            int count = reader.ReadInt32();

            // Read property ids and offsets.
            List<PropIdOffset> propIdOffsets = new List<PropIdOffset>();
            for (int i = 0; i < count; i++)
                propIdOffsets.Add(new PropIdOffset(reader));

            // Read optional code page indicator for non-Unicode strings.
            mCodePage = DefaultCodePage;
            int codePageOffset = GetPropOffset(propIdOffsets, CodePagePropId);
            if (codePageOffset != -1)
            {
                reader.BaseStream.Position = sectionStart + codePageOffset;
                object v = ReadPropValue(reader, 0);
                if (v is short)
                {
                    // This property is stored as VT_I2 so I have to cast it to short first to unbox.
                    short s = (short)v;
                    // But then I have to mask it to 16 bits when extending to int32 so it works correctly 
                    // for code pages like UTF-7 (65000) to work properly.
                    mCodePage = s & 0xffff;
                }
            }

            // Read optional property names.
            Dictionary<int, string> propNames = new Dictionary<int, string>();
            int propNamesOffset = GetPropOffset(propIdOffsets, DictionaryPropId);
            if (propNamesOffset != -1)
            {
                reader.BaseStream.Position = sectionStart + propNamesOffset;

                // WORDSNET-19571 Remove all custom properties if custom property name contain garbage.
                // ArgumentOutOfRangeException for .Net. IndexOutOfBoundsException for Java.
                try
                {
                    ReadPropNames(reader, propNames, mCodePage);
                }
                catch
                {
                    propNames.Clear();
                }
                
            }

            // Read property values
            for (int i = 0; i < propIdOffsets.Count; i++)
            {
                PropIdOffset propIdOffset = propIdOffsets[i];

                // Ignore all system properties since we have already read the ones we wanted.
                // But to do a compare we extend int into long while making sure the sign is not extended.
                // This is needed because the actual property id range is uint but we don't want to use uint here for better autoporting.
                long longPropId = propIdOffset.Id & 0xfffffffL;
                if ((longPropId < MinUserPropId) || (longPropId > MaxUserPropId))
                    continue;

                // Property offset is from the section start.
                reader.BaseStream.Position = sectionStart + propIdOffset.Offset;
                object propValue = ReadPropValue(reader, mCodePage);
                if (propValue != null)
                {
                    string propName = propNames.GetValueOrNull(propIdOffset.Id);
                    mProperties.Add(new Property(propIdOffset.Id, propName, propValue));
                }
            }
        }

        private static int GetPropOffset(IList<PropIdOffset> propIdOffsets, int propId)
        {
            for (int i = 0; i < propIdOffsets.Count; i++)
            {
                PropIdOffset propIdOffset = propIdOffsets[i];
                if (propIdOffset.Id == propId)
                    return propIdOffset.Offset;
            }
            return -1;
        }

        /// <summary>
        /// Writes this property set section into a byte array.
        /// </summary>
        public byte[] ToByteArray()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            // Size and Count.
            const int SectionHeaderSize = 4 + 4;    
            // PropertyId and Offset
            const int PropIdOffsetSize = 4 + 4;        

            int firstPropertyOffset = 
                SectionHeaderSize +
                PropIdOffsetSize + // Add one entry for code page.
                PropIdOffsetSize * mProperties.Count;
            
            // Add one entry for dictionary of property names if we are writing it.
            // WORDSNET-If we don't write it for user defined properties, MS Excel seems to lose the 
            // digital signature when you look at it in the VBA editor.
            // If we write it for built in properties, MS Excel does not show any properties at all.
            bool isWritingPropNames = (mProperties.HasNames || mFmtId.Equals(FMTID_UserDefined));
            if (isWritingPropNames)
                firstPropertyOffset += PropIdOffsetSize;

            stream.Position = firstPropertyOffset;

            // Written property ids and offset accumulate here so we write them at the end.
            List<PropIdOffset> propIdOffsets = new List<PropIdOffset>();

            if (isWritingPropNames)
            {
                propIdOffsets.Add(new PropIdOffset(DictionaryPropId, (int)stream.Position));
                WritePropNames(writer, mProperties, mCodePage);
            }

            // Write code page. 
            propIdOffsets.Add(new PropIdOffset(CodePagePropId, (int)stream.Position));
            WritePropValue(writer, (short)mCodePage, mCodePage, true);    // This property has to be VT_I2.

            // Write properties and remember their offsets.
            for (int i = 0; i < mProperties.Count; i++)
            {
                Property prop = mProperties[i];
                // Current position in the stream is the offset where the property will be written.
                propIdOffsets.Add(new PropIdOffset(prop.Id, (int)stream.Position));    
                WritePropValue(writer, prop.Value, mCodePage, true);
            }

            // Write section header
            stream.Position = 0;
            writer.Write((Int32)stream.Length);
            writer.Write((Int32)propIdOffsets.Count);

            // Write prop offsets
            for (int i = 0; i < propIdOffsets.Count; i++)
            {
                PropIdOffset propIdOffset = propIdOffsets[i];
                propIdOffset.Write(writer);
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Gets the properties read from the structured storage.
        /// </summary>
        public PropertyCollection Properties
        {
            get { return mProperties; }
        }

        public Guid FmtId
        {
            get { return mFmtId; }
        }

        public int CodePage
        {
            get { return mCodePage; }
        }

        /// <summary>
        /// Reads a map of prop ids to prop names (property name dictionary). Reader must be positioned properly.
        /// </summary>
        private static void ReadPropNames(BinaryReader reader, Dictionary<int, string> propNames, int codePage)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // WORDSNET-13831 Resilience against truncated stream.
                if (!StreamUtil.HasEnoughBytesToRead(reader, 4 /* Int32 */))
                    break;

                int propId = reader.ReadInt32();
                string propName = ReadString(reader, codePage);

                // WORDSNET-5247 Stop reading in case of error.
                if (!StringUtil.HasChars(propName))
                    break;

                propNames.Add(propId, propName);
            }
        }

        /// <summary>
        /// Writes a map of propids to prop names.
        /// </summary>
        private static void WritePropNames(BinaryWriter writer, PropertyCollection props, int codePage)
        {
            writer.Write((Int32)props.CountOfPropertiesWithNames);
            for (int i = 0; i < props.Count; i++)
            {
                Property prop = props[i];
                if (prop.HasName)
                {
                    writer.Write((Int32)prop.Id);
                    WriteString(writer, prop.Name, codePage);
                }
            }
        }

        /// <summary>
        /// Read one property value. The reader must be positioned at the beginning of the property.
        /// </summary>
        /// <remarks>
        /// Normally returns a property value. Returns null for some property types that we don't support.
        /// These property types are used for properties that we can ignore.
        /// </remarks>
        private static object ReadPropValue(BinaryReader reader, int codePage)
        {
            VarEnum propType = (VarEnum)reader.ReadInt32();
            switch (propType)
            {
                case VarEnum.VT_BSTR:
                {
                    return InvalidPropertyType;
                }
                case VarEnum.VT_LPSTR:
                {
                    return ReadLPSTR(reader, codePage);
                }
                case VarEnum.VT_LPWSTR:
                {
                    string result = ReadLPWSTR(reader);
                    // It looks like all LPWSTR strings in property sets are padded to 32 bits.
                    // This line of code is needed for correctly reading a vector of LPWSTR.
                    StreamUtil.SeekToNextPage(reader.BaseStream, PropValueAlignment);
                    return result;
                }
                case VarEnum.VT_FILETIME:
                {
                    long ticks = reader.ReadInt64();

                    //Date's boundaries check: accidental negative or too big ticks count causes an exception.
                    //"No date/time" (ticks == 0) is represented as DateTime.MinValue in the model.
                    //Lets display wrong ticks value by DateTime.MinValue too.
                    if (ticks <= DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks)
                        return DateTime.MinValue;

                    //The time is stored in UTC in the file, return it as UTC too.
                    return DateTimeUtil.FromFileTimeUtc(ticks, "ticks");
                }
                case VarEnum.VT_UI4:
                    return reader.ReadUInt32();
                case VarEnum.VT_I4:
                    return reader.ReadInt32();
                case VarEnum.VT_I2:
                    return reader.ReadInt16();
                case VarEnum.VT_BOOL:
                    return (reader.ReadInt16() != 0);
                case VarEnum.VT_R8:
                    return reader.ReadDouble();
                case VarEnum.VT_BLOB:
                {
                    int length = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(length);
                    return data;
                }
                case (VarEnum.VT_VECTOR | VarEnum.VT_VARIANT):
                {
                    // This code presumes this property is HeadingPairs.
                    // http://msdn2.microsoft.com/en-us/library/aa380374(VS.85).aspx
                    return ReadHeadingPairs(reader, codePage);
                }
                case (VarEnum.VT_VECTOR | VarEnum.VT_LPSTR):
                {
                    // WORDSNET-3942 Aspose.Words.FileCorruptedException” exception occurs during opening the document.
                    // RK The property set specifies Unicode code page, but TitlesOfParts are stored as
                    // ASCII strings so we have to override the code page.
                    int realCodePage = (codePage != UnicodeCodePage) ? codePage : DefaultCodePage;

                    return ReadTitlesOfParts(reader, realCodePage);
                }
                case (VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR):
                {
                    return ReadTitlesOfParts(reader, codePage);
                }
                default:
                {
                    //VarEnum.VT_CF used for Thumbnail that we do not support.
                    //http://msdn2.microsoft.com/en-us/library/aa380376(VS.85).aspx
                    return null;
                }
            }
        }

        private static string[] ReadTitlesOfParts(BinaryReader reader, int codePage)
        {
            // TitlesofParts as per the MSDN spec. Related to HeadingPairs.
            int count = reader.ReadInt32();
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
                result[i] = ReadString(reader, codePage);

            return result;
        }

        private static object[] ReadHeadingPairs(BinaryReader reader, int codePage)
        {
            // This code presumes this property is HeadingPairs.
            // http://msdn2.microsoft.com/en-us/library/aa380374(VS.85).aspx
            int itemCount = reader.ReadInt32();
            // This is an array of <string, int> that is <part name, heading count>
            object[] result = new object[itemCount];
            for (int i = 0; i < itemCount; i++)
                result[i] = ReadPropValue(reader, codePage); 

            // Perform quick validation.
            // AM. This property MUST be <string, int> array. Otherwise DOCX output is failed to open by Word.
            // Also I found that <int> value must be greater than 0.
            // So I do quick check and return empty property in case validation is failed.
            bool isValid = true;
            for (int i = 0; i < result.Length; i++)
            {
                object o = result[i];
                isValid &= (MathUtil.IsOdd(i) ? (o is int) && ((int)o > 0) : o is string);
            }

            return isValid ? result : null;
        }

        /// <summary>
        /// Writes a property type.
        /// Normally, properties need to be aligned to 4 byte boundary.
        /// However, when writing elements of an VT_VECTOR, there must be no alignment between elements.
        /// </summary>
        private static void WritePropValue(BinaryWriter writer, object value, int codePage, bool isAlignRequired)
        {
            string valueString = value as string;

            if (valueString != null)
            {
                if (codePage == UnicodeCodePage)
                    writer.Write((int)VarEnum.VT_LPWSTR);
                else
                    writer.Write((int)VarEnum.VT_LPSTR);

                WriteString(writer, valueString, codePage);
            }
            else if (value is short)
            {
                writer.Write((int)VarEnum.VT_I2);
                writer.Write((short)value);
            }
            else if (value is int)
            {
                writer.Write((int)VarEnum.VT_I4);
                writer.Write((int)value);
            }
            else if (value is uint)               // RK I don't like this because it only works in autoporting because we don't write VT_I8.
            {
                writer.Write((int)VarEnum.VT_UI4);
                writer.Write((uint)value);
            }
            else if (value is double)
            {
                writer.Write((int)VarEnum.VT_R8);
                writer.Write((double)value);
            }
            else if (value is bool)
            {
                writer.Write((int)VarEnum.VT_BOOL);
                bool b = (bool)value;
                writer.Write((short)((b) ? -1 : 0));
            }
            else if (value is DateTime)
            {
                writer.Write((int)VarEnum.VT_FILETIME);
                DateTime dateTime = (DateTime)value;
                if (dateTime != DateTime.MinValue)
                    writer.Write(DateTimeUtil.ToFileTimeUtc(dateTime, "dateTime"));
                else
                    writer.Write((long)0);
            }
            else if (value is byte[])
            {
                writer.Write((int)VarEnum.VT_BLOB);
                byte[] data = (byte[])value;
                writer.Write(data.Length);
                writer.Write(data);
            }
            else if (value is string[])
            {
                // This code presumes we are writing TitlesofParts.
                // WORDSNET-3393 Programmatically added custom properties are invisible in MSWord but visible.
                // It took me some time to figure out how to properly write this property in Unicode.
                if (codePage == UnicodeCodePage)
                    writer.Write((int)VarEnum.VT_VECTOR | (int)VarEnum.VT_LPWSTR);   
                else
                    writer.Write((int)VarEnum.VT_VECTOR | (int)VarEnum.VT_LPSTR);
                
                string[] strings = (string[])value;
                writer.Write(strings.Length);
                foreach (string s in strings)
                    WriteString(writer, s, codePage);
            }
            else if (value is object[])
            {
                // This code presumes we are writing HeadingPairs.
                writer.Write((int)VarEnum.VT_VECTOR | (int)VarEnum.VT_VARIANT);
                
                object[] items = (object[])value;
                writer.Write(items.Length);
                foreach (object item in items)
                    WritePropValue(writer, item, codePage, false);
            }
            else
            {
                throw new NotSupportedException("Do not know how to write a property value of this type.");
            }

            if (isAlignRequired)
                StreamUtil.SeekToNextPage(writer.BaseStream, PropValueAlignment);
        }

        /// <summary>
        /// Reads an LPSTR in the OLE property storage format.
        /// </summary>
        private static string ReadLPSTR(BinaryReader reader, int codePage)
        {
            // For an LPSTR a count of BYTES that includes a terminating zero is stored.
            // But since encoding can be Unicode there could actually be more than one zero byte,
            // therefore I don't strip the zero bytes yet and read all bytes.
            int byteCount = reader.ReadInt32();

            if (!StreamUtil.HasEnoughBytesToRead(reader, byteCount))
            {
                // WORDSNET-5247 Stop reading if beyond of stream and return empty string.
                byteCount = 0;
            }

            byte[] data = reader.ReadBytes(byteCount);
            string s = Encoding.GetEncoding(codePage).GetString(data, 0, byteCount);

            // It is time to remove the terminating zero, but I saw \0\0\0\0 for an empty string.
            // Does it means it pads minimum to 4 chars? Certainly not always, but we can have 
            // more than one zero at the end. Lets strip them all.
            s = s.TrimEnd('\0');

            return s;
        }

        /// <summary>
        /// Writes an LPSTR in the OLE property storage format.
        /// </summary>
        private static void WriteLPSTR(BinaryWriter writer, string s, int codePage)
        {
            // Add terminating zero.
            s += '\0';
            byte[] data = Encoding.GetEncoding(codePage).GetBytes(s);
            // For an LPSTR a count of BYTES that includes a terminating zero is stored.
            writer.Write((Int32)data.Length);
            writer.Write(data);
        }

        /// <summary>
        /// Reads an LPWSTR in the OLE property storage format.
        /// </summary>
        private static string ReadLPWSTR(BinaryReader reader)
        {
            // Ugly MS. For LPWSTR a count of CHARS is stored. 
            // We have to multiply it to get the number of bytes.
            int byteCount = reader.ReadInt32() * 2;
            byte[] data = reader.ReadBytes(byteCount);
            string result = Encoding.Unicode.GetString(data, 0, byteCount - 2); //Do not need terminating null.

            // WORDSNET-8605 If the number of meaningful characters is even, an extra terminator is added by MS Word,
            // since LPWSTR strings are padded to 32 bits. It should be trimmed either.
            if (StringUtil.HasChars(result))
                result = result.TrimEnd('\0');

            return result;
        }

        /// <summary>
        /// Writes an LPWSTR in the OLE property storage format.
        /// </summary>
        private static void WriteLPWSTR(BinaryWriter writer, string s)
        {
            // For an LPWSTR we need to write count of CHARS including zero terminator.
            writer.Write((Int32)s.Length + 1);    //One extra for terminator.
            writer.Write(Encoding.Unicode.GetBytes(s));
            writer.Write((short)0);    //Terminating null.
        }

        /// <summary>
        /// Reads a string either as LPWSTR or as LPSTR. When reading LPWSTR pads to 32 bits.
        /// </summary>
        private static string ReadString(BinaryReader reader, int codePage)
        {
            string result;
            if (codePage == UnicodeCodePage)
            {
                // When code page is Unicode, LPWSTR strings and padded to 32 bits.
                result = ReadLPWSTR(reader);
                StreamUtil.SeekToNextPage(reader.BaseStream, PropValueAlignment);
            }
            else
            {
                result = ReadLPSTR(reader, codePage);
            }
            return result;
        }

        /// <summary>
        /// Writes a string either as LPWSTR or as LPSTR. When Unicode, writes as LPWSTR and pads to 32 bits.
        /// </summary>
        private static void WriteString(BinaryWriter writer, string s, int codePage)
        {
            if (codePage == UnicodeCodePage)
            {
                // When code page is Unicode, LPWSTR strings and padded to 32 bits.
                WriteLPWSTR(writer, s);
                StreamUtil.SeekToNextPage(writer.BaseStream, PropValueAlignment);
            }
            else
            {
                WriteLPSTR(writer, s, codePage);
            }
        }

        private readonly Guid mFmtId = Guid.Empty;    // Must be initialized for Java to work.
        private readonly PropertyCollection mProperties;
        private readonly int mCodePage;

        /// <summary>
        /// We use 1252 code when reading properties and code page is not specified by the property set.
        /// </summary>
        private const short DefaultCodePage = 1252;
        private const short UnicodeCodePage = 1200;
        
        /// <summary>
        /// Property values are aligned on 32 bit boundaries.
        /// </summary>
        private const int PropValueAlignment = 4;        

        /// <summary>
        /// This property contains user defined property names.
        /// </summary>
        private const int DictionaryPropId = 0x00000000;
        /// <summary>
        /// Contains indicator as to what code page the strings are stored in.
        /// </summary>
        private const int CodePagePropId = 0x00000001;
        private const int MinUserPropId = 0x00000002;
        private const int MaxUserPropId = 0x7fffffff;
        /// <summary>
        /// Sometimes present in a property set. Not sure what to do with it. Let's ignore.
        /// </summary>
        private const int LocalePropId = unchecked((int)0x80000000);
        /// <summary>
        /// When this property value is 0x00 it means case-insensitive compare for property names (default).
        /// When this properyt value is 0x01 it means case-sensitive compare.
        /// Don't want to bother handling this, let's ignore.
        /// </summary>
        private const int BehaviorPropId = unchecked((int)0x80000003);

        /// <summary>
        /// Predefined property set identifiers (in fact these are section identifiers).
        /// </summary>
        public static readonly Guid FMTID_SummaryInfo = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");
        public static readonly Guid FMTID_DocSummaryInfo = new Guid("D5CDD502-2E9C-101B-9397-08002B2CF9AE");
        public static readonly Guid FMTID_UserDefined = new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

        public static readonly string InvalidPropertyType = new Guid().ToString();
    }
}
