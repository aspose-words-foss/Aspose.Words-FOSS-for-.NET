// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2005 by Dmitry Vorobyev
using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Collections;
using Aspose.Words.Nrx;

namespace Aspose.Words.Properties
{
    /// <summary>
    /// A collection of built-in document properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-document-properties/">Work with Document Properties</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Provides access to <see cref="DocumentProperty"/> objects by their names (using an indexer) and
    /// via a set of typed properties that return values of appropriate types.</para>
    /// </remarks>
    /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentProperties.Common"]/*'/>
    /// <seealso cref="Document"/>
    /// <seealso cref="Document.BuiltInDocumentProperties"/>
    /// <seealso cref="Document.CustomDocumentProperties"/>
    public class BuiltInDocumentProperties : DocumentPropertyCollection
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal BuiltInDocumentProperties()
        {
        }

        /// <overloads>Returns a <see cref="DocumentProperty"/> object.</overloads>
        /// <summary>
        /// Returns a <see cref="DocumentProperty"/> object by the name of the property.
        /// </summary>
        /// <remarks>
        /// <para>The string names of the properties correspond to the names of the typed
        /// properties available from <see cref="BuiltInDocumentProperties"/>.</para>
        ///
        /// <para>If you request a property that is not present in the document, but the name
        /// of the property is recognized as a valid built-in name, a new <see cref="DocumentProperty"/>
        /// is created, added to the collection and returned. The newly created property is assigned
        /// a default value (empty string, zero, <c>false</c> or DateTime.MinValue depending on the type
        /// of the built-in property).</para>
        ///
        /// <para>If you request a property that is not present in the document and the name
        /// is not recognized as a built-in name, a <c>null</c> is returned.</para>
        /// </remarks>
        /// <param name="name">The case-insensitive name of the property to retrieve.</param>
        public override DocumentProperty this[string name]
        {
            get
            {
                ArgumentUtil.CheckHasChars(name, "name");

                string newName = gOldNameToNewName[name];

                // Use actual (new) name to access or create property. This is required for example if old property name is requested and property does not exist.
                // In this case if use requested name for creating property - it will not be created because gNameToPropertyType is mapped to new names.
                string actualName = (newName == null) ? name : newName;

                DocumentProperty prop = base[actualName];

                if (prop == null)
                {
                    // A built in property was not found. This could be okay because it could really be missing
                    // from the document, in this case create it on demand, but it must be a known name.
                    // However, if an unknown built in property was requested, we will return null.
                    int propType = gNameToPropertyType[actualName];
                    if (!StringToIntDictionary.IsNullSubstitute(propType))
                    {
                        object defaultValue = DocumentProperty.GetDefaultValueForPropertyType((PropertyType)propType);
                        prop = Add(actualName, defaultValue);
                    }
                }

                return prop;
            }
        }

