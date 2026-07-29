// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System;
using System.Text;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Xml;

namespace Aspose.Words.RW.Dml
{
    /// <summary>
    /// Utility functions for working with diagram simple types.
    /// </summary>
    internal static class DmlDiagramEnum
    {
        internal static bool[] DmlToBoolList(string value)
        {
            string[] listItems = value.Split(ListSeparator);

            bool[] result = new bool[listItems.Length];
            for (int i = 0; i < listItems.Length; i++)
                result[i] = AnyXmlReader.ConvertToBool(listItems[i]);

            return result;
        }

        internal static string BoolListToDml(bool[] value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (bool b in value)
            {
                builder.Append(b ? "true" : "false");
                builder.Append(ListSeparator);
            }

            return builder.ToString().Trim();
        }

        internal static int[] DmlToIntList(string value)
        {
            string[] listItems = value.Split(ListSeparator);

            int[] result = new int[listItems.Length];
            for (int i = 0; i < listItems.Length; i++)
                result[i] = FormatterPal.XmlToInt(listItems[i]);

            return result;
        }

        internal static string IntListToDml(int[] value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int i in value)
            {
                builder.Append(i);
                builder.Append(ListSeparator);
            }

            return builder.ToString().Trim();
        }

        internal static DmlModelId DmlToModelId(string value)
        {
            if (!StringUtil.HasChars(value))
                return null;

            int intValue = FormatterPal.TryParseInt(value);
            if(intValue == int.MinValue)
                return new DmlModelIdGuid(new Guid(value));
            return new DmlModelIdInteger(intValue);
        }

        internal static string ModelIdToDml(DmlModelId value)
        {
            DmlModelIdInteger intId = value as DmlModelIdInteger;
            if (intId != null)
                return intId.ToString();

            DmlModelIdGuid guidId = value as DmlModelIdGuid;
            if (guidId != null)
                return "{" + guidId.ToString().ToUpper() + "}";

            return "";
        }

        internal static DmlConnectionType DmlToConnectionType(string value)
        {
            return (DmlConnectionType)gConnectionTypeMap.GetValue(value, (int)DmlConnectionType.ParentOf);
        }

        internal static string ConnectionTypeToDml(DmlConnectionType value)
        {
            return gConnectionTypeMap.GetValue((int)value, "");
        }

        internal static DmlPointType DmlToPointType(string value)
        {
            return (DmlPointType)gPointTypeMap.GetValue(value, (int)DmlPointType.Node);
        }

        internal static string PointTypeToDml(DmlPointType value)
        {
            return gPointTypeMap.GetValue((int)value, "");
        }

        internal static DmlChildOrder DmlToChildOrder(string value)
        {
            return (DmlChildOrder)gChildOrderMap.GetValue(value, (int)DmlChildOrder.Bottom);
        }

        internal static string ChildOrderToDml(DmlChildOrder value)
        {
            return gChildOrderMap.GetValue((int)value, "");
        }

        internal static DmlAlgorithmType DmlToAlgorithmType(string value)
        {
            return (DmlAlgorithmType)gAlgorithmTypeMap.GetValue(value, 0);
        }

        internal static string AlgorithmTypeToDml(DmlAlgorithmType value)
        {
            return gAlgorithmTypeMap.GetValue((int)value, "");
        }

        internal static DmlAxisType DmlToAxisType(string value)
        {
            return (DmlAxisType)gAxisTypeMap.GetValue(value, (int)DmlAxisType.None);
        }

        internal static string AxisTypeToDml(DmlAxisType value)
        {
            return gAxisTypeMap.GetValue((int)value, "");
        }

        internal static DmlAxisType[] DmlToAxisTypeList(string value)
        {
            string[] listItems = value.Split(ListSeparator);

            DmlAxisType[] result = new DmlAxisType[listItems.Length];
            for (int i = 0; i < listItems.Length; i++)
                result[i] = DmlToAxisType(listItems[i]);

            return result;
        }

        internal static string AxisTypeListToDml(DmlAxisType[] value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (DmlAxisType type in value)
            {
                builder.Append(AxisTypeToDml(type));
                builder.Append(ListSeparator);
            }

            return builder.ToString().Trim();
        }

        internal static DmlElementType DmlToElementType(string value)
        {
            return (DmlElementType)gElementTypeMap.GetValue(value, (int)DmlElementType.All);
        }

