// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2019 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Represents a collection of <see cref="VbaModule"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-vba-macros/">Working with VBA Macros</a> documentation article.</para>
    /// </summary>
    public sealed class VbaModuleCollection : IEnumerable<VbaModule>
    {
        internal VbaModuleCollection(VbaProject project)
        {
            mProject = project;
        }

        /// <summary>
        /// Adds a module to the collection.
        /// </summary>
        public void Add(VbaModule vbaModule)
        {
            AddInternal(vbaModule);
            mProject.MarkModified();
        }

        /// <summary>
        /// Retrieves a <see cref="VbaModule"/> object by index.
        /// </summary>
        /// <param name="index">Zero-based index of the module to retrieve.</param>
        public VbaModule this[int index]
        {
            get { return mModules[index]; }
        }

        /// <summary>
        /// Retrieves a <see cref="VbaModule"/> object by name, or Null if not found.
        /// </summary>        
        public VbaModule this[string name]
        {
            get { return mModulesByName.GetValueOrNull(name); }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Retrieves a <see cref="VbaModule"/> object by name, or Null if not found.
        /// </summary>
        public VbaModule GetByName(string name)
        {
            return this[name];
        }
#endif

        /// <summary>
        /// Returns the number of VBA modules in the collection.
        /// </summary>
        public int Count
        {
            get { return mModules.Count; }
        }

        /// <summary>
        /// Removes the specified module from the collection.
        /// </summary>
        /// <param name="module">The module to remove.</param>
        public void Remove(VbaModule module)
        {
            if (module == null)
                throw new ArgumentNullException("module");

            if (mModules.IndexOf(module) >= 0)
            {
                mModulesByName.Remove(module.Name);
                mModules.Remove(module);

                mProject.MarkModified();
            }
            else
                throw new ArgumentOutOfRangeException("module");
        }

        /// <summary>
        /// Removes all modules from the collection.
        /// </summary>
        internal void Clear()
        {
            mModulesByName.Clear();
            mModules.Clear();
        }

        /// <summary>
        /// Makes a copy of the object.
        /// </summary>
        internal VbaModuleCollection Clone(VbaProject project)
        {
            VbaModuleCollection lhs = new VbaModuleCollection(project);

            foreach (VbaModule module in mModules)
                lhs.AddInternal(module.Clone());

            return lhs;
        }

        /// <summary>
        /// Adds a module to collection without clearing storage. 
        /// </summary>        
        internal void AddInternal(VbaModule vbaModule)
        {
            if (!StringUtil.HasChars(vbaModule.Name))
                throw new ArgumentException("A module name must be specified before adding to the VBA Project.");

            if (mModulesByName.ContainsKey(vbaModule.Name))
                throw new ArgumentException("Cannot add a VbaModule because a module with the same name already exists.");

            vbaModule.Project = mProject;
            mModules.Add(vbaModule);
            mModulesByName.Add(vbaModule.Name, vbaModule);
        }

        IEnumerator<VbaModule> IEnumerable<VbaModule>.GetEnumerator()
        {
            return mModules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mModules.GetEnumerator();
        }

        private readonly List<VbaModule> mModules = new List<VbaModule>();

        /// <summary>
        /// A map of modules sorted by name. 
        /// </summary>
        private readonly SortedStringListGeneric<VbaModule> mModulesByName = new SortedStringListGeneric<VbaModule>();

        /// <summary>
        /// Parent VBA project.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly VbaProject mProject;
    }
}
