// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.229 view3D (View In 3D) element.
    /// This element specifies the 3-D view of the chart. 
    /// </summary>
    internal class DmlChartView3D : DmlExtensionListSource
    {
        internal DmlChartView3D Clone()
        {
            DmlChartView3D lhs = (DmlChartView3D)MemberwiseClone();
            lhs.Extensions = CloneExtensions();
            return lhs;

        }

        /// <summary>
        /// Valid values are integers between 20 and 2000 inclusive.
        /// </summary>
        internal int DepthPercent
        {
            get { return mDepthPercent; }
            set
            {
                if (value >= 20 && value <= 2000)
                    mDepthPercent = value;
            }
        }

        /// <summary>
        /// Valid values are integers between 5 and 500 inclusive.
        /// </summary>
        internal int HPercent
        {
            get { return mHPercent; }
            set
            {
                if (value >= 5 && value <= 500)
                    mHPercent = value;
            }
        }

        /// <summary>
        /// Valid values are integers between 0 and 100 inclusive.
        /// </summary>
        internal int Perspective
        {
            get { return mPerspective; }
            set
            {
                if (value >= 0 && value <= 100)
                    mPerspective = value;
            }
        }

        /// <summary>
        /// Specifies that the chart axes are at right angles, rather than drawn in perspective.
        /// </summary>
        internal bool RAngAx
        {
            get { return mRAngAx; }
            set { mRAngAx = value; }
        }

        /// <summary>
        /// Valid values are integers between -90 and 90 inclusive.
        /// By specification default value is zero, however in MS Word if this property is omitted, it uses value 15 degrees.
        /// Note: rotation X and Y in MS Word UI are switched.
        /// </summary>
        internal int RotX
        {
            get { return mRotX; }
            set
            {
                if (value >= -90 && value <= 90)
                    mRotX = value;
            }
        }

        /// <summary>
        /// Valid values are integers between 0 and 360 inclusive.
        /// By specification default value is zero, however in MS Word if this property is omitted, it uses value 20 degrees.
        /// Note: rotation X and Y in MS Word UI are switched.
        /// </summary>
        internal int RotY
        {
            get { return mRotY; }
            set
            {
                mHasRotY = true;
                if (value >= 0 && value <= 360)
                    mRotY = value;
            }
        }

        internal bool HasRotY
        {
            get { return mHasRotY; }
        }

        private int mDepthPercent = DefaultDepthPercent;
        private int mHPercent = DefaultHPercent;
        private int mPerspective = DefaultPerspective;
        private int mRotX = DefaultRotX;
        private int mRotY = DefaultRotY;
        private bool mRAngAx;
        private bool mHasRotY;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultDepthPercent = 100;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultHPercent = 100;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultPerspective = 30;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultRotX = 15;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultRotY = 20;
    }
}
