// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2009 by Alexey Noskov

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.JavaAttributes;
using Aspose.Xml;

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Base class for FlatOpcPackage and OpcPackage
    /// </summary>
    public abstract class OpcPackageBase
    {
        protected OpcPackageBase()
        {
            mParts = new OpcPackagePartCollection();
        }

        /// <summary>
        /// Creates the opc package parts using a specific comparator.
        /// </summary>
        protected OpcPackageBase(IComparer<string> comparer)
        {
            mParts = new OpcPackagePartCollection(comparer);
        }

        /// <summary>
        /// Load relationship data from package.
        /// </summary>
        internal void LoadRelationships()
        {           
            // WORDSNET-13471 Resilience fix to avoid exception upon loading when original part in package
            // is missing while relationship part exists.            
            foreach (OpcPackagePart part in mParts)
            {
                string oPartName = Path.GetFileName(part.Name);

                // Relationship parts will have names like "/word/_rels/document.xml.rels" or "/_rels/.rels"
                if (part.Extension == RelExt)
                {
                    // Remove "/_rels" and ".rels" and we end up with the name of the source part.
                    string sourcePartName = part.Name.Replace(gRelPathL, "").Replace(gRelExtF, "");

                    if (sourcePartName == RootPartName)
                        ReadRelationships(part, mRels);
                }
                // Some created parts have only folder path in the name and do not contain data. 
                // As example check test "TestDmlTextBox.TestDmlListsBulletFont".
                else if (StringUtil.HasChars(oPartName))
                {                   
                    // Append "/_rels" and ".rels" and we end up with the name of the relationship part.
                    string rPartName = part.Name.Replace(oPartName, gRelPathR + oPartName + gRelExtF); 

                    if (mParts.Contains(rPartName))
                    {
                        OpcPackagePart rPart = FetchPartByName(rPartName);
                        ReadRelationships(rPart, part.Rels);
                    }
                }
            }            
        }

        /// <summary>
        /// Reads relationship from the specified part into the specified relationship collection.
        /// </summary>
        /// <param name="part">PackagePart to read relationships from.</param>
        /// <param name="rels">RelationshipCollection to read relationships to.</param>
        private static void ReadRelationships(OpcPackagePart part, OpcRelationshipCollection rels)
        {
            AnyXmlReader partReader = new AnyXmlReader(part.Stream);
            while (partReader.ReadChild("Relationships"))
            {
                switch (partReader.LocalName)
                {
                    case "Relationship":
                        {
                            // Example:
                            //   <Relationship Id="rId1" Type="http://.../attachedTemplate" Target="file:///C:\...\template1.dot" TargetMode="External"/>
                            string id = null;
                            string type = null;
                            string target = null;
                            bool isExternal = false;

                            while (partReader.MoveToNextAttribute())
                            {
                                switch (partReader.LocalName)
                                {
                                    case "Id":
                                        id = partReader.Value;
                                        break;
                                    case "Type":
                                        type = partReader.Value;
                                        break;
                                    case "Target":
                                        target = partReader.Value;
                                        break;
                                    case "TargetMode":
                                        isExternal = (partReader.Value == "External");
                                        break;
                                    default:
                                        // Ignore.
                                        break;
                                }
                            }
                            // WORDSNET-9113 Ignore relationships with an empty target.
                            if (StringUtil.HasChars(target))
                                rels.Add(id, type, target, isExternal);
                            break;
                        }
                    default:
                        {
                            partReader.IgnoreElement();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// This includes ALL items in the package including Content_Types.xml and rels.
        /// </summary>
        public OpcPackagePartCollection Parts
        {
            get { return mParts; }
        }

        /// <summary>
        /// Relationships for this package.
        /// </summary>
        public OpcRelationshipCollection Rels
        {
            get { return mRels; }
        }

        /// <summary>
        /// Gets a part by name or returns null if the part does not exist.
        /// </summary>
        /// <param name="name">Must be the absolute part name.</param>
        public OpcPackagePart GetPartByName(string name)
        {
            return mParts[name];
        }

        /// <summary>
        /// Gets a part by name or throw an exception if the part does not exist.
        /// </summary>
        /// <param name="name">Must be the absolute part name.</param>
        public OpcPackagePart FetchPartByName(string name)
        {
            OpcPackagePart result = GetPartByName(name);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find part '{0}'.", name));
            else
                return result;
        }

        /// <summary>
        /// RK This is the proper way of getting singleton parts. By relationship type.
        /// </summary>
        /// <param name="parentPart">The parent part of the part we are getting. Null if the parent is the package.</param>
        /// <param name="relType">The relationship type from the parent to the child part.</param>
        /// <returns>The child part or null.</returns>
        public OpcPackagePart GetPartByRelationshipType(OpcPackagePart parentPart, string relType)
        {
            OpcRelationshipCollection rels;
            string rootName;
            if (parentPart == null)
            {
                rels = mRels;
                rootName = RootPartName;
            }
            else
            {
                rels = parentPart.Rels;
                rootName = parentPart.Name;
            }

            OpcRelationship rel = rels.GetFirstByType(relType);
            if (rel != null)
                return GetPartByName(MakeAbsolute(rootName, rel.Target));
            else
                return null;
        }

        public OpcPackagePart FetchPartByRelationshipType(OpcPackagePart parentPart, string relType)
        {
            OpcPackagePart result = GetPartByRelationshipType(parentPart, relType);
            if (result == null)
                throw new InvalidOperationException(string.Format("Cannot find target of relationship '{0}'", relType));
            else
                return result;
        }

        /// <summary>
        /// Check if there is part that matches the specified relationship type.
        /// </summary>
        public bool Exists(string relType)
        {
            return (mRels.GetFirstByType(relType) != null);
        }

        /// <summary>
        /// Creates a package part that is a child of the specified part.
        /// Adds the part to the collection adds a relationship from the parent part to the created part.
        /// </summary>
        public OpcPackagePart CreateChildPart(
            OpcPackagePart parentPart,
            string childPartName,
            string contentType,
            string relType)
        {
            string dummyRelId;
            OpcPackagePart part = CreateChildPart(parentPart, childPartName, contentType, relType, out dummyRelId);
            // RK Split assignment and return into two lines to allow autoporting to succeed.
            return part;
        }

        /// <summary>
        /// Same as above, but also returns the id of the created relationship.
        /// </summary>
        public OpcPackagePart CreateChildPart(
            OpcPackagePart parentPart,
            string childPartName,
            string contentType,
            string relType,
            out string relId)
        {
            if (parentPart != null)
                childPartName = MakeAbsolute(parentPart.Name, childPartName);

            OpcPackagePart childPart = new OpcPackagePart(childPartName, contentType);
            mParts.Add(childPart);

            OpcRelationshipCollection rels = (parentPart != null) ? parentPart.Rels : mRels;
            relId = rels.Add(relType, childPart.Name, false);

            return childPart;
        }

        /// <summary>
        /// When overridden saves package to stream. 
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        public virtual void Save(Stream stream)
        {
            // No default implementation.
        }

        /// <summary>
        /// Makes the specified target relative to the part name.
        /// Adds the "../" segments as appropriate.
        /// If target is already relative, return as is.
        /// </summary>
        public static string MakeRelative(string parentPartName, string targetName)
        {
            if (!targetName.StartsWith("/", StringComparison.Ordinal))
                return targetName;

            // Find the position where the difference starts.
            int lastSlashIdx = 0;
            int maxIdx = System.Math.Min(parentPartName.Length, targetName.Length);
            for (int i = 0; i < maxIdx; i++)
            {
                if (parentPartName[i] == '/')
                    lastSlashIdx = i;
                if (parentPartName[i] != targetName[i])
                    break;
            }

            // Add enough ../ to cover for the remaning segments in the parent part name.
            int diffStartIdx = lastSlashIdx + 1;
            StringBuilder result = new StringBuilder();
            for (int i = diffStartIdx; i < parentPartName.Length; i++)
            {
                if (parentPartName[i] == '/')
                    result.Append(StepUpSegment);
            }

            // Add the remainder of the target name.
            result.Append(targetName, diffStartIdx, targetName.Length - diffStartIdx);

            return result.ToString();
        }

        /// <summary>
        /// Builds a complete target name out of a relative target name.
        /// Resolves the "../" segments in the part name.
        /// If target name is absolute, returns it as is.
        /// If target name is "NULL", returns empty string.
        /// </summary>
        public static string MakeAbsolute(string parentPartName, string targetName)
        {
            if (targetName.StartsWith("/", StringComparison.Ordinal))
                return targetName;

            // WORDSNET-8253 In some cases rels Target can be equals "NULL" or "null"
            // MS Word does not throws when encounters such target as image uri, so we should not throw as well
            // such resource is not reachable anyway, so return an empty string
            if (StringUtil.EqualsIgnoreCase(targetName, "NULL"))
                return "";

            string result = RemoveLastSegment(parentPartName);

            int pos = 0;
            while (true)
            {
                int newPos = targetName.IndexOf(StepUpSegment, pos, StringComparison.Ordinal);
                if (newPos >= pos)
                {
                    // This code is for an unlikely case when the target name contains /../ in the middle, 
                    // for example "aaa/../bbb". It will copy "aaa/" to the output.
                    if (newPos > pos)
                        result += targetName.Substring(pos, newPos - pos);

                    // Execute the step up.
                    result = RemoveLastSegment(result);
                    pos = newPos + StepUpSegment.Length;
                }
                else
                {
                    // Flush the remainder of the target name to the result.
                    result += targetName.Substring(pos, targetName.Length - pos);
                    return result;
                }
            }
        }

        /// <summary>
        /// Removes the last segment from a path. Segments must be separated by "/" slashes.
        /// If a slash is at the end of the string, it is not counted as a segment (e.g. ignored).
        /// </summary>
        private static string RemoveLastSegment(string s)
        {
            if (!StringUtil.HasChars(s))
                return s;

            if (s == RootPartName)
                return s;

            int startIdx = s.Length - 1;
            for (int i = startIdx; i >= 0; i--)
            {
                if ((s[i] == '/') && (i < startIdx))
                    return s.Substring(0, i + 1);
            }

            return "";
        }

        private readonly OpcRelationshipCollection mRels = new OpcRelationshipCollection(RootPartName);
        private readonly OpcPackagePartCollection mParts;

        private const string RelExt = "rels";
        private const string RootPartName = "/";
        private const string StepUpSegment = "../";
        private static readonly string gRelExtF = string.Format(".{0}", RelExt);
        private static readonly string gRelPathL = string.Format("{0}_{1}", RootPartName, RelExt);
        private static readonly string gRelPathR = string.Format("_{0}{1}", RelExt, RootPartName); 
    }
}
