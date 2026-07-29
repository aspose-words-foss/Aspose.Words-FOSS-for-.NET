// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// Represents collection of shape effects.
    /// </summary>
    internal class DmlShapeEffectsCollection : IEnumerable<DmlShapeEffect>
    {
        internal DmlShapeEffectsCollection() : this(false, false)
        {
        }

        internal DmlShapeEffectsCollection(bool isTheme, bool isEffectDag)
        {
            mIsTheme = isTheme;
            mIsEffectDag = isEffectDag;
        }

        internal DmlShapeEffectsCollection Clone()
        {
            DmlShapeEffectsCollection lhs = (DmlShapeEffectsCollection)MemberwiseClone();

            lhs.mEffects = new IntToObjDictionary<DmlShapeEffect>();
            foreach (DmlShapeEffect srcEffect in mEffects.Values)
                lhs.AddEffect(srcEffect.Clone());

            return lhs;
        }

        /// <summary>
        /// Add effect to the collection.
        /// </summary>
        internal void AddEffect(DmlShapeEffect effect)
        {
            if (effect != null)
                mEffects[(int)effect.EffectType] = effect;
        }

        /// <summary>
        /// Removes effect from the collection.
        /// </summary>
        internal void RemoveEffect(DmlShapeEffectType type)
        {
            mEffects.Remove((int)type);
        }

        public IEnumerator<DmlShapeEffect> GetEnumerator()
        {
            return new EffectsEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns effect of the specified type from the collection.
        /// </summary>
        internal DmlShapeEffect this[DmlShapeEffectType effectType]
        {
            get { return mEffects[(int)effectType]; }
        }

        internal int Count
        {
            get { return mEffects.Count; }
        }

        /// <summary>
        /// Flag indicates that collection represents effects applied through document theme.
        /// </summary>
        internal bool IsTheme
        {
            get { return mIsTheme; }
            set { mIsTheme = value; }
        }

        /// <summary>
        /// Flag indicates that effects are applied in the order specified by the container type <see cref="DagType"/>.
        /// </summary>
        internal bool IsEffectDag
        {
            get { return mIsEffectDag; }
        }
        /// <summary>
        /// Specifies an optional name for this DAG list of effects, so that it can be referred to later.
        /// Must be unique across all effect trees and effect containers.
        /// </summary>
        internal string DagName
        {
            get { return mDagName; }
            set { mDagName = value; }
        }

        /// <summary>
        /// Specifies the type of DAG list of effects, either sibling or tree.
        /// Default is <see cref="EffectDagType.Sibling"/>
        /// </summary>
        internal EffectDagType DagType
        {
            get { return mDagType; }
            set { mDagType = value; }
        }

        private IntToObjDictionary<DmlShapeEffect> mEffects = new IntToObjDictionary<DmlShapeEffect>();
        private bool mIsTheme;
        private readonly bool mIsEffectDag;
        private string mDagName;
        private EffectDagType mDagType;

        /// <summary>
        /// Class wrapper for Enumerator.
        /// We cannot use mEffects.GetEnumerator() directly because it returns instance of <see cref="IntToObjDictionary{T}.Enumerator"/>,
        /// but we need <see cref="IEnumerator{T}"/>.
        /// </summary>
        private sealed class EffectsEnumerator : IEnumerator<DmlShapeEffect>
        {
            public EffectsEnumerator(DmlShapeEffectsCollection collection)
            {
                mEnumerator = collection.mEffects.GetEnumerator();
            }

            public bool MoveNext()
            {
                return mEnumerator.MoveNext();
            }

            public void Reset()
            {
                mEnumerator.Reset();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public DmlShapeEffect Current
            {
                get { return mEnumerator.CurrentValue; }
            }

            private readonly IntToObjDictionary<DmlShapeEffect>.Enumerator mEnumerator;
        }
    }
}
