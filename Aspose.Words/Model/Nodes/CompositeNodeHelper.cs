// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2023 by Evgeny Vasilev

using System;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    [CppOverrideAccessModifier(AccessModifiers.Public)]
    internal static class CompositeNodeHelper
    {
        /// <summary>
        /// DocumentBase inherits from CompositeNode, so translated C++ code does not include the DocumentBase header
        /// file in the CompositeNode header file. Because of this, it is not possible to call functions members of an
        /// instance of the DocumentBase class in the template method of the CompositeNode class. The following methods
        /// have been added to solve this problem.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static NodeChangingArgs DocumentBaseInternalEvent(DocumentBase doc, Node newChild, Node oldParent, Node newParent, NodeChangingAction action)
        {
            return doc.InternalEvent(newChild, oldParent, newParent, action);
        }

        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static void DocumentBaseBeforeEvent(DocumentBase doc, NodeChangingArgs args)
        {
            if (args != null)
                doc.BeforeEvent(args);
        }

        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static void DocumentBaseAfterEvent(DocumentBase doc, NodeChangingArgs args)
        {
            if (args != null)
                doc.AfterEvent(args);
        }

        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static bool DocumentBaseIsTrackRevisionsEnabled(DocumentBase doc)
        {
            return doc.IsTrackRevisionsEnabled;
        }

        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static void DocumentBaseCompareException(DocumentBase thisDoc, DocumentBase childDoc)
        {
            if (childDoc != thisDoc)
                throw new ArgumentException("The newChild was created from a different document than the one that created this node.");
        }

    }
}
