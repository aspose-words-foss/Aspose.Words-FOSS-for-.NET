// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Defines locks for a shape.
    /// </summary>
    /// <seealso cref="ShapeBase.Locks"/>
    internal class Locks
    {
        internal Locks(IShapeAttrSource parent)
        {
            Debug.Assert(parent != null);
            mParent = parent;
        }

        /// <summary>
        /// Determines whether the handles of a shape can be edited.
        /// </summary>
        public bool AdjustHandlesLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockAdjustHandles); }
            set { SetAttr(ShapeAttr.LockAdjustHandles, value); }
        }

        /// <summary>
        /// Determines whether the aspect ratio of a shape can be changed by an editor.
        /// </summary>
        public bool AspectRatioLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockAspectRatio); }
            set { SetAttr(ShapeAttr.LockAspectRatio, value); }
        }

        /// <summary>
        /// Determines whether cropping will be allowed in an editor.
        /// </summary>
        public bool CroppingLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockCropping); }
            set { SetAttr(ShapeAttr.LockCropping, value); }
        }

        /// <summary>
        /// Determines whether shapes can be grouped in an editor.
        /// </summary>
        public bool GroupingLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockAgainstGrouping); }
            set { SetAttr(ShapeAttr.LockAgainstGrouping, value); }
        }
        
        /// <summary>
        /// Determines whether the position of a shape is locked in an editor.
        /// </summary>
        public bool PositionLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockPosition); }
            set { SetAttr(ShapeAttr.LockPosition, value); }
        }

        /// <summary>
        /// Determines whether rotation of shapes will be allowed in an editor.
        /// </summary>
        public bool RotationLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockRotation); }
            set { SetAttr(ShapeAttr.LockRotation, value); }
        }

        /// <summary>
        /// Determines whether the shape is selectable in an editor.
        /// </summary>
        public bool SelectionLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockAgainstSelect); }
            set { SetAttr(ShapeAttr.LockAgainstSelect, value); }
        }

        /// <summary>
        /// Determines whether the AutoShapes type can be changed by an editor.
        /// </summary>
        public bool ShapeTypeLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockShapeType); }
            set { SetAttr(ShapeAttr.LockShapeType, value); }
        }

        /// <summary>
        /// Determines whether the text attached to a shape can be edited.
        /// </summary>
        public bool TextLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockText); }
            set { SetAttr(ShapeAttr.LockText, value); }
        }

        /// <summary>
        /// Determines whether the vertices of a path can be changed by an editor.
        /// </summary>
        public bool VerticesLocked
        {
            get { return (bool)FetchAttr(ShapeAttr.LockVertices); }
            set { SetAttr(ShapeAttr.LockVertices, value); }
        }

        private object FetchAttr(int key)
        {
            return mParent.FetchShapeAttr(key);
        }

        private void SetAttr(int key, object value)
        {
            mParent.SetShapeAttr(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IShapeAttrSource mParent;
    }
}
