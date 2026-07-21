// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the fraction object, consisting of a numerator and denominator separated by a fraction
    /// bar. The fraction bar can be horizontal or diagonal, depending on the fraction properties. The fraction object is
    /// also used to represent the stack function, which places one element above another, with no fraction bar.
    /// </summary>
    internal class MathObjectFraction : MathObject
    {
         /// <summary>
        /// Can only host m:numerator and m:denominator.
        /// </summary>
        internal override bool CanInsert(Node node)
        {
            if (node.NodeType == NodeType.OfficeMath)
            {
                MathObject mathObject = ((OfficeMath)node).MathObject;
                return (mathObject.MathObjectType == MathObjectType.Numerator) || (mathObject.MathObjectType == MathObjectType.Denominator);
            }
                    
            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Fraction; }
        }
        
        /// <summary>
        /// This element specifies the type of fraction sign for this fraction object.
        /// Default: FractionType.Default
        /// </summary>
        internal MathFractionType FractionType
        {
            get { return (MathFractionType)FetchAttr(MathAttr.FractionType); }
            set { SetAttr(MathAttr.FractionType, value, value != MathFractionType.Default); }
        }
    }
}
