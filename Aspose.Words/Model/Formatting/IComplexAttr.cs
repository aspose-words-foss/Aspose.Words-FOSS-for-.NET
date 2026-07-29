// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Complex attributes are the ones that are represented by a by-reference class
    /// (as opposed to a simple int, bool or string).
    ///
    /// Every complex attribute class needs to implement this interface
    /// so it can be correctly handled by AttrCollection.
    ///
    /// Some complex attributes (border or shading for example) can be inherited,
    /// when an attribute is inherited, it is skipped during cloning.
    /// </summary>
    internal interface IComplexAttr
    {
        /// <summary>
        /// Returns true if the attribute inherits the value from some parent.
        /// </summary>
        bool IsInheritedComplexAttr
        { 
            get;
        }

        /// <summary>
        /// Called to create a deep clone of the attribute. Will be called only for non inherited attribute values.
        /// </summary>
        IComplexAttr DeepCloneComplexAttr();
    }
}
