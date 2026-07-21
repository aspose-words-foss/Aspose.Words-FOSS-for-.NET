// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2011 by Alexey Titov

using System;
using Aspose.Drawing;
using _Aspose = Aspose.Words.Drawing;
using Drawing2D = System.Drawing.Drawing2D;

// NOTE! _Aspose prefix is added for autoporter. DashStyle class name clashes in JAVA without this prefex. Please do not remove it.
namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// 20.1.8.48 prstDash (Preset Dash)
    /// This element specifies that a preset line dashing scheme should be used.
    /// </summary>
    internal class DmlPresetDash : DmlDash
    {
        internal override DmlDash Clone()
        {
            DmlPresetDash result = new DmlPresetDash();
            result.Preset = Preset;
            return result;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlPresetDash value = (DmlPresetDash)obj;

            return (value.DashType == DashType) && (value.Preset == Preset);
        }

        public override int GetHashCode()
        {
            int hash = DashType.GetHashCode();
            hash ^= Preset.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies which preset dashing scheme is to be used.
        /// </summary>
        internal _Aspose.DashStyle Preset
        {
            get { return mPreset; }
            set { mPreset = value; }
        }

        internal override DmlDashType DashType
        {
            get { return DmlDashType.PresetDash; }
        }

        private static readonly float[] gDashDotPattern = new float[] {4, 3, 1, 3};
        private static readonly float[] gDashPattern = new float[] {4, 3};
        private static readonly float[] gDotPattern = new float[] {1, 3};
        private static readonly float[] gLargeDashDotDotPattern = new float[] {8, 3, 1, 3, 1, 3};
        private static readonly float[] gLargeDashDotPattern = new float[] {8, 3, 1, 3};
        private static readonly float[] gLargeDashPattern = new float[] {8, 3};
        private _Aspose.DashStyle mPreset = _Aspose.DashStyle.Solid;
    }
}
