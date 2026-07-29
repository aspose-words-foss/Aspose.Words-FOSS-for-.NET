// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2015 by Alexey Morozov

using Aspose.Words.Validation;

namespace Aspose.Words
{
    /// <summary>
    /// Implements Phonetic Guide. See [ECMA-376] 17.3.3.25 ruby (Phonetic Guide)
    /// </summary>
    internal class Ruby : IComplexAttr
    {
        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return false; }
        }

        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return Clone();
        }

        public override int GetHashCode()
        {
            int hashCode = Alignment.GetHashCode();
            hashCode = (hashCode * 397) ^ TopSize.GetHashCode();
            hashCode = (hashCode * 397) ^ BaseSize.GetHashCode();
            hashCode = (hashCode * 397) ^ Distance.GetHashCode();
            hashCode = (hashCode * 397) ^ Language.GetHashCode();

            foreach (RubyChunk chunk in mTop)
                hashCode = (hashCode * 397) ^ chunk.GetHashCode();

            foreach (RubyChunk chunk in mBase)
                hashCode = (hashCode * 397) ^ chunk.GetHashCode();

            return hashCode;
        }

        internal Ruby Clone()
        {
            Ruby clonedRuby = (Ruby)MemberwiseClone();

            clonedRuby.mTop = Top.Clone();
            clonedRuby.mBase = Base.Clone();

            return clonedRuby;
        }

        /// <summary>
        /// Validates ruby and pushes extra text before <paramref name="rubyRun"/>.
        /// </summary>
        internal void Validate(Run rubyRun)
        {
            string extraRun = CutToMaxLength();

            // Create extra run if needed.
            if (extraRun.Length > 0)
            {
                Run newRun = new Run(rubyRun.Document, extraRun, rubyRun.RunPr.Clone());
                newRun.RunPr.Remove(FontAttr.Ruby);
                rubyRun.InsertPrevious(newRun);
            }

            // AM. Word converts ruby back to equation field upon some conditions.
            // There is very strange behavior. I think that Word convert every ruby to equation field internally but
            // if ruby Top contains equation field Word cannot convert it back. I made tens of tests but still cannot get logic completely.
            // Put simple guard to EQ keyword to get ES customer satisfied.
            if (Top.Text.Contains("EQ"))
            {
                Node[] fieldNodes = RubyConverter.ConvertToFieldNodes(rubyRun);
                foreach (Node fieldNode in fieldNodes)
                    rubyRun.InsertPrevious(fieldNode);

                rubyRun.Remove();
            }
        }

        /// <summary>
        /// Cuts Top and Base to maximum allowed and returns extra text.
        /// </summary>
        /// <remarks>
        /// AM. Very weird Word logic.
        /// Instead of just limiting Top/Base text it pushes extra characters to run before the ruby.
        /// </remarks>
        private string CutToMaxLength()
        {
            if (Base.Text.Length <= MaxLength && Top.Text.Length <= MaxLength)
                return "";

            string newTop = Top.Text;
            string newBase = Base.Text;
            string extraRun = "";
            int count = 0;
            if (Base.Text.Length > MaxLength)
            {
                count = Base.Text.Length - MaxLength;
                // Move extra characters to the end of Top string.
                newTop = newTop + Base.Text.Substring(0, count);
                newBase = Base.Text.Substring(count);
            }

            if (newTop.Length > MaxLength)
            {
                count = newTop.Length - MaxLength;
            }

            extraRun = newTop.Substring(0, count);
            newTop = newTop.Substring(count);

            RubyChunk newTopChunk = new RubyChunk();
            newTopChunk.Text = newTop;
            newTopChunk.RunPr = Top[0].RunPr;

            RubyChunk newBaseChunk = new RubyChunk();
            newBaseChunk.Text = newBase;
            newBaseChunk.RunPr = Base[0].RunPr;

            Top.Clear();
            Base.Clear();

            Top.Add(newTopChunk);
            Base.Add(newBaseChunk);

            return extraRun;
        }

        internal RubyAlignment Alignment;
        internal int TopSize;
        internal int BaseSize;
        internal int Distance;
        internal Language Language;

        internal RubyChunkCollection Top
        {
            get { return mTop; }
        }

        internal RubyChunkCollection Base
        {
            get { return mBase; }
        }

        /// <summary>
        /// Gets the text of this ruby in a form "base(top)".
        /// </summary>
        internal string GetText()
        {
            return string.Format("{0}({1})", mBase.Text, mTop.Text);
        }

        private RubyChunkCollection mTop = new RubyChunkCollection();
        private RubyChunkCollection mBase = new RubyChunkCollection();

        // Seems to be maximal allowed length of ruby parts.
        private const int MaxLength = 30;
    }
}
