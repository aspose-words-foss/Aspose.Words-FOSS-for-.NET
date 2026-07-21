// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2005 by Roman Korchagin

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Stores and manages formatting of bulleted and numbered lists used in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-lists/">Working with Lists</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A list in a Microsoft Word document is a set of list formatting properties.
    /// The formatting of the lists is stored in the <see cref="ListCollection"/> collection separately
    /// from the paragraphs of text.</p>
    ///
    /// <p>You do not create objects of this class. There is always only one <see cref="ListCollection"/>
    /// object per document and it is accessible via the <see cref="Aspose.Words.DocumentBase.Lists"/> property.</p>
    ///
    /// <p>To create a new list based on a predefined list template or based on a list style,
    /// use the <see cref="Add(Style)"/> method.</p>
    ///
    /// <p>To create a new list with formatting identical to an existing list,
    /// use the <see cref="AddCopy(List)"/> method.</p>
    ///
    /// <p>To make a paragraph bulleted or numbered, you need to apply list formatting
    /// to a paragraph by assigning a <see cref="List"/> object to the
    /// <see cref="ListFormat.List"/> property of <see cref="ListFormat"/>.</p>
    ///
    /// <p>To remove list formatting from a paragraph, use the <see cref="ListFormat.RemoveNumbers"/>
    /// method.</p>
    ///
    /// <p>If you know a bit about WordprocessingML, then you might know it defines separate concepts
    /// for "list" and "list definition". This exactly corresponds to how list formatting is stored
    /// in a Microsoft Word document at the low level. List definition is like a "schema" and
    /// list is like an instance of a list definition.</p>
    ///
    /// <p>To simplify programming model, Aspose.Words hides the distinction between list and list
    /// definition in much the same way like Microsoft Word hides this in its user interface.
    /// This allows you to concentrate more on how you want your document to look like, rather than
    /// building low-level objects to satisfy requirements of the Microsoft Word file format.</p>
    ///
    /// <p>It is not possible to delete lists once they are created in the current version of Aspose.Words.
    /// This is similar to Microsoft Word where user does not have explicit control over list definitions.</p>
    ///
    /// <seealso cref="List"/>
    /// <seealso cref="ListLevel"/>
    /// <seealso cref="ListFormat"/>
    /// </remarks>
    public class ListCollection : IEnumerable<List>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal ListCollection(DocumentBase document)
        {
            Debug.Assert(document != null);
            mDocument = document;
        }

        /// <summary>
        /// Gets the enumerator object that will enumerate lists in the document.
        /// </summary>
        public IEnumerator<List> GetEnumerator()
        {
            return mLists.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <overloads>Creates a new list and adds it to the collection of lists in the document.</overloads>
        /// <summary>
        /// Creates a new list based on a predefined template and adds it to the collection of lists in the document.
        /// </summary>
        /// <param name="listTemplate">The template of the list.</param>
        /// <returns>The newly created list.</returns>
        /// <remarks>
        /// <p>Aspose.Words list templates correspond to the 21 list templates available
        /// in the Bullets and Numbering dialog box in Microsoft Word 2003.</p>
        ///
        /// <p>All lists created using this method have 9 list levels.</p>
        /// </remarks>
        public List Add(ListTemplate listTemplate)
        {
            return ListFactory.CreateList(this, listTemplate);
        }

        /// <summary>
        /// Creates a new single level list based on the predefined template and adds it to the list collection in the document.
        /// </summary>
        public List AddSingleLevelList(ListTemplate listTemplate)
        {
            return ListFactory.CreateSingleLevelList(this, listTemplate);
        }

        /// <summary>
        /// Creates a new list that references a list style and adds it to the collection of lists in the document.
        /// </summary>
        /// <param name="listStyle">The list style.</param>
        /// <returns>The newly created list.</returns>
        /// <remarks>
        /// <p>The newly created list references the list style. If you change the properties of the list
        /// style, it is reflected in the properties of the list. Vice versa, if you change the properties
        /// of the list, it is reflected in the properties of the list style.</p>
        /// </remarks>
        public List Add(Style listStyle)
        {
            if (listStyle == null)
                throw new ArgumentNullException("listStyle");
            if (listStyle.Document != Document)
                throw new ArgumentException("The list style belongs to a different document.");
            if (listStyle.Type != StyleType.List)
                throw new ArgumentException("The style is not a list style.");

            // This creates a copy of a list style that does not reference the original list style.
            List newList = AddCopy(listStyle.List);

            // By restoring the reference to the list style we now have got a list definition
            // that reference the list style.
            newList.ListDef.ListStyleIstd = listStyle.Istd;

            return newList;
        }

        /// <summary>
        /// Creates a new list by copying the specified list and adding it to the collection of lists in the document.
        /// </summary>
        /// <param name="srcList">The source list to copy from.</param>
        /// <returns>The newly created list.</returns>
        /// <remarks>
        /// <p>The source list can be from any document. If the source list belongs to a different document,
        /// a copy of the list is created and added to the current document.</p>
        ///
        /// <p>If the source list is a reference to or a definition of a list style,
        /// the newly created list is not related to the original list style.</p>
        /// </remarks>
        public List AddCopy(List srcList)
        {
            return AddCopy(srcList, true);
        }

        /// <summary>
        /// Creates a new list by copying the specified list and adding it to the collection of lists in the document.
        /// </summary>
        /// <remarks>
        /// When AW copies only style from other document, AW shouldn't import all dependent styles.
        /// </remarks>
        [JavaThrows(false)]
        internal List AddCopy(List srcList, bool isImport)
        {
            if (srcList == null)
                throw new ArgumentNullException("srcList");

            List newList;

            bool isSameDocument = (Document == srcList.Document);
            if (isSameDocument || !isImport)
            {
                // WORDSNET-14595 If we copy lists between different documents,
                // then first try to use existing list with the same 'ListDefId'.
                // These ids are random generated so chances they match
                // for different styles are near zero.
                // Additional information can be found in similar WORDSNET-4174
                if (!isSameDocument)
                {
                    newList = FindListWithEqualDefIdAndOverrides(srcList);
                    if (newList != null)
                        return newList;
                }

                // Clone and add the list.
                newList = srcList.Clone(mDocument, GetNextAvailableListId());
                AddList(newList);

                // Clone and add the list definition.
                ListDef newListDef = srcList.ListDef.Clone(Document, GetNextAvailableListDefId());
                AddListDef(newListDef);
                newList.ListDefId = newListDef.ListDefId;
            }
            else
            {
                // When the list is from a different document, import it because it might reference paragraph styles.
                ImportContext context = new ImportContext(srcList.Document, Document, ImportFormatMode.UseDestinationStyles);

                // Avoid reusing the list definition of this list
                // because we want to add copy in this method by design.
                context.SetNonReusableListDefId(srcList.ListDefId);

                newList = FetchListByListId(ImportList(context, srcList.ListId, false));
            }

            // If the list was a definition of a list style or a reference to a list style,
            // break it off because this function creates a standalone copy of a list.
            // It also means there is no hassle when importing the list style from a different document.
            newList.ListDef.ListStyleIstd = StyleIndex.Nil;

            return newList;
        }

        /// <summary>
        /// Gets a list by a list identifier.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>Returns the list object. Returns <c>null</c> if a list with the specified identifier was not found.</returns>
        /// <remarks>
        /// <p>You don't normally need to use this method. Most of the time you apply list formatting
        /// to paragraphs just by settings the <see cref="ListFormat.List"/> property
        /// of the <see cref="ListFormat"/> object.</p>
        /// </remarks>
        public List GetListByListId(int listId)
        {
            int translatedListId = mListIdTranslationTable[listId];

            if (IntToIntDictionary.IsNullSubstitute(translatedListId))
                return null;

            int listIndex = translatedListId - 1;

            // To the user, this function is to return null if the list with this list id is not found.
            // It does not matter that at the moment list id is actually an index, it might change later.
            if ((listIndex < 0) || (listIndex >= Count))
                return null;
            else
                return this[listIndex];
        }

        /// <summary>
        /// Gets a list by its unique identifier safely.
        /// In some corrupted documents I get list identifiers (ilfos) that refers to non-existent list,
        /// in this case we create a new list with default properties.
        /// </summary>
        internal List FetchListByListId(int listId)
        {
            // If there are no lists and the user requests one, we have to create one.
            if (Count == 0)
                AddEmpty(ListType.HybridMultiLevel);

            int translatedListId = mListIdTranslationTable[listId];

            int listIndex = IntToIntDictionary.IsNullSubstitute(translatedListId) ? listId - 1 : translatedListId - 1;

            // If the list index is out of the valid range, bring it back into the range.
            // This could make the paragraph to refer to a different list, but its acceptable.
            if (listIndex < 0)
                listIndex = 0;
            else if (listIndex >= Count)
                listIndex = Count - 1;

            return this[listIndex];
        }

        /// <summary>
        /// Gets a list by its unique identifier safely.
        ///
        /// If the list is a list style reference, resolves it into the corresponding list style definition.
        /// The resolution is needed to properly calculate full formatting attributes.
        ///
        /// The thing is that while DOC stores a copy of all attributes in the list style reference,
        /// DOCX and WordML do not store anything in the list style reference.
        /// </summary>
        internal List FetchListByListIdResolveStyleReference(int listId)
        {
            List list = FetchListByListId(listId);
            if (list.IsListStyleReference)
            {
                // The list is a list style reference, return the list style definition.
                return list.Style.List;
            }
            else
            {
                // Most likely, the list is not a list style reference, therefore return it.
                return list;
            }
        }

        /// <summary>
        /// Creates a new list and a list definition and returns the list.
        /// The list definition has 9 levels, but formatting for the levels is empty.
        /// </summary>
        internal List AddEmpty(ListType listType)
        {
            ListDef listDef = AddEmptyListDef(listType, GetNextAvailableListDefId());

            return AddList(listDef);
        }

        /// <summary>
        /// Creates a new list with provided list definition.
        /// </summary>
        private List AddList(ListDef listDef)
        {
            List list = new List(mDocument, GetNextAvailableListId());
            list.ListDefId = listDef.ListDefId;
            AddList(list);

            return list;
        }

        /// <summary>
        /// Creates a new list definition.
        /// The list definition has 9 levels, but formatting for the levels is empty.
        /// </summary>
        private ListDef AddEmptyListDef(ListType listType, int listDefId)
        {
            // Template code seems to be just random. I think it is enough to make it same as listDefId.
            int templateCode = listDefId;
            ListDef listDef = new ListDef(mDocument, listDefId, listType, templateCode);
            AddListDef(listDef);
            return listDef;
        }

        /// <summary>
        /// Adds a list to the collection. The list's owner document must be this document.
        /// </summary>
        internal void AddList(List list)
        {
            Debug.Assert((list != null) && (list.Document == this.Document));

            if (!mListIdTranslationTable.ContainsKey(list.ListId))
            {
                mLists.Add(list);
                mListIdTranslationTable.Add(list.ListId, mLists.Count);
            }
            else
            {
                // WARN. Non-unique ListId.
            }
        }

        /// <summary>
        /// Adds a list definition to the collection. The list definition's owner document must be this document.
        /// </summary>
        internal void AddListDef(ListDef listDef)
        {
            Debug.Assert((listDef != null) && (listDef.Document == this.Document));
            mListDefs.Add(listDef);
        }

        /// <summary>
        /// Gets a list definition by a zero based index.
        /// </summary>
        internal ListDef GetListDefByIndex(int index)
        {
            return mListDefs[index];
        }

        /// <summary>
        /// Gets a list definition by a listDefId. Returns null if not found.
        /// </summary>
        internal ListDef GetListDefByListDefId(int listDefId)
        {
            int defIdIdx = GetIndexOfListDefByListDefId(listDefId);
            return (defIdIdx != -1) ? mListDefs[defIdIdx] : null;
        }

        /// <summary>
        /// Safely gets a list definition by a list definition id.
        ///
        /// SLOW At the moment linear search, don't use too often.
        /// </summary>
        internal ListDef FetchListDefByListDefId(int listDefId)
        {
            int i = GetIndexOfListDefByListDefId(listDefId);

            if (i >= 0)
            {
                return GetListDefByIndex(i);
            }
            else
            {
                // RK A non existent list definition was requested. MS Word seems to default to the first
                // list definition and so do we (but only if it does exist). Otherwise create and return
                // an empty ListDef.
                if (ListDefCount > 0)
                    return GetListDefByIndex(0);
                else
                    return AddEmptyListDef(ListType.HybridMultiLevel, listDefId);
            }
        }

        /// <summary>
        /// Gets the index of a list definition by list definition id or negative value if not found.
        ///
        /// SLOW At the moment linear search, don't use too often.
        /// </summary>
        internal int GetIndexOfListDefByListDefId(int listDefId)
        {
            for (int i = 0; i < ListDefCount; i++)
            {
                if (GetListDefByIndex(i).ListDefId == listDefId)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Creates a deep copy of all lists. The new lists will have same ids as the originals.
        /// Suitable only when copying a complete document.
        /// </summary>
        internal ListCollection Clone(DocumentBase dstDocument, INodeCloningListener nodeCloningListener)
        {
            Debug.Assert((dstDocument != null) && (dstDocument != mDocument));

            ListCollection lhs = (ListCollection)MemberwiseClone();

            lhs.mDocument = dstDocument;

            // Since we are copying all lists, it means all list ids, list def ids, picture bullet ids
            // in the destination document will remain same as in the source document and therefore
            // we simply clone without importing and changes to the ids.

            lhs.mListIdTranslationTable = new IntToIntDictionary();

            // Copy lists.
            lhs.mLists = new List<List>();
            foreach (List list in mLists)
                lhs.AddList(list.Clone(dstDocument, list.ListId));

            // Copy list definitions.
            lhs.mListDefs = new List<ListDef>();
            foreach (ListDef listDef in mListDefs)
                lhs.AddListDef(listDef.Clone(dstDocument, listDef.ListDefId));

            // Copy list bullets.
            lhs.mPictureBullets = new List<Shape>();
            foreach (Shape shape in mPictureBullets)
            {
                // We do not import but clone because cloning a complete document.
                Shape dstShape = (Shape)shape.Clone(true, nodeCloningListener);
                dstShape.SetDocument(dstDocument);
                lhs.AddPictureBullet(dstShape);
            }

            return lhs;
        }

        /// <summary>
        /// Copies the list referenced by the specified list id in the source document into
        /// this document and returns the list id of the newly created list.
        /// Also imports the referenced list style and linked paragraph styles (if any).
        /// </summary>
        internal int ImportList(ImportContext context, int srcListId)
        {
            return ImportList(context, srcListId, true);
        }

        /// <summary>
        /// Copies list referenced by the specified list id in the source collection into this collection.
        /// </summary>
        /// <remarks>
        /// It does not import referenced list style and picture bullets.
        /// But it copies level-linked paragraph styles.
        /// </remarks>
        /// <returns>Imported list id.</returns>
        internal int CopyList(int srcListId, ImportContext context)
        {
            List srcList = context.SrcLists.GetListByListId(srcListId);
            if (srcList == null)
                return 0;

            // We must override list definition with the same id, so remove existing one from the destination collection.
            int dstIdx = GetIndexOfListDefByListDefId(srcList.ListDefId);
            // WORDSNET-17878 Skip removing of the list definition when it was imported previously from the source.
            if ((dstIdx != -1) && !context.ImportedListDefIds.ContainsKey(srcList.ListDefId))
                ListDefs.RemoveAt(dstIdx);

            // ListDefId that equals to some id in source collection was removed from the destination collection above,
            // so there is nothing to reuse. But let's set this id to non-reusable value to speed up import.
            context.SetNonReusableListDefId(srcList.ListDefId);

            bool isImported = ImportListCore(context, srcList);
            List dstList = context.DstLists.GetListByListId(context.ImportedListIds[srcListId]);

            if (!isImported)
                return dstList.ListId;

            // If the list that is being copied is a definition or a reference to a list style, copy the list style.
            if (srcList.Style != null)
                dstList.ListDef.ListStyleIstd = context.DstStyles.CopyStyle(srcList.Style, context);

            // Perform per-level copying tasks.
            ListLevelCollection srcLevels = srcList.ListDef.Levels;
            ListLevelCollection dstLevels = dstList.ListDef.Levels;
            for (int i = 0; i < srcLevels.Count; i++)
            {
                ListLevel srcLevel = srcLevels[i];
                ListLevel dstLevel = dstLevels[i];
                CopyLinkedParagraphStyle(context, srcLevel, dstLevel);
            }

            return dstList.ListId;
        }

        /// <summary>
        ///  Generates unique random ListDefId.
        /// </summary>
        internal int MakeUniqueListDefId()
        {
            int newListDefId = UniqueIdManager.GenerateInteger();

            // Do not generate zero nsids to avoid collisions with non-existent list definitions that might be set for list.
            while ((newListDefId == 0) || (GetListDefByListDefId(newListDefId) != null))
                newListDefId = UniqueIdManager.GenerateInteger();

            return newListDefId;
        }

        /// <summary>
        /// Imports the list referenced by the specified list id in the source document into
        /// this document and returns the list id of the newly created list.
        /// Imports the paragraph styles linked to list levels (if any).
        /// Optionally imports the referenced list style.
        /// </summary>
        private int ImportList(ImportContext context, int srcListId, bool isImportListStyle)
        {
            Debug.Assert(context != null);

            // WORDSNET-1462 System.ArgumentException during list import.
            // Paragraph style references a non-existing list, this was causing import of the last
            // list twice. Fixed to return 0 and therefore imported paragraph style becomes non list.
            List srcList = context.SrcLists.GetListByListId(srcListId);
            if (srcList == null)
                return 0;

            bool isImported = ImportListCore(context, srcList);
            List dstList = context.DstLists.GetListByListId(context.ImportedListIds[srcListId]);

            if (!isImported)
                return dstList.ListId;

            // If the list that is being imported is a definition or a reference to a list style, import the list style.
            if (isImportListStyle && (srcList.Style != null))
            {
                Style dstStyle = context.DstStyles.ImportStyle(context, srcList.Style);

                if (srcList.ListDef.IsListStyleDefinition && !context.IsImported(srcList.Style))
                {
                    // WORDSNET-11757 We must preserve IsListStyleDefinition on import - use dstStyle.List.ListDef.
                    dstList.ListDefId = dstStyle.List.ListDefId;
                    context.ImportedListDefIds[srcList.ListDefId] = dstList.ListDefId;
                    return dstList.ListId;
                }

                dstList.ListDef.ListStyleIstd = dstStyle.Istd;
            }

            // Perform per-level importing tasks.
            // WORDSNET-26460 There is some problem when going through List.GetListLevels because it goes via "terminal list def",
            // which goes into the list style and so on. I don't understand "terminal list def" fully so I think when importing
            // we need to deal with direct data, hence get the list, list def and list levels directly.
            ListLevelCollection srcLevels = srcList.ListDef.Levels;
            ListLevelCollection dstLevels = dstList.ListDef.Levels;
            for (int i = 0; i < srcLevels.Count; i++)
            {
                ListLevel srcLevel = srcLevels[i];
                ListLevel dstLevel = dstLevels[i];
                ImportLinkedParagraphStyle(context, srcLevel, dstLevel);
                ImportPictureBullet(context, srcLevel, dstLevel);
            }

            return dstList.ListId;
        }

        /// <summary>
        /// Core method for importing lists.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if a list and its definition was actually imported. Otherwise, if it was imported earlier or just reused,
        /// it returns <c>false</c>.
        /// </returns>
        private bool ImportListCore(ImportContext context, List srcList)
        {
            Debug.Assert(context != null);
            Debug.Assert(srcList != null);

            // If the list was already imported, just return.
            if (context.ImportedListIds.ContainsKey(srcList.ListId))
                return false;

            List dstList;
            // WORDSNET-4174 MS Word reuses lists with equal DefIds and equal Overrides.
            if (context.CanReuseListDefId(srcList.ListDefId))
            {
                dstList = FindListWithEqualDefIdAndOverrides(srcList);

                // WORDSNET-15788 Mimic MSW and re-use only lists which initially were placed in the destination.
                if ((dstList != null) && !context.ImportedListDefIds.ContainsValue(dstList.ListDefId))
                {
                    context.ImportedListIds.Add(srcList.ListId, dstList.ListId);
                    return false;
                }
            }

            // Clone and add the list object.
            dstList = srcList.Clone(mDocument, GetNextAvailableListId());
            AddList(dstList);
            context.ImportedListIds.Add(srcList.ListId, dstList.ListId);

            // Deal with the list definition.
            ListDef srcListDef = srcList.ListDef;
            // If the list definition was already imported, just return.
            if (context.ImportedListDefIds.ContainsKey(srcListDef.ListDefId))
            {
                dstList.ListDefId = context.ImportedListDefIds[srcListDef.ListDefId];
                dstList.ClearListDefCache();
                return false;
            }

            // First try to reuse existing ListDef with the same Id.
            ListDef dstListDef = GetListDefByListDefId(srcListDef.ListDefId);
            if ((dstListDef != null) && context.CanReuseListDefId(srcListDef.ListDefId))
            {
                context.ImportedListDefIds[srcListDef.ListDefId] = dstListDef.ListDefId;
                return false;
            }

            // Clone and add the list definition object.
            // Generate new ID if same ID already exists in destination collection,
            // otherwise use source ListDefId.
            int dstListDefId = (dstListDef == null) ? srcListDef.ListDefId : MakeUniqueListDefId();

            dstListDef = srcListDef.Clone(mDocument, dstListDefId);
            AddListDef(dstListDef);

            // Add imported list definition id to the cache so it will not be imported again.
            context.ImportedListDefIds[srcListDef.ListDefId] = dstListDefId;

            // Apply imported list definition to the list.
            dstList.ListDefId = dstListDefId;

            return true;
        }

        /// <summary>
        /// Imports a paragraph style if one is linked to the list level.
        /// </summary>
        private static void ImportLinkedParagraphStyle(ImportContext context, ListLevel srcLevel, ListLevel dstLevel)
        {
            Style srcLinkedStyle = srcLevel.LinkedStyle;
            if (srcLinkedStyle != null)
            {
                // WORDSNET-RESILIENCY 3321 - A document seems to have a list linked to a character style.
                // MS Word does not complain and show the style, but I think this is wrong.
                // Added resiliency to avoid importing linked style if it is not a paragraph style.
                Style dstLinkedStyle = srcLinkedStyle.Type == StyleType.Paragraph
                    ? context.DstStyles.ImportStyle(context, srcLinkedStyle)
                    : null;

                dstLevel.LinkedStyle = dstLinkedStyle;
            }
        }

        /// <summary>
        /// Copies linked style from a source to destination list level.
        /// </summary>
        private static void CopyLinkedParagraphStyle(ImportContext context, ListLevel srcLevel, ListLevel dstLevel)
        {
            Style srcLinkedStyle = srcLevel.LinkedStyle;
            if (srcLinkedStyle != null)
            {
                int dstLinkedStyleId = context.DstStyles.CopyStyle(srcLinkedStyle, context);
                dstLevel.LinkedStyle = context.DstStyles.GetByIstd(dstLinkedStyleId, false);
            }
        }

        /// <summary>
        /// Imports a picture bullet if there is one specified for the list level.
        /// </summary>
        private static void ImportPictureBullet(ImportContext context, ListLevel srcLevel, ListLevel dstLevel)
        {
            if (!srcLevel.HasPictureBullet)
                return;

            int dstPictureBulletId = context.ImportedPictureBulletIds[srcLevel.PictureBulletId];
            if (!IntToIntDictionary.IsNullSubstitute(dstPictureBulletId))
            {
                // The picture bullet was already imported, don't import it again.
                dstLevel.PictureBulletId = dstPictureBulletId;
            }
            else
            {
                // The picture bullet was not imported. Import it and add to the document and to the context.
                Shape srcShape = srcLevel.PictureBullet;
                Shape dstShape = (Shape)context.DstDoc.ImportNode(srcShape, true);
                dstLevel.PictureBulletId = context.DstLists.AddPictureBullet(dstShape);
                context.ImportedPictureBulletIds.Add(srcLevel.PictureBulletId, dstLevel.PictureBulletId);
            }
        }

        /// <summary>
        /// List definitions are sorted by ListDefId, so selecting the biggest
        /// and adding one will generate next id.
        /// </summary>
        internal int GetNextAvailableListDefId()
        {
            if (mMaxUsedListDefId == 0)
            {
                foreach (ListDef listDef in mListDefs)
                    mMaxUsedListDefId = System.Math.Max(mMaxUsedListDefId, listDef.ListDefId);
            }
            return ++mMaxUsedListDefId;
        }

        /// <summary>
        /// At the moment ListId is a 1-based index into the lists collection.
        /// </summary>
        internal int GetNextAvailableListId()
        {
            // andrnosk: WORDSNET-8699 To get correct next available ListId,
            // we have to find out current maximum ListId and increase it.
            if (mMaxUsedListId == 0)
            {
                foreach (List list in mLists)
                    mMaxUsedListId = System.Math.Max(mMaxUsedListId, list.ListId);
            }
            return ++mMaxUsedListId;
        }

        /// <summary>
        /// Clears cached list definition with a specified DefId.
        /// </summary>
        internal void ClearCachedListDef(int defId)
        {
            foreach (List list in mLists)
            {
                if (list.ListDefId == defId)
                    list.ClearListDefCache();
            }
        }

        /// <summary>
        /// Returns list with equal to the specified list's <see cref="List.ListDefId"/> and <see cref="List.Overrides"/>.
        /// </summary>
        private List FindListWithEqualDefIdAndOverrides(List refList)
        {
            List foundList = null;

            foreach (List list in mLists)
            {
                if ((list.ListDefId == refList.ListDefId) && list.Overrides.Equals(refList.Overrides))
                {
                    // Prefer a list with the same DurableId to return.
                    if (list.DurableId == refList.DurableId)
                        return list;

                    foundList = list;
                }
            }

            return foundList;
        }

        /// <summary>
        /// Gets the count of numbered and bulleted lists in the document.
        /// </summary>
        public int Count
        {
            get { return mLists.Count; }
        }

        /// <summary>
        /// Gets a list by index.
        /// </summary>
        public List this[int index]
        {
            get { return mLists[index]; }
        }

        /// <summary>
        /// Gets a list by <see cref="ListDef"/> name. Returns <c>null</c> if not found.
        /// </summary>
        /// <remarks>
        /// MS Word does not give an ability to create more than one list based on named <see cref="ListDef"/>
        /// through its GUI and AW does not set <see cref="ListDef.Name"/> anywhere outside of file reading
        /// routines so <see cref="ListDef"/> name can be the unique identifier for a list.
        /// </remarks>
        /// <param name="listDefName"></param>
        /// <returns></returns>
        internal List this[string listDefName]
        {
            get
            {
                if (!StringUtil.HasChars(listDefName))
                    return null;

                // An enumeration through mListDefs will not create unnecessary ListDef instances.
                foreach (ListDef listDef in mListDefs)
                {
                    if (StringUtil.EqualsIgnoreCase(listDef.Name, listDefName))
                        return EnsureListByListDef(listDef);
                }

                return null;
            }
        }

        private List GetListByListDef(ListDef listDef)
        {
            foreach (List list in mLists)
            {
                // A comparison by ListDefId will not cache unnecessary ListDef instances.
                if (list.ListDefId == listDef.ListDefId)
                    return list;
            }

            return null;
        }

        private List EnsureListByListDef(ListDef listDef)
        {
            return GetListByListDef(listDef) ?? AddList(listDef);
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        public DocumentBase Document
        {
            get { return mDocument; }
        }

        internal int ListDefCount
        {
            get { return mListDefs.Count; }
        }

        internal int PictureBulletCount
        {
            get { return mPictureBullets.Count; }
        }

        /// <summary>
        /// This value specifies that all abstract numbering definitions with an abstractNumId value higher
        /// than the NumIdMacAtCleanup value have not yet been reviewed by 3rd party software.
        /// </summary>
        /// <remarks>
        /// This property doesn't affect document view. We just preserves its value during conversion.
        /// </remarks>
        internal object NumIdMacAtCleanup { get; set; }

        /// <summary>
        /// Returns ListId to 1-based index translation table.
        /// </summary>
        /// <remarks>Exposed for validation purposes only.</remarks>
        internal IntToIntDictionary ListIdTranslationTable
        {
            get { return mListIdTranslationTable; }
        }

        /// <summary>
        /// Returns list definition collection.
        /// </summary>
        /// <remarks>Exposed for validation purposes only.</remarks>
        internal List<ListDef> ListDefs
        {
            get { return mListDefs; }
        }

        /// <summary>
        /// Adds a picture bullet shape to the collection of picture bullets available in this document.
        /// </summary>
        internal int AddPictureBullet(Shape shape)
        {
            if (shape == null)
                throw new ArgumentNullException("shape");

            if (shape.ParentNode != null)
                throw new ArgumentException("The picture bullet shape must have no parent node.");

            if (shape.Document != mDocument)
                throw new ArgumentException("The picture bullet shape must belong to this document.");

            shape.IsPictureBullet = true;

            mPictureBullets.Add(shape);
            return mPictureBullets.Count - 1;
        }

        /// <summary>
        /// Gets a picture bullet by index.
        /// </summary>
        internal Shape GetPictureBullet(int index)
        {
            return mPictureBullets[index];
        }

        /// <summary>
        /// Sets a picture bullet by index.
        /// </summary>
        internal void SetPictureBullet(int index, Shape value)
        {
            if (mPictureBullets.Count == index)
                mPictureBullets.Add(value);
            else
                mPictureBullets[index] = value;
        }

        /// <summary>
        /// Expands directly applied list into destination collection.
        /// </summary>
        internal void ExpandDirectList(ParaPr srcParaPr, ParaPr dstParaPr)
        {
            // Expand list formatting of the list applied to the paragraph directly.
            ListLevel listLevel = GetDirectListLevel(srcParaPr);
            if (listLevel != null)
            {
                ClearFirstListTabStop(dstParaPr);
                listLevel.ParaPr.ExpandTo(dstParaPr);

                List list = FetchListByListId(srcParaPr.ListId);
                ListLevelOverride over = list.GetFormattingOverride(srcParaPr.ListLevel);
                if (over != null)
                    over.ListLevel.ParaPr.ExpandTo(dstParaPr);
            }
            else
            {
                // WORDSNET-1545, 625 - Microsoft Word seems to be using zero indent for a paragraph that
                // is not bulleted, but formatted with a style that has bullets. We hack to mimic the same.
                if (srcParaPr.IsExplicitlyNotListItem)
                {
                    dstParaPr.FirstLineIndent = 0;
                    dstParaPr.LeftIndent = 0;

                    // Clear List tab stops.
                    if (dstParaPr.Contains(ParaAttr.TabStops))
                    {
                        for (int i = 0; i < dstParaPr.TabStops.Count; i++)
                        {
                            TabStop tabStop = dstParaPr.TabStops[i];
                            if (tabStop.Alignment == TabAlignment.List)
                                tabStop.Alignment = TabAlignment.Clear;
                        }
                        dstParaPr.TabStops.RemoveClearingWithTolerance();
                    }
                }
            }
        }

        /// <summary>
        /// Gets list level that is specified directly in the paragraph properties (not in a paragraph style).
        /// Returns null if the paragraph is not a list item.
        /// </summary>
        internal ListLevel GetDirectListLevel(ParaPr paraPr)
        {
            return GetDirectListLevel(paraPr, RevisionsView.Original);
        }

        internal ListLevel GetDirectListLevel(ParaPr paraPr, RevisionsView revisionsView)
        {
            object listId = paraPr.GetDirectAttr(ParaAttr.ListId, revisionsView);
            if (listId == null)
                return null;

            if ((int)listId == 0)
                return null;

            List list = FetchListByListIdResolveStyleReference((int)listId);

            object listLevel = paraPr.GetDirectAttr(ParaAttr.ListLevel, revisionsView);
            return list.GetListLevelOverrideAware(listLevel != null ? (int)listLevel : 0);
        }

        /// <summary>
        /// Removes list from collection. Does not update any references so document can become invalid.
        /// </summary>
        internal void RemoveCore(int listId)
        {
            IntToIntDictionary newTranslationTable = new IntToIntDictionary(mListIdTranslationTable.Count - 1);

            int removedListIndex = mListIdTranslationTable[listId];
            if (IntToIntDictionary.IsNullSubstitute(removedListIndex))
                throw new ArgumentOutOfRangeException("listId");

            mListIdTranslationTable.Remove(listId);

            // Update translation table.
            IntToIntDictionary.Enumerator enumerator = mListIdTranslationTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int key = enumerator.CurrentKey;
                int value = enumerator.CurrentValue;

                if(value < removedListIndex)
                    newTranslationTable[key] = value;
                else if(value > removedListIndex)
                    newTranslationTable[key] = value - 1;
            }

            mLists.RemoveAt(removedListIndex - 1);
            mListIdTranslationTable = newTranslationTable;
        }

        /// <summary>
        /// Returns translated ListId value.
        /// </summary>
        internal int GetTranslatedListId(int listId)
        {
            int newListId = mListIdTranslationTable[listId];

            return (IntToIntDictionary.IsNullSubstitute(newListId))
                ? listId
                : newListId;
        }

        /// <summary>
        /// Makes 1-1 translation table which actually means translation absence.
        /// </summary>
        internal void RemoveTranslation()
        {
            for (int i = 0; i < mLists.Count; i++)
                this[i].SetListId(i + 1);

            // Make 1to1 translation table so lists will be sorted by ListId.
            mListIdTranslationTable.Clear();
            for (int i = 0; i < mLists.Count; i++)
                mListIdTranslationTable.Add(i + 1, i + 1);
        }

        /// <summary>
        /// Clears list levels for a ListStyleReferences.
        /// </summary>
        internal void ClearListStyleReferencesLevels()
        {
            foreach (ListDef listDef in mListDefs)
                if (listDef.IsListStyleReference)
                    listDef.Levels.Clear();
        }

        /// <summary>
        /// Fixes circular references listStyles.
        /// </summary>
        internal void FixUpListsWithCircularReferences()
        {
            List<Style> circularStyles = new List<Style>();
            List<ListDef> listDefsToFix = new List<ListDef>();
            foreach (ListDef listDef in mListDefs)
            {
                if (circularStyles.Contains(listDef.Style))
                {
                    RemoveListDefFromCircularReference(listDef, circularStyles);
                }
                else
                {
                    listDefsToFix.Clear();
                    listDefsToFix = GetListDefsWithCircularReferences(listDefsToFix, listDef);

                    foreach (ListDef listDefToFix in listDefsToFix)
                        RemoveListDefFromCircularReference(listDefToFix, circularStyles);
                }
            }
        }

        /// <summary>
        /// Sets value of the latest used list definition identifier value.
        /// </summary>
        /// <param name="value">New value for the last used list definition identifier.</param>
        internal void SetMaxListDefId(int value)
        {
            mMaxUsedListDefId = value;
        }

        /// <summary>
        /// Updates ListDef in order to fix up circular reference.
        /// </summary>
        private static void RemoveListDefFromCircularReference(ListDef listDef, List<Style> circularStyles)
        {
            Style listStyle = listDef.Style;
            if (!circularStyles.Contains(listStyle))
                circularStyles.Add(listStyle);
            listStyle.ParaPr.Remove(ParaAttr.ListId);
            listDef.ListStyleIstd = StyleIndex.NoList;
            listDef.PurgeLevels();
        }

        /// <summary>
        /// Determines circular references listStyle collection, and returns ListDefs for such collection.
        /// </summary>
        private static List<ListDef> GetListDefsWithCircularReferences(List<ListDef> listDefs, ListDef listDef)
        {
            if ((listDef == null) || (listDef.Style == null) || listDef.IsListStyleDefinition)
            {
                listDefs.Clear();
                return listDefs;
            }

            if (listDefs.Contains(listDef))
            {
                foreach (ListDef item in listDefs)
                {
                    if (item == listDef)
                        break;
                    listDefs.Remove(item);
                }
                return listDefs;
            }

            listDefs.Add(listDef);

            return GetListDefsWithCircularReferences(listDefs, listDef.Style.List.ListDef);
        }

        /// <summary>
        /// Change first occurred List tab stop to clear tab stop.
        /// </summary>
        private static void ClearFirstListTabStop(ParaPr paraPr)
        {
            if (!paraPr.Contains(ParaAttr.TabStops))
                return;

            TabStopCollection clearTabStops = new TabStopCollection();
            for (int i = 0; i < paraPr.TabStops.Count; i++)
            {
                if (paraPr.TabStops[i].Alignment == TabAlignment.Clear)
                    clearTabStops.Add(paraPr.TabStops[i].Clone());
            }

            for (int i = 0; i < paraPr.TabStops.Count; i++)
                if (paraPr.TabStops[i].Alignment == TabAlignment.List)
                {
                    // andrnosk: WORDSNET-8769 We have to prevent clearing the same TabStop more then one time.
                    if (clearTabStops.GetByPositionTwips(paraPr.TabStops[i].PositionTwips) != null)
                        continue;

                    paraPr.TabStops.Add(new TabStop(paraPr.TabStops[i].Position, TabAlignment.Clear, TabLeader.None));
                    break;
                }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDocument;

        /// <summary>
        /// Contains instantiated lists (corresponds to LFO structures in binary DOC).
        /// Paragraphs in the document reference lists in this array by index.
        /// The lists are never to be deleted from this array because it will invalidate list id indexes in the model.
        /// The lists in this array reference list definitions.
        /// </summary>
        private List<List> mLists = new List<List>();

        /// <summary>
        /// Contains list definitions (corresponds to LST structures in binary DOC).
        ///
        /// In a correct document list definitions are most likely sorted by their id so it is easy to find a
        /// list definition quickly. Maybe MS Word spec confirms list defs are sorted, but I certainly had
        /// corrupted documents that have duplicate ids and not sorted.
        /// So I decided to keep it as array for resiliency.
        /// </summary>
        private List<ListDef> mListDefs = new List<ListDef>();

        /// <summary>
        /// Contains shapes that represent picture bullets.
        /// Index of the array element is the picture bullet id.
        /// </summary>
        private List<Shape> mPictureBullets = new List<Shape>();

        /// <summary>
        /// Used to translate ListId to 1-based index in list array.
        /// </summary>
        /// <remarks>
        /// AM. In common ListId equal to 1-based index i.e element with ListId = 1 is read first and therefore placed first and so on.
        /// But there are few files that have lists unsorted and even more not 1-indexed array (I've seen file where just two lists with ListId equal 16792839 and 57)
        /// and we should keep this information because there can be paragraph which refers to such list by ListId.
        /// </remarks>
        private IntToIntDictionary mListIdTranslationTable = new IntToIntDictionary();

        private int mMaxUsedListId;
        private int mMaxUsedListDefId;
    }
}
