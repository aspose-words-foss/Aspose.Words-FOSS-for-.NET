// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2010 by Alexey Morozov

using System;
using Aspose.Drawing;

namespace Aspose.Words.Framesets
{
    /// <summary>
    /// Represents a frames page or a single frame on a frames page.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// If the <see cref="ChildFramesets"/> property contains items, this instance is a frames page, otherwise it is
    /// a single frame.
    /// </remarks>
    /// <dev>
    /// The class represents both ordinary frame (window) and frameset (set of frames).
    /// 
    /// I don't like to split this class into separate classes for frame and frameset in two reasons:
    ///   - It simplifies reader because DOC uses one structure to defines them both.
    ///   - Proposed public API seems to has imitate Word's behavior i.e. split frame into two frames so 
    ///     is seems to me that dynamic frame type (Frame/Frameset) is better approach.
    /// 
    /// RK I am happy with this design. We are not making it public yet, so we can tweak things later if needed.
    /// An interesting observation in MS Word Automation this is called Frameset, not Frame. They use Frameset to represent
    /// functionality of both a single frame and a frameset. Maybe we should call this Frameset too?
    /// 
    /// Just in case we want to make our Frameset to look more like MS Word Automation Frameset here is how MS Word looks like:
    /// Frameset
    ///    ChildFramesetCount
    ///    ChildFramesetItem
    ///
    ///    FrameDefaultURL
    ///    FrameDisplayBorders
    ///    FrameLinkToFile
    ///    FrameName
    ///    FrameResizable
    ///    FrameScrollbarType
    ///
    ///    FramesetBorderColor    
    ///    FramesetBorderWidth
    ///
    ///    Height
    ///    HeightType
    ///    
    ///    ParentFrameset
    ///    
    ///    Type (frame/frameset)
    ///
    ///    Width
    ///    WidthType
    ///
    ///    AddNewFrame (top, bottom. left or right)
    ///    Delete - deletes this frameset object
    /// </dev>
    public class Frameset
    {
        /// <summary>
        /// Sets the <see cref="FrameDefaultUrl"/> property without any checks.
        /// </summary>
        internal void SetFrameDefaultUrlInternal(string value)
        {
            mFrameDefaultUrl = value;
        }

        /// <summary>
        /// Sets the <see cref="IsFrameLinkToFile"/> property without any checks.
        /// </summary>
        internal void SetIsFrameLinkToFile(bool value)
        {
            mIsFrameLinkToFile = value;
        }

