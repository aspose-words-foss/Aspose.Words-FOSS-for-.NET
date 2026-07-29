// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2017 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// Stores values of the 2.24.4.5 ST_DoubleOrAutomatic, 2.24.4.22 ST_ValueAxisUnit, 2.24.4.8 ST_GapWidthRatio
    /// simple types [MS-ODRAWXML].
    /// </summary>
    internal class DoubleOrAutomatic
    {
        /// <summary>
        /// Ctor that allows instance creation only from static methods.
        /// </summary>
        private DoubleOrAutomatic()
        {
        }

        /// <summary>
        /// Creates numeric value of <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        internal static DoubleOrAutomatic FromDouble(double value)
        {
            DoubleOrAutomatic result = new DoubleOrAutomatic();
            result.mValue = value;
            return result;
        }

        /// <summary>
        /// Creates not-defined value of <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        internal static DoubleOrAutomatic AsNull(double defaultValue)
        {
            DoubleOrAutomatic result = new DoubleOrAutomatic();
            result.mValue = defaultValue;
            result.IsNull = true;
            return result;
        }

        /// <summary>
        /// Creates automatic value of <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        internal static DoubleOrAutomatic AsAuto(double defaultValue)
        {
            DoubleOrAutomatic result = new DoubleOrAutomatic();
            result.mValue = defaultValue;
            result.IsAuto = true;
            return result;
        }

        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        internal DoubleOrAutomatic Clone()
        {
            return (DoubleOrAutomatic)MemberwiseClone();
        }

        /// <summary>
        /// Gets or sets numeric value of this instance of <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        internal double Value
        {
            get { return mValue; }
            set
            {
                mValue = value;
                mIsAuto = false;
                mIsNull = false;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating that automatic value should be used.
        /// </summary>
        internal bool IsAuto
        {
            get { return mIsAuto; }
            set
            {
                mIsAuto = value;
                if (value)
                    mIsNull = false;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating that no value is defined in this instance of <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        internal bool IsNull
        {
            get { return mIsNull; }
            set
            {
                mIsNull = value;
                if (value)
                    mIsAuto = false;
            }
        }

        /// <summary>
        /// Gets a flag indicating that this instance has not-defined or automatic value.
        /// </summary>
        internal bool IsNullOrAuto
        {
            get { return mIsNull || mIsAuto; }
        }

        private bool mIsNull;
        private bool mIsAuto;
        private double mValue;
    }
}
