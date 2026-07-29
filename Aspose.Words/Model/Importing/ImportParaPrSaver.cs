// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2017 by Dmitry Sokolov

using Aspose.Words.Settings;

namespace Aspose.Words
{
    /// <summary>
    /// Implements methods to resolve attributes of the paragraph properties
    /// while paragraph with list is importing.
    /// </summary>
    internal class ImportParaPrSaver : IExpandedAttrSaver
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        internal ImportParaPrSaver(ParaPr srcPr, ParaPr dstPr)
        {
            mSrcPr = srcPr;
            mDstPr = dstPr;
        }

        /// <summary>
        /// Saves attribute with specified key to destination collection.
        /// </summary>
        public void Save(AttrCollection dstAttrs, int key, object value)
        {
            if (dstAttrs.ContainsKey(key))
                return;

            // Mimic MSW and use left indent equal to default tab stop when source
            // properties does not contain appropriate attribute and value 
            // cannot be inherited from destination definition properties. 
            // It is operation with boxing, however it does not happened often.
            if ((key == ParaAttr.LeftIndent) && !mSrcPr.ContainsKey(key))
            {
                if ((int)mDstPr.FetchAttr(ParaAttr.LeftIndent) == DocPr.DefaultTabStopDefault)
                {
                    dstAttrs.Remove(key);
                    return;
                }

                value = DocPr.DefaultTabStopDefault;
            }

            dstAttrs[key] = value;
        }

        /// <summary>
        /// Properties of the source paragraph while importing.
        /// </summary>
        private readonly ParaPr mSrcPr;

        /// <summary>
        /// Properties of the destination paragraph while importing.
        /// </summary>
        private readonly ParaPr mDstPr;
    }
}
