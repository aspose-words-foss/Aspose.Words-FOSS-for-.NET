// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Represents a single value that occurs in a path of a shape.
    /// </summary>
    internal class PathValue
    {
        internal PathValue() : this(0, false)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal PathValue(int value) : this(value, false)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal PathValue(int value, bool isFormula)
        {
            mValue = value;
            mIsFormula = isFormula;
        }

        /// <summary>
        /// Returns true if this <see cref="PathValue"/> equals to <paramref name="other"/>.
        /// </summary>
        public bool Equals(PathValue other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return (mValue == other.mValue) && (mIsFormula == other.mIsFormula);
        }

        /// <summary>
        /// Clones this <see cref="PathValue"/>.
        /// </summary>
        internal PathValue Clone()
        {
            return (PathValue)MemberwiseClone();
        }
        
        /// <summary>
        /// True if the Value refers to a formula by index.
        /// False when the Value is just a value.
        /// </summary>
        internal bool IsFormula
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mIsFormula; }
        }

        /// <summary>
        /// The value or an index of a formula.
        /// </summary>
        internal int Value
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mValue; }
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", IsFormula ? "@" : "", Value);
        }

        private readonly bool mIsFormula;
        private readonly int mValue;
    }
}
