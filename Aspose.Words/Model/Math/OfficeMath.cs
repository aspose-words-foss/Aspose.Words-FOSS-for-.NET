// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2010 by Denis Darkin

using System;
using System.Text;
using Aspose.JavaAttributes;
using Aspose.Words.Revisions;

namespace Aspose.Words.Math
{
    /// <summary>
    /// Represents an Office Math object such as function, equation, matrix or alike. Can contain child elements
    /// including runs of mathematical text, bookmarks, comments, other <see cref="OfficeMath"/> instances and some other nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-officemath/">Working with OfficeMath</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>In this version of Aspose.Words, <see cref="OfficeMath"/> nodes do not provide public methods
    /// and properties to create or modify a <see cref="OfficeMath"/> object. In this version you are not able to instantiate
    /// <see cref="Aspose.Words.Math"/> nodes or modify existing except deleting them.</para>
    ///
    /// <p><see cref="OfficeMath"/> can only be a child of <see cref="Paragraph"/>.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public class OfficeMath : CompositeNode, IInline, ITrackableNode
    {
        internal OfficeMath(DocumentBase doc, MathObject mathObject, RunPr runPr) : base(doc)
        {
            mMathObject = mathObject;
            if (runPr == null)
                throw new ArgumentNullException("runPr");

            mRunPr = runPr;
        }

        internal OfficeMath(DocumentBase doc, MathObject mathObject) : this(doc, mathObject, new RunPr())
        {
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitOfficeMathStart"/>, then calls <see cref="Node.Accept"/> for all
        /// child nodes of the Office Math and calls <see cref="DocumentVisitor.VisitOfficeMathEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            OfficeMath lhs = (OfficeMath)base.Clone(isCloneChildren, cloningListener);
            lhs.mRunPr = mRunPr.Clone();
            lhs.mMathObject = mMathObject.Clone();
            return lhs;
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the office math.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitOfficeMathStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the office math.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitOfficeMathEnd(this);
        }

        /// <summary>
        /// Allows to insert other Office Math objects and some non-math nodes.
        /// Types of nodes allowed to be inserted depends on the particular math node type.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return mMathObject.CanInsert(newChild);
        }

        /// <summary>
        /// Returns top level office math that is ancestor of this office math.
        /// </summary>
        internal OfficeMath GetTopLevelOfficeMath()
        {
            OfficeMath parentMath = ParentNode as OfficeMath;
            return (parentMath == null) ? this : parentMath.GetTopLevelOfficeMath();
        }

        /// <summary>
        /// This method determines if this OfficeMath object has a delete revision.
        /// </summary>
        /// <summary>
        /// In HTML we can't write "ins" and "del" elements inside MathML.
        /// Thereby to produce valid HTML we consider that OfficeMath object has a revision
        /// if all its children that can have revisions have them.
        /// </summary>
        internal bool HasDeleteRevision()
        {
            return HasRevisableParts() && AllRevisablePartsHaveDeleteRevisions();
        }

        /// <summary>
        /// This method determines if this OfficeMath object has an insert revision.
        /// </summary>
        /// <summary>
        /// In HTML we can't write "ins" and "del" elements inside MathML.
        /// Thereby to produce valid HTML we consider that OfficeMath object has a revision
        /// if all its children that can have revisions have them.
        /// </summary>
        internal bool HasInsertRevision()
        {
            return HasRevisableParts() && AllRevisablePartsHaveInsertRevisions();
        }

        /// <summary>
        /// Recursively checks if this OfficeMath has at least one revisable part.
        /// </summary>
        private bool HasRevisableParts()
        {
            if (IsRevisable())
            {
                return true;
            }

            Node childNode = FirstChild;
            while (childNode != null)
            {
                OfficeMath childMath = childNode as OfficeMath;
                if ((childMath != null) && childMath.HasRevisableParts())
                {
                    return true;
                }
                childNode = childNode.NextSibling;
            }

            return false;
        }

        private bool AllRevisablePartsHaveDeleteRevisions()
        {
            if (IsRevisable() && !RunPr.HasDeleteRevision)
            {
                return false;
            }

            Node childNode = FirstChild;
            while (childNode != null)
            {
                OfficeMath childMath = childNode as OfficeMath;
                if ((childMath != null) && !childMath.AllRevisablePartsHaveDeleteRevisions())
                {
                    return false;
                }
                childNode = childNode.NextSibling;
            }

            return true;
        }

