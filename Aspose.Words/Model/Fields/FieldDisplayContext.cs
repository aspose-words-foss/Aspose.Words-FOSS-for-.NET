// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Michael Morozoff

using Aspose.JavaAttributes;
using Aspose.Words.Fields.Expressions;
using Aspose.Words.Notes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Display context is passed to the document so it can compute field display value.
    /// </summary>
    internal abstract class FieldDisplayContext
    {
        /// <summary>
        /// Seeks for the first paragraph satisfying the given <see cref="FieldParagraphFinder"/> scanning pages backward
        /// from the corresponding field's page to the first page. Returns <c>null</c> if nothing is found.
        /// </summary>
        [JavaThrows(true)]
        internal abstract Paragraph FindParagraph(FieldStart fieldStart, FieldParagraphFinder finder);

        /// <summary>
        /// Returns field value computed by layout.
        /// </summary>
        [JavaThrows(true)]
        protected abstract int GetFieldValueCore(Field field);

        /// <summary>
        /// Returns a value for the field if provided.
        /// </summary>
        internal Constant GetFieldValue(Field field)
        {
            int value = GetFieldValueCore(field);
            return (value != -1) ? new Int32Constant(value) : null;
        }

        /// <summary>
        /// Returns a section that contains the given field start node.
        /// </summary>
        /// <remarks>
        /// For a field inside header/footer, this section may be different from the parent section in a document model.
        /// This may happen because header/footer may be inherited from the previous sections.
        /// </remarks>
        internal Section GetSection(FieldStart fieldStart)
        {
            int sectionIndex = GetSectionIndex(fieldStart);
            return (sectionIndex >= 0) ? fieldStart.FetchDocument().Sections[sectionIndex] : null;
        }

        /// <summary>
        /// Gets a zero-based index of the section that contains the given field start node.
        /// Returns a negative index if the section is not found.
        /// </summary>
        /// <remarks>
        /// For a field inside header/footer, this section index may be different from the parent section in a document model.
        /// This may happen because header/footer may be inherited from the previous sections.
        /// </remarks>
        [JavaThrows(true)]
        protected abstract int GetSectionIndex(FieldStart fieldStart);

        /// <summary>
        /// Requests layout to update the specified field. Returns <c>true</c> if the field was updated by layout.
        /// Returns <c>false</c> if the field's update was rejected by layout and hence the field should be updated
        /// in a common way.
        /// </summary>
        [JavaThrows(true)]
        internal abstract bool UpdateField(Field field, bool isTopmost);

        /// <summary>
        /// Gets a fields updating session associated with this instance.
        /// </summary>
        internal abstract FieldUpdateSession FieldUpdateSession { get; }
    }
}
