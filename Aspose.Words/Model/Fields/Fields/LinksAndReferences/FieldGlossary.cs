// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the GLOSSARY field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts a glossary entry.
    /// </remarks>
    public class FieldGlossary : Field, IFieldAutoTextCode
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return FieldAutoTextUpdater.Update(this);
        }

        /// <summary>
        /// Gets or sets the name of the glossary entry to insert.
        /// </summary>
        public string EntryName
        {
            get { return FieldCodeCache.GetArgumentAsString(EntryNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(EntryNameArgumentIndex, value); }
        }

        [CppSkipEntity("C++ doesn't support interface properties and properties with the same names")]
        string IFieldAutoTextCode.EntryName
        {
            get { return EntryName; }
        }

        private const int EntryNameArgumentIndex = 0;
    }
}
