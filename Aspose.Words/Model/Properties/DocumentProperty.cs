// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2004 by Roman Korchagin
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.Properties
{
    /// <summary>
    /// Represents a custom or built-in document property.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-document-properties/">Work with Document Properties</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="DocumentPropertyCollection"/>
    public class DocumentProperty
    {
        internal DocumentProperty(string name, object value)
        {
            ArgumentUtil.CheckHasChars(name, "name");

            mName = name;
            this.Value = value;
        }

        /// <summary>
        /// Returns the name of the property.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c> and cannot be an empty string.</para>
        /// </remarks>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// </remarks>
        public object Value
        {
            get
            {
                object value = ValueInternal;

                // We need to detect and convert DateTime into java.util.Date because all we have is an Object here.
#if PLAIN_JAVA
            if (value instanceof DateTime)
                return ((DateTime)value).toJava();
#endif
                if (Type == PropertyType.String)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char ch in ((string) value)) // spare parenthesis for java autoporting
                    {
                        // WORDSNET-12511 Word replaces characters below 0x09 with its HEX representation while displays it in UI.
                        if (ch <= 0x08)
                            sb.AppendFormat("_x{0:X4}_", (int)ch);
                        else
                            sb.Append(ch);
                    }

                    value = sb.ToString();
                }

                return value;
            }
            set
            {
#if PLAIN_JAVA        // We need to detect and convert java.util.Date into DateTime because all we have is an Object here.
            if (value instanceof java.util.Date)
                value = DateTime.fromJava((java.util.Date)value);
#endif

                ValueInternal = value;
            }
        }

        internal object DefaultValueInternal
        {
            get { return mDefaultValueInternal; }
            set { mDefaultValueInternal = value; }
        }

        /// <summary>
        /// Gets the property value without converting it to java.util.Date for the public API in Java.
        /// </summary>
        internal object ValueInternal
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return (mDefaultValueInternal == null) ? mValue : mDefaultValueInternal; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                if (GetPropertyTypeFromValue(value) == PropertyType.Other)
                    throw new ArgumentException("The type of the value is not supported for a document property value.");

                mValue = value;
                mDefaultValueInternal = null;
            }
        }

        /// <summary>
        /// Gets the data type of the property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public PropertyType Type
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return GetPropertyTypeFromValue(ValueInternal); }
        }

        /// <summary>
        /// Gets the source of a linked custom document property.
        /// </summary>
        public string LinkSource
        {
            get { return mLinkTarget; }
        }

        /// <summary>
        /// Shows whether this property is linked to content or not.
        /// </summary>
        public bool IsLinkToContent
        {
            get
            {
                return (StringUtil.HasChars(mLinkTarget));
            }
        }

        /// <summary>
        /// Specifies the name of a bookmark in the current document to which the value
        /// of this custom document property is linked.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// </remarks>
        internal string LinkTarget
        {
            get { return mLinkTarget; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mLinkTarget = value;
            }
        }

        /// <summary>
        /// Returns the property value as a string formatted according to the current locale.
        /// </summary>
        /// <remarks>
        /// <p>Converts a boolean property into "Y" or "N".
        /// Converts a date property into a short date string.
        /// For all other types converts a property using Object.ToString().</p>
        /// </remarks>
        public override string ToString()
        {
            switch (Type)
            {
                case PropertyType.Boolean:
                    // This is what MS Word returns;
                    return ((bool)ValueInternal) ? "Y" : "N";
                case PropertyType.DateTime:
                    // Return only the date part since no time part is stored in a date time property.
                    return ((DateTime)ValueInternal).ToShortDateString();
                case PropertyType.Double:
                    // TestDocumentInformation.testJira12287Original()
                    // .Net formats '1.0' as '1'. Let's Java does the same.
                    return FormatterPal.DoubleToStr((double) ValueInternal);
                case PropertyType.String:
                case PropertyType.Number:
                default:
                    return ValueInternal.ToString();
            }
        }

        /// <summary>
        /// Returns the property value as integer.
        /// </summary>
        /// <remarks>
        /// Throws an exception if the property type is not <see cref="PropertyType.Number"/>.
        /// </remarks>
        public int ToInt()
        {
            return (int)ValueInternal;
        }

        /// <summary>
        /// Returns the property value as double.
        /// </summary>
        /// <remarks>
        /// Throws an exception if the property type is not <see cref="PropertyType.Number"/>.
        /// </remarks>
        public double ToDouble()
        {
            return (double)ValueInternal;
        }

        /// <summary>
        /// Returns the property value as <b>DateTime</b> in UTC.
        /// </summary>
        /// <remarks>
        /// <p>Throws an exception if the property type is not <see cref="PropertyType.DateTime"/>.</p>
        /// <p>Microsoft Word stores only the date part (no time) for custom date properties.</p>
        /// </remarks>
        public DateTime ToDateTime()
        {
            return (DateTime)ValueInternal;
        }

        /// <summary>
        /// Returns the property value as bool.
        /// </summary>
        /// <remarks>
        /// <p>Throws an exception if the property type is not <see cref="PropertyType.Boolean"/>.</p>
        /// </remarks>
        public bool ToBool()
        {
            return (bool)ValueInternal;
        }

        /// <summary>
        /// Returns the property value as byte array.
        /// </summary>
        /// <remarks>
        /// <p>Throws an exception if the property type is not <see cref="PropertyType.ByteArray"/>.</p>
        /// </remarks>
        public byte[] ToByteArray()
        {
            return (byte[])ValueInternal;
        }

        /// <summary>
        /// These are typed methods for setting the value. In our code in AW we use these methods instead of settings the object Value
        /// property. This allows proper autoporting to Java and ensures .NET DateTime is stored instead of Java's Date.
        /// </summary>
        internal void FromString(string value)
        {
            ValueInternal = value;
        }

        internal void FromInt(int value)
        {
            ValueInternal = value;
        }

        internal void FromDouble(double value)
        {
            ValueInternal = value;
        }

        internal void FromDateTime(DateTime value)
        {
            ValueInternal = value;
        }

        internal void FromBool(bool value)
        {
            ValueInternal = value;
        }

        internal void FromByteArray(byte[] data)
        {
            ValueInternal = data;
        }

        /// <summary>
        /// Makes a copy of the object.
        /// </summary>
        internal DocumentProperty Clone()
        {
            // Memberwise copy is enough because all member variables are immutable.
            return (DocumentProperty)this.MemberwiseClone();
        }

        internal static PropertyType GetPropertyTypeFromValue(object value)
        {
            if (value is string)
                return PropertyType.String;
            else if (value is bool)
                return PropertyType.Boolean;
            else if (value is DateTime)
                return PropertyType.DateTime;
            else if ((value is int) || (value is uint))
                return PropertyType.Number;
            else if (value is double)
                return PropertyType.Double;
            else if (value is string[])
                return PropertyType.StringArray;
            else if (value is object[])
                return PropertyType.ObjectArray;
            else if (value is byte[])
                return PropertyType.ByteArray;
            else
                return PropertyType.Other;
        }

        internal static object GetDefaultValueForPropertyType(PropertyType propType)
        {
            switch (propType)
            {
                case PropertyType.String:
                    return String.Empty;
                case PropertyType.Number:
                    return 0;
                case PropertyType.Double:
                    return 0d;
                case PropertyType.Boolean:
                    return false;
                case PropertyType.DateTime:
                    return DateTime.MinValue;
                case PropertyType.StringArray:
                    return ArrayUtil.EmptyStringArray;
                case PropertyType.ObjectArray:
                    return ArrayUtil.EmptyObjectArray;
                case PropertyType.ByteArray:
                    return ArrayUtil.EmptyByteArray;
                case PropertyType.Other:
                    return null;
                default:
                    throw new InvalidOperationException("Unknown property type.");
            }
        }

        private readonly string mName;
        private object mValue;
        private string mLinkTarget = "";
        private object mDefaultValueInternal = null;

        /// <summary>
        /// 0-1 IDs are reserved by structured storage.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MinUserPropId = 0x00000002;
    }
}
