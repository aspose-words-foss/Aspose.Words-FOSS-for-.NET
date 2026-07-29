// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Collections;

namespace Aspose.Words.Drawing.Core.Dml.Common
{
    internal class DmlHierarchicalPropertyBag : IDmlHierarchicalPropertyBag
    {
        public object GetProperty(int key)
        {
            // Properties specified in extension overrides properties specified on the base collection.
            object value = null;
            if (mExtensionProperties != null)
                value = mExtensionProperties.GetDirectProperty(key);

            if (value == null)
            {
                value = mProperties[key];
                if (value == null && ParentBagProvider != null)
                {
                    IDmlHierarchicalPropertyBag parentBag = ParentBagProvider.ParentBag;
                    if (parentBag != null)
                        return parentBag.GetProperty(key);
                }
            }

            return value;
        }

        public void Remove(int key)
        {
            mProperties.Remove(key);
        }

        /// <summary>
        /// Removes all items from this property collection.
        /// </summary>
        public void RemoveAll()
        {
            mProperties.Clear();
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        public object GetDirectProperty(int key)
        {
            return mProperties[key];
        }
        
        /// <summary>
        /// Determines whether the mProperties contains the specified property.
        /// </summary>
        /// <param name="key">The property key</param>
        /// <returns>"True", if the property is set directly, "false" otherwise</returns>
        public bool IsPropertySpecified(int key)
        {
            return mProperties.ContainsKey(key);
        }

        /// <summary>
        /// Sets the specified property. If the property value is null, removes the property.
        /// </summary>
        /// <param name="key">The property key</param>
        /// <param name="value">The property value</param>
        public void SetProperty(int key, object value)
        {
            if (value != null)
            {
                mProperties[key] = value;
            }
            else
            {
                if (mProperties.ContainsKey(key))
                    Remove(key);
            }
        }

        public IDmlHierarchicalPropertyBag Clone()
        {
            DmlHierarchicalPropertyBag clone = (DmlHierarchicalPropertyBag)MemberwiseClone();
            clone.mProperties = new IntToObjDictionary<object>(mProperties);
            if (mParentBagProvider != null)
                clone.mParentBagProvider = mParentBagProvider.Clone();
            return clone;
        }

        /// <summary>
        /// Returns a flag indicating whether any property of this bag has value different than in the parent bag.
        /// </summary>
        /// <param name="attributesToIgnore">Properties/attributes that should be excluded from the checking.</param>
        /// <param name="parent">If defined, this parent is used in comparison instead a bag of
        /// <see cref="ParentBagProvider"/>.</param>
        /// <returns>A flag indicating whether any property has non-default value.</returns>
        public bool HasNonDefaultFormatting(int[] attributesToIgnore, IDmlHierarchicalPropertyBag parent)
        {
            if ((Count == 0) &&
                ((mExtensionProperties == null) || (mExtensionProperties.Count == 0)))
                return false;

            if ((parent == null) &&
                ((ParentBagProvider == null) || (ParentBagProvider.ParentBag == null)))
                return true;

            IDmlHierarchicalPropertyBag parentBag = (parent != null) ? parent : ParentBagProvider.ParentBag;

            IntToObjDictionary<object>.Enumerator enumerator = mProperties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (ContainsAttribute(attributesToIgnore, enumerator.CurrentKey))
                    continue;

                object value = GetProperty(enumerator.CurrentKey);
                if (value == null)
                    continue;

                object parentValue = parentBag.GetProperty(enumerator.CurrentKey);
                if (!value.Equals(parentValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified array contains the specified attribute.
        /// </summary>
        private static bool ContainsAttribute(int[] attributes, int attribute)
        {
            if (attributes == null)
                return false;

            foreach (int attr in attributes)
            {
                if (attr == attribute)
                    return true;
            }

            return false;
        }

        public IDmlHierarchicalPropertyBagParentProvider ParentBagProvider
        {
            get { return mParentBagProvider; }
            set { mParentBagProvider = value; }
        }

        public IDmlHierarchicalPropertyBag ExtensionProperties
        {
            get { return mExtensionProperties; }
            set { mExtensionProperties = value; }
        }

        public int Count
        {
            get { return mProperties.Count; }
        }

        private IDmlHierarchicalPropertyBagParentProvider mParentBagProvider;
        private IDmlHierarchicalPropertyBag mExtensionProperties;

        private IntToObjDictionary<object> mProperties = new IntToObjDictionary<object>();
    }
}
