// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

using Aspose.Words.Settings;

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies a math paragraph, or display math zone, 
    /// that contains one or more oMath elements that are in display mode.
    /// </summary>
    internal class MathObjectOMathPara : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            // OMath object is only allowed inside OMathPara
            return (node.NodeType == NodeType.OfficeMath) &&
                    (((OfficeMath)node).MathObject.MathObjectType == MathObjectType.OMath);
        }
        
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.OMathPara; }
        }

        /// <summary>
        /// Specifies justification of the math paragraph (a series of adjacent instances of mathematical text 
        /// within the same paragraph).
        /// Default: should be obtained from <see cref="MathProperties.DefaultJustification"/>
        /// </summary>
        internal OfficeMathJustification Justification
        {
            get { return (OfficeMathJustification)FetchAttr(MathAttr.Justification); }
            set { SetAttr(MathAttr.Justification, value, value != OfficeMathJustification.Default); }
        }
    }
}
