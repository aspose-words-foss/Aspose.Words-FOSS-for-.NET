// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Function-Apply object, which consists of a function name and an argument element
    /// (e) acted upon. 
    /// </summary>
    /// <remarks>
    /// Function-Apply is often applied using a form of linear format. For example, in the linear format described in
    /// Unicode Technical Article #28, this object is applied by using the Function Application character (U+2061).
    /// </remarks>
    internal class MathObjectFunction : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
            
            if (node.NodeType == NodeType.OfficeMath)
            {
                // Function object can host arguments and also special object FunctionName which holds objects that are
                // associated with function name (it can be rather complex and can act as container for other math objects).
                return ((OfficeMath)node).MathObject.MathObjectType == MathObjectType.FunctionName;
            }

            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Function; }
        }
    }
}