        /// <summary>
        /// Gets or sets the name of the document's author.
        /// </summary>
        public string Author
        {
            get { return this[PropertyName.Author].ToString(); }
            set { this[PropertyName.Author].FromString(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of bytes in the document.
        /// </summary>
        /// <remarks>
        /// <para>Microsoft Word does not always set this property.</para>
        ///
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public int Bytes
        {
            get { return this[PropertyName.Bytes].ToInt(); }
            set { this[PropertyName.Bytes].FromInt(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of characters in the document.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int Characters
        {
            get { return this[PropertyName.Characters].ToInt(); }
            set { this[PropertyName.Characters].FromInt(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of characters (including spaces) in the document.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int CharactersWithSpaces
        {
            get { return this[PropertyName.CharactersWithSpaces].ToInt(); }
            set { this[PropertyName.CharactersWithSpaces].FromInt(value); }
        }

        /// <summary>
        /// Gets or sets the document comments.
        /// </summary>
        public string Comments
        {
            get { return this[PropertyName.Comments].ToString(); }
            set { this[PropertyName.Comments].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the category of the document.
        /// </summary>
        public string Category
        {
            get { return this[PropertyName.Category].ToString(); }
            set { this[PropertyName.Category].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the company property.
        /// </summary>
        public string Company
        {
            get { return this[PropertyName.Company].ToString(); }
            set { this[PropertyName.Company].FromString(value); }
        }

        /// <summary>
        /// Gets or sets date of the document creation in UTC.
        /// </summary>
        /// <remarks>
        /// <para>For documents originated from RTF format this property returns local time of the author's machine at the moment of document creation.</para>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public DateTime CreatedTime
        {
            get { return this[PropertyName.CreateTime].ToDateTime(); }
            set { this[PropertyName.CreateTime].FromDateTime(value); }
        }

        /// <summary>
        /// Specifies the base string used for evaluating relative hyperlinks in this document.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not use this property.</para>
        /// </remarks>
        public string HyperlinkBase
        {
            get { return this[PropertyName.HyperlinkBase].ToString(); }
            set { this[PropertyName.HyperlinkBase].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the document keywords.
        /// </summary>
        public string Keywords
        {
            get { return this[PropertyName.Keywords].ToString(); }
            set { this[PropertyName.Keywords].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the date when the document was last printed in UTC.
        /// </summary>
        /// <remarks>
        /// <para>For documents originated from RTF format this property returns the local time of last print operation.</para>
        /// <para>If the document was never printed, this property will return DateTime.MinValue.</para>
        ///
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public DateTime LastPrinted
        {
            get { return this[PropertyName.LastPrinted].ToDateTime(); }
            set { this[PropertyName.LastPrinted].FromDateTime(value); }
        }

        /// <summary>
        /// Gets or sets the name of the last author.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public string LastSavedBy
        {
            get { return this[PropertyName.LastSavedBy].ToString(); }
            set { this[PropertyName.LastSavedBy].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the time of the last save in UTC.
        /// </summary>
        /// <remarks>
        /// <para>For documents originated from RTF format this property returns the local time of last save operation.</para>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public DateTime LastSavedTime
        {
            get { return this[PropertyName.LastSavedTime].ToDateTime(); }
            set { this[PropertyName.LastSavedTime].FromDateTime(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of lines in the document.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int Lines
        {
            get { return this[PropertyName.Lines].ToInt(); }
            set { this[PropertyName.Lines].FromInt(value); }
        }

        /// <summary>
        /// Indicates whether hyperlinks in a document are up-to-date.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public bool LinksUpToDate
        {
            get { return this[PropertyName.LinksUpToDate].ToBool(); }
            set { this[PropertyName.LinksUpToDate].FromBool(value); }
        }

        /// <summary>
        /// Indicates whether document thumbnail is cropped or scaled to fit the display.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public bool ScaleCrop
        {
            get { return this[PropertyName.ScaleCrop].ToBool(); }
        }

        /// <summary>
        /// Indicates whether the document is a shared document.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public bool SharedDocument
        {
            get { return this[PropertyName.SharedDoc].ToBool(); }
        }

        /// <summary>
        /// Indicates whether hyperlinks in a document were changed.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public bool HyperlinksChanged
        {
            get { return this[PropertyName.HyperlinksChanged].ToBool(); }
        }

        /// <summary>
        /// Gets or sets the manager property.
        /// </summary>
        public string Manager
        {
            get { return this[PropertyName.Manager].ToString(); }
            set { this[PropertyName.Manager].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string NameOfApplication
        {
            get { return this[PropertyName.NameOfApplication].ToString(); }
            set { this[PropertyName.NameOfApplication].FromString(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of pages in the document.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words FOSS never updates this property.</para>
        /// </remarks>
        public int Pages
        {
            get { return this[PropertyName.Pages].ToInt(); }
            set { this[PropertyName.Pages].FromInt(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of paragraphs in the document.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int Paragraphs
        {
            get { return this[PropertyName.Paragraphs].ToInt(); }
            set { this[PropertyName.Paragraphs].FromInt(value); }
        }

        /// <summary>
        /// Gets or sets the document revision number.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        /// </remarks>
        public int RevisionNumber
        {
            get { return this[PropertyName.RevisionNumber].ToInt(); }
            set { this[PropertyName.RevisionNumber].FromInt(value); }
        }

        /// <summary>
        /// Specifies the security level of a document as a numeric value.
        /// </summary>
        /// <remarks>
        /// <para>Use this property for informational purposes only because Microsoft Word does not always
        /// set this property. This property is available in DOC and OOXML documents only.</para>
        ///
        /// <para>To protect or unprotect a document use the
        /// <see cref="Document.Protect(ProtectionType, string)"/> and <see cref="Document.Unprotect()"/> methods.</para>
        ///
        /// <para>Aspose.Words updates this property to a correct value before saving a document.</para>
        /// </remarks>
        public DocumentSecurity Security
        {
            get { return (DocumentSecurity)this[PropertyName.Security].ToInt(); }
            set { this[PropertyName.Security].FromInt((int)value); }
        }

        /// <summary>
        /// Gets or sets the subject of the document.
        /// </summary>
        public string Subject
        {
            get { return this[PropertyName.Subject].ToString(); }
            set { this[PropertyName.Subject].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the informational name of the document template.
        /// </summary>
        /// <remarks>
        /// <para>In Microsoft Word, this property is for informational purposes only and
        /// usually contains only the file name of the template without the path.</para>
        ///
        /// <para>Empty string means the document is attached to the Normal template.</para>
        ///
        /// <para>To get or set the actual name of the attached template, use the
        /// <see cref="Document.AttachedTemplate"/> property.</para>
        ///
        /// <seealso cref="Document.AttachedTemplate"/>
        /// </remarks>
        public string Template
        {
            get { return this[PropertyName.Template].ToString(); }
            set { this[PropertyName.Template].FromString(value); }
        }

        /// <summary>
        /// <para>Gets or sets the thumbnail of the document.</para>
        /// </summary>
        /// <remarks>
        /// <para>For now this property is used only when a document is being exported to ePub,
        /// it's not read from and written to other document formats.</para>
        ///
        /// <para>Image of arbitrary format can be set to this property, but the format is checked during export.</para>
        ///
        /// <para>Only gif, jpeg and png images can be used for ePub publication.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> Thrown if the image is invalid or its format is unsupported for specific format of document.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public byte[] Thumbnail
        {
            get { return this[PropertyName.Thumbnail].ToByteArray(); }
            set { this[PropertyName.Thumbnail].FromByteArray(value); }
        }

        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        public string Title
        {
            get { return this[PropertyName.Title].ToString(); }
            set { this[PropertyName.Title].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the total editing time in minutes.
        /// </summary>
        public int TotalEditingTime
        {
            get { return this[PropertyName.TotalEditingTime].ToInt(); }
            set { this[PropertyName.TotalEditingTime].FromInt(value); }
        }

        /// <summary>
        /// Gets or sets the content type of the document.
        /// </summary>
        public string ContentType
        {
            get { return this[PropertyName.ContentType].ToString(); }
            set { this[PropertyName.ContentType].FromString(value); }
        }

        /// <summary>
        /// Gets or sets the content status of the document.
        /// </summary>
        public string ContentStatus
        {
            get { return this[PropertyName.ContentStatus].ToString(); }
            set { this[PropertyName.ContentStatus].FromString(value); }
        }

        /// <summary>
        /// Sets the total editing time in minutes to the value if it is in the valid range, otherwise sets it to 0.
        /// </summary>
        internal void SetTotalEditingTimeSafe(int value)
        {
            TotalEditingTime = ((value > 0) && (value < int.MaxValue)) ? value : 0;
        }

        /// <summary>
        /// Represents the version number of the application that created the document.
        /// </summary>
        /// <remarks>
        /// <para>When a document was created by Microsoft Word, then high 16 bit represent
        /// the major version and low 16 bit represent the build number.</para>
        /// </remarks>
        public int Version
        {
            get { return this[PropertyName.Version].ToInt(); }
            set { this[PropertyName.Version].FromInt(value); }
        }

        /// <summary>
        /// Represents an estimate of the number of words in the document.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int Words
        {
            get { return this[PropertyName.Words].ToInt(); }
            set { this[PropertyName.Words].FromInt(value); }
        }

        /// <summary>
        /// Specifies document headings and their names.
        /// </summary>
        /// <remarks>
        /// <para>Every heading pair occupies two elements in this array.</para>
        ///
        /// <para>The first element of the pair is a <see cref="string"/> and specifies the heading name.
        /// The second element of the pair is an <see cref="int"/> and specifies the count of document
        /// parts for this heading in the <see cref="TitlesOfParts"/> property.</para>
        ///
        /// <para>The total sum of counts for all heading pairs in this property must be equal to the
        /// number of elements in the <see cref="TitlesOfParts"/> property.</para>
        ///
        /// <para>Aspose.Words does not update this property.</para>
        ///
        /// <seealso cref="TitlesOfParts"/>
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public object[] HeadingPairs
        {
            get { return (object[])this[PropertyName.HeadingPairs].ValueInternal; }
            set { this[PropertyName.HeadingPairs].ValueInternal = value; }
        }

        /// <summary>
        /// Each string in the array specifies the name of a part in the document.
        /// </summary>
        /// <remarks>
        /// <para>Aspose.Words does not update this property.</para>
        ///
        /// <seealso cref="HeadingPairs"/>
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public string[] TitlesOfParts
        {
            get { return (string[])this[PropertyName.TitlesOfParts].ValueInternal; }
            set { this[PropertyName.TitlesOfParts].ValueInternal = value; }
        }

        /// <summary>
        /// Gets a flag indicating that current document is created in MS Word 2007 or in lower version.
        /// </summary>
        internal bool IsWord2007OrLower
        {
            get
            {
                return ((NameOfApplication == "Microsoft Office Word") || (NameOfApplication == AssemblyConstants.Family)) &&
                    ((MsWordVersionCore)(Version >> 16) <= MsWordVersionCore.Word2007);
            }
        }

        /// <summary>
        /// RK MS Word 2007 seems to trim spaces for string properties when saving to most
        /// of the formats (it looks like does not trim in DOC only).
        /// To make all gold ExportImport tests work (and to look more like MS Word) we trim spaces too.
        /// </summary>
        internal void TrimSpaces()
        {
            foreach (DocumentProperty prop in this)
            {
                switch (prop.Type)
                {
                    case PropertyType.String:
                    {
                        prop.FromString(prop.ToString().Trim());
                        break;
                    }
                    case PropertyType.StringArray:
                    {
                        string[] values = (string[])prop.ValueInternal;
                        for (int i = 0; i < values.Length; i++)
                            values[i] = MakeEmptyStringIfAllSpaces(values[i]);
                        break;
                    }
                    case PropertyType.ObjectArray:
                    {
                        object[] values = (object[])prop.ValueInternal;
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i] is string)
                                values[i] = MakeEmptyStringIfAllSpaces((string)values[i]);
                        }
                        break;
                    }
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        /// <summary>
        /// Internal boolean property value setter.
        /// </summary>
        internal void SetProperty(string propertyName, bool value)
        {
            this[propertyName].FromBool(value);
        }

        private static string MakeEmptyStringIfAllSpaces(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != ' ')
                    return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static BuiltInDocumentProperties()
        {
            gOldNameToNewName = new StringToStringDictionary(false);
            gOldNameToNewName.Add("Last Author", PropertyName.LastSavedBy);
            gOldNameToNewName.Add("Revision Number", PropertyName.RevisionNumber);
            gOldNameToNewName.Add("Total Editing Time", PropertyName.TotalEditingTime);
            gOldNameToNewName.Add("Last Print Date", PropertyName.LastPrinted);
            gOldNameToNewName.Add("Creation Date", PropertyName.CreateTime);
            gOldNameToNewName.Add("Last Save Time", PropertyName.LastSavedTime);
            gOldNameToNewName.Add("Number of Pages", PropertyName.Pages);
            gOldNameToNewName.Add("Number of Words", PropertyName.Words);
            gOldNameToNewName.Add("Number of Characters", PropertyName.Characters);
            gOldNameToNewName.Add("Application Name", PropertyName.NameOfApplication);
            gOldNameToNewName.Add("Number of Bytes", PropertyName.Bytes);
            gOldNameToNewName.Add("Number of Lines", PropertyName.Lines);
            gOldNameToNewName.Add("Number of Paragraphs", PropertyName.Paragraphs);

            gNameToPropertyType = new StringToIntDictionary(false);
            gNameToPropertyType.Add(PropertyName.Title, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Subject, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Author, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Keywords, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Comments, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Template, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.LastSavedBy, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.RevisionNumber, (int)PropertyType.Number);    // This is string in OLE storage, but int in the model.
            gNameToPropertyType.Add(PropertyName.TotalEditingTime, (int)PropertyType.Number);  // This is date time in OLE storage, but int minutes in the model.
            gNameToPropertyType.Add(PropertyName.LastPrinted, (int)PropertyType.DateTime);
            gNameToPropertyType.Add(PropertyName.CreateTime, (int)PropertyType.DateTime);
            gNameToPropertyType.Add(PropertyName.LastSavedTime, (int)PropertyType.DateTime);
            gNameToPropertyType.Add(PropertyName.Pages, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.Words, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.Characters, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.Security, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.NameOfApplication, (int)PropertyType.String);

            gNameToPropertyType.Add(PropertyName.Category, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Bytes, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.Lines, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.Paragraphs, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.HeadingPairs, (int)PropertyType.ObjectArray);
            gNameToPropertyType.Add(PropertyName.TitlesOfParts, (int)PropertyType.StringArray);
            gNameToPropertyType.Add(PropertyName.Manager, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Company, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.LinksUpToDate, (int)PropertyType.Boolean);
            gNameToPropertyType.Add(PropertyName.ScaleCrop, (int)PropertyType.Boolean);
            gNameToPropertyType.Add(PropertyName.SharedDoc, (int)PropertyType.Boolean);
            gNameToPropertyType.Add(PropertyName.HyperlinksChanged, (int)PropertyType.Boolean);
            gNameToPropertyType.Add(PropertyName.CharactersWithSpaces, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.HyperlinkBase, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Version, (int)PropertyType.Number);
            gNameToPropertyType.Add(PropertyName.ContentStatus, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.ContentType, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.DocVersion, (int)PropertyType.String);
            gNameToPropertyType.Add(PropertyName.Language, (int)PropertyType.String);

            gNameToPropertyType.Add(PropertyName.Thumbnail, (int)PropertyType.ByteArray);
        }

        internal override DocumentPropertyCollection Create()
        {
            return new BuiltInDocumentProperties();
        }

        // Pending hyperlinks. See _PID_HLINKS property.
        internal Hlinks Hlinks;

        /// <summary>
        /// Stores pairs of "old" property names (used by Aspose.Words early versions) and "new" property names (used by MS Word).
        /// Case-insensitive.
        /// </summary>
        private static readonly StringToStringDictionary gOldNameToNewName;
        /// <summary>
        /// Stores names of all known built in properties and their data types.
        /// Case-insensitive.
        /// </summary>
        private static readonly StringToIntDictionary gNameToPropertyType;
    }
}
