// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Upper-Limit object, consisting of text on the baseline and reduced-size text
    /// immediately above it.
    /// </summary>
    internal class MathObjectUpperLimit : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if (node.NodeType == NodeType.OfficeMath)
            {
                // upper limit can host both e argument and limit object.
                return ((OfficeMath)node).MathObject.MathObjectType == MathObjectType.Limit;
            }

            return false;
        }
        
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.UpperLimit; }
        }
    }
}
