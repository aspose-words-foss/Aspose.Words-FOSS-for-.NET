// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Reader for all complex Dml diagram types.
    /// </summary>
    internal class DmlDiagramComplexTypesReader : DmlReaderBase
    {
        private DmlDiagramComplexTypesReader(NrxXmlReader reader)
        {
            mReader = reader;
        }

        internal static DmlDiagramString ReadString(NrxXmlReader reader)
        {
            DmlDiagramString str = new DmlDiagramString();
            str.Language = reader.ReadAttribute("lang", "");
            str.Value = reader.ReadAttribute("");
            return str;
        }

        internal static DmlColorTransformCategory[] ReadCategories(NrxXmlReader reader)
        {
            DmlDiagramComplexTypesReader complexTypesReader = new DmlDiagramComplexTypesReader(reader);
            List<DmlColorTransformCategory> categories = new List<DmlColorTransformCategory>();
            while (reader.ReadChild("catLst"))
            {
                switch (reader.LocalName)
                {
                    case "cat":
                        categories.Add(complexTypesReader.ReadCategory());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }

            return categories.ToArray();
        }

        private DmlColorTransformCategory ReadCategory()
        {
            DmlColorTransformCategory category = new DmlColorTransformCategory();
            category.CategoryType = mReader.ReadAttribute("type", "");
            category.Priority = FormatterPal.ParseInt(mReader.ReadAttribute("pri", "0"));
            return category;
        }

        internal static DmlLayoutVariablePropertySet ReadLayoutVariableList(NrxXmlReader reader)
        {
            DmlLayoutVariablePropertySet varList = new DmlLayoutVariablePropertySet();
            string tagName = reader.LocalName;
            while (reader.ReadChild(tagName))
            {
                switch (reader.LocalName)
                {
                    case "orgChart":
                        varList.OrgChart = reader.ReadBoolAttribute(false);
                        break;
                    case "chMax":
                        varList.ChildMax = new DmlDiagramNodeCount(reader.ReadIntAttribute(-1));
                        break;
                    case "chPref":
                        varList.ChildPref = new DmlDiagramNodeCount(reader.ReadIntAttribute(-1));
                        break;
                    case "bulletEnabled":
                        varList.BulletEnabled = reader.ReadBoolAttribute(false);
                        break;
                    case "dir":
                        varList.Direction = DmlDiagramEnum.DmlToDirection(reader.ReadAttribute("norm"));
                        break;
                    case "hierBranch":
                        varList.HierBranch = DmlDiagramEnum.DmlToHierBranchStyle(reader.ReadAttribute("norm"));
                        break;
                    case "animOne":
                        varList.AnimOne = DmlDiagramEnum.DmlToAnimOne(reader.ReadAttribute("one"));
                        break;
                    case "animLvl":
                        varList.AnimLevel = DmlDiagramEnum.DmlToAnimLevel(reader.ReadAttribute("none"));
                        break;
                    case "resizeHandles":
                        varList.ResizeHandles = DmlDiagramEnum.DmlToResizeHandles(reader.ReadAttribute("rel"));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
            return varList;
        }

        private readonly NrxXmlReader mReader;
    }
}
