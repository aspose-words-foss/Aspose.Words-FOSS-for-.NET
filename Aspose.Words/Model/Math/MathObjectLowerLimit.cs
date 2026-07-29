// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Lower-Limit object, consisting of text on the baseline and reduced-size text
    /// immediately below it.
    /// </summary>
    internal class MathObjectLowerLimit : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if (node.NodeType == NodeType.OfficeMath)
            {
                // lower limit can host both e argument and limit object.
                return ((OfficeMath)node).MathObject.MathObjectType == MathObjectType.Limit;
            }

            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.LowerLimit; }
        }
    }
}
