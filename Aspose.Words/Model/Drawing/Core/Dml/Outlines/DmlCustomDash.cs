// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// 20.1.8.21 custDash (Custom Dash)
    /// This element specifies a custom dashing scheme.
    /// It is a list of dash stop elements which represent
    /// building block atoms upon which the custom dashing scheme is built.
    /// </summary>
    internal class DmlCustomDash : DmlDash
    {
        internal override DmlDash Clone()
        {
            DmlCustomDash result = new DmlCustomDash();
            result.DashStops = new List<DmlDashStop>(DashStops.Count);
            foreach (DmlDashStop dashStop in DashStops)
                result.DashStops.Add(dashStop.Clone());

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

            DmlCustomDash value = (DmlCustomDash)obj;

            return (value.DashType == DashType) && ListUtil.CheckAreEquals(value.DashStops, DashStops);
        }

        public override int GetHashCode()
        {
            int hash = DashType.GetHashCode();
            foreach (DmlDashStop stop in DashStops)
                hash ^= stop.GetHashCode();
            return hash;
        }

        internal List<DmlDashStop> DashStops
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mDashStops == null)
                    mDashStops = new List<DmlDashStop>();
                return mDashStops;
            }
            set { mDashStops = value; }
        }

        internal override DmlDashType DashType
        {
            get { return DmlDashType.CustomDash; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private List<DmlDashStop> mDashStops;
    }
}
