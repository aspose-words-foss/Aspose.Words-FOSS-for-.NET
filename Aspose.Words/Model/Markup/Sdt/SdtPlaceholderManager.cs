// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2011 by Denis Darkin

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.BuildingBlocks;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Allows to handle various tasks related to supporting of placeholder life cycle, including:
    /// - creation of different placeholder building blocks into the document gallery;
    /// - Reuse of placeholders if possible the same way MS Word does;
    /// - counts references of placeholders and deletes the unused ones upon request.
    /// </summary>
    internal class SdtPlaceholderManager
    {
        /// <summary>
        /// ctor
        /// </summary>
        internal SdtPlaceholderManager(DocumentBase doc)
        {
            if (doc is Document)
            {
                mMainDocument = (Document)doc;
                mGlossary = mMainDocument.GlossaryDocument;
            }
            else
            {
                mGlossary = (GlossaryDocument)doc;
                mMainDocument = null;
            }

            if (!IsGlossaryEmpty)
                PopulatePlaceholderCacheFromGlossary();
        }

        /// <summary>
        /// Returns new or reuses existing placeholder, which is actually a building block in the gallery.
        /// Can return null if placeholder does not make sense for given SDT.
        /// </summary>
        /// <remarks>
        /// 1. Placeholders are reused between different levels of markup.
        /// 2. Generation of default SdtContent is different from generation of placeholder.
        /// </remarks>
        internal BuildingBlock FetchPlaceholderByType(SdtType type)
        {
            BuildingBlock placeholder = mTypeCache[(int)type];
            return (placeholder != null) ? placeholder : CreatePlaceholder(type);
        }

        /// <summary>
        /// Searches for existing placeholder in the glossarydoc. If not found then either creates
        /// a new one (if isCreateNewPlaceholder == true), or returns null (if isCreateNewPlaceholder == false)
        /// </summary>
        internal BuildingBlock FindPlaceholder(StructuredDocumentTag tag, bool isCreateNewPlaceholder)
        {
            BuildingBlock placeholder = null;
            if (tag.PlaceholderName != "")
                placeholder = GetPlaceholderByName(tag);

            // WORDSNET-15953 System.InvalidOperationException throws StructuredDocumentTag.PlaceholderName.
            // If SdtPlaceholderManager doesn't contain placeholder then try to find it in GlossaryDocument
            if (placeholder == null)
            {
                foreach (BuildingBlock buildingBlock in Glossary.BuildingBlocks)
                {
                    if (buildingBlock.Name.Equals(tag.PlaceholderName, StringComparison.Ordinal))
                    {
                        placeholder = buildingBlock;
                        break;
                    }
                }
            }

            if ((isCreateNewPlaceholder) && (placeholder == null))
                placeholder = FetchPlaceholderByType(tag.SdtType);

            return placeholder;
        }

        /// <summary>
        /// Initialized reference counter to all zeros
        /// </summary>
        internal void StartReferenceCounter()
        {
            mPlaceholderCounter.Reset();
        }

        /// <summary>
        /// Increments placeholder reference count if this placeholder is used by sdt.
        /// </summary>
        internal void IncrementReference(StructuredDocumentTag sdt)
        {
            if (sdt.Placeholder != null)
                mPlaceholderCounter.IncrementReference(sdt.Placeholder.Name);
        }

        /// <summary>
        /// Used to merge reference counters from document and glossary document.
        /// </summary>
        internal void AppendPlaceholderReferences(IEnumerable<object> referencedObjects)
        {
            foreach (object r in referencedObjects)
                mPlaceholderCounter.IncrementReference(r);
        }

        /// <summary>
        /// Removes unused placeholders from the document's glossary.
        /// </summary>
        internal bool RemoveUnusedPlaceholders()
        {
            bool result = false;
            foreach (BuildingBlock block in Glossary.BuildingBlocks)
            {
                if ((block.ParentNode != null) && IsSdtPlaceholder(block) && !mPlaceholderCounter.IsReferenced(block.Name))
                {
                    block.Remove();
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks whether placeholder name is one of MS predefined placeholder name.
        /// </summary>
        internal static bool IsPredefinedPlaceholderName(string placeholderName)
        {
            switch (placeholderName)
            {
                case TextPlaceholderName:
                case ListboxPlaceholderName:
                case DatePlaceholderName:
                case BuildingBlockPlaceholderName:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates custom Placeholder with the specified placeholder run text and returns its name.
        /// </summary>
        internal string CreateCustomPlaceholder(string placeholderRunText)
        {
            Debug.Assert(StringUtil.HasChars(placeholderRunText));
            return CreatePlaceholderCore(GetUnusedCustomPlaceholderName(), placeholderRunText).Name;
        }

        /// <summary>
        /// Searches placeholder by name and returns it. If not found returns null.
        /// </summary>
        private BuildingBlock GetPlaceholderByName(StructuredDocumentTag tag)
        {
            string name = tag.PlaceholderName;
            BuildingBlock result = (mNameCache.ContainsKey(name))
                ? mNameCache[name]
                : null;
            if (result == null)
            {
                // possible case where non-MS Word names are used, so we have to look some more.
                result = Glossary.GetBuildingBlock(BuildingBlockGallery.StructuredDocumentTagPlaceholderText, null, name);
                if (result != null)
                {
                    // some non MSWord documents do not reuse placeholders for same sdt types, so we might encounter different names
                    // for the same type of sdt placeholder.
                    AddCompatiblePlaceholderToCache(name, result);
                    AddCompatiblePlaceholderToCache(tag.SdtType, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Search for MS-style placeholders e.g. those that have
        /// </summary>
        private void PopulatePlaceholderCacheFromGlossary()
        {
            foreach (BuildingBlock block in Glossary)
            {
                if (block.Gallery == BuildingBlockGallery.StructuredDocumentTagPlaceholderText)
                {
                    switch (block.Name)
                    {
                        case TextPlaceholderName:
                            AddTextPlaceholderToCahce(block);
                            break;
                        case DatePlaceholderName:
                            AddDatePlaceholderToCache(block);
                            break;
                        case ListboxPlaceholderName:
                            AddListboxPlaceholderToCache(block);
                            break;
                        case BuildingBlockPlaceholderName:
                            AddBuildingBlockPlaceholderToCache(block);
                            break;
                        default:
                            // there exist blocks with non-MS default, we will deal with them in GetPlaceholderByName function.
                            break;
                    }
                }
            }
        }

        private void AddTextPlaceholderToCahce(BuildingBlock placeholder)
        {
            AddCompatiblePlaceholderToCache(SdtType.RichText, placeholder);
            AddCompatiblePlaceholderToCache(SdtType.PlainText, placeholder);
            AddCompatiblePlaceholderToCache(SdtType.Group, placeholder); // MS Word uses text-placeholder for Group sdt, although it seems silly.

            AddCompatiblePlaceholderToCache(TextPlaceholderName, placeholder);
        }

        private void AddListboxPlaceholderToCache(BuildingBlock placeholder)
        {
            AddCompatiblePlaceholderToCache(SdtType.ComboBox, placeholder);
            AddCompatiblePlaceholderToCache(SdtType.DropDownList, placeholder);

            AddCompatiblePlaceholderToCache(ListboxPlaceholderName, placeholder);
        }

        private void AddDatePlaceholderToCache(BuildingBlock placeholder)
        {
            mTypeCache[(int)SdtType.Date] = placeholder;
            mNameCache[DatePlaceholderName] = placeholder;
        }

        private void AddBuildingBlockPlaceholderToCache(BuildingBlock placeholder)
        {
            mTypeCache[(int)SdtType.BuildingBlockGallery] = placeholder;
            mNameCache[BuildingBlockPlaceholderName] = placeholder;
        }

        /// <summary>
        /// Creates different placeholders depending on type.
        /// </summary>
        private BuildingBlock CreatePlaceholder(SdtType type)
        {
            BuildingBlock placeholder;
            switch (type)
            {
                case SdtType.RichText:
                case SdtType.PlainText:
                case SdtType.Group:
                {
                    placeholder = CreatePlaceholderCore(TextPlaceholderName, "Click here to enter text.");
                    AddTextPlaceholderToCahce(placeholder);
                    break;
                }
                case SdtType.ComboBox:
                case SdtType.DropDownList:
                {
                    placeholder = CreatePlaceholderCore(ListboxPlaceholderName, "Choose an item.");
                    AddListboxPlaceholderToCache(placeholder);
                    break;
                }
                case SdtType.Date:
                {
                    placeholder = CreatePlaceholderCore(DatePlaceholderName, "Click here to enter a date.");
                    AddDatePlaceholderToCache(placeholder);
                    break;
                }
                case SdtType.BuildingBlockGallery:
                {
                    placeholder = CreatePlaceholderCore(BuildingBlockPlaceholderName, "Choose a building block.");
                    AddBuildingBlockPlaceholderToCache(placeholder);
                    break;
                }
                default:
                {
                    placeholder = null; // all other Sdts either don't require placeholder, or are not allowed for public creation.
                    break;
                }
            }

            return placeholder;
        }

        private BuildingBlock CreatePlaceholderCore(string placehoderName, string placeholderRunText)
        {
            BuildingBlock block = CreateBuildingBlock(placehoderName);
            Glossary.AppendChild(block);
            Section s = new Section(Glossary);
            block.AppendChild(s);

            Body b = new Body(Glossary);
            s.AppendChild(b);

            Paragraph para = new Paragraph(Glossary);
            b.AppendChild(para);

            Run run = new Run(Glossary, placeholderRunText);
            run.RunPr.SetAttr(FontAttr.Istd, mStyle.Istd);
            para.AppendChild(run);

            return block;
        }

        /// <summary>
        /// All building block for different sdts share the same properties, and can be created by one function.
        /// </summary>
        /// <returns></returns>
        private BuildingBlock CreateBuildingBlock(string blockName)
        {
            EnsureStyleCreation();

            BuildingBlock block = new BuildingBlock(Glossary);
            block = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(block, Glossary);
            block.Category = BuildingBlockCategory;
            block.Gallery = BuildingBlockGallery.StructuredDocumentTagPlaceholderText;
            block.Type = BuildingBlockType.StructuredDocumentTagPlaceholderText;
            block.Behavior = BuildingBlockBehavior.Content;
            block.Guid = RandomUtil.NewGuid(blockName);
            block.Name = blockName;

            return block;
        }

        /// <summary>
        /// Create style to be used for placeholder runs, reuse one if already created.
        /// </summary>
        private void EnsureStyleCreation()
        {
            if (mStyle == null)
            {
                Glossary.Styles.EnsureMinimum();

                mStyle = Glossary.Styles.GetByName("Placeholder Text", false);
                if (mStyle == null)
                {
                    mStyle = Glossary.Styles.Add(StyleType.Character, "Placeholder Text");
                    mStyle.BasedOnIstd = StyleIndex.DefaultParagraphFont;
                    mStyle.Priority = 99;
                    mStyle.SemiHidden = true;
                    mStyle.RunPr.Color = DrColor.FromArgb(0, 0x80, 0x80, 0x80);
                }
            }
        }

        /// <summary>
        /// When placeholders are used for compatible SDTs e.g. combo list vs. dropdown list, we have to make sure to check if one of them is already in cache
        /// due to being already loaded from a document.
        /// </summary>
        private void AddCompatiblePlaceholderToCache(string name, BuildingBlock placeholder)
        {
            if (!mNameCache.ContainsKey(name))
                mNameCache.Add(name, placeholder);
        }

        /// <summary>
        /// When placeholders are used for compatible SDTs e.g. combo list vs. dropdown list, we have to make sure to check if one of them is already in cache
        /// due to being already loaded from a document.
        /// </summary>
        private void AddCompatiblePlaceholderToCache(SdtType type, BuildingBlock placeholder)
        {
            if (mTypeCache[(int)type] == null)
                mTypeCache[(int)type] = placeholder;
        }

        /// <summary>
        /// Returns true if given block has properties sufficient to consider it to be a placeholder.
        /// </summary>
        /// <remarks>There might be other non-MS compliant placeholders that don't follow this criteria.</remarks>
        private static bool IsSdtPlaceholder(BuildingBlock block)
        {
            return (block.Gallery == BuildingBlockGallery.StructuredDocumentTagPlaceholderText) &&
                   (block.Type == BuildingBlockType.StructuredDocumentTagPlaceholderText);
        }

        /// <summary>
        /// Gets the unused custom placeholder name in the "CustomPlaceholder_1" style. 
        /// </summary>
        private string GetUnusedCustomPlaceholderName()
        {
            const string nameStart = "CustomPlaceholder_";

            long number;
            long customPlaceholderNameCount = 0;
            foreach (BuildingBlock block in Glossary)
            {
                if (block.Name.StartsWith(nameStart, StringComparison.Ordinal))
                {
                    number = FormatterPal.TryParseLong(block.Name.Substring(nameStart.Length));
                    if (customPlaceholderNameCount < number)
                        customPlaceholderNameCount = number;
                }
            }
            return string.Format("{0}{1}", nameStart, ++customPlaceholderNameCount);
        }

        /// <summary>
        /// Collection of placeholder reference counts showing which placeholders are used by SDTS in the parent document.
        /// </summary>
        internal IEnumerable<object> PlaceholdersUsed
        {
            get { return mPlaceholderCounter.ReferencedObjects; }
        }

        /// <summary>
        /// Returns true if this glossary reference stored in this manager is an empty entity containing no useful data.
        /// This usually happens when there is no SDT nodes in the document or when these nodes don't require placeholders.
        /// </summary>
        internal bool IsGlossaryEmpty
        {
            get { return (mGlossary == null); }
        }

        /// <summary>
        /// Used only internally by SdtPlaceholderManager to create placeholder blocks.
        /// In new AW documents Glossary is null, so SdtPlaceholderManager might have to create one, that is why we store main doc in the mMainDocument.
        /// On the other hand, we don't always want to create glossary in cases when no SDTs are used in the document.
        /// </summary>
        private GlossaryDocument Glossary
        {
            get
            {
                if (mGlossary == null) // delay creation of GlossaryDocument until it is actually requested by this SdtPlaceholderManager
                {
                    if (mMainDocument != null)
                    {
                        if (mMainDocument.GlossaryDocument == null)
                            mMainDocument.GlossaryDocument = new GlossaryDocument();

                        mGlossary = mMainDocument.GlossaryDocument;
                    }
                    else
                    {
                        throw new InvalidOperationException("Please report exception.");
                    }
                }

                return mGlossary;
            }
        }

        /// <summary>
        /// Default style for all placeholders, created upon demand.
        /// </summary>
        private Style mStyle;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private GlossaryDocument mGlossary;

        /// <summary>
        /// Reference to main doc is needed in order to do delayed creation of Glossary in case if it was null initially.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mMainDocument;

        /// <summary>
        /// Stores sdt type to building block mapping;
        /// used in model when new SDTs are created.
        /// </summary>
        private readonly BuildingBlock[] mTypeCache = new BuildingBlock[gSdtTypeCount];

        /// <summary>
        /// Stores placeholder name to building block mapping,
        /// used during placeholder lookup.
        /// </summary>
        private readonly Dictionary<string, BuildingBlock> mNameCache = new Dictionary<string, BuildingBlock>();

        private readonly ReferenceCounter mPlaceholderCounter = new ReferenceCounter();

        private static readonly int gSdtTypeCount = EnumUtilPal.GetEffectiveArrayLength(SdtType.Group.GetType(), 17);

        /// <summary>
        /// Exposed as internal for testing purposes.
        /// </summary>
        internal const string BuildingBlockCategory = "General";

        // MS Word always uses these fancy names for placeholders of particular sdt types.
        // Exposed as internal for testing purposes.
        internal const string TextPlaceholderName = "DefaultPlaceholder_22675703";
        internal const string ListboxPlaceholderName = "DefaultPlaceholder_22675704";
        internal const string DatePlaceholderName = "DefaultPlaceholder_22675705";
        internal const string BuildingBlockPlaceholderName = "DefaultPlaceholder_22675706";
    }
}
