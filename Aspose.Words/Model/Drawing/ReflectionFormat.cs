// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2024 by Ilya Navrotskiy

using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents the reflection formatting for an object.
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="ShapeBase.Reflection"/> property to access reflection properties of an object.
    /// You do not create instances of the <see cref="ReflectionFormat"/> class directly.</p>
    /// </remarks>
    /// <dev>
    /// This class is the public facade for <see cref="IReflection"/>.
    /// https://learn.microsoft.com/en-us/office/vba/api/word.reflectionformat
    /// </dev>
    public class ReflectionFormat
    {
        /// <summary>
        /// Creates an instance of the <see cref="ReflectionFormat"/> class.
        /// </summary>
        internal ReflectionFormat(IReflection parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Removes <see cref="ReflectionFormat"/> from the parent object.
        /// </summary>
        public void Remove()
        {
            mParent.RemoveReflection();
        }

        /// <summary>
        /// Gets or sets a double value that specifies the degree of blur effect applied to the reflection effect in points.
        /// The default value is 0.0.
        /// </summary>
        public double Blur
        {
            get { return mParent.Blur; }
            set { mParent.Blur = value; }
        }

        /// <summary>
        /// Gets or sets a double value that specifies the amount of separation of the reflected image from the object in points.
        /// The default value is 0.0.
        /// </summary>
        public double Distance
        {
            get { return mParent.Distance; }
            set { mParent.Distance = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 and 1.0 representing the size of the reflection
        /// as a percentage of the reflected object.
        /// The default value is 0.0.
        /// </summary>
        public double Size
        {
            get { return mParent.ReflectionSize; }
            set { mParent.ReflectionSize = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency for the reflection effect.
        /// The default value is 0.0.
        /// </summary>
        public double Transparency
        {
            get { return mParent.ReflectionTransparency; }
            set { mParent.ReflectionTransparency = value; }
        }

        /// <summary>
        /// The parent object.
        /// </summary>
        private readonly IReflection mParent;
    }
}