        private bool AllRevisablePartsHaveInsertRevisions()
        {
            if (IsRevisable() && !RunPr.HasInsertRevision)
            {
                return false;
            }

            Node childNode = FirstChild;
            while (childNode != null)
            {
                OfficeMath childMath = childNode as OfficeMath;
                if ((childMath != null) && !childMath.AllRevisablePartsHaveInsertRevisions())
                {
                    return false;
                }
                childNode = childNode.NextSibling;
            }

            return true;
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0} [{1}]", base.ToString(), MathObjectType);
        }
#endif

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        private string ToStringInternal(string indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(indent + "MathObject:" + mMathObject.MathObjectType);
            ChildNodesToString(sb, this, indent + "  ");
            return sb.ToString();
        }

        /// <summary>
        /// Determines if OfficeMath object can have revisions.
        /// </summary>
        private bool IsRevisable()
        {
            return (MathObjectType != MathObjectType.OMathPara) &&
                   (MathObjectType != MathObjectType.OMath) &&
                   (MathObjectType != MathObjectType.Argument) &&
                   (MathObjectType != MathObjectType.SubscriptPart) &&
                   (MathObjectType != MathObjectType.SuperscriptPart) &&
                   (MathObjectType != MathObjectType.Numerator) &&
                   (MathObjectType != MathObjectType.Denominator) &&
                   (MathObjectType != MathObjectType.FunctionName) &&
                   (MathObjectType != MathObjectType.Degree);
        }

        private static void ChildNodesToString(StringBuilder sb, CompositeNode rootNode, string indent)
        {
            foreach (Node node in rootNode)
            {
                if (node is OfficeMath)
                {
                    sb.AppendLine(((OfficeMath)node).ToStringInternal(indent));
                }
                else
                {
                    sb.AppendLine(indent + NodeTypeToString(node.NodeType));
                    if (node is CompositeNode)
                        ChildNodesToString(sb, (CompositeNode)node, indent + "  ");
                }
            }
        }

        internal bool IsTopLevel
        {
            get { return (ParentNode != null) && (ParentNode.NodeType != NodeType.OfficeMath); }
        }

        /// <summary>
        /// Returns true if office math must be rendered in the inline mode.
        /// </summary>
        internal bool IsInline
        {
            get { return OfficeMathUtil.IsInline(this); }
        }

        /// <summary>
        /// Returns true if office math is converted from Ruby.
        /// </summary>
        internal bool IsConvertedFromRuby
        {
            set { mIsConvertedFromRuby = value; }
            get { return mIsConvertedFromRuby; }
        }

        /// <summary>
        /// Returns true if office math is converted from EQ field.
        /// </summary>
        internal bool IsConvertedFromEQ
        {
            set { mIsConvertedFromEQ = value; }
            get { return mIsConvertedFromEQ; }
        }

        /// <summary>
        /// Specifies the horizontal offset.
        /// </summary>
        /// <remarks>
        /// Specifies the offset applied to the element defined by the \d EQ token (displacement).
        /// </remarks>
        internal int Displacement { get; set; }

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return mRunPr.GetDirectAttr(key);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mRunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int fontAttr)
        {
            return InlineHelper.FetchInheritedAttr(this, fontAttr);
        }

        void IRunAttrSource.SetRunAttr(int fontAttr, object value)
        {
            mRunPr.SetAttr(fontAttr, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mRunPr.Clear();
        }

        #endregion IRunAttrSource

        /// <summary>
        /// Returns <see cref="NodeType.OfficeMath"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.OfficeMath; }
        }

        /// <summary>
        /// Retrieves the parent <see cref="Paragraph"/> of this node.
        /// </summary>
        public Paragraph ParentParagraph
        {
            get { return (Paragraph)GetAncestor(NodeType.Paragraph); }
        }

        /// <summary>
        /// All properties specific to particular math entities are encapsulated inside these descendants.
        /// </summary>
        internal MathObject MathObject
        {
            get { return mMathObject; }
            set { mMathObject = value; }
        }

        internal RunPr RunPr
        {
            get { return mRunPr; }
        }

        Paragraph IInline.ParentParagraph_IInline
        {
            get { return this.ParentParagraph; }
        }

        DocumentBase IInline.Document_IInline
        {
            get { return base.Document; }
        }

        RunPr IInline.GetExpandedRunPr_IInline(RunPrExpandFlags flags)
        {
            return InlineHelper.GetExpandedRunPr(this, flags);
        }

        RunPr IInline.RunPr_IInline
        {
            get { return mRunPr; }
            set { mRunPr = value; }
        }

        /// <summary>
        /// Full copy of #DEBUG Node.ToString(). Used by OdtWriter to generate Id for OfficeMath objects.
        /// </summary>
        internal string GetId()
        {
            StringBuilder sb = new StringBuilder(NodeTypeToString(NodeType));
            sb.Append(' ');

            for (Node node = this; node != null && node.ParentNode != null; node = node.ParentNode)
            {
                int index = 0;
                for (Node n = node.ParentNode.FirstChild; n != null && n != node; n = n.NextSibling)
                    index++;
                sb.AppendFormat("{0}{1}", (node == this ? "" : "."), index);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets type <see cref="MathObjectType"/> of this Office Math object.
        /// </summary>
        public MathObjectType MathObjectType
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return mMathObject.MathObjectType; }
        }

        /// <summary>
        /// Gets/sets an encoding that was used to encode equation XML, if this office math object is read from
        /// equation XML.
        /// </summary>
        /// <dev>
        /// We use the encoding on saving a document to write in same encoding that it was read.
        /// </dev>
        internal Encoding EquationXmlEncoding
        {
            get { return mEquationXmlEncoding; }
            set { mEquationXmlEncoding = value; }
        }

        /// <summary>
        /// Gets/sets Office Math justification.
        /// </summary>
        /// <remarks>
        /// <para>Justification cannot be set to the Office Math with display format type <see cref="OfficeMathDisplayType.Inline"/>.</para>
        /// <para>Inline justification cannot be set to the Office Math with display format type <see cref="OfficeMathDisplayType.Display"/>.</para>
        /// <para>Corresponding <see cref="DisplayType"/> has to be set before setting Office Math justification.</para>
        /// </remarks>
        public OfficeMathJustification Justification
        {
            get
            {
                if (DisplayType == OfficeMathDisplayType.Display)
                    return ((MathObjectOMathPara)MathObject).Justification;

                return OfficeMathJustification.Inline;
            }
            set
            {
                switch (DisplayType)
                {
                    case OfficeMathDisplayType.Display:
                    {
                        if (value == OfficeMathJustification.Inline)
                            throw new ArgumentException("Inline justification cannot be set to the Office Math displayed on its own line. " +
                                "Please, use OfficeMath.DisplayType property to change OfficeMathDisplayType.");

                        ((MathObjectOMathPara)MathObject).Justification = value;
                        break;
                    }
                    case OfficeMathDisplayType.Inline:
                    {
                        if (value != OfficeMathJustification.Inline)
                            throw new ArgumentException("Justification cannot be set to the Office Math displayed inline with text. " +
                                "Please, use OfficeMath.DisplayType property to change OfficeMathDisplayType.");
                        break;
                    }
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        /// <summary>
        /// Gets/sets Office Math display format type which represents whether an equation is displayed inline with the text
        /// or displayed on its own line.
        /// </summary>
        /// <remarks>
        /// <para>Display format type has effect for top level Office Math only.</para>
        /// <para>Returned display format type is always <see cref="OfficeMathDisplayType.Inline"/> for nested Office Math.</para>
        /// </remarks>
        public OfficeMathDisplayType DisplayType
        {
            get
            {
                if (MathObject.MathObjectType == MathObjectType.OMathPara)
                    return OfficeMathDisplayType.Display;

                return OfficeMathDisplayType.Inline;
            }
            set
            {
                // DisplayType can be changed for top level Office Math only.
                // OMath paragraph cannot be nested inside another OMath paragraph.
                if (!IsTopLevel)
                    throw new ArgumentException("DisplayType cannot be changed for the nested Office Math. Please, check the parent node type to make sure it is top level Office Math.");

                switch (value)
                {
                    case OfficeMathDisplayType.Inline:
                        OfficeMathUtil.ChangeToInline(this);
                        break;
                    case OfficeMathDisplayType.Display:
                        OfficeMathUtil.ChangeToDisplay(this);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }

        private MathObject mMathObject;
        private RunPr mRunPr;
        private Encoding mEquationXmlEncoding;
        private bool mIsConvertedFromRuby;
        private bool mIsConvertedFromEQ;

        EditRevision ITrackableNode.InsertRevision
        {
            get { return RunPr.InsertRevision; }
            set { RunPr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return RunPr.DeleteRevision; }
            set { RunPr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return RunPr.MoveFromRevision; }
            set { RunPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return RunPr.MoveToRevision; }
            set { RunPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            RunPr.Remove(RevisionAttr.MoveFromRevision);
            RunPr.Remove(RevisionAttr.MoveToRevision);
        }
    }
}
