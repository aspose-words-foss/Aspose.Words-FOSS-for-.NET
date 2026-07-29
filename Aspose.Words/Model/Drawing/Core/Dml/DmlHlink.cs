// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2016 by Andrey Noskov

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Common object for:
    /// 5.1.5.3.5 hlinkClick (Click Hyperlink)
    /// 5.1.2.1.23 hlinkHover (Hyperlink for Hover)
    /// 21.1.2.3.5 hlinkClick (Click Hyperlink)
    /// 21.1.2.3.6 hlinkMouseOver (Mouse-Over Hyperlink)
    /// </summary>
    internal class DmlHlink : DmlExtensionListSource
    {
        public DmlHlink Clone()
        {
            DmlHlink lhs = (DmlHlink)MemberwiseClone();
            lhs.Extensions = lhs.CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Drawing Object Hyperlink Target.
        /// Specifies the relationship id that when looked up in this slides relationship file contains 
        /// the target of this hyperlink. This attribute cannot be omitted.
        /// </summary>
        public string Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// Specifies the target frame that is to be used when opening this hyperlink. 
        /// </summary>
        public string TargetFrame
        {
            get { return mTargetFrame; }
            set { mTargetFrame = value; }
        }

        /// <summary>
        /// Hyperlink Tooltip.
        /// Specifies the tooltip that should be displayed when the hyperlink text is hovered over with the mouse.
        /// If this attribute is omitted, than the hyperlink text itself can be displayed.
        /// </summary>
        public string Tooltip
        {
            get { return mTooltip; }
            set { mTooltip = value; }
        }

        private string mId;
        private string mTargetFrame;
        private string mTooltip;
    }
}
