// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// The description here applies to all IXXXAttrSource interfaces.
    ///
    /// In our model, different objects, for example Style and Paragraph, can both have
    /// paragraph formatting and it is exposed as ParagraphFormat to the public.
    /// The problem is that paragraph formatting on Style and Paragraph is resolved
    /// differently. For example, when getting an attribute from a style, it needs to
    /// look in the attributes of the style and then navigate through the based-on styles.
    /// When getting and attribute from a paragraph, need to look for direct formatting,
    /// then get the style of the paragraph and look for the attribute in the style.
    ///
    /// It is the purpose of IXXXAttrSource interfaces to solve this problem and
    /// allow single ParagraphFormat to expose paragraph attributes to the public,
    /// yet allow variation in attribute resolution depending on what object
    /// ParagraphFormat is obtained from.
    /// </summary>
    internal interface IRunAttrSource
    {
        /// <summary>
        /// Gets the attribute specified directly on the source or null by the key.
        /// </summary>
        object GetDirectRunAttr(int key);

        object GetDirectRunAttr(int key, RevisionsView revisionsView);

        /// <summary>
        /// Gets the attribute from one of the parents or from global defaults.
        /// Throws if the attribute is not defined anywhere.
        /// If the attribute needs resolution, it is returned resolved.
        /// </summary>
        object FetchInheritedRunAttr(int key);

        /// <summary>
        /// Sets run attribute.
        /// </summary>
        void SetRunAttr(int key, object value);

        /// <summary>
        /// Removes direct attribute.
        /// </summary>
        void RemoveRunAttr(int key);

        /// <summary>
        /// Clears run attributes.
        /// </summary>
        void ClearRunAttrs();
    }
}
