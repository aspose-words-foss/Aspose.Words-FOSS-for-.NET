// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.Path
{
    /// <summary>
    /// Represents a path.
    /// </summary>
    /// <remarks>
    /// 20.1.9.15 path (Shape Path)
    /// This element specifies a creation path consisting of a series of moves, lines and curves that when 
    /// combined forms a geometric shape. This element is only utilized if a custom geometry is specified. 
    /// </remarks>
    internal class DmlPath
    {
        /// <summary>
        /// Adds the path part.
        /// </summary>
        /// <param name="dmlPathPart">The DML path part. If null then the method do nothing.</param>
        internal void AddPathPart(IDmlPathPart dmlPathPart)
        {
            if (dmlPathPart == null)
                return;
            mPathParts.Add(dmlPathPart);
        }

        /// <summary>
        /// Clones this instance of DML path.
        /// </summary>
        internal DmlPath Clone()
        {
            DmlPath lhs = (DmlPath)MemberwiseClone();

            if (lhs.mPathParts != null)
            {
                lhs.mPathParts = new List<IDmlPathPart>();
                foreach (IDmlPathPart part in mPathParts)
                    lhs.mPathParts.Add(part.Clone());
            }

            return lhs;
        }

        /// <summary>
        /// Gets the path parts.
        /// </summary>
        internal IList<IDmlPathPart> PathParts
        {
            get { return mPathParts; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// Once set, <see cref="WidthDefined"/> assigns to true.
        /// </summary>
        /// <remarks>
        /// Specifies the width, or maximum x coordinate that should be used for within the path coordinate 
        /// system. This value determines the horizontal placement of all points within the corresponding 
        /// path as they are all calculated using this width attribute as the max x coordinate.
        /// </remarks>
        internal double Width
        {
            get { return mWidth; }
            set
            {
                mWidth = value;
                mWidthDefined = true;
            }
        }

        /// <summary>
        /// Gets or sets the height. 
        /// Once set, <see cref="HeightDefined"/> assigns to true.
        /// </summary>
        /// <remarks>
        /// Specifies the height, or maximum y coordinate that should be used for within the path coordinate 
        /// system. This value determines the vertical placement of all points within the corresponding 
        /// path as they are all calculated using this height attribute as the max y coordinate.
        /// </remarks>
        internal double Height
        {
            get { return mHeight; }
            set
            {
                mHeight = value;
                mHeightDefined = true;
            }
        }

        /// <summary>
        /// Gets or sets the stroke .
        /// </summary>
        /// <remarks>
        /// Specifies if the corresponding path should have a path stroke shown. This is a Boolean value that 
        /// affect the outline of the path. If this attribute is omitted, a value of true is assumed.
        /// </remarks>
        internal bool Stroke
        {
            get { return mStroke; }
            set { mStroke = value; }
        }

        /// <summary>
        /// Specifies how the corresponding path should be filled. 
        /// If this attribute is omitted, a value of "norm" is assumed.
        /// </summary>
        internal DmlPathFillMode FillMode
        {
            get { return mFillMode; }
            set { mFillMode = value; }
        }

        /// <summary>
        /// Shows if <see cref="Width"/> was defined in the document. 
        /// </summary>
        /// <remarks>
        /// Used for writing purposes.
        /// </remarks>
        internal bool WidthDefined
        {
            get { return mWidthDefined; }
        }

        /// <summary>
        /// Shows if <see cref="Height"/> was defined in the document. 
        /// </summary>
        /// <remarks>
        /// Used for writing purposes.
        /// </remarks>        
        internal bool HeightDefined
        {
            get { return mHeightDefined; }
        }

        /// <summary>
        /// Specifies that the use of 3D extrusions are possible on this path. 
        /// </summary>
        internal bool ExtrusionOk
        {
            get { return mExtrusionOk; }
            set { mExtrusionOk = value; }
        }        

        private DmlPathFillMode mFillMode;

        private double mHeight = -1; 
        private List<IDmlPathPart> mPathParts = new List<IDmlPathPart>();
        private bool mStroke = true;
        private double mWidth = -1; 
        private bool mWidthDefined = false;
        private bool mHeightDefined = false;
        private bool mExtrusionOk = false;
    }
}