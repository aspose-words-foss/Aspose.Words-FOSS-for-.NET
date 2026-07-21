// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/16/2015 by Alexey Noskov

using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Words.Math;
using Aspose.Words.Nrx;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Utility class contains common methods for Dml and Docx math readers.
    /// In both Docx and Dml the same math syntax is used, but run properties and content are represented differently.
    /// Reading of run properties and content must be implemented in derived classes.
    /// </summary>
    internal static class NrxMathReaderUtil
    {
        /// <summary>
        /// Read root-level math object m:oMathPara
        /// </summary>
        internal static void ReadOMathPara(INrxMathReader reader)
        {
            MathObjectOMathPara mathParent = new MathObjectOMathPara();
            reader.AddAndPushMathContainer(mathParent);

            // Read elements.
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("oMathPara"))
            {
                switch (xmlReader.LocalName)
                {
                    case "oMath":
                        ReadOMath(reader);
                        break;
                    case "oMathParaPr":
                        ReadOMathParaPr(xmlReader, mathParent);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }
        
        /// <summary>
        /// Read root-level math object m:oMath
        /// </summary>
        internal static void ReadOMath(INrxMathReader reader)
        {
            ReadArgument(reader, new MathObjectOMath(), "oMath");
        }
        
        private static void ReadOMathParaPr(NrxXmlReader xmlReader, MathObjectOMathPara oMathPara)
        {
            while (xmlReader.ReadChild("oMathParaPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "jc":
                        oMathPara.Justification = DocxEnum.DocxToMathJustification(xmlReader.ReadVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// "default" argument for most math objects is "m:e", so create a special function for reading it.
        /// </summary>
        private static void ReadDefaultArgument(INrxMathReader reader)
        {
            ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.Argument), "e");
        }

        private static void ReadArgument(INrxMathReader reader, MathObject mathParent, string tagName)
        {
            ReadArgument(reader, mathParent, tagName, true);    
        }
        
        /// <summary>
        /// Reads a collection of math objects that can act as arguments for current parent.
        /// </summary>
        private static void ReadArgument(INrxMathReader reader, MathObject mathParent, string tagName, bool createParent)
        {
            IMathRunPr rPr = reader.CreateRunPr();
            if (createParent)
                reader.AddAndPushMathContainer(mathParent, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "acc": // Accent §22.1.2.
                        ReadMathObjectDefault(reader, new MathObjectAccent(), "acc");
                        break;
                    case "bar": // (Bar) §22.1.2.7
                        ReadMathObjectDefault(reader, new MathObjectBar(), "bar");
                        break;
                    case "borderBox": // Border-Box Object §22.1.2.11
                        ReadMathObjectDefault(reader, new MathObjectBorderBox(), "borderBox");
                        break;
                    case "box": // Box Object §22.1.2.13
                        ReadMathObjectDefault(reader, new MathObjectBox(), "box");
                        break;
                    case "d": // Delimiter Object §22.1.2.24
                        ReadMathObjectDefault(reader, new MathObjectDelimiter(), "d");
                        break;
                    case "eqArr": // Array Object §22.1.2.34
                        ReadMathObjectDefault(reader, new MathObjectArray(), "eqArr");
                        break;
                    case "f": // Fraction Object §22.1.2.36
                        ReadFraction(reader);
                        break;
                    case "func": // Function Apply Object §22.1.2.39
                        ReadFunction(reader);
                        break;
                    case "groupChr": // Group-Character Object §22.1.2.41
                        ReadMathObjectDefault(reader, new MathObjectGroupCharacter(), "groupChr");
                        break;
                    case "limLow": // Lower-Limit Object §22.1.2.54
                        ReadLimUppLow(reader, new MathObjectLowerLimit(), "limLow");
                        break;
                    case "limUpp": // Upper-Limit Object §22.1.2.56
                        ReadLimUppLow(reader, new MathObjectUpperLimit(), "limUpp");
                        break;
                    case "m": // Matrix Object §22.1.2.60
                        ReadMatrix(reader);
                        break;
                    case "nary": // n-ary Operator Object §22.1.2.70
                        ReadNAry(reader);
                        break;
                    case "oMath": // Office Math §22.1.2.77
                    case "oMathPara": // Office Math Paragraph §22.1.2.78
                        // resiliency                
                        // MS-OI29500, 22.1.2.77. (b) Word fails to open a file with an oMath element inside a math object argument 
                        // MS-OI29500, 22.1.2.78. (b) Word fails to open a file with oMathPara as a descendant of any math element.
                        ReadArgument(reader, mathParent, tagName, false);
                        break;
                    case "phant": // (Phantom Object) §22.1.2.81
                        ReadMathObjectDefault(reader, new MathObjectPhantom(), "phant");
                        break;
                    case "rad": // Radical Object §22.1.2.88
                        ReadRadical(reader);
                        break;
                    case "sPre": // Pre-Sub-Superscript Object §22.1.2.99
                        ReadSubSupObjects(reader, new MathObjectPreSubSuperscript(), "sPre");
                        break;
                    case "sSub": // Subscript Object §22.1.2.101
                        ReadSubSupObjects(reader, new MathObjectSubscript(), "sSub");
                        break;
                    case "sSubSup": // Sub-Superscript Object §22.1.2.103
                        ReadSubSupObjects(reader, new MathObjectSubSuperscript(), "sSubSup");
                        break;
                    case "sSup": // Superscript Object §22.1.2.105
                        ReadSubSupObjects(reader, new MathObjectSuperscript(), "sSup");
                        break;
                    case "argPr": // Argument Properties §22.1.2.5
                        ReadArgPr(reader, mathParent);
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                    {
                        // inline level elements can appear inside math, so we delegate reading for all unknown tags.
                        reader.ReadInlineChildren();
 
                        // WORDSNET-13720 Try decoding text of recently read run.
                        Run run = reader.DocumentReader.CurContainer.LastChild as Run;
                        if (run != null)
                            TryDetectEncoding(run.Text, reader.DocumentReader);

                        break;
                    }
                }
            }

            if (createParent) 
                reader.PopMathContainer();
        }

        /// <summary>
        /// Read Math objects by "default". A lot of math object have similar structure and differ only in properties.
        /// So we read those by this default method delegating specific pr reads to specialized reading methods throught
        /// cases defined in <see cref="ReadMathObjectPr"/>
        /// </summary>
        private static void ReadMathObjectDefault(INrxMathReader reader, MathObject obj, string tagName)
        {
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(obj, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild(tagName))
            {
                switch(xmlReader.LocalName)
                {
                    case "e":
                        ReadDefaultArgument(reader);
                        break;
                    default:
                        ReadMathObjectPr(reader, obj, rPr);
                        break;
                }
            }

            reader.PopMathContainer();        
        }
        
        /// <summary>
        /// Reads properties of a given math object. Provides resiliency against wrong objPr tags.
        /// </summary>
        private static void ReadMathObjectPr(INrxMathReader reader, MathObject obj, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            if ((obj.MathObjectType == MathObjectType.Accent) && (xmlReader.LocalName == "accPr"))
            {
                ReadAccentPr(reader, (MathObjectAccent)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.Bar) && (xmlReader.LocalName == "barPr"))
            {
                ReadBarPr(reader, (MathObjectBar)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.BorderBox) && (xmlReader.LocalName == "borderBoxPr"))
            {
                ReadBorderBoxPr(reader, (MathObjectBorderBox)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.Box) && (xmlReader.LocalName == "boxPr"))
            {
                ReadBoxPr(reader, (MathObjectBox)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.Delimiter) && (xmlReader.LocalName == "dPr"))
            {
                ReadDelimiterPr(reader, (MathObjectDelimiter)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.Array) && (xmlReader.LocalName == "eqArrPr"))
            {
                ReadArrayPr(reader, (MathObjectArray)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.GroupCharacter) && (xmlReader.LocalName == "groupChrPr"))
            {
                ReadGroupChrPr(reader, (MathObjectGroupCharacter)obj, rPr);
            }
            else if ((obj.MathObjectType == MathObjectType.Phantom) && (xmlReader.LocalName == "phantPr"))
            {
                ReadPhantomPr(reader, (MathObjectPhantom)obj, rPr);
            }
            else
            {
                // objPr tag does not correspond to obj type, so skip this objPr tag for resiliency
                xmlReader.IgnoreElement();
            }
        }

        private static void ReadFraction(INrxMathReader reader)
        {
            MathObjectFraction fraction = new MathObjectFraction();

            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(fraction, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("f"))
            {
                switch (xmlReader.LocalName)
                {
                    case "fPr":
                        ReadFractionPr(reader, fraction, rPr);
                        break;
                    case "den":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.Denominator), "den");
                        break;
                    case "num":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.Numerator), "num");
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }

        private static void ReadFractionPr(INrxMathReader reader, MathObjectFraction f, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("fPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "type":
                        f.FractionType = DocxEnum.DocxToMathFractionType(xmlReader.ReadVal());
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadFunction(INrxMathReader reader)
        {
            MathObjectFunction function = new MathObjectFunction();
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(function, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("func"))
            {
                switch (xmlReader.LocalName)
                {
                    case "funcPr": // func properties
                        ReadDefaultPr(reader, rPr, "funcPr");
                        break;
                    case "fName":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.FunctionName), "fName");
                        break;
                    case "e":
                        ReadDefaultArgument(reader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }
        
        /// <summary>
        /// Common reader function for <see cref="MathObjectLowerLimit"/> and <see cref="MathObjectUpperLimit"/>.
        /// </summary>
        private static void ReadLimUppLow(INrxMathReader reader, MathObject obj, string objTag)
        {
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(obj, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild(objTag))
            {
                switch (xmlReader.LocalName)
                {
                    case "lim":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.Limit), "lim");
                        break;
                    case "e":
                        ReadDefaultArgument(reader);
                        break;
                    case "limUppPr":
                        ReadDefaultPr(reader, rPr, "limUppPr");
                        break;
                    case "limLowPr":
                        ReadDefaultPr(reader, rPr, "limLowPr");
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }

        private static void ReadMatrix(INrxMathReader reader)
        {
            MathObjectMatrix m = new MathObjectMatrix();
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(m, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("m"))
            {
                switch (xmlReader.LocalName)
                {
                    case "mr":
                        ReadMatrixRow(reader, m.ColumnPrCollection);
                        break;
                    case "mPr":
                        ReadMatrixPr(reader, m, rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }

        private static void ReadMatrixRow(INrxMathReader reader, MathMatrixColumnPrCollection columns)
        {
            reader.AddAndPushMathContainer(new MathObjectMatrixRow());
            NrxXmlReader xmlReader = reader.XmlReader;
            int actualColumnCount = 0;
            while (xmlReader.ReadChild("mr"))
            {
                ++actualColumnCount;
                if (xmlReader.LocalName == "e")
                {
                    ReadDefaultArgument(reader);
                }
                else
                    xmlReader.IgnoreElement();
            }
            // WORDSNET-17185 Correct number of columns in case it is specified incorrectly in matrix properties
            // (or is not specified at all).
            if (actualColumnCount > columns.Count)
            {
                columns.Add(new MathMatrixColumnPr(), actualColumnCount - columns.Count);
            }

            reader.PopMathContainer();
        }

        private static void ReadMatrixPr(INrxMathReader reader, MathObjectMatrix m, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("mPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "baseJc":
                        m.BaseJustification = ReadBaseJustification(xmlReader, reader.DocumentReader.ComplianceInfo);
                        break;
                    case "cGp":
                        m.ColumnGap = xmlReader.ReadIntVal();
                        break;
                    case "cGpRule":
                        m.ColumnSpacingRule = ReadGapSpacingRule(xmlReader);
                        break;
                    case "cSp":
                        m.MinimumColumnWidth = xmlReader.ReadIntVal();
                        break;
                    case "mcs":
                        ReadMatrixColumns(reader, m.ColumnPrCollection);
                        break;
                    case "plcHide":
                        m.IsHidePlaceholders = xmlReader.ReadBoolVal();
                        break;
                    case "rSp":
                        m.RowSpacing = xmlReader.ReadIntVal();
                        break;
                    case "rSpRule":
                        m.RowSpacingRule = ReadGapSpacingRule(xmlReader);
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadMatrixColumns(INrxMathReader reader, MathMatrixColumnPrCollection columns)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("mcs"))
            {
                switch (xmlReader.LocalName)
                {
                    case "mc":
                        ReadMatrixColumn(reader, columns);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadMatrixColumn(INrxMathReader reader, MathMatrixColumnPrCollection columns)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("mc"))
            {
                switch (xmlReader.LocalName)
                {
                    case "mcPr":
                        ReadMatrixColumnPr(reader, columns);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadMatrixColumnPr(INrxMathReader reader, MathMatrixColumnPrCollection columns)
        {
            int nColumns = 1;
            MathMatrixColumnPr mcPr = new MathMatrixColumnPr();

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("mcPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "count":
                        nColumns = ReadInt255(xmlReader);
                        break;
                    case "mcJc":
                        mcPr.HorizontalAlignment = StyleConvertUtil.XmlToHorizontalAlignment(xmlReader.ReadVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
            
            columns.Add(mcPr, nColumns);
        }

        private static void ReadNAry(INrxMathReader reader)
        {
            MathObjectNAry nary = new MathObjectNAry();
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(nary, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("nary"))
            {
                switch (xmlReader.LocalName)
                {
                    case "e":
                        ReadDefaultArgument(reader);
                        break;
                    case "naryPr":
                        ReadNAryPr(reader, nary, rPr);
                        break;
                    case "sub":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.SubscriptPart), "sub");
                        break;
                    case "sup":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.SuperscriptPart), "sup");
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }

        private static void ReadNAryPr(INrxMathReader reader, MathObjectNAry nary, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("naryPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "chr":
                        nary.Character = ReadChar(reader);
                        break;
                    case "grow":
                        nary.GrowToMatchOperand = xmlReader.ReadBoolVal();
                        break;
                    case "limLoc":
                        nary.LimitLocation = DocxEnum.DocxToMathLimitLocation(xmlReader.ReadVal());
                        break;
                    case "subHide":
                        nary.IsHideSubscript = xmlReader.ReadBoolVal();
                        break;
                    case "supHide":
                        nary.IsHideSuperscript = xmlReader.ReadBoolVal();
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadRadical(INrxMathReader reader)
        {
            MathObjectRadical rad = new MathObjectRadical();
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(rad, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("rad"))
            {
                switch (xmlReader.LocalName)
                {
                    case "e":
                        ReadDefaultArgument(reader);
                        break;
                    case "deg":
                        ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.Degree), "deg");
                        break;
                    case "radPr":
                        ReadRadicalPr(reader, rad, rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }

        private static void ReadRadicalPr(INrxMathReader reader, MathObjectRadical rad, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("radPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "degHide":
                        rad.IsHideDegree = xmlReader.ReadBoolVal();
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }
        
        /// <summary>
        /// Common reader function for the following quasi-similar objects.
        /// "sPre": (Pre-Sub-Superscript Object) §22.1.2.99
        /// "sSub": (Subscript Object) §22.1.2.101
        /// "sSubSup": (Sub-Superscript Object) §22.1.2.103       
        /// "sSup": (Superscript Object) §22.1.2.105
        /// </summary>
        private static void ReadSubSupObjects(INrxMathReader reader, MathObject obj, string objTag)
        {
            IMathRunPr rPr = reader.CreateRunPr();
            reader.AddAndPushMathContainer(obj, rPr);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild(objTag))
            {
                switch (xmlReader.LocalName)
                {
                    case "e":
                        ReadDefaultArgument(reader);
                        break;
                    case "sup":
                        {
                            if (obj.MathObjectType == MathObjectType.Subscript) // subscripts shouldn't have "sup" objTag inside
                                xmlReader.IgnoreElement();
                            else
                                ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.SuperscriptPart), "sup");
                            break;
                        }
                    case "sub":
                        {
                            if (obj.MathObjectType == MathObjectType.Supercript) // superscripts shouldn't have "sub" objTag inside
                                xmlReader.IgnoreElement();
                            else
                                ReadArgument(reader, new MathObjectArgumentBase(MathObjectType.SubscriptPart), "sub");
                            break;
                        }
                    case "sPrePr":
                        ReadDefaultPr(reader, rPr, "sPrePr");
                        break;
                    case "sSubPr":
                        ReadDefaultPr(reader, rPr, "sSubPr");
                        break;
                    case "sSubSupPr":
                        {
                            if (obj.MathObjectType == MathObjectType.SubSuperscript) // SubSuperscript is the only obj that has non-default pr.
                                ReadSubSupPr(reader, (MathObjectSubSuperscript)obj, rPr);
                            else
                                xmlReader.IgnoreElement();
                            break;
                        }
                    case "sSupPr":
                        ReadDefaultPr(reader, rPr, "sSupPr");
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.PopMathContainer();
        }

        private static void ReadSubSupPr(INrxMathReader reader, MathObjectSubSuperscript subSuperscript, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("sSubSupPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "alnScr":
                        subSuperscript.IsAlignScripts = xmlReader.ReadBoolVal();
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadArgPr(INrxMathReader reader, MathObject mathParent)
        {
            int argSz = MathObjectArgumentBase.DefaultArgumentSize;
            
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("argPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "argSz":
                        argSz = xmlReader.ReadIntVal();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            if ((argSz != MathObjectArgumentBase.DefaultArgumentSize)  && (mathParent is MathObjectArgumentBase))
                ((MathObjectArgumentBase)mathParent).ArgumentSize = argSz;
        }
        
        /// <summary>
        /// Reads a default "almost-empty" MathObject pr that should contain only one child "m:ctrlPr"
        /// </summary>
        private static void ReadDefaultPr(INrxMathReader reader, IMathRunPr rPr, string prTag)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild(prTag))
            {
                if (xmlReader.LocalName == "ctrlPr")
                    reader.ReadCtrlPr(rPr);
                else
                    xmlReader.IgnoreElement();
            }

        }

        private static void ReadAccentPr(INrxMathReader reader, MathObjectAccent acc, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("accPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "chr":
                        acc.Character = ReadChar(reader, true);
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadBarPr(INrxMathReader reader, MathObjectBar bar, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            // WORDSNET-6085 There was a mistake in the code, instead of 'barPr' method looked for 'accPr'.
            while (xmlReader.ReadChild("barPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "pos":
                        bar.Position = ReadPositionVal(xmlReader);
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadBorderBoxPr(INrxMathReader reader, MathObjectBorderBox bbox, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("borderBoxPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "hideBot":
                        bbox.HideBottomEdge = xmlReader.ReadBoolVal();
                        break;
                    case "hideLeft":
                        bbox.HideLeftEdge = xmlReader.ReadBoolVal();
                        break;
                    case "hideRight":
                        bbox.HideRightEdge = xmlReader.ReadBoolVal();
                        break;
                    case "hideTop":
                        bbox.HideTopEdge = xmlReader.ReadBoolVal();
                        break;
                    case "strikeBLTR":
                        bbox.StrikeBLTR = xmlReader.ReadBoolVal();
                        break;
                    case "strikeH":
                        bbox.StrikeH = xmlReader.ReadBoolVal();
                        break;
                    case "strikeTLBR":
                        bbox.StrikeTLBR = xmlReader.ReadBoolVal();
                        break;
                    case "strikeV":
                        bbox.StrikeV = xmlReader.ReadBoolVal();
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadBoxPr(INrxMathReader reader, MathObjectBox box, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("boxPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "aln":
                        box.IsAlignmentPoint = xmlReader.ReadBoolVal();
                        break;
                    case "brk":
                        box.LineBreak = ReadMathLineBreak(xmlReader);
                        break;
                    case "diff":
                        box.IsDifferential = xmlReader.ReadBoolVal();
                        break;
                    case "noBreak":
                        box.IsUnbreakable = xmlReader.ReadBoolVal();
                        break;
                    case "opEmu":
                        box.IsOperatorEmulation = xmlReader.ReadBoolVal();
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadDelimiterPr(INrxMathReader reader, MathObjectDelimiter delimiter, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("dPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "begChr":
                        delimiter.BeginningCharacter = ReadChar(reader);
                        break;
                    case "endChr":
                        delimiter.EndingCharacter = ReadChar(reader);
                        break;
                    case "grow":
                        delimiter.GrowToMatchOperand = xmlReader.ReadBoolVal();
                        break;
                    case "sepChr":
                        delimiter.SeparatorCharacter = ReadChar(reader);
                        break;
                    case "shp":
                        delimiter.DelimiterShape = DocxEnum.DocxToMathDelimiterShape(xmlReader.ReadVal());
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadArrayPr(INrxMathReader reader, MathObjectArray array, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("eqArrPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "baseJc":
                        array.BaseJustification = ReadBaseJustification(xmlReader, 
                            reader.DocumentReader.ComplianceInfo);
                        break;
                    case "maxDist":
                        array.IsMaximumDistribution = xmlReader.ReadBoolVal();
                        break;
                    case "objDist":
                        array.IsObjectDistribution = xmlReader.ReadBoolVal();
                        break;
                    case "rSp":
                        array.RowSpacing = xmlReader.ReadIntVal();
                        break;
                    case "rSpRule":
                        array.RowSpacingRule = ReadGapSpacingRule(xmlReader);
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadGroupChrPr(INrxMathReader reader, MathObjectGroupCharacter groupChar, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("groupChrPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "chr":
                        groupChar.Character = ReadChar(reader);
                        break;
                    case "pos":
                        groupChar.Position = ReadPositionVal(xmlReader);
                        break;
                    case "vertJc":
                        groupChar.VerticalJustification =
                            DocxEnum.DocxToMathVerticalJustificationType(xmlReader.ReadVal());
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadPhantomPr(INrxMathReader reader, MathObjectPhantom phantom, IMathRunPr rPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("phantPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "show":
                        phantom.IsShown = xmlReader.ReadBoolVal();
                        break;
                    case "transp":
                        phantom.IsTransparent = xmlReader.ReadBoolVal();
                        break;
                    case "zeroAsc":
                        phantom.IsZeroAscent = xmlReader.ReadBoolVal();
                        break;
                    case "zeroDesc":
                        phantom.IsZeroDescent = xmlReader.ReadBoolVal();
                        break;
                    case "zeroWid":
                        phantom.IsZeroWidth = xmlReader.ReadBoolVal();
                        break;
                    case "ctrlPr":
                        reader.ReadCtrlPr(rPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Read math specific type brk (Break), see. OOXML ISO29500 ch. 22.1.2.15
        /// </summary>
        /// <remarks>
        /// Semantically this function is better suited as member <see cref="NrxXmlReader"/>,
        /// but it is only used for math reading, so we hide it here to reduce complexity.
        /// </remarks>
        internal static MathLineBreak ReadMathLineBreak(NrxXmlReader reader)
        {
            MathLineBreak result = new MathLineBreak();

            string val = reader.ReadAttribute("alnAt", null);
            result.Alignment = (StringUtil.HasChars(val)) ? FormatterPal.XmlToInt(val) : 0;

            return result;
        }
        
        /// <summary>
        /// Read val value of ST_Integer255 type.
        /// </summary>
        private static int ReadInt255(NrxXmlReader reader)
        {
            int val = reader.ReadIntVal();
            return (val < 1) ? 1 : (val > 255) ? 255 : val;
        }

        /// <summary>
        /// Read m:pos val contents. (ST_TopBot OOXML type)
        /// </summary>
        /// <remarks>
        /// Semantically this function is better suited as member <see cref="NrxXmlReader"/>,
        /// but it is only used for math reading, so we hide it here to reduce complexity.
        /// </remarks>
        private static MathPosition ReadPositionVal(NrxXmlReader xmlReader)
        {
            return DocxEnum.DocxToMathPositionType(xmlReader.ReadVal());
        }
        
        /// <summary>
        /// Read value of (CT_SpacingRule) Office Math type.
        /// </summary>
        /// <remarks>
        /// Semantically this function might better suited as member <see cref="NrxXmlReader"/>,
        /// but it is only used for math reading, so we hide it here to reduce complexity.
        /// </remarks>
        private static MathSpacingRule ReadGapSpacingRule(NrxXmlReader xmlReader)
        {
            int val = xmlReader.ReadIntVal();
            return (MathSpacingRule)((val < 0)? 0 : (val > 4)? 4 : val);
        }
        
        /// <summary>
        /// Read value of (CT_YAlign) type.
        /// </summary>
        private static MathBaseJustification ReadBaseJustification(NrxXmlReader xmlReader, 
            OoxmlComplianceInfo complianceInfo)
        {
            return DocxEnum.DocxToMathBaseJustification(xmlReader.ReadVal(), complianceInfo);
        }

        /// <summary>
        /// Reads 'val' attribute of character type. If there is more than 1 char, then tries to decode it.
        /// MS Word encodes equation XML to UTF-8 twice. So, we may need additional decoding to single byte encoding
        /// that was default in Windows on document creation and then to UTF-8.
        /// </summary>
        private static char ReadChar(INrxMathReader reader)
        {
            return ReadChar(reader, false);
        }

        /// <summary>
        /// Reads 'val' attribute of character type. If there is more than 1 char, then tries decoding it.
        /// MS Word encodes equation XML to UTF-8 twice. So, we may need additional decoding to single byte encoding
        /// that was default in Windows on document creation and then to UTF-8.
        /// For accent we do more reliable check of result.
        /// </summary>
        private static char ReadChar(INrxMathReader reader, bool isAccent)
        {
            string result = reader.XmlReader.ReadVal();

            if (result.Length > 1)
                result = TryDecodingThroughAscii(result, isAccent, reader.DocumentReader);

            return (result.Length > 0) ? result[0] : MathObject.EmptyCharacter;
        }

        /// <summary>
        /// Tries to detect encoding of text.
        /// </summary>
        private static void TryDetectEncoding(string val, NrxDocumentReaderBase docReader)
        {
            // WORDSNET-14155 Currently process only the first two characters from the string.
            // It is not good fix, see comment for OfficeMathToShapeConverter.EquationXmlToOfficeMath method.
            if (val.Length < 2)
                return;

            TryDecodingThroughAscii(val, false, docReader);
        }

        /// <summary>
        /// Tries to perform additional decoding of read string through converting to single byte encoding 
        /// and then to UTF-8. Does it for all Windows encodings until correct result is got.
        /// </summary>
        private static string TryDecodingThroughAscii(string charValue, bool isAccent, NrxDocumentReaderBase docReader)
        {
            List<int> codePageOrder = new List<int>();
            codePageOrder.AddRange(new int[] { 1252, 1250, 1251, 1253, 1254, 1255, 1256, 1257, 1258, 874 });

            // Different computers can use different encodings as the default, and the default encoding can even change on 
            // a single computer. In addition, the encoding returned by the Default property uses 
            // best fit fallback to map unsupported characters to characters supported by the code page. For these two reasons, 
            // using the default encoding is generally not recommended.
            // Let's try UTF-8 (Encoding.UTF8) which supports all of the characters that the framework can handle.
            MoveToFirst(codePageOrder, Encoding.UTF8.CodePage);
            if (docReader.DetectedEncoding != null)
                MoveToFirst(codePageOrder, docReader.DetectedEncoding.CodePage);

            foreach (int codePage in codePageOrder)
            {
                string result = DecodeThroughAscii(charValue, codePage);
                if (!string.IsNullOrEmpty(result))
                {
                    // For accent we do more checks and get more reliable result, so overwrite the encoding.
                    if (docReader.DetectedEncoding == null || (isAccent && IsValidAccent(result)))
                    {
                        docReader.DetectedEncoding = Encoding.GetEncoding(codePage);
                    }

                    return result;
                }
            }

            return charValue;
        }

        /// <summary>
        /// Moves the specified value of array list that contains integer values to the index 0.
        /// </summary>
        private static void MoveToFirst(List<int> listOfInt, int value)
        {
            int i = listOfInt.IndexOf(value);
            if (i > 0)
            {
                listOfInt.RemoveAt(i);
                listOfInt.Insert(0, value);
            }
        }

        /// <summary>
        /// Decodes string by converting it to single byte encoding and then to UTF-8 like MS Word does
        /// for equation XML.
        /// </summary>
        private static string DecodeThroughAscii(string charValue, int codePage)
        {
            try
            {
                DecoderErrorFallBack decoderErrorFallback = new DecoderErrorFallBack();
                EncoderErrorFallBack encoderErrorFallback = new EncoderErrorFallBack();
                Encoding encoding = Encoding.GetEncoding(codePage, encoderErrorFallback, decoderErrorFallback);
                byte[] asciiBytes = encoding.GetBytes(charValue);

                Encoding utf8Encoding = Encoding.GetEncoding(65001, encoderErrorFallback, decoderErrorFallback);
                string result = utf8Encoding.GetString(asciiBytes);
                return (!decoderErrorFallback.IsError && !encoderErrorFallback.IsError)? result : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the specified value presents a valid accent.
        /// </summary>
        private static bool IsValidAccent(string value)
        {
            return ((value != null) && MathObjectAccent.IsValidAccent(value[0]));
        }
    }
}
