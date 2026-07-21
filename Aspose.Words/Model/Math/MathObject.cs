// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

using Aspose.Words.Markup;

namespace Aspose.Words.Math
{
    /// <summary>
    /// Base class for all math objects that exist in Office Math.
    /// Encapsulates differences in properties and semantics of all Office Math elements.
    /// </summary>
    /// <remarks>
    /// Specific math properties are implemented through AttrCollection for two reasons:
    /// - Use mechanisms of default/non-default attrs for writing(non-defaults only) into docx
    /// - Use same AttrCollection pattern for consistency with other attr-bearing nodes.
    /// - We might want to include math attr collections in later refactoring effort <see cref="Attr"/>.
    /// </remarks>
    internal abstract class MathObject : AttrCollection
    {
        /// <summary>
        /// Each math object has its own list of allowed children per spec.
        /// This method allows to maintain tree integrity.
        /// </summary>
        internal virtual bool CanInsert(Node node)
        {
            bool result;

            if (NeedsMathObjectArgumentWrapper)
            {
                // this object needs every argument wrapped into special kind of MathObjects.
                result = (node.NodeType == NodeType.OfficeMath) &&
                        (((OfficeMath)node).MathObject.MathObjectType == MathObjectType.Argument);
            }
            else
            {
                // this object allows arguments inserted as direct children.
                result = CanBeMathArgument(node);
            }

            return result;
        }

        /// <summary>
        /// Returns true if a <see cref="Node"/> can be argument of an Office Math node.
        /// </summary>
        internal static bool CanBeMathArgument(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.OfficeMath:
                    return (((OfficeMath)node).MathObject).CanBeArgument;

                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.Comment:
                case NodeType.SmartTag:
                case NodeType.Run:
                // andrnosk: WORDSNET-6505 MathML contains MERGEFIELD.
                case NodeType.FieldStart:
                case NodeType.FieldSeparator:
                case NodeType.FieldEnd:
                // WORDSNET-12977 MathML contains FormField.
                case NodeType.FormField:
                // These are not in the model, but dont forget to insert them here when needed.
                // NodeType.customXmlDelRangeStart - not supported.
                // NodeType.customXmlDelRangeEnd - not supported.
                // NodeType.customXmlInsRangeStart - not supported.
                // NodeType.customXmlInsRangeEnd - not supported.
                // NodeType.customXmlMoveFromRangeStart - not supported.
                // NodeType.customXmlMoveFromRangeEnd - not supported.
                // NodeType.customXmlMoveToRangeStart - not supported.
                // NodeType.customXmlMoveToRangeEnd - not supported.
                // WORDSNET-9221 It seems that Shape can be argument.
                case NodeType.Shape:
                // WORDSNET-25441 GroupShape can be argument.
                case NodeType.GroupShape:
                // WORDSNET-10346 Footnote can be argument.
                case NodeType.SpecialChar:
                case NodeType.Footnote:
                    return true;

                // WORDSNET-20848 An inline-level SDT can be argument.
                case NodeType.StructuredDocumentTag:
                    return (((StructuredDocumentTag)node).Level == MarkupLevel.Inline);

                // Any other node can not be inserted as Math argument.
                default:
                    return false;
            }
        }

        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        internal abstract MathObjectType MathObjectType { get; }

        /// <summary>
        /// For most objects it is needed to have MathObjectArgumentBase as a wrapper for every argument.
        /// The only exclusions are <see cref="MathObjectArgumentBase"/> and <see cref="MathObjectOMath"/>
        /// </summary>
        internal virtual bool NeedsMathObjectArgumentWrapper
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if a Math entity can act as an argument for other Math entity.
        /// More formally this means that this math element can be inside "e" element as described in Iso29500 part 1,
        /// chapter 22.1.2.32 e (Element (Argument))
        /// </summary>
        internal virtual bool CanBeArgument
        {
            get { return true; }
        }

