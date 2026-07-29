// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2007 by Vladimir Averkin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.OpcPackaging
{
    public sealed class OpcRelationshipCollection : IEnumerable<OpcRelationship>
    {
        public OpcRelationshipCollection(string partName)
        {
            mPartName = partName;
            CreateTables();
        }

        /// <summary>
        /// Adds a relationship. Returns the relationship id.
        /// If a relationship to the specified target already exists, returns the existing relationship id.
        /// </summary>
        public string Add(string relType, string absoluteTarget, bool isExternal)
        {
            if (isExternal)
            {
                // RK It looks like MS Word writes full URL with protocol for file paths.
                if (UriUtil.IsFilePath(absoluteTarget))
                    absoluteTarget = UriUtil.EscapeHrefAnyway(UriUtil.AddFileSchemePrefix(absoluteTarget));
                else if (UriUtil.HasFileScheme(absoluteTarget))
                    absoluteTarget = UriUtil.EscapeHrefAnyway(UriUtil.UnescapeHref(absoluteTarget));
            }
            else
                absoluteTarget = OpcPackageBase.MakeRelative(mPartName, absoluteTarget);

            OpcRelationship rel = (OpcRelationship)mTargetToRelTable[GetTypedTarget(absoluteTarget, relType)];

            // WORDSNET-27535 Write separate relationship for external file links.
            // WORDSNET-27670 Write all external links as separate relationships.
            if ((rel != null) && !isExternal)
            {
                // RK We are returning the existing relationship id and it is a good thing for OOXML files.
                // For example, the main document can refer to the same image multiple times,
                // it is just one relationship.
                return rel.Id;
            }

            string relId = string.Format("rId{0}", (mTargetToRelTable.Count + 1));

            Add(relId, relType, absoluteTarget, isExternal);
            return relId;
        }

        /// <summary>
        /// This is used to add relationships when loading a package.
        /// For resiliency purposes, if a duplicate id is encountered, it overrides the existing value.
        /// </summary>
        public void Add(string relId, string relType, string absoluteTarget, bool isExternal)
        {
            // WORDSNET-15049 Replace back slash in the target path with slash ("/") for internal package parts.
            if (!isExternal)
                absoluteTarget = absoluteTarget.Replace("\\", "/");

            OpcRelationship rel = new OpcRelationship(relId, relType, absoluteTarget, isExternal);
            Add(rel);
        }

        /// <summary>
        /// Removes relationship from the collection.
        /// </summary>
        public void Remove(string relId)
        {
            OpcRelationship rel = GetById(relId);

            mIdToRelTable.Remove(rel.Id);
            mTargetToRelTable.Remove(rel.Target);
        }

        /// <summary>
        /// Gets relationship by its id. Returns null if not found.
        /// </summary>
        public OpcRelationship GetById(string id)
        {
            return mIdToRelTable.GetValueOrNull(id);
        }

        /// <summary>
        /// Gets a first relationship that matches the specified relationship type.
        /// </summary>
        public OpcRelationship GetFirstByType(string type)
        {
            foreach (KeyValuePair<string, OpcRelationship> pair in mIdToRelTable)
            {
                if (pair.Value.Type == type)
                    return pair.Value;
            }
            return null;
        }

        /// <summary>
        /// Clones this instance of relationship collection.
        /// </summary>
        internal OpcRelationshipCollection Clone()
        {
            OpcRelationshipCollection lhs = (OpcRelationshipCollection)MemberwiseClone();

            lhs.CreateTables();
            foreach (OpcRelationship rel in mIdToRelTable.Values)
                lhs.Add(rel.Clone());

            return lhs;
        }

        public IEnumerator<OpcRelationship> GetEnumerator()
        {
            return mIdToRelTable.Values.GetEnumerator();
        }

        /// <summary>
        /// Creates internal tables to store relationship items.
        /// </summary>
        private void CreateTables()
        {
            mIdToRelTable = new SortedStringListGeneric<OpcRelationship>(true);
            mTargetToRelTable = new StringListGeneric<OpcRelationship>();
        }

        /// <summary>
        /// Adds relationship object into this collection.
        /// </summary>
        private void Add(OpcRelationship rel)
        {
            mIdToRelTable[rel.Id] = rel;
            mTargetToRelTable.Add(GetTypedTarget(rel.Target, rel.Type), rel);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return mIdToRelTable.Count; }
        }

        /// <summary>
        /// Gets the name of the part to which these relationships belong to.
        /// </summary>
        public string PartName
        {
            get { return mPartName; }
        }

        private static string GetTypedTarget(string target, string type)
        {
            return string.Format("{0}{1}", target, type);
        }

        private readonly string mPartName;
        private SortedStringListGeneric<OpcRelationship> mIdToRelTable;
        private StringListGeneric<OpcRelationship> mTargetToRelTable;
    }
}
