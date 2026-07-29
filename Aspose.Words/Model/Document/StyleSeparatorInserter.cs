// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2017 by Dmitry Sokolov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Implements operation of the style separator insertion. Style separator can be inserted 
    /// using "ctrl" + "alt" + "enter" keys in the MSW.
    /// </summary>
    internal class StyleSeparatorInserter
    {
        /// <summary>
        /// Inserts style separator in the current position by the way like MSW does.
        /// </summary>
        /// <param name="builder">Document builder.</param>
        internal static void InsertStyleSeparator(DocumentBuilder builder)
        {
            Debug.Assert(builder != null);

            if (!CanInsertStyleSeparator(builder))
                return;

            StyleSeparatorInserter inserter = new StyleSeparatorInserter();
            inserter.mBuilder = builder;

            bool needInsertAgain = inserter.Insert();
            if(needInsertAgain)
                inserter.Insert();
        }

        /// <summary>
        /// Determines that style separator can be inserted into current position of the builder.
        /// </summary>
        private static bool CanInsertStyleSeparator(DocumentBuilder builder)
        {
            Paragraph para = builder.CurrentParagraph;
            CompositeNode parentShape = para.GetAncestor(NodeType.Shape);

            if (para.IsInMainTextStory && (parentShape == null))
                return true;
 
            WarningUtil.WarnUnexpected(builder.Document.WarningCallback, 
                "Insertion of style separator allowed in the main text story.");       

            return false;
        }

        // Hide public constructor.
        private StyleSeparatorInserter() { }

        /// <summary>
        /// Insertion of the style separator.
        /// </summary> 
        private bool Insert()
        {
            // 1. Determine paragraph to which style separator properties has to be applied and
            // paragraph with line break.
            StyleSeparatorContext context = ResolveStyleSeparatorContext();
            Paragraph styleSeparatorPara = context.StyleSeparatorParagraph;
            Paragraph lineBreakPara = context.LineBreakParagraph;

            if ((styleSeparatorPara == null) || (lineBreakPara == null))
                return false;

            // 2. Apply style separator properties.
            ApplyStyleSeparatorProperties(styleSeparatorPara);

            // 3. Insert paragraph with line break to the appropriate position in document tree (new paragraph was created).
            if (lineBreakPara.ParentNode == null)
            {
                if (context.LineBreakNextSibling != null)
                    context.LineBreakParentNode.InsertBefore(lineBreakPara, context.LineBreakNextSibling);
                else if (context.LineBreakPrevSibling != null)
                    context.LineBreakParentNode.InsertAfter(lineBreakPara, context.LineBreakPrevSibling);
            }

            // 4. Update content of the paragraph with line break and move document builder if it is needed. 
            StyleSeparatorPostProcess(context);
                    
            return context.IsRepeatedInsertExpected;
        }

        /// <summary>
        /// Applies style separator properties to the specified paragraph.
        /// </summary>
        private static void ApplyStyleSeparatorProperties(Paragraph para)
        {
            Debug.Assert(para != null);

            para.ParagraphBreakRunPr[FontAttr.SpecialHidden] = AttrBoolEx.True;
            para.ParagraphBreakRunPr[FontAttr.Hidden] = AttrBoolEx.True;
        }
        
        /// <summary>
        /// Implements post processing after insertion of the style separator.
        /// </summary>
        private void StyleSeparatorPostProcess(StyleSeparatorContext context)
        {
            Debug.Assert(context != null);

            Paragraph lineBreakPara = context.LineBreakParagraph;

            // 1. Update line break paragraph content.
            Run run = lineBreakPara.FirstRun;
            bool hasText = (run != null) && StringUtil.HasChars(run.Text);
            bool startWithSpace = hasText && run.Text.StartsWith(gSpaceStr, StringComparison.Ordinal);

            // MSW does not append the space for first paragraph into SDT and for last empty
            // paragraph in cell.
            if (!context.IsRepeatedInsertExpected && 
                (!lineBreakPara.IsEndOfCell || !lineBreakPara.IsEmptyOrContainsOnlyCrossAnnotation))
            {
                if (!hasText)
                    lineBreakPara.AppendChild(new Run(Doc, gSpaceStr));

                if (!lineBreakPara.FirstRun.Text.StartsWith(gSpaceStr, StringComparison.Ordinal))
                    lineBreakPara.PrependChild(new Run(Doc, gSpaceStr));
            }

            // 2. Move position of the builder.
            if (IsNeedMovePositon(context.StyleSeparatorParagraph))
            {
                if (!hasText)
                    MoveTo(lineBreakPara);
                else if (startWithSpace && !context.IsRepeatedInsertExpected)
                    MoveTo(lineBreakPara.FirstRun);
                else
                    MoveTo(lineBreakPara.Runs[1]);
            }
        }

        /// <summary>
        /// Detects that current position of the document builder has to be changed.
        /// </summary>
        private bool IsNeedMovePositon(Paragraph styleSeparatorPara)
        {
            if (!ReferenceEquals(CurrentParagraph, styleSeparatorPara) || !IsAtEndOfParagraph)
                return false;

            // Step here when current paragraph of the document builder is style separator paragraph
            // and document builder is positioned in the end of paragraph.

            // Builder has to move to new position when current paragraph not in SDT.
            CompositeNode curParent = styleSeparatorPara.ParentNode;
            if (curParent.NodeType != NodeType.StructuredDocumentTag)
                return true;

            // Builder has not to change position when current paragraph is last paragraph in SDT.
            if (curParent.LastNonMarkupCompositeDescendant == styleSeparatorPara)
                return false;

            return true;
        } 

        /// <summary>
        /// Populates style separator context.
        /// </summary>
        private StyleSeparatorContext ResolveStyleSeparatorContext()
        {
            StyleSeparatorContext context = new StyleSeparatorContext();        
            // Detect style separator paragraph for context.
            ResolveStyleSeparatorParagraph(context, CurrentParagraph);

            // Style separator paragraph can not be determined.
            if (context.StyleSeparatorParagraph == null)
                return context;

            // Determine paragraph with line break. 
            ResolveLineBreakParagraph(context, context.StyleSeparatorParagraph);

            return context;
        }

        /// <summary>
        /// Calculates paragraph with line break and set it in to context.
        /// </summary>
        private void ResolveLineBreakParagraph(StyleSeparatorContext context, Node currentNode)
        {
            Debug.Assert(context != null);

            bool isCurNodeStyleSeparator;

            while (currentNode != null)
            {
                if (currentNode.NodeType == NodeType.Table)
                    context.IsRepeatedInsertExpected = false;

                if (IsBlockLevelSDT(currentNode))
                {
                    // MSW appends additional paragraph before SDT with the space. However, if reopen document
                    // and insert style separator then MSW append one more additional paragraph. So, it looks like
                    // AW has to store some context for this case in the document. Currently simplified logic was
                    // implemented: fact that additional paragraph was inserted determined by it content.
                    context.IsRepeatedInsertExpected = !context.StyleSeparatorParagraph.HasNonWhitespaceChildren;
   
                    if (!context.IsRepeatedInsertExpected)
                    {
                        // Append additional paragraph.
                        context.LineBreakNextSibling = currentNode;
                        context.LineBreakParagraph = new Paragraph(Doc);
                        context.LineBreakParentNode = currentNode.ParentNode;
                        break;
                    }

                    // Additional paragraph already exist and position of the document builder has to be changed
                    // after insertion of the style separator. At this case MSW does nothing.
                    if (IsNeedMovePositon(context.StyleSeparatorParagraph))
                        break;
                }

                isCurNodeStyleSeparator = (context.StyleSeparatorParagraph == currentNode);

                // Next paragraph after style separator is what are looking for.
                if (currentNode.NodeType == NodeType.Paragraph && 
                    (!isCurNodeStyleSeparator || context.StyleSeparatorParagraph.IsEndOfSection))
                {             
                    if (isCurNodeStyleSeparator)
                    {
                        context.LineBreakParagraph = new Paragraph(Doc);
                        context.LineBreakPrevSibling = currentNode;                         
                    }
                    else
                    {
                        context.LineBreakParagraph = (Paragraph)currentNode;
                    }

                    context.LineBreakParentNode = currentNode.ParentNode;
                    break;                
                }

                currentNode = currentNode.NextPreOrder(Doc);
            }                       
        }

        /// <summary>
        /// Calculates style separator paragraph and set it in to context.
        /// </summary>  
        private void ResolveStyleSeparatorParagraph(StyleSeparatorContext context, Node currentNode)
        {
            Debug.Assert(context != null);
           
            while (currentNode != null)
            {
                if (currentNode.NodeType == NodeType.Paragraph)
                {
                    // Current style separator paragraph is the last paragraph in the section and this paragraph is
                    // last paragraph in the cell.
                    // MSW just skip any processing at this case.
                    Paragraph para = (Paragraph)currentNode;

                    // MSW do nothing when positioned in the last paragraph of the cell or in the end
                    // of the not last section or in the last paragraph of the last section which positioned inside SDT.
                    if (para.IsEndOfCell || (para.IsEndOfSection && IsBlockLevelSDT(para.ParentNode) &&
                                             para.IsEmptyOrContainsOnlyCrossAnnotation))
                    {
                        context.StyleSeparatorParagraph = null;
                        break;
                    }

                    // Paragraph without style separator attributes is what are looking for.
                    RunPr paraRunPr = para.ParagraphBreakRunPr;
                    if (!paraRunPr.ContainsKey(FontAttr.SpecialHidden) || !paraRunPr.ContainsKey(FontAttr.Hidden))
                    {
                        context.StyleSeparatorParagraph = para;
                        break;
                    }
                }

                currentNode = currentNode.NextPreOrder(Doc);
            }
        }    

        /// <summary>
        /// Returns true when current node is block level structured document tag.
        /// </summary>
        private static bool IsBlockLevelSDT(Node node)
        {
            Debug.Assert(node != null);

            return (node.NodeType == NodeType.StructuredDocumentTag) && NodeUtil.IsBlockLevelNode(node);
        }

        /// <summary>
        /// Moves position of the document builder to the specified node.
        /// </summary>
        private void MoveTo(Node node)
        {
            mBuilder.MoveTo(node);
        }

        /// <summary>
        /// Gets the document object which is attached to document builder.
        /// </summary>
        private Document Doc
        {
            get { return mBuilder.Document; }
        }

        /// <summary>
        /// Returns the paragraph that is currently selected in the document builder.
        /// </summary>
        private Paragraph CurrentParagraph
        {
            get { return mBuilder.CurrentParagraph; }
        }

        /// <summary>
        /// Returns true if the cursor of the document builder is at the end of the current paragraph.
        /// CrossStructureAnnotations must be ignored.
        /// </summary>
        private bool IsAtEndOfParagraph
        {
            get
            {
                Node cursor = mBuilder.CurrentNode;
                if (cursor == null)
                    return true;

                // WORDSNET-19622 CrossStructureAnnotations must be ignored.
                // Walk all nodes from the end of the paragraph backwards.
                // If we have something other than CrossStructureAnnotation, it means we are not at the end of the paragraph.
                Node node = CurrentParagraph.LastChild;
                while ((node != null) && (node != cursor))
                {
                    if (NodeUtil.IsCrossStructureAnnotation(node))
                        node = node.PreviousSibling;
                    else
                        return false;
                }
                return NodeUtil.IsCrossStructureAnnotation(node);
            }
        }

        /// <summary>
        /// Document builder.
        /// </summary>
        private DocumentBuilder mBuilder;

        /// <summary>
        /// Space string.
        /// </summary>
        private static string gSpaceStr = " ";
    }
}