        internal static string ElementTypeToDml(DmlElementType value)
        {
            return gElementTypeMap.GetValue((int)value, "");
        }

        internal static DmlElementType[] DmlToElementTypeList(string value)
        {
            string[] listItems = value.Split(ListSeparator);

            DmlElementType[] result = new DmlElementType[listItems.Length];
            for (int i = 0; i < listItems.Length; i++)
                result[i] = DmlToElementType(listItems[i]);

            return result;
        }

        internal static string ElementTypeListToDml(DmlElementType[] value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (DmlElementType type in value)
            {
                builder.Append(ElementTypeToDml(type));
                builder.Append(ListSeparator);
            }

            return builder.ToString().Trim();
        }

        internal static DmlBooleanOperator DmlToBooleanOperator(string value)
        {
            return (DmlBooleanOperator)gBooleanOperatorMap.GetValue(value, (int)DmlBooleanOperator.None);
        }

        internal static string BooleanOperatorToDml(DmlBooleanOperator value)
        {
            return gBooleanOperatorMap.GetValue((int)value, "");
        }

        internal static DmlConstraintRelationship DmlToConstraintRelationship(string value)
        {
            return (DmlConstraintRelationship)gConstraintRelationshipMap.GetValue(value, (int)DmlConstraintRelationship.Self);
        }

        internal static string ConstraintRelationshipToDml(DmlConstraintRelationship value)
        {
            return gConstraintRelationshipMap.GetValue((int)value, "");
        }

        internal static DmlDiagramDirection DmlToDirection(string value)
        {
            return (DmlDiagramDirection)gDirectionMap.GetValue(value, (int)DmlDiagramDirection.Normal);
        }

        internal static string DirectionToDml(DmlDiagramDirection value)
        {
            return gDirectionMap.GetValue((int)value, "");
        }

        internal static bool IsDirection(string value)
        {
            return !StringToIntBidirectionalMap.IsNullSubstitute(gDirectionMap.TryGetValue(value));
        }

        internal static DmlHierBranchStyle DmlToHierBranchStyle(string value)
        {
            return (DmlHierBranchStyle)gHierBranchStyleMap.GetValue(value, (int)DmlHierBranchStyle.Standard);
        }

        internal static string HierBranchStyleToDml(DmlHierBranchStyle value)
        {
            return gHierBranchStyleMap.GetValue((int)value, "");
        }

        internal static bool IsHierBranchStyle(string value)
        {
            return !StringToIntBidirectionalMap.IsNullSubstitute(gHierBranchStyleMap.TryGetValue(value));
        }

        internal static DmlAnimOne DmlToAnimOne(string value)
        {
            return (DmlAnimOne)gAnimOne.GetValue(value, (int)DmlAnimOne.One);
        }

        internal static string AnimOneToDml(DmlAnimOne value)
        {
            return gAnimOne.GetValue((int)value, "");
        }

        internal static bool IsAnimOne(string value)
        {
            return !StringToIntBidirectionalMap.IsNullSubstitute(gAnimOne.TryGetValue(value));
        }

        internal static DmlAnimLevel DmlToAnimLevel(string value)
        {
            return (DmlAnimLevel)gAnimLevelMap.GetValue(value, (int)DmlAnimLevel.None);
        }

        internal static string AnimLevelToDml(DmlAnimLevel value)
        {
            return gAnimLevelMap.GetValue((int)value, "");
        }

        internal static bool IsAnimLevel(string value)
        {
            return !StringToIntBidirectionalMap.IsNullSubstitute(gAnimLevelMap.TryGetValue(value));
        }

        internal static DmlResizeHandles DmlToResizeHandles(string value)
        {
            return (DmlResizeHandles)gResizeHandlesMap.GetValue(value, (int)DmlResizeHandles.Relative);
        }

        internal static string ResizeHandlesToDml(DmlResizeHandles value)
        {
            return gResizeHandlesMap.GetValue((int)value, "");
        }

        internal static bool IsResizeHandles(string value)
        {
            return !StringToIntBidirectionalMap.IsNullSubstitute(gResizeHandlesMap.TryGetValue(value));
        }

        internal static DmlFunctionType DmlToFunctionType(string value)
        {
            return (DmlFunctionType)gFunctionTypeMap.GetValue(value, 0);
        }

