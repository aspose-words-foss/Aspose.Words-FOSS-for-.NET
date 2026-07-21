// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// The Frame control is a rectangular box with an optional label.
    /// </summary>
    internal class FrameControl : FormControl
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FrameControl(string name) : base(name)
        {
            Size = mDefaultSize;
        }

        /// <summary>
        /// Gets type of Forms 2.0 control.
        /// </summary>
        public override Forms2OleControlType Type
        {
            get { return Forms2OleControlType.Frame; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the Forms2 OleControl is composite.
        /// </summary>
        internal override bool IsComposite
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        protected override string ClsidVirtual
        {
            get { return FrameControlClsid; }
        }

        private readonly OleSize mDefaultSize = OleSize.FromPoints(216, 144);
    }
}