        /// <summary>
        /// Some character properties defined for MathObject descendants, e.g. <see cref="MathObjectNAry"/>
        /// can contain empty characters, since there is no such thing in .NET, we will emulate empty char by UNICODE symbol 0x0000
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char EmptyCharacter = '\u0000';

        static MathObject()
        {
            gDefaults = new MathObjectOMath();
            gDefaults.Add(MathAttr.Justification, OfficeMathJustification.Default);
            gDefaults.Add(MathAttr.Position, MathPosition.Default);
            gDefaults.Add(MathAttr.AccentCharacter, MathObjectAccent.DefaultCharacter);
            gDefaults.Add(MathAttr.HideBottom, false);
            gDefaults.Add(MathAttr.HideTop, false);
            gDefaults.Add(MathAttr.HideLeft, false);
            gDefaults.Add(MathAttr.HideRight, false);
            gDefaults.Add(MathAttr.StrikeBLTR, false);
            gDefaults.Add(MathAttr.StrikeH, false);
            gDefaults.Add(MathAttr.StrikeTLBR, false);
            gDefaults.Add(MathAttr.StrikeV, false);
            gDefaults.Add(MathAttr.IsOpEmu, false);
            gDefaults.Add(MathAttr.IsAlignmentPoint, false);
            gDefaults.Add(MathAttr.IsDifferential, false);
            gDefaults.Add(MathAttr.NoBreaks, true);
            gDefaults.Add(MathAttr.BeginChar, MathObjectDelimiter.DefaultBeginningCharacter);
            gDefaults.Add(MathAttr.EndChar, MathObjectDelimiter.DefaultEndingCharacter);
            gDefaults.Add(MathAttr.SeparatorChar, MathObjectDelimiter.DefaultSeparatorCharacter);
            gDefaults.Add(MathAttr.GrowOperand, false);
            gDefaults.Add(MathAttr.DelimiterShape, MathDelimiterShape.Default);
            gDefaults.Add(MathAttr.RowSpacingRule, MathSpacingRule.Default);
            gDefaults.Add(MathAttr.RowSpacing, 0);
            gDefaults.Add(MathAttr.BaseJustification, MathBaseJustification.Default);
            gDefaults.Add(MathAttr.MaxDist, false);
            gDefaults.Add(MathAttr.ObjDist, false);
            gDefaults.Add(MathAttr.GroupCharPosition, MathPosition.Default);
            gDefaults.Add(MathAttr.VerticalJustification, MathVerticalJustification.Default);
            gDefaults.Add(MathAttr.GroupChar, MathObjectGroupCharacter.DefaultCharacter);
            gDefaults.Add(MathAttr.IsShown, true);
            gDefaults.Add(MathAttr.IsTransparent, false);
            gDefaults.Add(MathAttr.IsZeroAscent, false);
            gDefaults.Add(MathAttr.IsZeroWidth, false);
            gDefaults.Add(MathAttr.IsZeroDescent, false);
            gDefaults.Add(MathAttr.FractionType, MathFractionType.Default);
            gDefaults.Add(MathAttr.IsHidePlaceholders, false);
            gDefaults.Add(MathAttr.MinColumnWidth, 0);
            gDefaults.Add(MathAttr.ColumnGapRule, MathSpacingRule.Default);
            gDefaults.Add(MathAttr.ColumnGap, 0);
            gDefaults.Add(MathAttr.LimitLocation, MathLimitLocation.Undefined);
            gDefaults.Add(MathAttr.IsHideSubscript, false);
            gDefaults.Add(MathAttr.IsHideSuperscript, false);
            gDefaults.Add(MathAttr.NaryChar, MathObjectNAry.DefaultCharacter);
            gDefaults.Add(MathAttr.DegreeHide, false);
            gDefaults.Add(MathAttr.IsAlignScripts, false);
        }

        private static readonly AttrCollection gDefaults;
    }
}
