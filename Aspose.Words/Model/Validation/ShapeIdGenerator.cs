// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2007 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Lists;
using Aspose.Words.Notes;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Generates shape identifiers. Should be used as part of the document validation before save.
    /// We do not maintain shape identifiers while in the model, but we have to set them straight before save.
    ///
    /// Updates <see cref="SaveInfo.Shapes"/> and <see cref="SaveInfo.LinkedShapeIds"/> collections.
    /// </summary>
    internal class ShapeIdGenerator
    {
        internal ShapeIdGenerator(SaveInfo saveInfo)
        {
            mSaveInfo = saveInfo;

            // Visit picture bullet shapes as they need ids too.
            ListCollection lists = saveInfo.Document.Lists;
            for (int i = 0; i < lists.PictureBulletCount; i++)
            {
                Shape shape = lists.GetPictureBullet(i);
                VisitShape(shape, false);
            }
        }

        internal void EndDocument()
        {
            int shapeId = ShapeBase.MainDrawingPatriarchShapeId + 1;

            if (mSaveInfo.Document.BackgroundShape != null)
                mSaveInfo.Document.BackgroundShape.Id = shapeId++;

            foreach (ShapeBase shape in mBodyShapes)
                UpdateShapeIds(shape, shapeId++);

            // Now we know last id used for the shapes in the body, we can generate ids for shapes in the headers/footers.
            // Shape ids are in segments, find the beginning of a next segment.
            int headerDrawingPatriarchShapeId = MathUtil.RoundUp(shapeId, ShapeBase.ShapeIdClusterSize);
            shapeId = headerDrawingPatriarchShapeId + 1;

            foreach (ShapeBase shape in mHeaderShapes)
                UpdateShapeIds(shape, shapeId++);

            // WORDSNET-5295 Update footnote separator's shape id.
            // AM. I believe this collection should be small so it won't be expensive task.
            foreach (FootnoteSeparator separator in mSaveInfo.Document.FootnoteSeparators)
            {
                // Update each shape in footnote separators.
                foreach (ShapeBase shape in separator.GetChildNodes(NodeType.Shape, true))
                    UpdateShapeIds(shape, shapeId++);
            }

            UpdateNextShapeIds();
            UpdateConnectors();
        }

        internal void VisitShape(ShapeBase shape, bool isInHeaderFooter)
        {
            if (isInHeaderFooter)
                mHeaderShapes.Add(shape);
            else
                mBodyShapes.Add(shape);

            // Collect all connectors.
            if (shape.ShapePr[ShapeAttr.ConnectorRule] != null)
                mConnectors.Add(shape);

            // Verify OleObject Id is unique.
            if (shape.IsOle)
                EnsureUniqueOleId(shape);
        }

        /// <summary>
        /// Implements checks for VML or DML shapes to cover cases which can cause
        /// conversion errors and can be excluded on initial steps of the shape validation.
        /// </summary>
        /// <param name="shape">Shape which has to be validated.</param>
        internal static void ValidateFluently(ShapeBase shape)
        {
            Debug.Assert(shape != null);

            // AM. Word sometimes has TextboxNextShapeId attribute referred for shape itself and sometimes not.
            // In order to reduce new golds needs to be accepted and simplify code I set this attribute only if shape is actually linked.
            if (shape.Id == shape.TextboxNextShapeId)
                shape.RemoveShapeAttrInternal(ShapeAttr.TextboxNextShapeId);
        }

        /// <summary>
        /// Updates ShapeAttr.TextboxNextShapeId attribute.
        /// </summary>
        /// <remarks>
        /// Shape ids are changed upon saving so ids of chained shape should be updated too.
        /// </remarks>
        private void UpdateNextShapeIds()
        {
            HashSetGeneric<int> linkTarget = new HashSetGeneric<int>();
            foreach (ShapeBase shape in mAllShapes)
            {
                int oldNextShapeId = shape.TextboxNextShapeId;
                // Remove the old linked shape id value so any broken links will be cleared.
                shape.RemoveShapeAttrInternal(ShapeAttr.TextboxNextShapeId);

                if (oldNextShapeId != 0)
                {
                    ShapeBase nextShape = mPreviousShapeIds.GetValueOrNull(oldNextShapeId);

                    // TestDefect815. TextboxNextShapeId contains Id of missed shape.
                    if (nextShape != null)
                    {
                        // TestJira9154. Shape in Header is linked to shape in Body.
                        if (linkTarget.Contains(nextShape.Id) || !HaveSameDocumentPart(shape, nextShape))
                        {
                            WarningUtil.WarnDataLoss(mSaveInfo.Document.WarningCallback, WarningSource.Validator, WarningStrings.InvalidTextboxLink, shape.Id);
                        }
                        else
                        {
                            shape.TextboxNextShapeId = nextShape.Id;

                            // Store the linked shape ids so this info can be accessible by the writers.
                            mSaveInfo.LinkedShapeIds.Add(shape.Id);
                            mSaveInfo.LinkedShapeIds.Add(nextShape.Id);

                            linkTarget.Add(nextShape.Id);
                        }
                    }
                }
            }
        }

        private void UpdateConnectors()
        {
            foreach (ShapeBase connectorShape in mConnectors)
            {
                ConnectorRule connectorRule = (ConnectorRule)connectorShape.ShapePr[ShapeAttr.ConnectorRule];

                ShapeBase shapeA = mPreviousShapeIds.GetValueOrNull(connectorRule.ShapeAId);
                ShapeBase shapeB = mPreviousShapeIds.GetValueOrNull(connectorRule.ShapeBId);

                if ((shapeA == null) || (shapeB == null))
                {
                    // If there is any problem remove connector rule.
                    WarningUtil.WarnDataLoss(mSaveInfo.Document.WarningCallback,
                        WarningSource.Validator, WarningStrings.InvalidConnectorRule, connectorShape.Id);
                    connectorShape.ShapePr.Remove(ShapeAttr.ConnectorRule);
                }
                else
                {
                    // Update shape ids.
                    connectorRule.ShapeAId = shapeA.Id;
                    connectorRule.ShapeBId = shapeB.Id;

                    // Collect connectors for writing.
                    Story shapeBody = (Story)connectorShape.GetAncestor(NodeType.Body);

                    if (shapeBody != null)
                        mSaveInfo.ConnectorsBody.Add(connectorShape);
                    else
                        mSaveInfo.ConnectorsHdr.Add(connectorShape);
                }
            }
        }

        /// <summary>
        /// Returns true if shapes are both belong to body or both belong to header/footer.
        /// </summary>
        private static bool HaveSameDocumentPart(ShapeBase shape1, ShapeBase shape2)
        {
            bool isBody1 = (shape1.GetAncestor(NodeType.HeaderFooter) == null);
            bool isBody2 = (shape2.GetAncestor(NodeType.HeaderFooter) == null);

            return (isBody1 == isBody2);
        }

        /// <summary>
        /// Set new shape id, while preserving old value in table.
        /// </summary>
        private void UpdateShapeIds(ShapeBase shape, int id)
        {
            // AM. I read TextboxTxId for unit testing. Remove it before saving.
            shape.RemoveShapeAttrInternal(ShapeAttr.TextboxTxid);

            // WORDSNET-10647 Shapes both belongs to headers but NextShapeId equal to 1025.
            // Such value belongs to body shape in general and Word ignores such link.
            if (!DocumentBase.HaveSameIdMap(shape.Id, shape.TextboxNextShapeId))
                shape.RemoveShapeAttrInternal(ShapeAttr.TextboxNextShapeId);

            // Before renumbering shapes we can encounter shapes with duplicate id. This can happen for example during
            // shape cloning. In this case only the first shape will be stored so only the first shape will be linked.
            // Further shapes with duplicate ids will simply be assigned new ids and will not be linked.
            // WORDSNET-14369 We can ignore picture bullet shapes.
            if ((!mPreviousShapeIds.ContainsKey(shape.Id) ||
                    // WORDSNET-27492 Prefer textboxes to be able to resolve TextboxNextShapeId.
                    (!mPreviousShapeIds[shape.Id].IsTextBox && shape.IsTextBox)) &&
                (shape.ParentNode != null))
            {
                mPreviousShapeIds[shape.Id] = shape;
            }

            mAllShapes.Add(shape);

            // Finally set the new shape id.
            shape.Id = id;

            // We store the map of id->shape in SaveInfo so this can be accessible for writers.
            mSaveInfo.Shapes[id] = shape;
        }

        /// <summary>
        /// Ensures uniqueness of a shape embedded object id.
        /// </summary>
        private void EnsureUniqueOleId(ShapeBase shape)
        {
            IEmbeddedObject embeddedObject = shape.EmbeddedObject;
            if (embeddedObject == null)
                return;

            // AM. I think non unique id will not be occurred too often so algorithm as simple as possible.
            while (mObjectIds.Contains(embeddedObject.Id))
                embeddedObject.Id++;

            mObjectIds.Add(embeddedObject.Id);
        }

        /// <summary>
        /// Provides all the input and output data for this class.
        /// </summary>
        private readonly SaveInfo mSaveInfo;
        /// <summary>
        /// Strictly speaking we do not need to cache these shapes as for headers/footers, but to make code consistent we cache them.
        /// This list also includes shapes for bullet pictures.
        /// </summary>
        private readonly List<ShapeBase> mBodyShapes = new List<ShapeBase>();
        /// <summary>
        /// We have to cache shapes in the header until the end of the document before we can assign ids to them.
        /// </summary>
        private readonly List<ShapeBase> mHeaderShapes = new List<ShapeBase>();
        /// <summary>
        /// Keeps old shape ids to be able to update id of linked shapes.
        /// </summary>
        private readonly Dictionary<int, ShapeBase> mPreviousShapeIds = new Dictionary<int, ShapeBase>();
        /// <summary>
        /// All shapes of the document. In comparison with <see cref="mPreviousShapeIds"/>, this list contains all
        /// shapes including ones that have duplicate IDs.
        /// </summary>
        private List<ShapeBase> mAllShapes = new List<ShapeBase>();
        /// <summary>
        /// Collects embedded object ids. Used to ensure ids uniqueness.
        /// </summary>
        private readonly HashSetGeneric<int> mObjectIds = new HashSetGeneric<int>();

        /// <summary>
        /// Collects connector shapes i.e shape which has connector rule attribute.
        /// </summary>
        private readonly List<ShapeBase> mConnectors = new List<ShapeBase>();
    }
}
