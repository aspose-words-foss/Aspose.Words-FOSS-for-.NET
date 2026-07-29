// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/01/2019 by Ilya Navrotskiy

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements a Tab of the TabStripControl.
    /// </summary>
    internal class TabStripTab
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TabStripTab(string caption)
        {
            mCaption = caption;
        }

        /// <summary>
        /// Gets or sets a caption of the tab.
        /// </summary>
        internal string Caption
        {
            get { return mCaption; }
            set { mCaption = value; }
        }

        /// <summary>
        /// Gets or sets a tooltip of the tab.
        /// </summary>
        internal string Tip
        {
            get { return mTip; }
            set { mTip = value; }
        }

        /// <summary>
        /// Gets or sets a name of the tab.
        /// </summary>
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets or sets a tag of the tab.
        /// </summary>
        internal string Tag
        {
            get { return mTag; }
            set { mTag = value; }
        }

        /// <summary>
        /// Gets or sets an accelerator of the tab.
        /// </summary>
        internal char Accelerator
        {
            get { return mAccelerator; }
            set { mAccelerator = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether the tab is visible.
        /// </summary>
        internal bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }
        
        /// <summary>
        /// Gets or sets a boolean value that specifies whether the tab is enabled.
        /// </summary>
        internal bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a TabFlag value, that combines visible and enabled properties.
        /// </summary>
        internal TabFlag TabFlag
        {
            get
            {
                TabFlag tabFlag = TabFlag.None;

                if (Visible)
                    tabFlag |= TabFlag.Visible;
                if (Enabled)
                    tabFlag |= TabFlag.Enabled;

                return tabFlag;
            }
            set
            {
                Visible = ((value & TabFlag.Visible) != 0);
                Enabled = ((value & TabFlag.Enabled) != 0);
            }
        }

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.9.7 TabStripTabFlag.
        /// </summary>
        private bool mVisible = true;
        private bool mEnabled = true;

        /// <summary>
        /// Implements [MS-OFORMS] 2.2.9.4 TabStripExtraDataBloc.
        /// </summary>
        private string mCaption;
        private string mTip = "";
        private string mName = "";
        private string mTag = "";
        private char mAccelerator = '\0';
    }
}
