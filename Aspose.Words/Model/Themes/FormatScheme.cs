// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2011 by Alexey Titov
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// 20.1.4.1.14 fmtScheme (Format Scheme)
    /// This element contains the background fill styles, 
    /// effect styles, fill styles, and line styles which 
    /// define the style matrix for a theme. The style matrix 
    /// consists of subtle, moderate, and intense fills, lines, 
    /// and effects. The background fills are not generally 
    /// thought of to directly be associated with the matrix, 
    /// but do play a role in the style of the overall document. 
    /// Usually, a given object chooses a single line style, 
    /// a single fill style, and a single effect style in order 
    /// to define the overall final look of the object.
    /// </summary>
    internal class FormatScheme
    {
        /// <summary>
        /// Clones this instance of format scheme.
        /// </summary>
        internal FormatScheme Clone()
        {
            FormatScheme lhs = (FormatScheme)MemberwiseClone();

            lhs.mBackgroundFillStyles = new List<DmlFill>();
            foreach (DmlFill fill in mBackgroundFillStyles)
                lhs.mBackgroundFillStyles.Add(fill.Clone());

            lhs.mFillStyles = new List<DmlFill>();
            foreach (DmlFill fill in mFillStyles)
                lhs.mFillStyles.Add(fill.Clone());

            lhs.mLineStyles = new List<DmlOutline>();
            foreach (DmlOutline outline in mLineStyles)
                lhs.mLineStyles.Add(outline.Clone());

            lhs.mEffectStyles = new List<EffectStyle>(mEffectStyles);

            return lhs;
        }

        internal DmlFill GetBackgroundFillStyle(int index)
        {
            if (mBackgroundFillStyles.Count == 0)
                return new DmlNoFill();
            // If index is greater than number of elements 
            // the return the last element
            if (index >= mBackgroundFillStyles.Count)
                index = mBackgroundFillStyles.Count - 1;

            return mBackgroundFillStyles[index];
        }

        internal int AddBackgroundFillStyle(DmlFill fillStyle)
        {
            mBackgroundFillStyles.Add(fillStyle);
            return mBackgroundFillStyles.Count - 1;
        }

        internal DmlFill GetFillStyle(int index)
        {
            if (mFillStyles.Count == 0)
                return new DmlNoFill();

            // If index is greater than number of elements 
            // the return the last element
            if (index >= mFillStyles.Count)
                index = mFillStyles.Count - 1;

            return mFillStyles[index];
        }

        internal int AddFillStyle(DmlFill fillStyle)
        {
            mFillStyles.Add(fillStyle);
            return mFillStyles.Count - 1;
        }

        internal DmlOutline GetLineStyle(int index)
        {
            if (mLineStyles.Count == 0)
                return new DmlOutline(); // Return outline with no fill.

            // If index is greater than number of elements 
            // the return the last element
            if (index >= mLineStyles.Count)
                index = mLineStyles.Count - 1;
            return mLineStyles[index];
        }

        internal int AddLineStyle(DmlOutline lineStyle)
        {
            mLineStyles.Add(lineStyle);
            return mLineStyles.Count - 1;
        }

        internal EffectStyle GetEffectStyle(int index)
        {
            if (mEffectStyles.Count == 0)
                return new EffectStyle();

            // If index is greater than number of elements 
            // the return the last element
            if (index >= mEffectStyles.Count)
                index = mEffectStyles.Count - 1;
            return mEffectStyles[index];
        }

        internal int AddEffectStyle(EffectStyle effectStyle)
        {
            mEffectStyles.Add(effectStyle);
            return mEffectStyles.Count - 1;
        }

        /// <summary>
        /// Defines the name for the format scheme. 
        /// The name is simply a human readable string which 
        /// identifies the format scheme in the user interface.
        /// </summary>
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Returns number of background fill styles.
        /// </summary>
        internal int BackgroundFillStyleCount
        {
            get { return mBackgroundFillStyles.Count; }
        }

        /// <summary>
        /// Returns number of fill styles.
        /// </summary>
        internal int FillStyleCount
        {
            get { return mFillStyles.Count; }
        }

        /// <summary>
        /// Returns number of line styles.
        /// </summary>
        internal int LineStyleCount
        {
            get { return mLineStyles.Count; }
        }

        /// <summary>
        /// Returns number of effect styles.
        /// </summary>
        internal int EffectStyleCount
        {
            get { return mEffectStyles.Count; }
        }

        private List<DmlFill> mBackgroundFillStyles = new List<DmlFill>();
        private List<DmlFill> mFillStyles = new List<DmlFill>();
        private List<DmlOutline> mLineStyles = new List<DmlOutline>();
        private List<EffectStyle> mEffectStyles = new List<EffectStyle>();
        private string mName;
    }
}