        internal static string FunctionTypeToDml(DmlFunctionType value)
        {
            return gFunctionTypeMap.GetValue((int)value, "");
        }

        internal static DmlVariableType DmlToVariableType(string value)
        {
            return (DmlVariableType)gVariableTypeMap.GetValue(value, (int)DmlVariableType.None);
        }

        internal static string VariableTypeToDml(DmlVariableType value)
        {
            return gVariableTypeMap.GetValue((int)value, "");
        }

        internal static DmlFunctionOperator DmlToFunctionOperator(string value)
        {
            return (DmlFunctionOperator)gFunctionOperatorMap.GetValue(value, 0);
        }

        internal static string FunctionOperatorToDml(DmlFunctionOperator value)
        {
            return gFunctionOperatorMap.GetValue((int)value, "");
        }

        internal static DmlColorApplicationMethod DmlToColorApplicationMethod(string value)
        {
            return (DmlColorApplicationMethod)gColorApplicationMethodMap.GetValue(value, (int)DmlColorApplicationMethod.Span);
        }

        internal static string ColorApplicationMethodToDml(DmlColorApplicationMethod value)
        {
            return gColorApplicationMethodMap.GetValue((int)value, "");
        }

        internal static DmlHueDirection DmlToHueDirection(string value)
        {
            return (DmlHueDirection)gHueDirectionMap.GetValue(value, (int)DmlHueDirection.Clockwise);
        }

        internal static string HueDirectionToDml(DmlHueDirection value)
        {
            return gHueDirectionMap.GetValue((int)value, "");
        }

        internal static DmlNodeHorizontalAlignment DmlToNodeHorizontalAlignment(string value)
        {
            return (DmlNodeHorizontalAlignment)gNodeHorizontalAlignmentMap.GetValue(value, 0);
        }

        internal static string NodeHorizontalAlignmentToDml(DmlNodeHorizontalAlignment value)
        {
            return gNodeHorizontalAlignmentMap.GetValue((int)value, "");
        }

        internal static DmlNodeVerticalAlignment DmlToNodeVerticalAlignment(string value)
        {
            return (DmlNodeVerticalAlignment)gNodeVerticalAlignmentMap.GetValue(value, 0);
        }

        internal static string NodeVerticalAlignmentToDml(DmlNodeVerticalAlignment value)
        {
            return gNodeVerticalAlignmentMap.GetValue((int)value, "");
        }

        internal static DmlFallbackDimension DmlToFallbackDimension(string value)
        {
            return (DmlFallbackDimension)gFallbackDimensionsMap.GetValue(value, 0);
        }

        internal static string FallbackDimensionToDml(DmlFallbackDimension value)
        {
            return gFallbackDimensionsMap.GetValue((int)value, "");
        }

        internal static DmlDiagramHorizontalAlignment DmlToDiagramHorizontalAlignment(string value)
        {
            return (DmlDiagramHorizontalAlignment)gDiagramHorizontalAlignmentMap.GetValue(value, (int)DmlDiagramHorizontalAlignment.None);
        }

        internal static string DiagramHorizontalAlignmentToDml(DmlDiagramHorizontalAlignment value)
        {
            return gDiagramHorizontalAlignmentMap.GetValue((int)value, "");
        }

        public static DmlVerticalAlignment DmlToVerticalAlignment(string value)
        {
            return (DmlVerticalAlignment)gVerticalAlignmentMap.GetValue(value, (int)DmlVerticalAlignment.None);
        }

        internal static string VerticalAlignmentToDml(DmlVerticalAlignment value)
        {
            return gVerticalAlignmentMap.GetValue((int)value, "");
        }

        internal static DmlBreakpoint DmlToBreakpoint(string value)
        {
            return (DmlBreakpoint)gBreakpointMap.GetValue(value, (int)DmlBreakpoint.EndOfCanvas);
        }

        internal static DmlOffset DmlToOffset(string value)
        {
            return (DmlOffset)gOffsetMap.GetValue(value, (int)DmlOffset.Offset);
        }

        internal static DmlContinueDirection DmlToContinueDirection(string value)
        {
            return (DmlContinueDirection)gContinueDirection.GetValue(value, (int)DmlContinueDirection.SameDirection);
        }

