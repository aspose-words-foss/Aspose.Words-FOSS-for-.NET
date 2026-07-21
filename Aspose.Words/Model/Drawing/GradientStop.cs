// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2021 by Ilya Navrotskiy

using System;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents one gradient stop.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-graphic-elements/">Working with Graphic Elements</a> documentation article.</para>
    /// </summary>
    public class GradientStop
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStop" /> class.
        /// </summary>
        /// <param name="color">Represents the color of the gradient stop.</param>
        /// <param name="position">Represents the position of a stop within
        /// the gradient expressed as a percent in range 0.0 to 1.0.</param>
        public GradientStop(Color color, double position)
        {
            ArgumentUtil.CheckNotNull(color, "Color");
            ArgumentUtil.CheckRangeInclusive(position, 0.0, 1.0, "Position");

            DmlColor dmlColor =  DmlColor.CreateFromDrColor(DrColor.FromNativeColor(color));
            DmlGradientStop = new DmlGradientStop(position, dmlColor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStop" /> class.
        /// </summary>
        /// <param name="color">Represents the color of the gradient stop.</param>
        /// <param name="position">Represents the position of a stop within
        /// the gradient expressed as a percent in range 0.0 to 1.0.</param>
        /// <param name="transparency">Represents the transparency of a stop within
        /// the gradient expressed as a percent in range 0.0 to 1.0.</param>
        public GradientStop(Color color, double position, double transparency) : this(color, position)
        {
            Transparency = transparency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientStop" /> class.
        /// </summary>
        internal GradientStop(DmlGradientStop dmlGradientStop, IThemeProvider themeProvider,
            GradientStopCollection parentCollection)
        {
            ArgumentUtil.CheckNotNull(dmlGradientStop, "dmlGradientStop");

            DmlGradientStop = dmlGradientStop;
            mThemeProvider = themeProvider;
            mParentCollection = parentCollection;
        }

        /// <summary>
        /// Removes the gradient stop from the parent <see cref="GradientStopCollection"/>.
        /// </summary>
        public void Remove()
        {
            if (ParentCollection == null)
                throw new InvalidOperationException("The gradient stop doesn't belong to any collection.");

            ParentCollection.Remove(this);
        }

#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return string.Format("[{0} : {1}; Transparency = {2}]", Position, DmlColor, Transparency);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || (obj.GetType() != typeof(GradientStop)))
                return false;

            GradientStop other = (GradientStop)obj;
            return DmlGradientStop.Equals(other.DmlGradientStop);
        }
#endif

        /// <summary>
        /// Gets or sets a value representing the color of the gradient stop.
        /// </summary>
        public Color Color
        {
            get
            {
                return DrColor.ToColorFixAlpha().ToNativeColor();
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "Color");
                DmlGradientStop.Color = DmlColor.CreateFromDrColor(DrColor.FromNativeColor(value));
            }
        }

        /// <summary>
        /// Gets a value representing the color of the gradient stop without any modifiers.
        /// </summary>
        public Color BaseColor
        {
            get { return DmlColor.CreateUnmodifiedDrColor(mThemeProvider).ToColorFixAlpha().ToNativeColor(); }
        }

        /// <summary>
        /// Gets or sets a value representing the position of a stop within the gradient
        /// expressed as a percent in range 0.0 to 1.0.
        /// </summary>
        public double Position
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return DmlGradientStop.Position; }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Position");
                DmlGradientStop.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets a value representing the transparency of the gradient fill
        /// expressed as a percent in range 0.0 to 1.0.
        /// </summary>
        public double Transparency
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                if (ParentCollection != null)
                    return ParentCollection.ParentFill.Parent.GetTransparency(DmlColor);

                return (DmlColor.Alpha == null) ? 0.0 : DmlColor.Alpha.Value;
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Transparency");

                if (ParentCollection == null)
                    DmlColor.UpdateAlpha(value);
                else
                    ParentCollection.ParentFill.Parent.SetTransparency(DmlColor, value);
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="GradientStopCollection"/>.
        /// </summary>
        internal GradientStopCollection ParentCollection
        {
            get { return mParentCollection; }
            set
            {
                mParentCollection = value;

                // After the parent is set, we should recalculate transparency,
                // as it should be stored inverted for non-font fills.
                if ((mParentCollection != null) && (DmlColor.Alpha != null) && !(mParentCollection.ParentFill.Parent is Font))
                    DmlColor.UpdateAlpha(1 - DmlColor.Alpha.Value);
            }
        }

        /// <summary>
        /// The internal representation of this facade gradient stop.
        /// </summary>
        /// <remarks>
        /// This reference equals to a corresponding stop in internal <see cref="DmlGradientFill.GradientStops"/>
        /// of the parent collection when the parent collection is set up.
        /// </remarks>
        internal DmlGradientStop DmlGradientStop { get; }

        /// <summary>
        /// Gets color of the gradient stop.
        /// </summary>
        private DmlColor DmlColor
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return DmlGradientStop.Color; }
        }

        /// <summary>
        /// Gets color of the gradient stop.
        /// </summary>
        private DrColor DrColor
        {
            get { return DmlColor.CreateDrColor(mThemeProvider, null); }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private GradientStopCollection mParentCollection;
        private readonly IThemeProvider mThemeProvider;
    }
}
