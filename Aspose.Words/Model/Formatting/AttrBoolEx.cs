// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2010 by Roman Korchagin

using System;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// A boolean that can also act as toggle.
    /// </summary>
    /// <remarks>
    /// In the model this class is used for all "boolean" attributes of character properties.
    /// Everywhere in the code (default attribute values, values stored in collection,
    /// expanded attribute values etc) AttrBoolEx are used. AttrBoolEx are only converted to boolean
    /// in the public properties of the Font object and in the writers that need to write booleans.
    /// </remarks>
    internal class AttrBoolEx : Attr
    {
        /// <summary>
        /// We only have four global instances. No public creation of instances of this class is allowed.
        /// </summary>
        private AttrBoolEx(int value)
        {
            mValue = value;
        }

        /// <summary>
        /// Creates an AttrBoolEx from a boolean.
        /// </summary>
        internal static AttrBoolEx FromBool(bool value)
        {
            return (value) ? True : False;
        }

        /// <summary>
        /// This is the "raw" value. Should only be used in the binary DOC format I suppose.
        /// </summary>
        internal int Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Converts a true or false AttrBoolEx into a boolean.
        /// If the boolex value is "same" or "toggle", throws an exception.
        /// </summary>
        internal bool ToBool()
        {
            switch (mValue)
            {
                case TrueValue:
                    return true;
                case FalseValue:
                    return false;
                default:
                    throw new InvalidOperationException("Not expected other boolex values here.");
            }
        }

        /// <summary>
        /// Inverts a boolex value.
        /// If the boolex value is "same" or "toggle", throws an exception.
        /// </summary>
        internal AttrBoolEx Invert()
        {
            switch (mValue)
            {
                case TrueValue:
                    return False;
                case FalseValue:
                    return True;
                default:
                    throw new InvalidOperationException("Not expected other boolex values here.");
            }
        }

        /// <summary>
        /// RK This is a result of refactoring of earlier BoolEx to new AttrBoolEx.
        /// Will need more refactoring with other methods in this class, but later on.
        /// </summary>
        internal AttrBoolEx ResolveFetchAttr(AttrCollection parentAttrs, int key)
        {
            switch (mValue)
            {
                case TrueValue:
                case FalseValue:
                    return this;
                case SameValue:
                    // New value should be same as the existing value.
                    return (AttrBoolEx)parentAttrs.FetchAttr(key);
                case ToggleValue:
                    AttrBoolEx parentValue = (AttrBoolEx)parentAttrs.FetchAttr(key);

                    // New value should be the opposite of the existing value.
                    // AM. There are might be the case when we trying to resolve AttrBoolEx.Toggle over AttrBoolEx.Toggle.
                    // For example if collection having Bold = AttrBoolEx.Toggle expanded to the same collection.
                    // I don't fully understand value we need to get in this case and suspect it should be True.
                    return (parentValue.Value == ToggleValue) ? True : parentValue.Invert();
                default:
                    throw new InvalidOperationException("Unknown boolex value.");
            }
        }

        /// <summary>
        /// RK This is a result of refactoring of earlier BoolEx to new AttrBoolEx.
        /// Will need more refactoring with other methods in this class, but later on.
        /// </summary>
        internal AttrBoolEx ResolveFetchAttr(Font dstAttrs, int key)
        {
            switch (mValue)
            {
                case TrueValue:
                case FalseValue:
                    return this;
                case SameValue:
                    // New value should be same as the existing value.
                    return (AttrBoolEx)dstAttrs.FetchAttr(key);
                case ToggleValue:
                    // New value should be the opposite of the existing value.
                    return ((AttrBoolEx)dstAttrs.FetchAttr(key)).Invert();
                default:
                    throw new InvalidOperationException("Unknown boolex value.");
            }
        }

        /// <summary>
        /// RK This is a result of refactoring of earlier BoolEx to new AttrBoolEx.
        /// Will need more refactoring with other methods in this class, but later on.
        /// </summary>
        [JavaThrows(false)]
        internal AttrBoolEx ResolveFetchInheritedRunAttr(IRunAttrSource src, int key)
        {
            switch (mValue)
            {
                case TrueValue:
                case FalseValue:
                    return this;
                case SameValue:
                    return (AttrBoolEx)src.FetchInheritedRunAttr(key);
                case ToggleValue:
                    return ((AttrBoolEx)src.FetchInheritedRunAttr(key)).Invert();
                default:
                    throw new InvalidOperationException("Unknown BoolEx value.");
            }
        }

        /// <summary>
        /// RK This is a result of refactoring of earlier BoolEx to new AttrBoolEx.
        /// Will need more refactoring with other methods in this class, but later on.
        /// </summary>
        internal object ResolveFetchInheritedRunAttrWithNull(IRunAttrSource src, int key)
        {
            switch (mValue)
            {
                case TrueValue:
                    return true;
                case FalseValue:
                    return false;
                case SameValue:
                    return null;
                case ToggleValue:
                {
                    // Invert the inherited BoolEx value and convert to boolean.
                    if (src != null)
                        return ((AttrBoolEx)src.FetchInheritedRunAttr(key)).Invert().ToBool();
                    else
                        return null;
                }
                default:
                    throw new InvalidOperationException("Unknown BoolEx value.");
            }
        }

        public override int GetHashCode()
        {
            return mValue.GetHashCode();
        }

#if DEBUG
        public override string ToString()
        {
            switch (mValue)
            {
                case FalseValue: return "False";
                case TrueValue: return "True";
                case SameValue: return "Same";
                case ToggleValue: return "Toggle";
                default:
                    Debug.Fail("Invalid value.");
                    return "";
            }
        }
#endif

        private readonly int mValue;

        private const int FalseValue = 0;
        private const int TrueValue = 1;
        private const int SameValue = 128;
        private const int ToggleValue = 129;

        internal static readonly AttrBoolEx False = new AttrBoolEx(FalseValue);
        internal static readonly AttrBoolEx True = new AttrBoolEx(TrueValue);

        /// <summary>
        /// The value is same as the inherited value.
        /// </summary>
        internal static readonly AttrBoolEx Same = new AttrBoolEx(SameValue);

        /// <summary>
        /// The value is the opposite of the inherited value.
        /// </summary>
        internal static readonly AttrBoolEx Toggle = new AttrBoolEx(ToggleValue);
    }
}