        internal static DmlAutoTextRotation DmlToAutoTextRotation(string value)
        {
            return (DmlAutoTextRotation)gAutoTextRotation.GetValue(value, (int)DmlAutoTextRotation.Upright);
        }

        internal static DmlDiagramTextAlignment DmlToDiagramTextAlignment(string value)
        {
            return (DmlDiagramTextAlignment)gDiagramTextAlignment.GetValue(value, (int)DmlDiagramTextAlignment.Center);
        }

        internal static DmlTextAnchorHorizontal DmlToTextAnchorHorizontal(string value)
        {
            return (DmlTextAnchorHorizontal) gTextAnchorHorizontal.GetValue(value, (int) DmlTextAnchorHorizontal.None);
        }

        internal static DmlTextAnchorVertical DmlToTextAnchorVertical(string value)
        {
            return (DmlTextAnchorVertical) gTextAnchorVertical.GetValue(value, (int) DmlTextAnchorVertical.Middle);
        }

        internal static DmlTextBlockDirection DmlToTextBlockDirection(string value)
        {
            return (DmlTextBlockDirection) gTextBlockDirection.GetValue(value, (int) DmlTextBlockDirection.Horizontal);
        }

        internal static DmlConstraintType DmlToConstraintType(string value)
        {
            return (DmlConstraintType) gConstraintType.GetValue(value, (int) DmlConstraintType.None);
        }

        internal static string ConstraintTypeToDml(DmlConstraintType constraintType)
        {
            return gConstraintType.GetValue((int) constraintType, "");
        }

        internal static DmlConnectorDimensions DmlToConnectorDimensions(string value)
        {
            return (DmlConnectorDimensions) gConnectorDimension.GetValue(value, (int) DmlConnectorDimensions.TwoDimensional);
        }

        internal static DmlConnectorPoint DmlToConnectorPoint(string value)
        {
            return (DmlConnectorPoint) gConnectorPoint.GetValue(value, (int) DmlConnectorPoint.Auto);
        }

        internal static DmlArrowheadStyle DmlToArrowheadStyle(string value)
        {
            return (DmlArrowheadStyle)gArrowheadStyle.GetValue(value, (int)DmlArrowheadStyle.Auto);
        }

        internal static DmlConnectorRouting DmlToConnectorRouting(string value)
        {
            return (DmlConnectorRouting)gConnectorRouting.GetValue(value, (int)DmlConnectorRouting.Straight);
        }

