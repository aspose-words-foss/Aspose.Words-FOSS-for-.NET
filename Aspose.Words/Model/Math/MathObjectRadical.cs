// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the radical object (root of n power), consisting of a radical, a base e, and an optional degree deg.
    /// </summary>
    /// <remarks>If the degree argument is omitted, radical is considered to be sqare root.</remarks>
    internal class MathObjectRadical : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if (node.NodeType == NodeType.OfficeMath)
            {
                // Radical object can host arguments and also special object degree.
                return ((OfficeMath)node).MathObject.MathObjectType == MathObjectType.Degree;
            }

            return false;
        }
        
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Radical; }
        }

        /// <summary>
        /// Specifies the per-object option to hide the degree of a radical. Every rad has a degree, but the degree can
        /// appear or not appear.
        /// Default is false.
        /// </summary>
        internal bool IsHideDegree
        {
            get { return (bool)FetchAttr(MathAttr.DegreeHide); }
            set { SetAttr(MathAttr.DegreeHide, value, value); }
        }
    }
}
