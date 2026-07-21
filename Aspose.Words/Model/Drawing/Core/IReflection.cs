// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2024 by Ilya Navrotskiy

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Implements interface for object that can have <see cref="ReflectionFormat"/>.
    /// </summary>
    internal interface IReflection
    {
        /// <summary>
        /// Removes reflection from the parent object;
        /// </summary>
        void RemoveReflection();

        /// <summary>
        /// Gets or sets a double value that specifies the degree of blur effect applied to the reflection effect.
        /// </summary>
        double Blur { get; set; }

        /// <summary>
        /// Gets or sets a double value that specifies the amount of separation of the reflected image from the object.
        /// </summary>
        double Distance { get; set; }

        /// <summary>
        /// Gets or sets a double value between 0.0 and 1.0 representing the size of the reflection
        /// as a percentage of the reflected object.
        /// </summary>
        double ReflectionSize { get; set; }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency for the reflection effect.
        /// </summary>
        double ReflectionTransparency { get; set; }
    }
}
