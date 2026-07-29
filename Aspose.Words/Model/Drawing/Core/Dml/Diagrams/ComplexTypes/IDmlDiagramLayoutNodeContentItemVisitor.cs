// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// Interface for a visitor of layout node content items.
    /// </summary>
    internal interface IDmlDiagramLayoutNodeContentItemVisitor
    {
        void VisitLayoutNode(DmlDiagramLayoutNode layoutNode);
        void VisitAlgorithm(DmlAlgorithm algorithm);
        void VisitChoose(DmlChoose choose);
        void VisitConstraintList(DmlConstraintList constraintList);
        void VisitRuleList(DmlNumericRuleList ruleList);
        void VisitShape(DmlDiagramShape shape);
        void VisitForEach(DmlForEach forEach);
        void VisitLayoutVariablePropertySet(DmlLayoutVariablePropertySet layoutVariablePropertySet);
        void VisitPresentationOf(DmlPresentationOf presentationOf);
    }
}
