// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2007 by Vladimir Averkin
using System;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the properties which shall be applied to a single latent style for this document.
    /// Latent styles refer to any set of known style definitions which have not been included
    /// in the current document but are known to a hosting application.
    /// </summary>
    /// <example>
    /// [Taken from ECMA TC 45]
    /// Consider a WordprocessingML document which contains text specified in one of two styles: Heading1 or Normal.
    /// Based on this, the document only needs to store the formatting properties for those two styles,
    /// saving the additional overhead which would be required to save all of the styles supported by the hosting application.
    /// However, if the documentProtection element (§2.15.1.28) specifies that the hosting application 
    /// shall prevent the use of any style whose locked element (§2.7.3.7) is set to false, 
    /// then the locking state of all styles known to that application become useful and necessary 
    /// to maintain the current state of the document. Using latent styles, this information may be stored
    /// without storing any formatting properties for those styles
    /// </example>
    internal class LatentStyle
    {
        internal LatentStyle(
            StyleIdentifier styleIdentifier, 
            bool locked, 
            bool quickStyle, 
            bool semiHidden, 
            int uiPriority,
            bool unhideWhenUsed)
        {
            if (styleIdentifier == StyleIdentifier.User)
                throw new ArgumentOutOfRangeException("styleIdentifier");

            mStyleIdentifier = styleIdentifier;
            mUIPriority = uiPriority;
            mLocked = locked;
            mSemiHidden = semiHidden;
            mUnhideWhenUsed = unhideWhenUsed;
            mQuickStyle = quickStyle;
        }

        /// <summary>
        /// Makes a deep clone.
        /// </summary>
        internal LatentStyle Clone()
        {
            // RK Memberwise clone is enough.
            return (LatentStyle)MemberwiseClone();
        }

        public bool Equals(LatentStyle lsd)
        {
            return
                (lsd.mLocked == mLocked) &&
                (lsd.mQuickStyle == mQuickStyle) &&
                (lsd.mSemiHidden == mSemiHidden) &&
                (lsd.mUIPriority == mUIPriority) &&
                (lsd.mUnhideWhenUsed == mUnhideWhenUsed);
        }
        
        internal StyleIdentifier StyleIdentifier
        {
            get { return mStyleIdentifier; }
        }

        /// <summary>
        /// Specifies the default setting for the locked element (§2.7.3.7) which shall be applied to
        /// the latent style with the matching style name value.
        /// </summary>
        internal bool Locked
        {
            get { return mLocked; }
            set { mLocked = value; }
        }

        /// <summary>
        /// Specifies the default setting for the qFormat element.
        /// </summary>
        internal bool QuickStyle
        {
            get { return mQuickStyle; }
            set { mQuickStyle = value; }
        }

        /// <summary>
        /// Specifies the default setting for the semiHidden element.
        /// </summary>
        internal bool SemiHidden
        {
            get { return mSemiHidden; }
            set { mSemiHidden = value; }
        }

        /// <summary>
        /// Specifies the default setting for the uiPriority element.
        /// </summary>
        internal int UIPriority
        {
            get { return mUIPriority; }
            set { mUIPriority = value; }
        }

        /// <summary>
        /// Specifies the default setting for the unhideWhenUsed element.
        /// </summary>
        internal bool UnhideWhenUsed
        {
            get { return mUnhideWhenUsed; }
            set { mUnhideWhenUsed = value; }
        }

        private readonly StyleIdentifier mStyleIdentifier;
        private bool mLocked;
        private bool mQuickStyle;
        private bool mSemiHidden;
        private int mUIPriority;
        private bool mUnhideWhenUsed;
    }
}
