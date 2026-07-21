// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2013 by Alexey Butalov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Revisions;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Resolves HTML attributes that don't have CSS analogs.
    /// </summary>
    internal class HtmlAttributeResolver
    {
        internal HtmlAttributeResolver()
        {
            mLocaleIdStack = new IntStack();
            mLocaleIdBiStack = new IntStack();
            mLocaleIdFarEastStack = new IntStack();
            mInsertionRevisions = new Stack<EditRevision>();
            mDeletionRevisions = new Stack<EditRevision>();
        }

        /// <summary>
        /// Adds an element to the stack and calculates values of HTML attributes for this element.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="elementDeclarations">HTML element actual declarations. The actual declarations are the used declarations after any
        /// such adjustments have been made.</param>
        internal void PushElement(IHtmlElementProvider element, CssDeclarationCollection elementDeclarations)
        {
            ProcessLangAttribute(element, elementDeclarations);
            ProcessRevisions(element);
        }

        /// <summary>
        /// Removes a top element from the stack.
        /// </summary>
        internal void PopElement()
        {
            mInsertionRevisions.Pop();
            mDeletionRevisions.Pop();
            mLocaleIdStack.Pop();
            mLocaleIdBiStack.Pop();
            mLocaleIdFarEastStack.Pop();
        }

        private void ProcessRevisions(IHtmlElementProvider element)
        {
            EditRevision insertionRevision = (element.ElementName == "ins")
                                                 ? GetRevision(element, EditRevisionType.Insertion)
                                                 : GetCurrentRevision(mInsertionRevisions);
            mInsertionRevisions.Push(insertionRevision);

            EditRevision deletionRevision = (element.ElementName == "del")
                                                ? GetRevision(element, EditRevisionType.Deletion)
                                                : GetCurrentRevision(mDeletionRevisions);
            mDeletionRevisions.Push(deletionRevision);
        }

        private static EditRevision GetRevision(IHtmlElementProvider element, EditRevisionType revisionType)
        {
            string author = element.GetAttributeValue("cite", string.Empty);
            DateTime datetime = FormatterPal.XmlToDateTime(element.GetAttributeValue("datetime", string.Empty));
            return new EditRevision(revisionType, author, datetime);
        }

        private static EditRevision GetCurrentRevision(Stack<EditRevision> revisions)
        {
            return revisions.Top();
        }

        private void ProcessLangAttribute(IHtmlElementProvider element, CssDeclarationCollection elementDeclarations)
        {
            // Language information is inherited by elements inside the one where the declaration was made.
            int localeId = LocaleId;
            int localeIdBi = LocaleIdBi;
            int localeIdFarEast = LocaleIdFarEast;

            string lang = element.GetAttributeValue("lang");
            if (StringUtil.HasChars(lang))
            {
                int langLocaleId = HtmlLocaleConverter.TagToLocale(lang);
                if (langLocaleId != (int)Language.InvariantCulture)
                {
                    if (CssUtil.GetDirection(elementDeclarations) == CssDirection.Rtl)
                    {
                        localeIdBi = langLocaleId;
                    }
                    else
                    {
                        localeId = langLocaleId;
                        localeIdFarEast = langLocaleId;
                    }
                }
            }

            mLocaleIdStack.Push(localeId);
            mLocaleIdBiStack.Push(localeIdBi);
            mLocaleIdFarEastStack.Push(localeIdFarEast);
        }

        internal int LocaleId
        {
            get
            {
                return (mLocaleIdStack.Count != 0)
                           ? mLocaleIdStack.Peek()
                           : (int)Language.NoProof;
            }
        }

        internal int LocaleIdBi
        {
            get
            {
                return (mLocaleIdBiStack.Count != 0)
                           ? mLocaleIdBiStack.Peek()
                           : (int)Language.NoProof;
            }
        }

        internal int LocaleIdFarEast
        {
            get
            {
                return (mLocaleIdFarEastStack.Count != 0)
                           ? mLocaleIdFarEastStack.Peek()
                           : (int)Language.NoProof;
            }
        }


        /// <summary>
        /// Gets an insertion revision for the current element.
        /// Can be null, if the element doesn't have an insertion revision.
        /// </summary>
        internal EditRevision InsertionRevision
        {
            get { return mInsertionRevisions.Top(); }
        }

        /// <summary>
        /// Gets a deletion revision for the current element.
        /// Can be null, if the element doesn't have a deletion revision.
        /// </summary>
        internal EditRevision DeletionRevision
        {
            get { return mDeletionRevisions.Top(); }
        }

        private readonly IntStack mLocaleIdStack;
        private readonly IntStack mLocaleIdBiStack;
        private readonly IntStack mLocaleIdFarEastStack;

        /// <summary>
        /// Stack of insertion revisions.
        /// </summary>
        private readonly Stack<EditRevision> mInsertionRevisions;

        /// <summary>
        /// Stack of deletion revisions.
        /// </summary>
        private readonly Stack<EditRevision> mDeletionRevisions;
    }
}
