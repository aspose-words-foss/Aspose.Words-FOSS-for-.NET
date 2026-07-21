// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2012 by Denis Darkin

using System.Collections.Generic;

namespace Aspose.Words.Tests
{
    /// <summary>
    /// Collects all warnings for later assertions.
    /// </summary>
    public class TestWarningCallback : IWarningCallback
    {
        public void Warning(WarningInfo info)
        {
            mWarnings.Add(info);
            WarningAdded();
        }

        public WarningInfo this[int i]
        {
            get { return mWarnings[i]; }
        }

        /// <summary>
        /// Clears warning collection.
        /// </summary>
        public void Clear()
        {
            mWarnings.Clear();
        }

        public int Count
        {
            get { return mWarnings.Count; }
        }

        /// <summary>
        /// Returns true if a warning with the specified properties has been generated.
        /// </summary>
        public bool Contains(WarningSource source, WarningType type, string description)
        {
            foreach (WarningInfo warning in mWarnings)
            {
                if ((warning.Source == source) && (warning.WarningType == type) && (warning.Description == description))
                    return true;
            }
            return false;
        }

        protected virtual void WarningAdded()
        {
            // Can be overridden in a subclass
        }

        private readonly List<WarningInfo> mWarnings = new List<WarningInfo>();
    }
}
