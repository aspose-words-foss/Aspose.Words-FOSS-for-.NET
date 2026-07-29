// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2022 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a value for charts that display multilevel data.
    /// </summary>
    /// <dev>
    /// The Word 2016 chart types allow three or fewer number of levels. The older chart types allow an unlimited
    /// number of levels, but multilevel values are rarely used in these charts: let's limit the number of levels
    /// to three for now.
    /// </dev>
    public class ChartMultilevelValue
    {
        /// <summary>
        /// Initializes a new instance of this class that represents a three-level value.
        /// </summary>
        public ChartMultilevelValue(string level1, string level2, string level3)
        {
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
        }

        /// <summary>
        /// Initializes a new instance of this class that represents a two-level value.
        /// </summary>
        public ChartMultilevelValue(string level1, string level2)
        {
            Level1 = level1;
            Level2 = level2;
        }

        /// <summary>
        /// Initializes a new instance of this class that represents a single-level value.
        /// </summary>
        public ChartMultilevelValue(string level1)
        {
            Level1 = level1;
        }

        /// <summary>
        /// Gets a hash code for the current multilevel data object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 0;

                if (Level1 != null)
                    hash = hash * 31 + Level1.GetHashCode();

                if (Level2 != null)
                    hash = hash * 31 + Level2.GetHashCode();

                if (Level3 != null)
                    hash = hash * 31 + Level3.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified object is equal to the current multilevel data object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            else if (ReferenceEquals(this, obj))
                return true;
            else if (obj.GetType() != GetType())
                return false;

            ChartMultilevelValue value = (ChartMultilevelValue)obj;
            
            return (value.Level1 == Level1) && (value.Level2 == Level2) && (value.Level3 == Level3);
        }

        /// <summary>
        /// Gets the name of the chart top level that this value refers to.
        /// </summary>
        public string Level1 { get; }

        /// <summary>
        /// Gets the name of the chart intermediate level that this value refers to.
        /// </summary>
        public string Level2 { get; }

        /// <summary>
        /// Gets the name of the chart bottom level that this value refers to.
        /// </summary>
        public string Level3 { get; }
    }
}
