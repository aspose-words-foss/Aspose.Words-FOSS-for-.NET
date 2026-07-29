// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2008 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Extracts fields from a given node or collection of nodes into a <see cref="List{Field}"/>.
    /// </summary>
    internal class FieldExtractorToCollection : FieldExtractor
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldExtractorToCollection() : this(false)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="isDeep">True to extract nested fields, false to extract top level fields only.</param>
        internal FieldExtractorToCollection(bool isDeep) : this(isDeep, null)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="isDeep">True to extract nested fields, false to extract top level fields only.</param>
        /// <param name="fieldTypes">Types of fields to extract.</param>
        internal FieldExtractorToCollection(bool isDeep, params FieldType[] fieldTypes) : base(fieldTypes)
        {
            Fields = new List<Field>();

            mIsDeep = isDeep;
        }

        /// <summary>
        /// Access to the result of the extraction.
        /// Note that the outer fields will appear in the collection after the nested fields.
        /// </summary>
        internal List<Field> Fields { get; }

        protected override void OnFieldExtracted()
        {
            if (mIsDeep || !IsInField)
                Fields.Add(CurrentField);
        }

        private readonly bool mIsDeep;
    }
}
