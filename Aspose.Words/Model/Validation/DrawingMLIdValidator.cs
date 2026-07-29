// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/12/2012 by Andrey Noskov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Used to validate DrawingMLs (checks that there is no duplicated ids to avoid producing non-conformant documents).
    /// </summary>
    /// <remarks>
    /// andrnosk: WORDSNET-7343 If multiple DrawingML objects within the same document 
    /// share the same id attribute value, then the document shall be considered non-conformant. (ISO_IEC_29500-1_2011->20.4.2.5)
    /// </remarks>
    internal class DrawingMLIdValidator
    {
        internal VisitorAction VisitDrawingML(ShapeBase drawingML)
        {
            CollectNextTxbx(drawingML);
            mDrawindMls.Add(drawingML);        
            
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Validate DrawingML ids to avoid duplication and references of the linked shapes.
        /// </summary>
        internal void Validate()
        {          
            IntToObjDictionary<ShapeBase> shapes = new IntToObjDictionary<ShapeBase>();
            // Stores textboxes to recover links after VML to DML conversion.
            IntToObjDictionary<ShapeBase> brokenTxbx = new IntToObjDictionary<ShapeBase>();            

            foreach (ShapeBase dml in mDrawindMls)
            {
                // WORDSNET-17926 ShapeId might be missing.
                // Assign any value it will be validated below anyway.
                int prevId = dml.ShapePr.Contains(ShapeAttr.ShapeId)
                    ? (int)dml.ShapePr[ShapeAttr.ShapeId]
                    : 0;

                if (dml.IsTopLevel)
                {                   
                    int newUniqueId = DrawingMLIdManager.AddUniqueId(prevId);

                    // andrnosk: WORDSNET-7398 MS Word 2010 crashes when id=0, 
                    // this situation is possible just it test mode but we still need to avoid this.
                    dml.ShapePr[ShapeAttr.ShapeId] = (newUniqueId == 0)
                        ? DrawingMLIdManager.AddUniqueId(newUniqueId)
                        : newUniqueId;
                }                

                if ((dml.DmlNode.NonVisualPr != null) && (dml.DmlNode.NonVisualPr.NvDrawingProperties != null))
                {
                    // WORDSNET-13577 wp:docPr->id and wps:cNvPr->id have to be unique within the current document.
                    DmlNvDrawingProperties nvPr = dml.DmlNode.NonVisualPr.NvDrawingProperties;
                    
                    if (dml.IsTopLevel)
                    {
                        // WORDSNET-19042 Set the image id to get the same results when save the same document.
                        nvPr.Id = (int)dml.ShapePr[ShapeAttr.ShapeId];
                    }
                    else
                    {
                        int newUniqueNvPrId = DrawingMLIdManager.AddUniqueId(nvPr.Id);

                        nvPr.Id = (newUniqueNvPrId == 0)
                            ? DrawingMLIdManager.AddUniqueId(newUniqueNvPrId)
                            : newUniqueNvPrId;
                    }
                }

                // It is necessary to update relations between shapes after changes in shape identifiers.
                UpdateNextTxbxId(prevId, dml.Id);

                // Collect textboxes with unassigned identifiers to recover chain on textbox validation.
                if (dml.HasTextbox && dml.TextboxNextShapeId > 0 && dml.TextboxId == 0 && dml.LinkedTextboxId == 0)
                    brokenTxbx[dml.Id] = dml;

                if (dml.DmlNode.DmlNodeType == DmlNodeType.WordprocessingShape)
                    shapes[dml.Id] = dml;
            }

            ValidateTxbx(shapes, brokenTxbx);
        }

        /// <summary>
        /// Update identifiers for current shape (textbox).
        /// </summary>
        /// <param name="dml">Shape to update identifiers.</param>
        /// <param name="lnkItems">Linked items collection.</param>
        /// <param name="doc">Document which holds specified shape.</param>
        internal static void UpdateTxbxId(ShapeBase dml, SortedIntegerListGeneric<ShapeBase> lnkItems, DocumentBase doc)
        {
            Debug.Assert((dml != null) && (lnkItems != null) && (doc != null));

            // Update to unique value.     
            if (dml.TextboxId > 0)
                dml.TextboxId = doc.GetNextDmlTextBoxId();

            // Assign unique number from document level to avoid intersection of the values.      
            int currentId = doc.MapShapeToRange(doc.GetNextShapeId(dml), lnkItems.Count);
            int nextId = doc.GetNextShapeId();

            dml.ShapePr.SetAttr(ShapeAttr.ShapeId, currentId);
            dml.ShapePr.SetAttr(ShapeAttr.TextboxNextShapeId, nextId);

            // Set values in fallback shape.
            if (dml.FallbackShape != null)
            {
                dml.FallbackShape.ShapePr.SetAttr(ShapeAttr.ShapeId, currentId);
                dml.FallbackShape.ShapePr.SetAttr(ShapeAttr.TextboxNextShapeId, nextId);
            }

            UpdateLinkedTextboxes(dml, lnkItems, doc);
        }

        /// <summary>
        /// Processes linked textboxes identifiers.
        /// </summary>
        /// <param name="dmlShapes">Collection of the drawingML textboxes."</param>
        /// <param name="dmlLinkedShapes">Dictionary of the linked textboxes stores by textbox identifier.</param>       
        internal static void ProcessLinkedTxbxId(IntToObjDictionary<ShapeBase> dmlShapes, 
            Dictionary<int, SortedIntegerListGeneric<ShapeBase>> dmlLinkedShapes)
        {
            Debug.Assert((dmlShapes != null) && (dmlLinkedShapes != null));

            foreach (ShapeBase dml in dmlShapes.Values)
            {
                SortedIntegerListGeneric<ShapeBase> lnkItems = dmlLinkedShapes.GetValueOrNull(dml.TextboxId);

                // Skip identifiers updating for not linked shapes.            
                if (lnkItems == null)
                    continue;

                UpdateTxbxId(dml, lnkItems, dml.Document);
            }
        }

        /// <summary>
        /// Assigns identifiers to items into linked textboxes chain.
        /// </summary>       
        /// <param name="txbx">First textbox in the sequence of the textboxes.</param>
        /// <param name="lnkItems">Collection of linked textboxes of one chain.</param>
        /// <param name="doc">>Document which holds specified textbox.</param>
        private static void UpdateLinkedTextboxes(ShapeBase txbx, SortedIntegerListGeneric<ShapeBase> lnkItems, DocumentBase doc)
        {
            Debug.Assert((txbx != null) && (lnkItems != null) && (doc != null));

            int txbxId = txbx.TextboxId;
            int nextId = (int)txbx.ShapePr[ShapeAttr.TextboxNextShapeId];

            for (int i = 0; i < lnkItems.Count; i++)
            {
                Shape nextTextbox = (Shape)lnkItems.GetByIndex(i);

                nextTextbox.LinkedTextboxId = txbxId;
                nextTextbox.ShapePr.SetAttr(ShapeAttr.ShapeId, nextId);

                ShapeBase nextFallbackShape = nextTextbox.FallbackShape;
                if (nextFallbackShape != null)
                    nextFallbackShape.ShapePr.SetAttr(ShapeAttr.ShapeId, nextId);

                // The last linked textbox in chain does not have TextboxNextShapeId set.
                if (i < (lnkItems.Count - 1))
                {
                    nextId = doc.GetNextShapeId();
                    nextTextbox.ShapePr.SetAttr(ShapeAttr.TextboxNextShapeId, nextId);
                    if (nextFallbackShape != null)
                        nextFallbackShape.ShapePr.SetAttr(ShapeAttr.TextboxNextShapeId, nextId);
                }
            }
        }

        /// <summary>
        /// Collects textboxes which refer to another.
        /// </summary>
        /// <param name="dml">Shape to collect.</param>
        private void CollectNextTxbx(ShapeBase dml)
        {
            Debug.Assert(dml != null);

            if (dml.TextboxNextShapeId == 0)
                return;

            int nextId = dml.TextboxNextShapeId;

            if (!mNextTxbx.ContainsKey(nextId))
                mNextTxbx[nextId] = new List<ShapeBase>();

            mNextTxbx[nextId].Add(dml); 
        }

        /// <summary>
        /// Updates relations between textboxes after changes in shape identifiers.
        /// After processing in this method shapes still can hold links to textbox which does not exist.
        /// </summary>
        /// <param name="prevId">Previous identifier of the shape.</param>
        /// <param name="newNextId">New identifier of the shape.</param>
        private void UpdateNextTxbxId(int prevId, int newNextId)
        {
            if (!mNextTxbx.ContainsKey(prevId))
                return;

            List<ShapeBase> dmls = mNextTxbx[prevId];

            // Replace reference for first shape in the sequence.
            ShapeBase dml = dmls[0];
            dml.TextboxNextShapeId = newNextId;

            // Another shapes have duplicate relationships.
            // This code for resilience purposes, on this step it is necessary to resolve
            // invalid relations between textboxes.
            for (int i = 1; i < dmls.Count; ++i)
                InvalidateNextTxbx(dml);
           
            // Remove items with updated references.           
            mNextTxbx.Remove(prevId);
        }   

        /// <summary>
        /// Validates textbox identifiers, linked textbox identifiers and links between them.
        /// </summary>
        /// <param name="shapes">Dictionary which contains textbox and linked textbox shapes.</param>
        /// <param name="brokenTxbx">Dictionary with textboxes which have to be recovered.</param>
        private void ValidateTxbx(IntToObjDictionary<ShapeBase> shapes, IntToObjDictionary<ShapeBase> brokenTxbx)
        {
            Debug.Assert((shapes != null) && (brokenTxbx != null));

            // Invalidate links to next textbox for shape which has wrong references to it.
            // This code for resilience purposes, on this step it is necessary to resolve
            // invalid relations between textboxes.
            foreach (List<ShapeBase> txbxes in mNextTxbx.Values)
                foreach (ShapeBase txbx in txbxes)
                    InvalidateNextTxbx(txbx);

            // Here dictionary does not hold valid references to items, so clear it.
            mNextTxbx.Clear();

            // Key is a textbox identifier, value is a sorted collection of the linked textboxes.
            Dictionary<int, SortedIntegerListGeneric<ShapeBase>> linkedChains = 
                new Dictionary<int, SortedIntegerListGeneric<ShapeBase>>();            

            // Assign identifiers for textboxes and linked textboxes according to relations between shapes.
            // Textbox identifiers and linked textbox identifiers are not assigning during VML to DML conversion.
            foreach (ShapeBase txbx in brokenTxbx.Values)
            {
                int seqNum = 1;
                ShapeBase nextTxbx = GetNextTxbx(txbx, shapes);
                int txbxId = (nextTxbx != null) ? txbx.Document.GetNextDmlTextBoxId() : 0;

                while (nextTxbx != null)
                {
                    txbx.TextboxId = txbxId;
                    nextTxbx.LinkedTextboxId = txbxId;
                    nextTxbx.LinkedTextboxSeq = seqNum++;

                    if (!linkedChains.ContainsKey(txbx.TextboxId))
                        linkedChains[txbx.TextboxId] = new SortedIntegerListGeneric<ShapeBase>();

                    SortedIntegerListGeneric<ShapeBase> linkedTextboxes = linkedChains[txbx.TextboxId];
                    linkedTextboxes[nextTxbx.LinkedTextboxSeq] = nextTxbx;

                    nextTxbx = GetNextTxbx(nextTxbx, shapes);
                }
            }

            ProcessLinkedTxbxId(brokenTxbx, linkedChains);    
        }

        /// <summary>
        /// Returns next textbox for shape.
        /// </summary>
        /// <param name="dml">Shape with reference to next linked textbox.</param>
        /// <param name="shapes">Dictionary which contains textboxes and linked textboxes.</param>       
        /// <returns>Next textbox or "null" when reference to next textbox does not specified.</returns>
        private static ShapeBase GetNextTxbx(ShapeBase dml, IntToObjDictionary<ShapeBase> shapes)
        {
            Debug.Assert((dml != null) && (shapes != null));

            return (dml.TextboxNextShapeId > 0) ? shapes[dml.TextboxNextShapeId] : null;
        }

        /// <summary>
        /// Handles all id-related tasks for DrawingMLs.
        /// </summary>
        private UniqueIdManager DrawingMLIdManager
        {
            get
            {
                if (mDrawingMLIdManager == null)
                    mDrawingMLIdManager = new UniqueIdManager();

                return mDrawingMLIdManager;
            }
        }

        /// <summary>
        /// Removes reference to next textbox and registers warning. 
        /// </summary>
        /// <param name="dml">Shape to remove link.</param>
        private static void InvalidateNextTxbx(ShapeBase dml)
        {
            Debug.Assert(dml != null);

            dml.RemoveShapeAttrInternal(ShapeAttr.TextboxNextShapeId);
            WarningUtil.Warn(dml.Document.WarningCallback, WarningType.MinorFormattingLoss,
                    WarningSource.Validator, WarningStrings.InvalidTextboxLink, dml.Id);
        }

        /// <summary>
        /// Handles uniqueness of Ids across DrawingMLs in the document descendants. 
        /// This applies both to Document and GlossaryDocument, as they both can host DrawingMLs.
        /// </summary>
        private UniqueIdManager mDrawingMLIdManager;

        /// <summary>
        /// Container for DrawingMLs. Keys
        /// </summary>
        private readonly List<ShapeBase> mDrawindMls = new List<ShapeBase>();

        /// <summary>
        /// Stores shapes with textboxes that is references by another shapes.
        /// Elements will be deleted from the dictionary on shape identifiers validation.
        /// </summary>
        private readonly Dictionary<int, List<ShapeBase>> mNextTxbx = new Dictionary<int, List<ShapeBase>>();
    }
}
