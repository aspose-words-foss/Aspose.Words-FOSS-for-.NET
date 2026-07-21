// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/09/2016 by Andrey Noskov

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Represents an extension that specifies the properties for displaying an online video inside document.
    /// </summary>
    /// <remarks>
    /// [MS-ODRAWXML] 2.20.1.1 webVideoPr.
    /// </remarks>
    internal class DmlWebVideoProperties
    {
        internal DmlWebVideoProperties()
        {
        }

        internal DmlWebVideoProperties(string embedHtml, double frameWidth, double frameHeight)
        {
            mEmbedHtml = embedHtml;
            mFrameWidth = frameWidth;
            mFrameHeight = frameHeight;
        }

        internal DmlWebVideoProperties Clone()
        {
            return (DmlWebVideoProperties)MemberwiseClone();
        }

        /// <summary>
        /// Specifies the embedded HTML to be rendered within the BLIP. 
        /// This attribute SHOULD NOT be omitted if the video playback experience is to be fully preserved.
        /// </summary>
        internal string EmbedHtml
        {
            get { return mEmbedHtml; }
            set { mEmbedHtml = value; }
        }

        /// <summary>
        /// Specifies the width of the rendered html page in pixels. 
        /// This attribute SHOULD NOT be omitted if the video playback experience is to be fully preserved.
        /// </summary>
        internal double FrameWidth
        {
            get { return mFrameWidth; }
            set { mFrameWidth = value; }
        }

        /// <summary>
        /// Specifies the height of the rendered html page in pixels. 
        /// This attribute SHOULD NOT be omitted if the video playback experience is to be fully preserved.
        /// </summary>
        internal double FrameHeight
        {
            get { return mFrameHeight; }
            set { mFrameHeight = value; }
        }

        private string mEmbedHtml;
        private double mFrameWidth;
        private double mFrameHeight;
    }
}