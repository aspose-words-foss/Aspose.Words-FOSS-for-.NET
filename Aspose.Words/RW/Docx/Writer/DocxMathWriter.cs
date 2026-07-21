// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2011 by Denis Darkin

using System;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Math;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Able to write m:oMath and m:oMathPara root elements and all their Office Math children.
    /// </summary>
    internal static class DocxMathWriter
    {
        internal static void WriteStart(MathObject mathObject, IMathRunPr runPr, IMathWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartMath();
            switch (mathObject.MathObjectType)
            {
                case MathObjectType.OMath:
                    builder.StartElement("m:oMath");
                    break;
                case MathObjectType.OMathPara:
                    WriteOMathPara(mathObject, writer);
                    break;
                case MathObjectType.Accent:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:acc");
                    break;
                case MathObjectType.Bar:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:bar");
                    break;
                case MathObjectType.BorderBox:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:borderBox");
                    break;
                case MathObjectType.Box:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:box");
                    break;
                case MathObjectType.Delimiter:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:d");
                    break;
                case MathObjectType.Array:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:eqArr");
                    break;
                case MathObjectType.GroupCharacter:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:groupChr");
                    break;
                case MathObjectType.Phantom:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:phant");
                    break;
                case MathObjectType.Degree:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:deg");
                    break;
                case MathObjectType.Argument:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:e");
                    break;
                case MathObjectType.Fraction:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:f");
                    break;
                case MathObjectType.Denominator:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:den");
                    break;
                case MathObjectType.Numerator:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:num");
                    break;
                case MathObjectType.Function:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:func");
                    break;
                case MathObjectType.FunctionName:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:fName");
                    break;
                case MathObjectType.Limit:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:lim");
                    break;
                case MathObjectType.LowerLimit:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:limLow");
                    break;
                case MathObjectType.UpperLimit:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:limUpp");
                    break;
                case MathObjectType.Matrix:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:m");
                    break;
                case MathObjectType.MatrixRow:
                    builder.StartElement("m:mr");
                    break;
                case MathObjectType.NAry:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:nary");
                    break;
                case MathObjectType.Radical:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:rad");
                    break;
                case MathObjectType.SubscriptPart:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:sub");
                    break;
                case MathObjectType.SuperscriptPart:
                    WriteMathObjectWithArgPr(mathObject, runPr, writer, "m:sup");
                    break;
                case MathObjectType.PreSubSuperscript:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:sPre");
                    break;
                case MathObjectType.Subscript:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:sSub");
                    break;
                case MathObjectType.SubSuperscript:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:sSubSup");
                    break;
                case MathObjectType.Supercript:
                    WriteMathObjectWithObjPr(mathObject, runPr, writer, "m:sSup");
                    break;
                default:
                    throw new InvalidOperationException("Please report exception.");
            }
        }

        /// <summary>
        /// This function could have been merged with <see cref="WriteMathObjectWithObjPr"/> but
        /// it is a special case separated from it to simplify writing.
        /// </summary>
        private static void WriteOMathPara(MathObject mathObject, IMathWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("m:oMathPara");
            if (mathObject.Count > 0)
            {
                builder.StartElement("m:oMathParaPr");
                WriteMathAttrs(mathObject, writer);
                builder.EndElement();
            }
        }

        private static void WriteMathObjectWithObjPr(MathObject mathObject, IMathRunPr runPr, IMathWriterContext writer,
            string tagName)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(tagName);

            if ((mathObject.Count > 0) || (NeedToWriteCtrlPr(runPr)) || IsNonEmptyMatrix(mathObject))
            {
                builder.StartElement(tagName+"Pr");
                WriteMathAttrs(mathObject, writer);

                if (IsNonEmptyMatrix(mathObject))
                    WriteMcs(((MathObjectMatrix)mathObject).ColumnPrCollection, builder);

                WriteControlPr(runPr, writer);

                builder.EndElement();
            }
        }

        private static bool IsNonEmptyMatrix(MathObject m)
        {
            if (m.MathObjectType == MathObjectType.Matrix)
                return ((MathObjectMatrix)m).ColumnPrCollection.Count != 0;

            return false;
        }

        /// <summary>
        /// Write column-specific properties. If two or more columns have equal properties, then only one columnPr is written,
        /// but counter inside of it denotes number of columns this pr applies to.
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="builder"></param>
        private static void WriteMcs(MathMatrixColumnPrCollection columns, NrxXmlBuilder builder)
        {
            builder.StartElement("m:mcs");

            int counter = 1;
            for (int i = 0; i < columns.Count; i++)
            {
                MathMatrixColumnPr cur = columns[i];
                MathMatrixColumnPr next = (i + 1 < columns.Count) ? columns[i + 1] : null;
                if ((next == null) || (cur.HorizontalAlignment != next.HorizontalAlignment) || (counter > MaxCountAllowedByWord))
                {

                    // cur is the last el or alignments mismatch, write cur alignment.
                    WriteColumnPr(cur, counter, builder);
                    counter = 1;
                }
                else if (cur.HorizontalAlignment == next.HorizontalAlignment)
                {
                    counter++;
                }
            }

            builder.EndElement();
        }

        private static void WriteColumnPr(MathMatrixColumnPr columnPr, int count, NrxXmlBuilder builder)
        {
            builder.StartElement("m:mc");
            builder.StartElement("m:mcPr");

            // WORDSNET-26355 OOXML Spec says  count == 1 is default and we shouldn't write it
            // MS Word displays visual difference in matrices if we omit writing count == 1
            builder.WriteVal("m:count", count);

            builder.WriteVal("m:mcJc", StyleConvertUtil.HorizontalAlignmentToXml(columnPr.HorizontalAlignment));

            builder.EndElement();
            builder.EndElement();
        }

        private static void WriteMathAttrs(AttrCollection attrs, IMathWriterContext writer)
        {
            for (int iAttr = 0; iAttr < attrs.Count; iAttr++)
                ProcessAttr(attrs.GetKey(iAttr), attrs.GetByIndex(iAttr), writer);
        }

        private static void ProcessAttr(int key, object value, IMathWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            switch (key)
            {
                case MathAttr.Justification:
                    builder.WriteVal("m:jc", DocxEnum.MathJustificationToDocx((OfficeMathJustification)value));
                    break;
                case MathAttr.GroupChar:
                case MathAttr.AccentCharacter:
                case MathAttr.NaryChar:
                    WriteChar(builder, "m:chr", (char)value);
                    break;
                case MathAttr.Position:
                case MathAttr.GroupCharPosition:
                    builder.WriteVal("m:pos", DocxEnum.MathPositionTypeToDocx((MathPosition)value));
                    break;
                case MathAttr.HideBottom:
                    builder.WriteVal("m:hideBot", (bool)value);
                    break;
                case MathAttr.HideLeft:
                    builder.WriteVal("m:hideLeft", (bool)value);
                    break;
                case MathAttr.HideRight:
                    builder.WriteVal("m:hideRight", (bool)value);
                    break;
                case MathAttr.HideTop:
                    builder.WriteVal("m:hideTop", (bool)value);
                    break;
                case MathAttr.StrikeBLTR:
                    builder.WriteVal("m:strikeBLTR", (bool)value);
                    break;
                case MathAttr.StrikeH:
                    builder.WriteVal("m:strikeH", (bool)value);
                    break;
                case MathAttr.StrikeTLBR:
                    builder.WriteVal("m:strikeTLBR", (bool)value);
                    break;
                case MathAttr.StrikeV:
                    builder.WriteVal("m:strikeV", (bool)value);
                    break;
                case MathAttr.IsAlignmentPoint:
                    builder.WriteVal("m:aln", (bool)value);
                    break;
                case MathAttr.LineBreak:
                    WriteLineBreak((MathLineBreak)value, builder);
                    break;
                case MathAttr.IsDifferential:
                    builder.WriteVal("m:diff", (bool)value);
                    break;
                case MathAttr.NoBreaks:
                    builder.WriteVal("m:noBreak", (bool)value);
                    break;
                case MathAttr.IsOpEmu:
                    builder.WriteVal("m:opEmu", (bool)value);
                    break;
                case MathAttr.BeginChar:
                    WriteChar(builder, "m:begChr", (char)value);
                    break;
                case MathAttr.EndChar:
                    WriteChar(builder, "m:endChr", (char)value);
                    break;
                case MathAttr.SeparatorChar:
                    WriteChar(builder, "m:sepChr", (char)value);
                    break;
                case MathAttr.GrowOperand:
                    builder.WriteVal("m:grow", (bool)value);
                    break;
                case MathAttr.DelimiterShape:
                    builder.WriteVal("m:shp", DocxEnum.MathDelimiterShapeToDocx((MathDelimiterShape)value));
                    break;
                case MathAttr.RowSpacingRule:
                    builder.WriteVal("m:rSpRule", (int)value);
                    break;
                case MathAttr.RowSpacing:
                    builder.WriteVal("m:rSp", (int)value);
                    break;
                case MathAttr.BaseJustification:
                {
                    bool ooxmlCompliance = !writer.IsDocx || (writer.Compliance != OoxmlComplianceCore.Ecma376);

                    builder.WriteVal("m:baseJc",
                                     DocxEnum.MathBaseJustificationToDocx((MathBaseJustification) value, ooxmlCompliance));
                    break;
                }
                case MathAttr.MaxDist:
                    builder.WriteVal("m:maxDist", (bool)value);
                    break;
                case MathAttr.ObjDist:
                    builder.WriteVal("m:objDist", (bool)value);
                    break;
                case MathAttr.VerticalJustification:
                    {
                        MathVerticalJustification vj = (MathVerticalJustification)value;
                        if (vj != MathVerticalJustification.Bottom) // WORDSNET-26354
                            builder.WriteVal("m:vertJc", DocxEnum.MathVerticalJustificationTypeToDocx(vj));
                        else
                            builder.WriteEmptyElement("m:vertJc");
                        break;
                    }
                case MathAttr.IsShown:
                    builder.WriteVal("m:show", (bool)value);
                    break;
                case MathAttr.IsTransparent:
                    builder.WriteVal("m:transp", (bool)value);
                    break;
                case MathAttr.IsZeroAscent:
                    builder.WriteVal("m:zeroAsc", (bool)value);
                    break;
                case MathAttr.IsZeroDescent:
                    builder.WriteVal("m:zeroDesc", (bool)value);
                    break;
                case MathAttr.IsZeroWidth:
                    builder.WriteVal("m:zeroWid", (bool)value);
                    break;
                case MathAttr.FractionType:
                    builder.WriteVal("m:type", DocxEnum.MathFractionTypeToDocx((MathFractionType)value));
                    break;
                case MathAttr.ColumnGapRule:
                    builder.WriteVal("m:cGpRule", (int)value);
                    break;
                case MathAttr.ColumnGap:
                    builder.WriteVal("m:cGp", (int)value);
                    break;
                case MathAttr.IsHidePlaceholders:
                    builder.WriteVal("m:plcHide",(bool)value);
                    break;
                case MathAttr.MinColumnWidth:
                    builder.WriteVal("m:cSp", (int)value);
                    break;
                case MathAttr.LimitLocation:
                    builder.WriteVal("m:limLoc", DocxEnum.MathLimitLocationToDocx((MathLimitLocation)value));
                    break;
                case MathAttr.IsHideSubscript:
                    builder.WriteVal("m:subHide", (bool)value);
                    break;
                case MathAttr.IsHideSuperscript:
                    builder.WriteVal("m:supHide", (bool)value);
                    break;
                case MathAttr.DegreeHide:
                    builder.WriteVal("m:degHide", (bool)value);
                    break;
                case MathAttr.IsAlignScripts:
                    builder.WriteVal("m:alnScr", (bool)value);
                    break;
                default:
                    // Ignore this unknown attribute.
                    break;
            }
        }

        internal static void WriteLineBreak(MathLineBreak lineBreak, NrxXmlBuilder builder)
        {
            if (lineBreak.IsDefaultAlignment)
                builder.WriteEmptyElement("m:brk");
            else
                builder.WriteElementWithAttributes("m:brk", new object[] { "m:alnAt", lineBreak.Alignment.ToString() });
        }

        private static void WriteMathObjectWithArgPr(MathObject mathObject, IMathRunPr runPr, IMathWriterContext writer,
            string tagName)
        {

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(tagName);

            WriteControlPr(runPr, writer);

            MathObjectArgumentBase arg = (MathObjectArgumentBase)mathObject;
            int argSize = arg.ArgumentSize;
            if (argSize != MathObjectArgumentBase.DefaultArgumentSize)
            {
                builder.StartElement("m:argPr");
                builder.WriteVal("m:argSz", argSize);
                builder.EndElement();
            }
        }

        private static bool NeedToWriteCtrlPr(IMathRunPr rPr)
        {
            return (rPr.Count != 0);// has non-empty rPr
        }

        private static void WriteControlPr(IMathRunPr rPr, IMathWriterContext writer)
        {
            if (!NeedToWriteCtrlPr(rPr))
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("m:ctrlPr");

            if (rPr.IsDml)
            {
                DmlTextShapeWriter.WriteDmlRunProperties("a:rPr", (DmlRunProperties)rPr, (IDmlShapeWriterContext)writer);
            }
            else
            {
                RunPr runPr = (RunPr)rPr;
                // Normal case is when only one revision is present. But if we receive two, need to write them.
                if (runPr.HasInsertRevision || runPr.HasDeleteRevision)
                {
                    if (runPr.HasInsertRevision)
                    {
                        builder.WriteRevisionStart(runPr.InsertRevision, writer.GetNextAnnotationId());
                        writer.WriteRunPr(runPr);
                        builder.WriteRevisionEnd();

                    }

                    if (runPr.HasDeleteRevision)
                    {
                        builder.WriteRevisionStart(runPr.DeleteRevision, writer.GetNextAnnotationId());
                        writer.WriteRunPr(runPr);
                        builder.WriteRevisionEnd();
                    }
                }
                else // write plain rPr only when no revisions exist
                {
                    writer.WriteRunPr(runPr);
                }
            }

            builder.EndElement("m:ctrlPr");
    }

        internal static void WriteEnd(NrxXmlBuilder builder)
        {
            builder.EndElement();
            builder.EndMath();
        }

        private static void WriteChar(NrxXmlBuilder builder, string elementName, char elementValue)
        {
            string value = (elementValue != MathObject.EmptyCharacter) ? elementValue.ToString() : "";
            builder.StartElement(elementName);
            builder.WriteAttributeString("m:val", value);
            builder.EndElement(elementName);
        }

        /// <summary>
        /// The standard states that the values of are defined by ST_Integer255 simple type.
        /// MS Office restricts the value of this attribute to be at most 64.
        /// </summary>
        private const int MaxCountAllowedByWord = 64;
    }
}
