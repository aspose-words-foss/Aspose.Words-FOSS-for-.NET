// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/10/2021 by Ilya Navrotskiy

using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing.Ole.Core
{
    /// <summary>
    /// Interface to access common properties of embedded objects.
    /// </summary>
    internal interface IEmbeddedObject
    {
        /// <summary>
        /// Returns name of the embedded object.
        /// </summary>
        [JavaThrows(true)]
        string GetName();

        /// <summary>
        /// Returns instance of <see cref="OleObject"/> object.
        /// </summary>
        [JavaThrows(true)]
        OleObject GetOleObject();

        /// <summary>
        /// Gets a Clsid of the embedded object.
        /// </summary>
        string ClsidInternal { get; }

        /// <summary>
        /// Returns the extension (with the leading dot).
        /// The extension must be for the inner (e.g. unwrapped) data that can be saved directly to a file.
        /// </summary>
        [JavaThrows(true)]
        string GetExtensionForUser(string progId);

        /// <summary>
        /// Returns the file name.
        /// </summary>
        [JavaThrows(true)]
        string GetFileNameForUser();

        /// <summary>
        /// Saves the embedded object data in a way that makes it a valid standalone file.
        /// </summary>
        [JavaThrows(true)]
        void SaveForUser(Stream stream, IShapeAttrSource attrSource);

        /// <summary>
        /// Gets a boolean value, indicating either the embedded object is Forms2Ole control.
        /// </summary>
        bool IsForms2OleControlInternal { get; }

        /// <summary>
        /// Gets or sets Id of the embedded object.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets a boolean value, indicating either the embedded object is empty.
        /// </summary>
        [JavaThrows(true)]
        bool IsEmpty { get; }
    }
}
