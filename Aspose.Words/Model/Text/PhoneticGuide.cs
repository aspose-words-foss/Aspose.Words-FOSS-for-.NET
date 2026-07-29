// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/04/2023 by Ilya Navrotskiy

namespace Aspose.Words
{
    /// <summary>
    /// Represents Phonetic Guide.
    /// </summary>
    public class PhoneticGuide
    {
        /// <summary>
        /// Creates a new instance of <see cref="PhoneticGuide"/> object.
        /// </summary>
        internal PhoneticGuide(Run parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Gets base text of the phonetic guide.
        /// </summary>
        public string BaseText
        {
            get
            {
                Ruby ruby = Ruby;
                if (ruby == null)
                    return string.Empty;

                return ruby.Base.Text;
            }

        }

        /// <summary>
        /// Gets ruby text of the phonetic guide.
        /// </summary>
        public string RubyText
        {
            get
            {
                Ruby ruby = Ruby;
                if (ruby == null)
                    return string.Empty;

                return ruby.Top.Text;
            }
        }

        /// <summary>
        /// Gets Ruby object of the parent Run.
        /// </summary>
        private Ruby Ruby
        {
            get { return mParent.RunPr[FontAttr.Ruby] as Ruby; }
        }

        private readonly Run mParent;
    }
}
