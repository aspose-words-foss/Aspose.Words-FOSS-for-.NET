// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2016 by Alexey Morozov

using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Lists;
using Aspose.Words.Tables;

namespace Aspose.Words.Formatting.Intern
{
    /// <summary>
    /// Implements interning for attribute collections <see cref="AttrCollection" />.
    /// </summary>
    /// <remarks>
    /// Attribute interning is a process similar to C# string interning. If equal attribute collections are added to the intern manager only one copy 
    /// of actual attribute values remains in memory. First added collection become <see cref="InternState.Pooled" /> and holds actual values, other collections
    /// are cleared and go to <see cref-="InternState.Interned" /> state.
    /// 
    /// Although idea is clear there are many cases that is needed to be carefully analyzed, for example, complex attributes, cloning, etc. 
    /// That's the reason we decided to try very limited implementation first. Most important thing is that collections can be interned only during saving and 
    /// this eliminates cases when customer can change collection or complex attribute value and we don't need to handle whole document life cycle.
    /// </remarks>
    internal class InternManager : DocumentVisitor
    {
        /// <summary>
        /// Specifies InternManager working mode.
        /// </summary>
        private enum Mode
        {
            /// <summary>
            /// InternaManager interns attributes currently.
            /// </summary>
            Add,

            /// <summary>
            /// InternaManager un-interns attributes currently.
            /// </summary>
            Remove
        }

        #region DocumentVisitor

