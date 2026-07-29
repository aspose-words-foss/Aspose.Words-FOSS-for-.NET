// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to <see cref="HeaderFooter"/> nodes of a <see cref="Section"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-headers-and-footers/">Working with Headers and Footers</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>There can be maximum of one <see cref="HeaderFooter"/></p> of each <see cref="HeaderFooterType"/> per
    /// <see cref="Section"/>.
    /// <p><see cref="HeaderFooter"/> objects can occur in any order in the collection.</p>
    /// </remarks>
    [JavaGenericArguments("NodeCollection<HeaderFooter>")]
    public class HeaderFooterCollection : NodeCollection
    {
        internal HeaderFooterCollection(CompositeNode parent) : base(parent, NodeType.HeaderFooter, false)
        {
            ParentSection = (Section)parent;
        }

        /// <summary>
        /// Retrieves a <see cref="HeaderFooter"/> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new HeaderFooter this[int index]
        {
            get { return (HeaderFooter)base[index]; }
        }

        /// <summary>
        /// Retrieves a <see cref="HeaderFooter"/> of the specified type.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if the header/footer of the specified type is not found.
        /// </remarks>
        /// <param name="headerFooterType">A <see cref="HeaderFooterType"/> value
        /// that specifies the type of the header/footer to retrieve.</param>
        public HeaderFooter this[HeaderFooterType headerFooterType]
        {
            get
            {
                foreach (HeaderFooter headerFooter in this)
                {
                    if (headerFooter.HeaderFooterType == headerFooterType)
                        return headerFooter;
                }
                return null;
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Retrieves a <b>HeaderFooter</b> of the specified type.
        /// </summary>
        public HeaderFooter GetByHeaderFooterType(HeaderFooterType headerFooterType)
        {
            return this[headerFooterType];
        }

        /// <summary>
        /// Retrieves a header for even numbered pages.
        /// </summary>
        public HeaderFooter HeaderEven
        {
            get
            {
                return this[HeaderFooterType.HeaderEven];
            }
        }

        /// <summary>
        /// Retrieves a primary header, also used for odd numbered pages.
        /// </summary>
        public HeaderFooter HeaderPrimary
        {
            get
            {
                return this[HeaderFooterType.HeaderPrimary];
            }
        }

        /// <summary>
        /// Retrieves a footer for even numbered pages.
        /// </summary>
        public HeaderFooter FooterEven
        {
            get
            {
                return this[HeaderFooterType.FooterEven];
            }
        }

        /// <summary>
        /// Retrieves a primary footer, also used for odd numbered pages.
        /// </summary>
        public HeaderFooter FooterPrimary
        {
            get
            {
                return this[HeaderFooterType.FooterPrimary];
            }
        }

        /// <summary>
        /// Retrieves a header for the first page of the section.
        /// </summary>
        public HeaderFooter HeaderFirst
        {
            get
            {
                return this[HeaderFooterType.HeaderFirst];
            }
        }

        /// <summary>
        /// Retrieves a footer for the first page of the section.
        /// </summary>
        public HeaderFooter FooterFirst
        {
            get
            {
                return this[HeaderFooterType.FooterFirst];
            }
        }
#endif

        /// <summary>
        /// Links or unlinks all headers and footers to the corresponding
        /// headers and footers in the previous section.
        /// </summary>
        /// <remarks>
        /// <p>If any of the headers or footers do not exist, creates them automatically.</p>
        /// </remarks>
        /// <param name="isLinkToPrevious"><c>true</c> to link the headers and footers to the previous section;
        /// <c>false</c> to unlink them.</param>
        public void LinkToPrevious(bool isLinkToPrevious)
        {
            LinkToPrevious(HeaderFooterType.HeaderPrimary, isLinkToPrevious);
            LinkToPrevious(HeaderFooterType.HeaderFirst, isLinkToPrevious);
            LinkToPrevious(HeaderFooterType.HeaderEven, isLinkToPrevious);
            LinkToPrevious(HeaderFooterType.FooterPrimary, isLinkToPrevious);
            LinkToPrevious(HeaderFooterType.FooterFirst, isLinkToPrevious);
            LinkToPrevious(HeaderFooterType.FooterEven, isLinkToPrevious);
        }

        /// <summary>
        /// Links or unlinks the specified header or footer to the corresponding
        /// header or footer in the previous section.
        /// </summary>
        /// <remarks>
        /// <p>If the header or footer of the specified type does not exist, creates it automatically.</p>
        /// </remarks>
        /// <param name="headerFooterType">A <see cref="HeaderFooterType"/> value
        /// that specifies the header or footer to link/unlink.</param>
        /// <param name="isLinkToPrevious"><c>true</c> to link the header or footer to the previous section;
        /// <c>false</c> to unlink.</param>
        public void LinkToPrevious(HeaderFooterType headerFooterType, bool isLinkToPrevious)
        {
            HeaderFooter headerFooter = this[headerFooterType];
            if (headerFooter == null)
            {
                headerFooter = new HeaderFooter(((INodeCollection)this).Container.Document, headerFooterType);
                Add(headerFooter);
            }

            headerFooter.IsLinkedToPrevious = isLinkToPrevious;
        }

        /// <summary>
        /// Copies all <c>HeaderFooter</c>s from the collection to a new array of <c>HeaderFooter</c>s.
        /// </summary>
        /// <returns>An array of <c>HeaderFooter</c>s.</returns>
        public new HeaderFooter[] ToArray()
        {
            return ToList<HeaderFooter>().ToArray();
        }

        /// <summary>
        /// Returns true, if collection has any of the Headers.
        /// </summary>
        internal bool HasHeaders
        {
            get
            {
                foreach (HeaderFooter headerFooter in this)
                {
                    if (headerFooter.IsHeader)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns true, if collection has any of the Footers.
        /// </summary>
        internal bool HasFooters
        {
            get
            {
                foreach (HeaderFooter headerFooter in this)
                {
                    if (!headerFooter.IsHeader)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a parent section.
        /// </summary>
        internal Section ParentSection { get; private set; }
    }
}
