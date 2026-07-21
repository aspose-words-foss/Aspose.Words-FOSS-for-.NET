// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2010 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// An implementation of <see cref="INodeModifier"/> that contains a collection of modifiers and applies
    /// all of them one by one.
    /// </summary>
    internal class CompositeModifier : INodeModifier
    {
        internal CompositeModifier(params INodeModifier[] modifiers)
        {
            mModifiers = new List<INodeModifier>(modifiers.Length);
            foreach (INodeModifier modifier in modifiers)
                AddModifier(modifier);
        }

        internal void AddModifier(INodeModifier modifier)
        {
            if (modifier != null)
                mModifiers.Add(modifier);
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            foreach (INodeModifier modifier in mModifiers)
            {
                if (nodeToModify != null)
                    nodeToModify = modifier.Modify(referenceNode, nodeToModify, modifyChildren, cloningListener);
            }

            return nodeToModify;
        }

        private readonly List<INodeModifier> mModifiers;
    }
}
