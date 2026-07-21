// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2023 by Vadim Saltykov

using System;
using System.Collections.Generic;
using Aspose.Crypto;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing link definitions into markdown.
    /// </summary>
    internal class MarkdownLinkDefinitionWriter
    {
        internal MarkdownLinkDefinitionWriter(MarkdownWriter writer)
        {
            mWriter = writer;
        }

        /// <summary>
        /// Processes a specified ShapeBase.
        /// </summary>
        internal void OnShape(ShapeBase shapeBase, string shapeDestination)
        {
            if (!ReferenceNeeded(shapeBase))
                return;

            string reference = GetReferenceString(shapeBase, shapeDestination);

            string title = shapeBase.Title;
            if ((shapeBase.NodeType == NodeType.Shape) && !StringUtil.HasChars(title))
                title = ((Shape)shapeBase).ImageData.Title;

            LinkDefinition linkDefinition = (reference == string.Empty)
                ? GetCollapsedLinkDefinition(shapeDestination, title)
                : GetFullReferenceLinkDefinition(reference, shapeDestination, title);

            if (!StringUtil.HasChars(reference))
                reference = StringUtil.HasChars(shapeBase.AlternativeText)
                    ? FormatReferenceString(shapeBase.AlternativeText)
                    : FormatReferenceString(GenerateShapeReference());

            if (linkDefinition == null)
                mLinkDefinitions.Add(new LinkDefinition(reference, shapeDestination, title));
        }

        /// <summary>
        /// Processes a specified FieldCodeHyperlink.
        /// </summary>
        internal void OnHyperlink(FieldCodeHyperlink hyperlink)
        {
            if (!ReferenceNeeded(hyperlink))
                return;

            string reference = GetReferenceString(hyperlink);

            LinkDefinition linkDefinition = (reference == string.Empty)
                ? GetCollapsedLinkDefinition(hyperlink.HRef.ToString(false), hyperlink.ScreenTip)
                : GetFullReferenceLinkDefinition(reference, hyperlink.HRef.ToString(false), hyperlink.ScreenTip);

            if (linkDefinition == null)
                mLinkDefinitions.Add(new LinkDefinition(reference, hyperlink.HRef.ToString(false), hyperlink.ScreenTip));
            else if (!StringUtil.HasChars(hyperlink.DocLocation))
                hyperlink.DocLocation = linkDefinition.Reference;
        }

        /// <summary>
        /// Writes prepared link definitions to a specified writer.
        /// </summary>
        internal void WriteDefinitions(MarkdownWriter writer)
        {
            // Adds a newline if it hasn't been added yet.
            if ((mLinkDefinitions.Count > 0) &&
                (!writer.Builder.ToString().EndsWith(string.Format("{0}{0}", StringUtil.NewLine), StringComparison.InvariantCulture)))
                writer.Builder.AppendLine();

            // Writes reference link definitions.
            foreach (LinkDefinition linkData in mLinkDefinitions)
            {
                string label = linkData.Reference;
                // Spaces in the explicit form or in the form of a Url-code in link definitions are detected as the end of the link.
                // Therefore such link shall be enclosed in angle brackets <...>.
                string hRef = (StringUtil.IndexOfWhitespace(linkData.Destination) > -1) ||
                              StringUtil.Contains(linkData.Destination, "%20", true)
                    ? string.Format("<{0}>", linkData.Destination)
                    : linkData.Destination;
                string title = linkData.Title;
                if (title.Length > 0)
                    title = string.Format(" \"{0}\"", title);
                writer.Builder.AppendLine(string.Format("{0}: {1}{2}", label, hRef, title));
            }
        }

        /// <summary>
        /// Returns the reference label for the specified shape.
        /// </summary>
        internal string GetReferenceString(ShapeBase shape, string shapeDestination)
        {
            if (IsReference(shape.Name))
                return shape.Name;

            LinkDefinition linkDefinition = GetCollapsedLinkDefinition(shapeDestination, shape.Title);
            return (linkDefinition == null) ? string.Empty : linkDefinition.Reference;
        }

        /// <summary>
        /// Returns the reference label for the specified hyperlink.
        /// </summary>
        internal static string GetReferenceString(FieldCodeHyperlink hyperlink)
        {
            return IsReference(hyperlink.DocLocation)
                ? hyperlink.DocLocation
                : string.Empty;
        }

        /// <summary>
        /// Returns True if the link definition with the specified parameters exists.
        /// </summary>
        internal bool HasCollapsedLinkDefinition(string destination, string title)
        {
            return GetCollapsedLinkDefinition(destination, title) != null;
        }

        /// <summary>
        /// Returns True if the link definition with the specified parameters exists.
        /// </summary>
        internal bool HasFullReferenceLinkDefinition(string reference, string destination, string title)
        {
            return GetFullReferenceLinkDefinition(reference, destination, title) != null;
        }

        /// <summary>
        /// Sets the specified reference value to the specified collapsed link definition.
        /// </summary>
        internal void SetReference(string reference, string destination, string title)
        {
            LinkDefinition definition = GetCollapsedLinkDefinition(destination, title);
            if (definition != null)
                definition.Reference = reference;
        }

        /// <summary>
        /// Returns true if the current shape is exported as a reference image.
        /// </summary>
        private bool ReferenceNeeded(ShapeBase shapeBase)
        {
            switch (mWriter.SaveOptions.LinkExportMode)
            {
                case MarkdownLinkExportMode.Inline:
                    return false;
                case MarkdownLinkExportMode.Reference:
                    return true;
            }

            if (GetReferenceString(shapeBase, string.Empty).Length > 0)
                return true;

            // The counter of shapes similar to shapeBase.
            int linkCount = 0;

            if (shapeBase.NodeType == NodeType.Shape)
            {
                Shape shape = (Shape)shapeBase;
                foreach (Shape shapeNode in Shapes)
                {
                    if ((shapeNode.ImageData.Title != shape.ImageData.Title) || (shapeNode.Title != shape.Title))
                        continue;

                    if ((shapeNode.ImageData.ImageBytes != null) && (shape.ImageData.ImageBytes != null) &&
                        (shapeNode.ImageData.ImageBytes.Length != shape.ImageData.ImageBytes.Length))
                        continue;

                    if (StringUtil.HasChars(shapeNode.ImageData.SourceFullName) &&
                        (shapeNode.ImageData.SourceFullName == shape.ImageData.SourceFullName))
                        linkCount++;
                    else if (!shapeNode.ImageData.IsLinkOnly &&
                        ArrayUtil.IsArrayEqual(GetShapeBytesHash(shapeNode), GetShapeBytesHash(shape)))
                        linkCount++;

                    if (linkCount > 1)
                        return true;
                }
            }

            if (shapeBase.NodeType == NodeType.GroupShape)
            {
                GroupShape group = (GroupShape)shapeBase;
                foreach (GroupShape groupNode in GroupShapes)
                {
                    if (groupNode.Title != group.Title)
                        continue;

                    if (ArrayUtil.IsArrayEqual(GetShapeBytesHash(groupNode), GetShapeBytesHash(group)))
                        linkCount++;

                    if (linkCount > 1)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the hash sum for the specified Shape.
        /// </summary>
        private byte[] GetShapeBytesHash(ShapeBase shapeBase)
        {
            byte[] result;
            if (mShapeBytesMap.TryGetValue(shapeBase, out result))
                return result;

            byte[] imageBytes = (shapeBase.NodeType == NodeType.Shape)
                ? ((Shape)shapeBase).ImageData.ImageBytes
                : null;

            // FOSS: non-image shapes were rendered to a PNG to obtain bytes for content comparison.
            // Rendering is unavailable, so treat shapes without image bytes as having no comparable
            // content (empty hash) instead of dereferencing a null byte array.
            result = ((imageBytes != null) && (imageBytes.Length != 0))
                ? HashUtil.ComputeHash(DigestAlgorithm.MD5, imageBytes)
                : new byte[0];
            mShapeBytesMap.Add(shapeBase, result);

            return result;
        }

        /// <summary>
        /// Formats a specified link reference label as a reference string definition.
        /// </summary>
        private static string FormatReferenceString(string referenceLabel)
        {
            return string.Format("{0}{1}{2}", LinkTextBlock.OpeningDelimiter, referenceLabel, LinkTextBlock.ClosingDelimiter);
        }

        /// <summary>
        /// Returns true if the current hyperlink is exported as a reference link.
        /// </summary>
        private bool ReferenceNeeded(FieldCodeHyperlink hyperlink)
        {
            switch (mWriter.SaveOptions.LinkExportMode)
            {
                case MarkdownLinkExportMode.Reference:
                    return true;

                case MarkdownLinkExportMode.Auto:
                {
                    if (GetReferenceString(hyperlink).Length > 0)
                        return true;

                    // The counter of links similar to the hyperlink.
                    int linkCount = 0;

                    Document doc = mWriter.Document.FetchDocument();
                    foreach (Field field in doc.Range.Fields)
                    {
                        FieldHyperlink hyperlinkField = field as FieldHyperlink;
                        if (hyperlinkField == null)
                            continue;

                        if ((hyperlinkField.Address == hyperlink.HRef.ToString(false)) &&
                            (hyperlinkField.ScreenTip == hyperlink.ScreenTip))
                            linkCount++;

                        if (linkCount > 1)
                            return true;
                    }

                    return false;
                }

                case MarkdownLinkExportMode.Inline:
                    return false;

                default:
                    throw new InvalidOperationException("Unexpected link export mode.");
            }
        }

        /// <summary>
        /// Returns true if a specified string is a link reference string.
        /// </summary>
        private static bool IsReference(string text)
        {
            return (text.Length > 2) &&
                    text.StartsWith("[", StringComparison.InvariantCulture) &&
                    text.EndsWith("]", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Generate a new reference string for a Shape.
        /// </summary>
        private string GenerateShapeReference()
        {
            mShapeReferenceNameGenerator++;

            foreach (Shape shape in Shapes)
            {
                if (string.Equals(shape.Name, string.Format("ref{0}", mShapeReferenceNameGenerator), StringComparison.OrdinalIgnoreCase))
                    mShapeReferenceNameGenerator++;
            }

            return string.Format("ref{0}", mShapeReferenceNameGenerator);
        }

        /// <summary>
        /// Returns LinkDefinition with the given destination and title.
        /// </summary>
        private LinkDefinition GetCollapsedLinkDefinition(string destination, string title)
        {
            foreach (LinkDefinition linkDefinition in mLinkDefinitions)
            {
                if ((linkDefinition.Title == title) &&
                    (linkDefinition.Destination == destination))
                    return linkDefinition;
            }

            return null;
        }

        /// <summary>
        /// Returns LinkDefinition with the given destination, title and reference label.
        /// </summary>
        private LinkDefinition GetFullReferenceLinkDefinition(string reference, string destination, string title)
        {
            foreach (LinkDefinition linkDefinition in mLinkDefinitions)
            {
                if ((reference != string.Empty) && (linkDefinition.Reference == reference) &&
                    (linkDefinition.Title == title) && (linkDefinition.Destination == destination))
                    return linkDefinition;
            }

            return null;
        }

        /// <summary>
        /// The cached Shape collection of the document.
        /// </summary>
        private NodeCollection Shapes
        {
            get
            {
                if (mShapeCollection == null)
                    mShapeCollection = mWriter.Document.GetChildNodes(NodeType.Shape, true);

                return mShapeCollection;
            }
        }

        /// <summary>
        /// The cached GroupShape collection of the document.
        /// </summary>
        private NodeCollection GroupShapes
        {
            get
            {
                if (mGroupShapeCollection == null)
                    mGroupShapeCollection = mWriter.Document.GetChildNodes(NodeType.GroupShape, true);

                return mGroupShapeCollection;
            }
        }

        /// <summary>
        /// The class that stores LinkDefinitionBlock string data.
        /// </summary>
        private class LinkDefinition
        {
            internal LinkDefinition(string reference, string destination, string title)
            {
                Reference = reference;
                Destination = destination;
                Title = title;
            }

            internal string Reference;

            internal readonly string Destination;

            internal readonly string Title;
        }

        private NodeCollection mShapeCollection;
        private NodeCollection mGroupShapeCollection;

        private readonly  List<LinkDefinition> mLinkDefinitions = new List<LinkDefinition>();

        private readonly MarkdownWriter mWriter;

        /// <summary>
        /// The collection of shapes and their hashes.
        /// </summary>
        private readonly Dictionary<ShapeBase, byte[]> mShapeBytesMap = new Dictionary<ShapeBase, byte[]>();

        private int mShapeReferenceNameGenerator;
    }
}
