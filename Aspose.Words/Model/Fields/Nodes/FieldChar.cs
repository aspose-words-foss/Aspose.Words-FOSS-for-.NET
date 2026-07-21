// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2006 by Roman Korchagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Base class for nodes that represent field characters in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Field.Common"]/*'/>
    /// </remarks>
    /// <seealso cref="FieldStart"/>
    /// <seealso cref="FieldSeparator"/>
    /// <seealso cref="FieldEnd"/>
    public abstract class FieldChar : SpecialChar
    {
        internal FieldChar(DocumentBase doc, char fieldChar, RunPr runPr, FieldType type) :
            base(doc, fieldChar, runPr)
        {
            FieldType = type;
        }

        /// <summary>
        /// Returns a field for the field char.
        /// </summary>
        /// <remarks>
        /// A new <see cref="Field"/> object is created each time the method is called.
        /// </remarks>
        /// <returns>A field for the field char.</returns>
        public Field GetField()
        {
            return FieldBundle
                .GetFieldBundle(this)
                .GetField();
        }

        /// <summary>
        /// Returns the type of the field.
        /// </summary>
        public FieldType FieldType
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
            // Sets field type for this field char node. Used during WordML import for late assignment of field type.
            internal set;
        }

        /// <summary>
        /// Gets or sets whether the parent field is locked (should not recalculate its result).
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets whether the current result of the field is no longer correct (stale) due to other modifications
        /// made to the document.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// If true field result is not intended to be visible to the user.
        /// </summary>
        internal bool IsPrivate { get; set; }

#if DEBUG
        public override string ToString()
        {
            return string.Format(
                "{0} {1}{2}{3}",
                base.ToString(),
                FieldType,
                IsLocked ? " [locked]" : "",
                IsDirty ? " [dirty]" : "");
        }
#endif
    }
}
