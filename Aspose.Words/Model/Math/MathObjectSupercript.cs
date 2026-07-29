// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the superscript object sSup, which consists of a base e and 
    /// a reduced-size script placed above and to the right.
    /// </summary>
    internal class MathObjectSuperscript : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if (node.NodeType == NodeType.OfficeMath)
            {
                // This object can host arguments and also special object superscript part.
                return (((OfficeMath)node).MathObject.MathObjectType == MathObjectType.SuperscriptPart);
            }

            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Supercript; }
        }
    }
}
