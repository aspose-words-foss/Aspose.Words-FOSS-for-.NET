// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Pre-Sub-Superscript object, which consists of a base e and a 
    /// subscript and superscript placed to the left of the base.
    /// </summary>
    internal class MathObjectPreSubSuperscript : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if (node.NodeType == NodeType.OfficeMath)
            {
                // This object can host arguments and also special objects subscript part and
                // superscript part.
                MathObjectType type = ((OfficeMath)node).MathObject.MathObjectType;
                return (type == MathObjectType.SubscriptPart) || (type == MathObjectType.SuperscriptPart);
            }

            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.PreSubSuperscript; }
        }
    }
}
