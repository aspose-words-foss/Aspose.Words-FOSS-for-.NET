// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the sub-superscript object, which consists of a base e,
    /// a reduced-size script placed below  and to the right, 
    /// and a reduced-size scr placed above and to the right.
    /// </summary>
    internal class MathObjectSubSuperscript : MathObject
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
            get { return MathObjectType.SubSuperscript; }
        }

        /// <summary>
        /// Specifies the alignment of scripts in the subscript/superscript function. When 
        /// true, subscripts and superscripts are aligned to each other. 
        /// When false, they are kerned to the shape of the base. 
        /// Default: false.
        /// </summary>
        internal bool IsAlignScripts
        {
            get { return (bool)FetchAttr(MathAttr.IsAlignScripts); }
            set { SetAttr(MathAttr.IsAlignScripts, value, value); }
        }
    }
}
