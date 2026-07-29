// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the subscript object sSub, which consists of a base argument e 
    /// and a reduced-size script placed below and to the right.
    /// </summary>
    internal class MathObjectSubscript : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if (node.NodeType == NodeType.OfficeMath)
            {
                // This object can host arguments and also special object subscript.
                return ((OfficeMath)node).MathObject.MathObjectType == MathObjectType.SubscriptPart;
            }

            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Subscript; }
        }
    }
}