        static DmlDiagramEnum()
        {
            gConnectionTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] {"parOf", "presOf", "presParOf", "unknownRelationship"},
                new int[]
                    {
                        (int) DmlConnectionType.ParentOf, (int) DmlConnectionType.PresentationOf,
                        (int) DmlConnectionType.PresentationParentOf, (int) DmlConnectionType.UnknownRelationship
                    });


            gPointTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] {"node", "asst", "doc", "pres", "parTrans", "sibTrans"},
                new int[]
                    {
                        (int) DmlPointType.Node, (int) DmlPointType.AssistantElement,
                        (int) DmlPointType.Document, (int) DmlPointType.Presentation,
                        (int) DmlPointType.ParentTransition, (int) DmlPointType.SiblingTransition
                    });

            gChildOrderMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "b", "t" },
                new int[]
                    {
                        (int) DmlChildOrder.Bottom, (int) DmlChildOrder.Top
                    });

            gAlgorithmTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] {"composite", "conn", "cycle", "hierChild", "hierRoot", "lin", "pyra", "snake", "sp", "tx"},
                new int[]
                    {
                        (int) DmlAlgorithmType.Composite, (int) DmlAlgorithmType.Connector,
                        (int) DmlAlgorithmType.Cycle, (int) DmlAlgorithmType.HierarchyChild,
                        (int) DmlAlgorithmType.HierarchyRoot, (int) DmlAlgorithmType.Linear,
                        (int) DmlAlgorithmType.Pyramid, (int) DmlAlgorithmType.Snake, (int) DmlAlgorithmType.Space,
                        (int) DmlAlgorithmType.Text
                    });

            gAxisTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ancst", "ancstOrSelf", "ch", "des", "desOrSelf", "follow", "followSib", "none", "par", "preced",
                        "precedSib", "root", "self"
                    },
                new int[]
                    {
                        (int) DmlAxisType.Ancestor, (int) DmlAxisType.AncestorOrSelf,
                        (int) DmlAxisType.Child, (int) DmlAxisType.Descendant,
                        (int) DmlAxisType.DescendantOrSelf, (int) DmlAxisType.Follow,
                        (int) DmlAxisType.FollowSibling, (int) DmlAxisType.None, (int) DmlAxisType.Parent,
                        (int) DmlAxisType.Preceding, (int) DmlAxisType.PrecedingSibling, (int) DmlAxisType.Root,
                        (int) DmlAxisType.Self
                    });

            gElementTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "all", "asst", "doc", "node", "nonAsst", "nonNorm", "norm", "parTrans", "pres", "sibTrans"
                    },
                new int[]
                    {
                        (int) DmlElementType.All, (int) DmlElementType.Assistant,
                        (int) DmlElementType.Document, (int) DmlElementType.Node,
                        (int) DmlElementType.NonAssistant, (int) DmlElementType.NonNormal,
                        (int) DmlElementType.Normal, (int) DmlElementType.ParentTransition,
                        (int) DmlElementType.Presentation, (int) DmlElementType.SiblingTransition
                    });


            gBooleanOperatorMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "equ", "gte", "lte", "none"
                    },
                new int[]
                    {
                        (int) DmlBooleanOperator.Equal, (int) DmlBooleanOperator.GreaterOrEqual,
                        (int) DmlBooleanOperator.LessOrEqual, (int) DmlBooleanOperator.None
                    });

            gConstraintRelationshipMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ch", "des", "self"
                    },
                new int[]
                    {
                        (int) DmlConstraintRelationship.Child, (int) DmlConstraintRelationship.Descendant,
                        (int) DmlConstraintRelationship.Self
                    });

            gDirectionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "norm", "rev"
                    },
                new int[]
                    {
                        (int) DmlDiagramDirection.Normal, (int) DmlDiagramDirection.Reversed
                    });

            gHierBranchStyleMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "hang", "init", "l", "r", "std"
                    },
                new int[]
                    {
                        (int) DmlHierBranchStyle.Hanging, (int) DmlHierBranchStyle.Initial,
                        (int) DmlHierBranchStyle.Left, (int) DmlHierBranchStyle.Right,
                        (int) DmlHierBranchStyle.Standard
                    });

            gAnimOne = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "branch", "none", "one"
                    },
                new int[]
                    {
                        (int) DmlAnimOne.Branch,
                        (int) DmlAnimOne.None,
                        (int) DmlAnimOne.One
                    });

            gAnimLevelMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ctr", "lvl", "none"
                    },
                new int[]
                    {
                        (int) DmlAnimLevel.Center,
                        (int) DmlAnimLevel.Level,
                        (int) DmlAnimLevel.None
                    });

            gResizeHandlesMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "exact", "rel"
                    },
                new int[]
                    {
                        (int) DmlResizeHandles.Exact,
                        (int) DmlResizeHandles.Relative
                    });

            gFunctionTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "cnt", "depth", "maxDepth", "pos", "posEven", "posOdd", "revPos", "var"
                    },
                new int[]
                    {
                        (int) DmlFunctionType.Count, (int) DmlFunctionType.Depth,
                        (int) DmlFunctionType.MaxDepth, (int) DmlFunctionType.Position,
                        (int) DmlFunctionType.PositionEven, (int) DmlFunctionType.PositionOdd,
                        (int) DmlFunctionType.ReversePosition, (int) DmlFunctionType.Variable
                    });

            gVariableTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "animLvl", "animOne", "bulEnabled", "chMax", "chPref", "dir", "hierBranch", "none", "orgChart",
                        "resizeHandles"
                    },
                new int[]
                    {
                        (int) DmlVariableType.AnimLevel, (int) DmlVariableType.AnimOne,
                        (int) DmlVariableType.BulletEnabled, (int) DmlVariableType.ChildMax,
                        (int) DmlVariableType.ChildPref, (int) DmlVariableType.Direction,
                        (int) DmlVariableType.HierBranchStyle, (int) DmlVariableType.None,
                        (int) DmlVariableType.OrgChart,
                        (int) DmlVariableType.ResizeHandles
                    });

            gFunctionOperatorMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "equ", "gt", "gte", "lt", "lte", "neq"
                    },
                new int[]
                    {
                        (int) DmlFunctionOperator.Equal, (int) DmlFunctionOperator.Greater,
                        (int) DmlFunctionOperator.GreaterOrEqual, (int) DmlFunctionOperator.Less,
                        (int) DmlFunctionOperator.LessOrEqual, (int) DmlFunctionOperator.NotEqual
                    });

            gColorApplicationMethodMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "cycle", "repeat", "span"
                    },
                new int[]
                    {
                        (int) DmlColorApplicationMethod.Cycle, (int) DmlColorApplicationMethod.Repeat,
                        (int) DmlColorApplicationMethod.Span
                    });

            gHueDirectionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "cw", "ccw"
                    },
                new int[]
                    {
                        (int) DmlHueDirection.Clockwise, (int) DmlHueDirection.Counterclockwise
                    });

            gNodeHorizontalAlignmentMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ctr", "l", "r"
                    },
                new int[]
                    {
                        (int) DmlNodeHorizontalAlignment.Center, (int) DmlNodeHorizontalAlignment.Left,
                        (int) DmlNodeHorizontalAlignment.Right
                    });

            gNodeVerticalAlignmentMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "b", "mid", "t"
                    },
                new int[]
                    {
                        (int) DmlNodeVerticalAlignment.Bottom, (int) DmlNodeVerticalAlignment.Middle,
                        (int) DmlNodeVerticalAlignment.Top
                    });

            gFallbackDimensionsMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "1D", "2D"
                    },
                new int[]
                    {
                        (int) DmlFallbackDimension.OneDimension, (int) DmlFallbackDimension.TwoDimensions
                    });

            gDiagramHorizontalAlignmentMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ctr", "l", "none", "r"
                    },
                new int[]
                    {
                        (int) DmlDiagramHorizontalAlignment.Center, (int) DmlDiagramHorizontalAlignment.Left,
                        (int) DmlDiagramHorizontalAlignment.None, (int) DmlDiagramHorizontalAlignment.Right
                    });

            gVerticalAlignmentMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "b", "mid", "none", "t"
                    },
                new int[]
                    {
                        (int) DmlVerticalAlignment.Bottom, (int) DmlVerticalAlignment.Middle,
                        (int) DmlVerticalAlignment.None, (int) DmlVerticalAlignment.Top
                    });

            gBreakpointMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "bal", "endCnv", "fixed"
                    },
                new int[]
                    {
                        (int) DmlBreakpoint.Balanced, (int) DmlBreakpoint.EndOfCanvas,
                        (int) DmlBreakpoint.Fixed
                    });

            gOffsetMap = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ctr", "off"
                    },
                new int[]
                    {
                        (int) DmlOffset.Center, (int) DmlOffset.Offset
                    });

            gContinueDirection = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "revDir", "sameDir"
                    },
                new int[]
                    {
                        (int) DmlContinueDirection.ReverseDirection, (int) DmlContinueDirection.SameDirection
                    });

            gAutoTextRotation = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "none", "upr", "grav"
                    },
                new int[]
                    {
                        (int) DmlAutoTextRotation.None, (int) DmlAutoTextRotation.Upright,
                        (int) DmlAutoTextRotation.Gravity
                    });

            gDiagramTextAlignment = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "l", "ctr", "r"
                    },
                new int[]
                    {
                        (int) DmlDiagramTextAlignment.Left, (int) DmlDiagramTextAlignment.Center,
                        (int) DmlDiagramTextAlignment.Right
                    });

            gTextAnchorHorizontal = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "ctr", "none"
                    },
                new int[]
                    {
                        (int) DmlTextAnchorHorizontal.Center, (int) DmlTextAnchorHorizontal.None
                    });

            gTextAnchorVertical = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "b", "mid", "t"
                    },
                new int[]
                    {
                        (int) DmlTextAnchorVertical.Bottom, (int) DmlTextAnchorVertical.Middle,
                        (int) DmlTextAnchorVertical.Top
                    });

            gTextBlockDirection = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                    {
                        "horz", "vert"
                    },
                new int[]
                    {
                        (int) DmlTextBlockDirection.Horizontal, (int) DmlTextBlockDirection.Vertical
                    });

            gConstraintType = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                {
                    "none", "alignOff", "begMarg", "bendDist", "begPad", "b", "bMarg", "bOff", "ctrX", "ctrXOff", "ctrY",
                    "ctrYOff", "connDist", "diam", "endMarg", "endPad", "h", "hArH", "hOff", "l", "lMarg", "lOff", "r",
                    "rMarg", "rOff", "primFontSz", "pyraAcctRatio", "secFontSz", "sibSp", "secSibSp", "sp", "stemThick",
                    "t", "tMarg", "tOff", "userA", "userB", "userC", "userD", "userE", "userF", "userG", "userH",
                    "userI", "userJ", "userK", "userL", "userM", "userN", "userO", "userP", "userQ", "userR", "userS",
                    "userT", "userU", "userV", "userW", "userX", "userY", "userZ", "w", "wArH", "wOff"
                },
                new int[]
                {
                    (int) DmlConstraintType.None, (int) DmlConstraintType.AlignmentOffset,
                    (int) DmlConstraintType.BeginningMargin, (int) DmlConstraintType.BendingDistance,
                    (int) DmlConstraintType.BeginPadding, (int) DmlConstraintType.Bottom,
                    (int) DmlConstraintType.BottomMargin, (int) DmlConstraintType.BottomOffset,
                    (int) DmlConstraintType.CenterX, (int) DmlConstraintType.CenterXOffset,
                    (int) DmlConstraintType.CenterY, (int) DmlConstraintType.CenterYOffset,
                    (int) DmlConstraintType.ConnectionDistance, (int) DmlConstraintType.Diameter,
                    (int) DmlConstraintType.EndMargin, (int) DmlConstraintType.EndPadding,
                    (int) DmlConstraintType.Height, (int) DmlConstraintType.ArrowheadHeight,
                    (int) DmlConstraintType.HeightOffset, (int) DmlConstraintType.Left,
                    (int) DmlConstraintType.LeftMargin, (int) DmlConstraintType.LeftOffset,
                    (int) DmlConstraintType.Right, (int) DmlConstraintType.RightMargin,
                    (int) DmlConstraintType.RightOffset, (int) DmlConstraintType.PrimaryFontSize,
                    (int) DmlConstraintType.PyramidAccentRatio, (int) DmlConstraintType.SecondaryFontSize,
                    (int) DmlConstraintType.SiblingSpacing, (int) DmlConstraintType.SecondarySiblingSpacing,
                    (int) DmlConstraintType.Spacing, (int) DmlConstraintType.StemThickness,
                    (int) DmlConstraintType.Top, (int) DmlConstraintType.TopMargin, (int) DmlConstraintType.TopOffset,
                    (int) DmlConstraintType.UserA, (int) DmlConstraintType.UserB, (int) DmlConstraintType.UserC,
                    (int) DmlConstraintType.UserD, (int) DmlConstraintType.UserE, (int) DmlConstraintType.UserF,
                    (int) DmlConstraintType.UserG, (int) DmlConstraintType.UserH, (int) DmlConstraintType.UserI,
                    (int) DmlConstraintType.UserJ, (int) DmlConstraintType.UserK, (int) DmlConstraintType.UserL,
                    (int) DmlConstraintType.UserM, (int) DmlConstraintType.UserN, (int) DmlConstraintType.UserO,
                    (int) DmlConstraintType.UserP, (int) DmlConstraintType.UserQ, (int) DmlConstraintType.UserR,
                    (int) DmlConstraintType.UserS, (int) DmlConstraintType.UserT, (int) DmlConstraintType.UserU,
                    (int) DmlConstraintType.UserV, (int) DmlConstraintType.UserW, (int) DmlConstraintType.UserX,
                    (int) DmlConstraintType.UserY, (int) DmlConstraintType.UserZ,
                    (int) DmlConstraintType.Width, (int) DmlConstraintType.ArrowheadWidth,
                    (int) DmlConstraintType.WidthOffset
                });

            gConnectorDimension = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                {
                    "1D", "2D", "cust"
                },
                new int[]
                {
                    (int) DmlConnectorDimensions.OneDimensional, (int) DmlConnectorDimensions.TwoDimensional,
                    (int) DmlConnectorDimensions.Custom
                });

            gConnectorPoint = DmlEnumUtil.InitHashtableWithValues(
                new string[]
                {
                    "auto", "bCtr", "bL", "bR", "ctr", "midL", "midR", "radial", "tCtr", "tL", "tR"
                },
                new int[]
                {
                    (int) DmlConnectorPoint.Auto, (int) DmlConnectorPoint.BottomCenter,
                    (int) DmlConnectorPoint.BottomLeft, (int) DmlConnectorPoint.BottomRight,
                    (int) DmlConnectorPoint.Center, (int) DmlConnectorPoint.MiddleLeft,
                    (int) DmlConnectorPoint.MiddleRight, (int) DmlConnectorPoint.Radial,
                    (int) DmlConnectorPoint.TopCenter, (int) DmlConnectorPoint.TopLeft,
                    (int) DmlConnectorPoint.TopRight
                });

            gArrowheadStyle = DmlEnumUtil.InitHashtableWithValues(
                new string[] {"arr", "auto", "noArr"},
                new int[]
                {
                    (int)DmlArrowheadStyle.ArrowheadPresent, (int)DmlArrowheadStyle.Auto,
                    (int)DmlArrowheadStyle.NoArrowhead
                });

            gConnectorRouting = DmlEnumUtil.InitHashtableWithValues(
                new string[] {"bend", "curve", "longCurve", "stra"},
                new int[]
                {
                    (int)DmlConnectorRouting.Bending, (int)DmlConnectorRouting.Curve,
                    (int)DmlConnectorRouting.LongCurve, (int)DmlConnectorRouting.Straight
                });
        }

        private static readonly StringToIntBidirectionalMap gConnectionTypeMap;
        private static readonly StringToIntBidirectionalMap gPointTypeMap;
        private static readonly StringToIntBidirectionalMap gChildOrderMap;
        private static readonly StringToIntBidirectionalMap gAlgorithmTypeMap;
        private static readonly StringToIntBidirectionalMap gAxisTypeMap;
        private static readonly StringToIntBidirectionalMap gElementTypeMap;
        private static readonly StringToIntBidirectionalMap gBooleanOperatorMap;
        private static readonly StringToIntBidirectionalMap gConstraintRelationshipMap;
        private static readonly StringToIntBidirectionalMap gDirectionMap;
        private static readonly StringToIntBidirectionalMap gHierBranchStyleMap;
        private static readonly StringToIntBidirectionalMap gAnimOne;
        private static readonly StringToIntBidirectionalMap gAnimLevelMap;
        private static readonly StringToIntBidirectionalMap gResizeHandlesMap;
        private static readonly StringToIntBidirectionalMap gFunctionTypeMap;
        private static readonly StringToIntBidirectionalMap gVariableTypeMap;
        private static readonly StringToIntBidirectionalMap gFunctionOperatorMap;
        private static readonly StringToIntBidirectionalMap gColorApplicationMethodMap;
        private static readonly StringToIntBidirectionalMap gHueDirectionMap;
        private static readonly StringToIntBidirectionalMap gNodeHorizontalAlignmentMap;
        private static readonly StringToIntBidirectionalMap gNodeVerticalAlignmentMap;
        private static readonly StringToIntBidirectionalMap gFallbackDimensionsMap;
        private static readonly StringToIntBidirectionalMap gDiagramHorizontalAlignmentMap;
        private static readonly StringToIntBidirectionalMap gVerticalAlignmentMap;
        private static readonly StringToIntBidirectionalMap gBreakpointMap;
        private static readonly StringToIntBidirectionalMap gOffsetMap;
        private static readonly StringToIntBidirectionalMap gContinueDirection;
        private static readonly StringToIntBidirectionalMap gAutoTextRotation;
        private static readonly StringToIntBidirectionalMap gDiagramTextAlignment;
        private static readonly StringToIntBidirectionalMap gTextAnchorHorizontal;
        private static readonly StringToIntBidirectionalMap gTextAnchorVertical;
        private static readonly StringToIntBidirectionalMap gTextBlockDirection;
        private static readonly StringToIntBidirectionalMap gConstraintType;
        private static readonly StringToIntBidirectionalMap gConnectorDimension;
        private static readonly StringToIntBidirectionalMap gConnectorPoint;
        private static readonly StringToIntBidirectionalMap gArrowheadStyle;
        private static readonly StringToIntBidirectionalMap gConnectorRouting;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ListSeparator = ' ';
    }
}
