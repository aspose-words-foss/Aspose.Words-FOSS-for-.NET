// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/10/2019 by Alexander Sevidov

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Specifies the type of a model in a VBA project.
    /// </summary>
    public enum VbaModuleType
    {
        /// <summary>
        /// A type of VBA project item that specifies a module for embedded macros and programmatic access operations 
        /// that are associated with a document.
        /// </summary>
        DocumentModule,

        /// <summary>
        /// A collection of subroutines and functions.
        /// </summary>
        ProceduralModule,

        /// <summary>
        /// A module that contains the definition for a new object. Each instance of a class creates a new object,
        /// and procedures that are defined in the module become properties and methods of the object.
        /// </summary>
        ClassModule,

        /// <summary>
        /// A VBA module that extends the methods and properties of an ActiveX control that has been registered with the project.
        /// </summary>
        DesignerModule
    }
}