        public override VisitorAction VisitDocumentStart(Document doc)
        {
            foreach (Style style in doc.Styles)
            {
                doc.InternManager.Process(style.RunPr);

                if(style.Type == StyleType.Paragraph)
                    doc.InternManager.Process(style.ParaPr);

                if(style.Type == StyleType.Table)
                    doc.InternManager.Process(((TableStyle)style).TablePr);
            }

            foreach (ListDef listDef in doc.Lists.ListDefs)
                foreach (ListLevel level in listDef.Levels)
                {
                    doc.InternManager.Process(level.RunPr);
                    doc.InternManager.Process(level.ParaPr);
                }
            
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            para.Document.InternManager.Process(para.ParaPr);
            para.Document.InternManager.Process(para.ParagraphBreakRunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRowStart(Row row)
        {
            row.Document.InternManager.Process(row.TablePr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellStart(Cell cell)
        {
            cell.Document.InternManager.Process(cell.CellPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRun(Run run)
        {
            run.Document.InternManager.Process(run.RunPr);
            return VisitorAction.Continue;
        }

        #endregion DocumentVisitor

        /// <summary>
        /// Interns <see cref="InternManager" /> whole document.
        /// </summary>
        internal static void Add(DocumentBase doc)
        {
            if (doc.InternManager == null)
                doc.InternManager = new InternManager();

            doc.InternManager.mMode = Mode.Add;
            doc.Accept(doc.InternManager);
        }

        /// <summary>
        /// Un-interns whole document.
        /// </summary>
        internal static void Remove(DocumentBase doc)
        {
            if (doc.InternManager == null)
                return;

            doc.InternManager.mMode = Mode.Remove;
            doc.Accept(doc.InternManager);

            doc.InternManager = null;
        }

        /// <summary>
        /// Interns <see cref="InternManager" /> attribute collection if possible.
        /// </summary>
        internal void Add(AttrCollection pr)
        {
            CalledCount++;

            // Do not intern empty collections.
            if (pr.Count == 0)
            {
                EmptyCount++;
                return;
            }

            // Do not intern already interned collection.
            if (pr.InternState != InternState.None)
            {
                RepeatedCount++;
                return;
            }

            bool hasComplexAttrs = false;
            for (int i = 0; i < pr.Count; i++)
            {
                object srcValue = pr.GetByIndex(i);

                if (srcValue is IComplexAttr)
                {
                    hasComplexAttrs = true;

                    // Don't know how to handle inherited values.
                    if ((!(srcValue is InternableComplexAttr)) || ((IComplexAttr)srcValue).IsInheritedComplexAttr)
                    {
                        ComplexCount++;
                        return;
                    }
                }
            }

            if(hasComplexAttrs)
                ComplexCount++;

            InternPoolItem poolItem = mPool.GetValueOrNull(pr);

            // Attach all complex attrs to collection.
            for (int i = 0; i < pr.Count; i++)
            {
                object srcValue = pr.GetByIndex(i);
                InternableComplexAttr internable = srcValue as InternableComplexAttr;
                
                if (internable != null)
                    internable.Attach(pr);
            }

            if (poolItem == null)
            {
                poolItem = new InternPoolItem(this);
                poolItem.Pr = pr;
                poolItem.Id = Id++;
                mPool.Add(pr, poolItem);

                PooledCount++;

                Debug.WriteLineIf(mPool.Count % 1000 == 0, string.Format("Pool size: {0}", mPool.Count));
            }
            else
            {
                poolItem.RefCount++;
                pr.Clear();

                InternedCount++;
            }

            pr.PoolItem = poolItem;

        }

        /// <summary>
        /// Un-intern attribute collection.
        /// </summary>
        internal void Remove(AttrCollection pr)
        {
            switch (pr.InternState)
            {
                case InternState.Interned:
                    UnIntern(pr);
                    break;
                case InternState.Pooled:
                    UnPool(pr);
                    break;
                case InternState.None:
                    // Do nothing
                    break;
                default:
                    Debug.Assert(false, "Unexpected value.");
                    break;
            }
        }

        private void Process(AttrCollection pr)
        {
            if (mMode == Mode.Add)
            {
                Add(pr);
            }
            else
            {
                Remove(pr);
            }
        }

        [JavaConvertCheckedExceptions]
        private void UnIntern(AttrCollection pr)
        {
            UnInternedCount++;

            InternPoolItem poolItem = pr.PoolItem;
            poolItem.RefCount--;

            pr.PoolItem = null;

            for (int i = 0; i < poolItem.Pr.Count; i++)
            {
                int key = poolItem.Pr.GetKey(i);
                object srcValue = poolItem.Pr.GetByIndex(i);

                if (srcValue is InternableComplexAttr)
                {
                    Debug.Assert(srcValue is IComplexAttr);

                    IComplexAttr complexAttr = (IComplexAttr)srcValue;
                    if (!complexAttr.IsInheritedComplexAttr)
                    {
                        object dstValue = complexAttr.DeepCloneComplexAttr();
                        ((InternableComplexAttr)dstValue).Detach();
                        pr[key] = dstValue;
                    }
                }
                else
                {
                    // We should not have IComplexAttr which are not inherited from InternableComplexAttr.
                    Debug.Assert(!(srcValue is IComplexAttr));

                    pr[key] = srcValue;
                }
            }
        }

        [JavaConvertCheckedExceptions]
        private void UnPool(AttrCollection pr)
        {
            UnPooledCount++;
            Debug.Assert(pr.InternState == InternState.Pooled);

            InternPoolItem poolItem = mPool.GetValueOrNull(pr);
            pr.PoolItem = null;
            mPool.Remove(pr);

            if (poolItem.RefCount == 0)
            {
                // This is only instance and we can easily unpool this item.
            }
            else
            {
                // We need to preserve attributes in pool.
                AttrCollection newPr = pr.Clone();

                poolItem.Pr = newPr;
                mPool.Add(newPr, poolItem);
                newPr.PoolItem = poolItem;
            }
        }

        internal IDictionary<AttrCollection, InternPoolItem> Pool
        {
            get { return mPool; }
        }

        /// <summary>
        /// Counts how many collections tried to be interned.
        /// </summary>
        internal int CalledCount;

        /// <summary>
        /// Counts how many collections were interned.
        /// </summary>
        internal int InternedCount;

        /// <summary>
        /// Counts how many collections were pooled.
        /// </summary>
        internal int PooledCount;

        /// <summary>
        /// Shows how many collections with complex attributes tried to be interned.
        /// </summary>
        internal int ComplexCount;
        
        /// <summary>
        /// Counts how many empty collections tried to be interned.
        /// </summary>
        internal int EmptyCount;
        
        /// <summary>
        /// Counts how many already interned collections tried to be interned.
        /// </summary>
        internal int RepeatedCount;

        /// <summary>
        /// Counts how many collections where un-pooled.
        /// </summary>
        internal int UnPooledCount;
        
        /// <summary>
        /// Counts how many collections where un-interned.
        /// </summary>
        internal int UnInternedCount;

        internal int Id = 1;

        private Mode mMode;

        private readonly Dictionary<AttrCollection, InternPoolItem> mPool = 
            new Dictionary<AttrCollection, InternPoolItem>();
    }
}