        /// <summary>
        /// Gets or sets the web page URL or document file name to display in this frame.
        /// </summary>
        public string FrameDefaultUrl
        {
            get { return (HasChild ? null : mFrameDefaultUrl); }
            set
            {
                if (HasChild)
                {
                    throw new InvalidOperationException(
                        "The property cannot be set if the current instance is a frames page.");
                }

                mFrameDefaultUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the web page or document file name specified in the
        /// <see cref="FrameDefaultUrl"/> property is an external resource the frame is linked with.
        /// </summary>
        public bool IsFrameLinkToFile
        {
            get { return !HasChild && mIsFrameLinkToFile; }
            set
            {
                if (HasChild)
                {
                    throw new InvalidOperationException(
                        "The property cannot be set if the current instance is a frames page.");
                }

                mIsFrameLinkToFile = value;
            }
        }

        /// <summary>
        /// Gets the collection of child frames and frames pages.
        /// </summary>
        public FramesetCollection ChildFramesets
        {
            get { return mChildFramesets; }
        }

        /// <summary>
        /// Specifies advisory information about a single frame or frameset. 
        /// This property is analogous to the title attribute on the frame or frameset element in HTML.
        /// </summary>
        /// <remarks>
        /// This frame property is absent in binary DOC but present in DOCX.
        /// </remarks>
        internal string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Frame name or null if object is a frameset.
        /// </summary>
        internal string Name
        {
            get { return (HasChild ? null : mName); }
            set
            {
                // Frameset shouldn't have a name.
                Debug.Assert(!HasChild);
                // 17.15.2.30. name (Frame Name) (a) The standard does not limit the length of the value of the val attribute.
                // Word restricts the value of this attribute to be at most 255 characters.
                if (StringUtil.HasChars(value) && value.Length > 255)
                    value = value.Substring(0, 255);

                mName = value;
            }
        }

        /// <summary>
        /// Frame filename's data or <c>null</c> if this object is frameset.
        /// </summary>
        internal byte[] FileData
        {
            get { return ((HasChild) ? null : mFileData); }
            set
            {
                // If this object is frameset it shouldn't have file and data.
                Debug.Assert(!HasChild);
                mFileData = value;
            }
        }

        /// <summary>
        /// Specifies the units of a frame divider position.
        /// </summary>
        internal FrameDividerPositionType DividerPositionType
        {
            get { return mDividerPositionType; }
            set { mDividerPositionType = value; }
        }

        /// <summary>
        /// Specifies the position of a frame divider.
        /// </summary>
        internal int DividerPositionValue
        {
            get { return mDividerPositionValue; }
            set { mDividerPositionValue = value; }
        }

        /// <summary>
        /// Specifies how child frames are arranged.
        /// </summary>
        internal FrameLayoutType LayoutType
        {
            get { return mLayoutType; }
            set { mLayoutType = value; }
        }

        /// <summary>
        /// Specifies the left and right margins, in pixels, for this frame.
        /// </summary>
        internal int MarginX
        {
            get { return mMarginX; }
            set { mMarginX = value; }
        }

        /// <summary>
        /// Specifies the top and bottom margins, in pixels, for this frame.
        /// </summary>
        internal int MarginY
        {
            get { return mMarginY; }
            set { mMarginY = value; }
        }

        /// <summary>
        /// Specifies the scrollbar behavior for the frame.
        /// </summary>
        internal FrameScrollType ScrollType
        {
            get { return mScrollType; }
            set { mScrollType = value; }
        }

        /// <summary>
        /// Specifies whether the size of the frame is locked and cannot be changed.
        /// </summary>
        internal bool NoResize
        {
            get { return mNoResize; }
            set { mNoResize = value; }
        }

        /// <summary>
        /// Returns true if Frame object has any child Frame.
        /// </summary>
        internal bool HasChild
        {
            get { return ChildFramesets.Count > 0; }
        }

        /// <summary>
        /// Specifies frame border type for entire frameset.
        /// </summary>
        internal FramesetBorderType FramesetBorderType
        {
            get { return mFramesetBorderType; }
            set { mFramesetBorderType = value; }
        }

        /// <summary>
        /// Specifies frame border color for entire frameset.
        /// </summary>
        internal DrColor FramesetBorderColor
        {
            get { return mFramesetBorderColor; }
            set { mFramesetBorderColor = value; }
        }

        /// <summary>
        /// Specifies the width, in twips, of the borders and dividers.
        /// </summary>
        /// <remarks>
        /// If this value is 0 the default border width is used.
        /// 
        /// In docx default width of the splitters in this document shall be 4.5 points (90 twentieths of a point) wide.
        /// Spec says If the noBorder element (§17.15.2.31) is also specified, then this element shall be ignored and no splitters shall be displayed.
        /// you should mention about this in remarks for this property.
        /// </remarks>
        internal int FramesetBorderWidth
        {
            get { return mFramesetBorderWidth; }
            set { mFramesetBorderWidth = value; }
        }

        /// <summary>
        /// Gets <c>true</c> if the frame object has border, <c>false</c> otherwise.
        /// </summary>
        internal bool HasBorder
        {
            get { return (mFramesetBorderType != FramesetBorderType.None) && (mFramesetBorderWidth > 0); }
        }

        private string mTitle;
        private string mName;
        private string mFrameDefaultUrl;
        private byte[] mFileData;
        private FrameScrollType mScrollType = FrameScrollType.Auto;
        private FrameLayoutType mLayoutType = FrameLayoutType.Vertical;
        private FrameDividerPositionType mDividerPositionType = FrameDividerPositionType.Relative;
        private int mDividerPositionValue = 1;
        private bool mIsFrameLinkToFile;
        private bool mNoResize;
        private int mMarginX;
        private int mMarginY;
        private readonly FramesetCollection mChildFramesets = new FramesetCollection();
        private DrColor mFramesetBorderColor = DrColor.Empty;
        private int mFramesetBorderWidth; 
        private FramesetBorderType mFramesetBorderType = FramesetBorderType.Simple;
    }
}
