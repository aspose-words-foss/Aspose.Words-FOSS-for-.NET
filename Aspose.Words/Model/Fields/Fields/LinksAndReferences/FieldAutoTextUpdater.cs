// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2017 by Edward Voronov

using Aspose.Words.BuildingBlocks;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Updates the <see cref="FieldAutoText"/> and <see cref="FieldGlossary"/> fields.
    /// </summary>
    internal class FieldAutoTextUpdater
    {
        private FieldAutoTextUpdater(Field field, IFieldAutoTextCode fieldCode)
        {
            mField = field;
            mFieldCode = fieldCode;
        }

        internal static FieldUpdateAction Update(Field field)
        {
            FieldAutoTextUpdater updater = new FieldAutoTextUpdater(field, (IFieldAutoTextCode)field);
            return updater.Update();
        }

        private FieldUpdateAction Update()
        {
            if (!StringUtil.HasChars(mFieldCode.EntryName))
                return new FieldUpdateActionInsertErrorMessage(mField, "Error! No AutoText entry specified.");

            BuildingBlock buildingBlock = mField.Updater.AutoTextEntryExtractor.Extract(mFieldCode.EntryName);
            if (buildingBlock == null)
                return null;

            using (mField.UpdateContext.RemoveOldResultSafe())
            {
                NodeRange result = FieldUtil.BuildFieldResultNodeRange(buildingBlock.FirstSection, buildingBlock.LastSection);
                ExternalDocumentModifier modifier = new ExternalDocumentModifier(result.Document, mField.FetchDocument(), ImportFormatMode.UseDestinationStyles);
                NodeCopier.Copy(result, mField.End, modifier, null,
                    NodeCopierOptions.UseSourceStartAncestorPr | NodeCopierOptions.CloneNode);
                CopyFirstParagraphAttrs(result, modifier);
            }

            return new FieldUpdateActionFormatResult(mField);
        }

        private void CopyFirstParagraphAttrs(NodeRange result, INodeModifier modifier)
        {
            if (result.IsInSameAncestor(NodeType.Paragraph))
                return;

            Paragraph sourcePara = (Paragraph)result.Start.Node.GetAncestor(NodeType.Paragraph);
            if (sourcePara != null)
            {
                sourcePara = (Paragraph)modifier.Modify(sourcePara, sourcePara, false, null);

                Paragraph resultPara = mField.Start.ParentParagraph;
                resultPara.ParaPr = sourcePara.ParaPr.Clone();
                resultPara.ParagraphBreakRunPr = sourcePara.ParagraphBreakRunPr.Clone();
            }
        }

        private readonly Field mField;
        private readonly IFieldAutoTextCode mFieldCode;
    }
}
