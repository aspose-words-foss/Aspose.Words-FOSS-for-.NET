// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// AG_ConstraintAttributes
    /// </summary>
    internal class DmlConstraintAttributes
    {
        /// <summary>
        /// Create empty self constraint attributes.
        /// </summary>
        public DmlConstraintAttributes()
            : this(DmlConstraintType.None)
        {
        }
        
        /// <summary>
        /// Create self constraint attributes with specified type.
        /// </summary>
        public DmlConstraintAttributes(DmlConstraintType type)
        : this(type, DmlConstraintRelationship.Self, string.Empty, DmlElementType.All)
        {
            
        }
        
        /// <summary>
        /// Create constraint attributes.
        /// </summary>
        public DmlConstraintAttributes(DmlConstraintType type, DmlConstraintRelationship forAxis, string forName)
            : this(type, forAxis, forName, DmlElementType.All)
        {
            
        }
        
        /// <summary>
        /// Create constraint attributes.
        /// </summary>
        public DmlConstraintAttributes(DmlConstraintType type, DmlConstraintRelationship forAxis, string forName,
            DmlElementType pointType)
        {
            Type = type;
            For = forAxis;
            ForName = forName;
            PointType = pointType;
        }
        
        protected bool Equals(DmlConstraintAttributes other)
        {
            return Type == other.Type && For == other.For && ForName == other.ForName &&
                   PointType == other.PointType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((DmlConstraintAttributes)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (int)For;
                hashCode = (hashCode * 397) ^ (ForName != null
                    ? ForName.GetHashCode()
                    : 0);
                hashCode = (hashCode * 397) ^ (int)PointType;
                return hashCode;
            }
        }

        internal DmlConstraintType Type { get; }
        internal DmlConstraintRelationship For { get; }
        internal string ForName { get; }
        internal DmlElementType PointType { get; }
    }
}
