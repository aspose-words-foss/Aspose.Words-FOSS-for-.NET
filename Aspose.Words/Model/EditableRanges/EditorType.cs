// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/06/2013 by Andrey Noskov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the set of possible aliases (or editing groups) which can be used as aliases to
    /// determine if the current user shall be allowed to edit a single range 
    /// defined by an editable range within a document.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum EditorType
    {
        /// <summary>
        /// Means that editor type is not specified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Specifies that users associated with the Administrators group shall be allowed to edit editable ranges using
        /// this editing type when document protection is enabled.
        /// </summary>
        Administrators,

        /// <summary>
        /// Specifies that users associated with the Contributors group shall be allowed to edit editable ranges using
        /// this editing type when document protection is enabled.
        /// </summary>
        Contributors,

        /// <summary>
        /// Specifies that users associated with the Current group shall be allowed to edit editable ranges using this
        /// editing type when document protection is enabled.
        /// </summary>
        Current,

        /// <summary>
        /// Specifies that users associated with the Editors group shall be allowed to edit editable ranges using this
        /// editing type when document protection is enabled.
        /// </summary>
        Editors,

        /// <summary>
        /// Specifies that all users that open the document shall be allowed to edit editable ranges using this editing
        /// type when document protection is enabled.
        /// </summary>
        Everyone,

        /// <summary>
        /// Specifies that none of the users that open the document shall be allowed to edit editable ranges
        /// using this editing type when document protection is enabled.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that users associated with the Owners group shall be allowed to edit editable ranges using this
        /// editing type when document protection is enabled.
        /// </summary>
        Owners,

        /// <summary>
        /// Same as <see cref="Unspecified"/>.
        /// </summary>
        Default = Unspecified
    }
}
