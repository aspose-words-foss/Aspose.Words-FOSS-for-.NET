// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2017 by Edward Voronov

using System.IO;
using System.Xml;
using Aspose.Collections;
using Aspose.Words.Loading;
using Aspose.Xml;

namespace Aspose.Words.Fields
{
    internal class FieldIncludeTextUpdater
    {
        internal static FieldUpdateAction Update(Field field)
        {
            FieldIncludeTextUpdater updater = new FieldIncludeTextUpdater(field, (IFieldIncludeTextCode)field);
            return updater.Update();
        }

        private FieldIncludeTextUpdater(Field field, IFieldIncludeTextCode fieldCode)
        {
            mField = field;
            mFieldCode = fieldCode;
        }

        private FieldUpdateAction Update()
        {
            string sourceFileName = GetSourceFileName();
            try
            {
                using (Stream stream = FieldUtil.OpenStream(sourceFileName, mField.Document.ResourceLoadingCallback))
                {
                    if (stream == null)
                        return CreateNotValidFileNameErrorMessage(sourceFileName);

                    if (HasXPathExpression)
                        return EvaluateXPath(stream);

                    if (HasXslTransformation)
                        return EvaluateXslTransformation(stream, sourceFileName);

                    return UpdateFromSourceDocument(stream, sourceFileName);
                }
            }
            catch
            {
                return CreateNotValidFileNameErrorMessage(sourceFileName);
            }
        }

        private FieldUpdateAction CreateNotValidFileNameErrorMessage(string sourceFileName)
        {
            return new FieldUpdateActionInsertErrorMessage(mField, GetNotValidFileNameErrorMessage(sourceFileName));
        }

        private string GetNotValidFileNameErrorMessage(string sourceFileName)
        {
            return HasXPathExpression
                ? string.Format("Error! Word encountered an error while loading the XML file {0}.", sourceFileName)
                : "Error! Not a valid filename.";
        }

        private FieldUpdateAction UpdateFromSourceDocument(Stream sourceDocStream, string sourceFileName)
        {
            Document sourceDoc = LoadSourceDocument(sourceDocStream);
            return UpdateFromDocument(sourceDoc, sourceFileName);
        }

        private Document LoadSourceDocument(Stream stream)
        {
            // Only include Aspose.Words documents.
            LoadFormat loadFormat = FileFormatUtil.DetectFileFormat(stream).LoadFormat;
            if (loadFormat == LoadFormat.Unknown)
                return null;

            string textConverter = mFieldCode.TextConverter;
            if (textConverter != null)
                textConverter = textConverter.ToLower();

            switch (loadFormat)
            {
                case LoadFormat.Html:
                    if (textConverter == "xml" && !StartsWith(stream, "<!doctype"))
                        return LoadXmlDocument(stream);

                    return LoadHtmlDocument(stream);
                case LoadFormat.Text:
                    return textConverter == "html"
                        ? LoadPlainTextAsHtmlDocument(stream)
                        : LoadMonospaceTextDocument(stream);
                case LoadFormat.Xml:
                    return LoadXmlContent(stream, textConverter);
                default:
                    break;
            }

            return new Document(stream, null, false);
        }

        private static Document LoadXmlContent(Stream stream, string textConverter)
        {
            // MS Word does not treat document without XML declaration as XML document.
            if (string.IsNullOrEmpty(textConverter) && !StartsWith(stream, "<?xml"))
                return LoadMonospaceTextDocument(stream);

            return textConverter == "html"
                ? LoadHtmlDocument(stream)
                : LoadXmlDocument(stream);
        }

        private static bool StartsWith(Stream stream, string value)
        {
            try
            {
                StreamReader reader = new StreamReader(stream);
                foreach (char c in value)
                {
                    if (reader.Read() != c)
                        return false;
                }

                return true;
            }
            finally
            {
                stream.Position = 0;
            }
        }

        private static Document LoadPlainTextAsHtmlDocument(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            using (MemoryStream htmlStream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(htmlStream);
                writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><html>");
                writer.Write(text);
                writer.Write("</html>");
                writer.Flush();
                htmlStream.Position = 0;
                return LoadHtmlDocument(htmlStream);
            }
        }

        private static Document LoadMonospaceTextDocument(Stream stream)
        {
            TxtLoadOptions loadOptions = new TxtLoadOptions();
            loadOptions.LeadingSpacesOptions = TxtLeadingSpacesOptions.Preserve;
            loadOptions.LoadFormat = LoadFormat.Text;
            return new Document(stream, loadOptions, false);
        }

        private static Document LoadXmlDocument(Stream stream)
        {
            XmlDocument xml = XmlUtilPal.LoadXml(stream, false);
            DocumentBuilder builder = new DocumentBuilder(new Document(DocumentCtorMode.BlankDocumentNode));

            LoadXmlNode(builder, xml.DocumentElement, 0);

            return builder.Document;
        }

        private static void LoadXmlNode(DocumentBuilder builder, XmlNode node, int depth)
        {
            if (!node.HasChildNodes)
            {
                builder.CurrentParagraph.ParagraphFormat.LeftIndent = depth * 18;
                builder.Writeln(node.Value.Trim());
            }
            else
            {
                if (node.ChildNodes.Count == 1 && XmlUtilPal.IsNodeType(node.FirstChild, XmlNodeType.Text))
                {
                    LoadXmlNode(builder, node.FirstChild, depth);
                    return;
                }

                builder.CurrentParagraph.ParagraphFormat.LeftIndent = depth * 18;
                builder.Writeln();

                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode childNode = node.ChildNodes[i];
                    if (!XmlUtilPal.IsNodeType(childNode, XmlNodeType.Whitespace))
                        LoadXmlNode(builder, childNode, depth + 1);
                }

                builder.CurrentParagraph.ParagraphFormat.LeftIndent = depth * 18;
                builder.Writeln();
            }
        }

        private static Document LoadHtmlDocument(Stream stream)
        {
            HtmlLoadOptions loadOptions = new HtmlLoadOptions();
            loadOptions.LoadFormat = LoadFormat.Html;
            return new Document(stream, loadOptions, false);
        }

        private FieldUpdateAction UpdateFromDocument(Document document, string sourceFileName)
        {
            if (document == null)
                return new FieldUpdateActionInsertErrorMessage(mField, "Cannot include document of this type at the moment.");

            SectionStart endSectionStart = ((Section)mField.End.GetAncestor(NodeType.Section)).PageSetup.SectionStart;
            NodeRange sourceRange;
            bool needMergeFirstParagraph = false;

            FieldUpdateActionFormatResult result;

            // Remove the old result.
            using (mField.UpdateContext.RemoveOldResultSafe())
            {
                UnpairedBookmarkNodeRemover listener = null;

                if (!StringUtil.HasChars(mFieldCode.BookmarkName))
                {
                    // The source range is the whole document.
                    Node endNode = document.LastSection.Body.LastChild;
                    Node startNode = document.FirstSection.Body.FirstChild;
                    needMergeFirstParagraph =
                        document.FirstSection.Body.FirstNonAnnotationChild.NodeType == NodeType.Paragraph;
                    sourceRange = new NodeRange(startNode, endNode);
                }
                else
                {
                    // The source range is a bookmark.
                    // SPEED No need to get a bookmark from a source document cache as the cache is not to be used anywhere else.
                    Bookmark sourceBookmark = document.Range.Bookmarks[mFieldCode.BookmarkName];
                    if (sourceBookmark == null)
                        return new FieldUpdateActionInsertErrorMessage(mField, Bookmark.ErrorBookmarkNotDefined);

                    // WORDSNET-11285 MS Work copies text with all related bookmarks
                    sourceRange = sourceBookmark.GetMostWideNodeRange();
                    listener = new UnpairedBookmarkNodeRemover();
                }

                // Updating field result may change formating source node. Preserve it by creating update action beforehand.
                result = new FieldUpdateActionFormatResult(mField);

                ImportedStylesIstdsCollector stylesIstdsCollector = mField.Updater.ImportedStylesIstdsCollector;
                IntToIntBidirectionalMap importedIstds = stylesIstdsCollector.GetImportedStylesIstds(sourceFileName);

                ExternalDocumentModifier modifier = new ExternalDocumentModifier(document, mField.FetchDocument(), importedIstds);
                NodeCopier.Copy(sourceRange, mField.End, modifier, listener,
                    NodeCopierOptions.UseSourceStartAncestorPr | NodeCopierOptions.CloneNode |
                        NodeCopierOptions.ProcessBoundBlockAnnotationAsInline);

                // WORDSNET-11285 If copied range contains unpaired bookmark nodes - remove them.
                if (listener != null)
                    listener.RemoveUnpairedBookmarkNodes();
            }

            if (needMergeFirstParagraph)
                MergeFirstParagraph();

            UpdateLastSectionProperties(sourceRange, endSectionStart);

            if (mFieldCode.LockFields)
                LockChildFieldsInResult();

            return result;
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition] // XPath isn't supported yet
        private FieldUpdateAction EvaluateXPath(Stream stream)
        {
            XmlDocument document = XmlUtilPal.LoadXml(stream, true);
            string result = XPathEvaluator.Evaluate(document, mFieldCode.XPath, mFieldCode.NamespaceMappings);
            return new FieldUpdateActionApplyResult(mField, result);
        }

        private FieldUpdateAction EvaluateXslTransformation(Stream stream, string sourceFileName)
        {
            XslTransform transform = new XslTransform();
            transform.Load(LoadXslTransformation());

            using (Stream xsltOutput = transform.Transform(stream))
            {
                return UpdateFromXslTransformedDocument(xsltOutput, sourceFileName);
            }
        }

        private FieldUpdateAction UpdateFromXslTransformedDocument(Stream stream, string sourceFileName)
        {
            Document document = LoadXslTransformedDocument(stream);
            return UpdateFromDocument(document, sourceFileName);
        }

        private static Document LoadXslTransformedDocument(Stream stream)
        {
            // Only include Aspose.Words documents.
            LoadFormat loadFormat = FileFormatUtil.DetectFileFormat(stream).LoadFormat;
            if (loadFormat == LoadFormat.Unknown)
                return null;

            switch (loadFormat)
            {
                case LoadFormat.Html:
                    return LoadHtmlDocument(stream);
                case LoadFormat.Text:
                    return LoadMonospaceTextDocument(stream);
                case LoadFormat.Xml:
                    return LoadXmlDocument(stream);
                default:
                    return new Document(stream, null, false);
            }
        }

        private Stream LoadXslTransformation()
        {
            try
            {
                return FieldUtil.OpenStream(mFieldCode.XslTransformation, mField.Document.ResourceLoadingCallback);
            }
            catch
            {
                return null;
            }
        }

        private bool HasXPathExpression
        {
            get { return !string.IsNullOrEmpty(mFieldCode.XPath); }
        }

        private bool HasXslTransformation
        {
            get { return !string.IsNullOrEmpty(mFieldCode.XslTransformation); }
        }

        private static bool IsFirstChild(Node node, NodeType ancestorType)
        {
            if (node.NodeType == ancestorType)
                return true;

            if (!node.IsFirstChild &&
                !(node.IsComposite && (node.PreviousNonAnnotationSibling == null)))
            {
                return false;
            }

            return IsFirstChild(node.ParentNode, ancestorType);
        }

        private static bool IsLastChild(Node node, NodeType ancestorType)
        {
            if (node.NodeType == ancestorType)
                return true;

            if (!node.IsLastChild &&
                !(node.IsComposite && (node.NextNonAnnotationSibling == null)))
            {
                return false;
            }

            return IsLastChild(node.ParentNode, ancestorType);
        }

        private void UpdateLastSectionProperties(NodeRange sourceRange, SectionStart endSectionStart)
        {
            bool startIsFirstChild = IsFirstChild(mField.Start, NodeType.Section);
            bool endIsLastChild = IsLastChild(mField.End, NodeType.Section);
            if (startIsFirstChild && endIsLastChild)
            {
                CopyLastSectionProperties(sourceRange);
            }
            else
            {
                SetLastSectionBreakType(endSectionStart);
            }
        }

        private void CopyLastSectionProperties(NodeRange sourceRange)
        {
            // WORDSNET-10770 MS Word additionally copies properties of the last section, if there is no content except field.
            Section fieldEndSection = (Section)mField.End.GetAncestor(NodeType.Section);
            Section sourceEndSection = (Section)sourceRange.End.Node.GetAncestor(NodeType.Section);

            fieldEndSection.SectPr = sourceEndSection.SectPr.Clone();
        }

        private void SetLastSectionBreakType(SectionStart endSectionStart)
        {
            // WORDSNET-10786, WORDSNET-12104 MS Word retains last section type, if there is any content in source section except INCLUDETEXT field.
            Section fieldEndSection = (Section)mField.End.GetAncestor(NodeType.Section);
            fieldEndSection.PageSetup.SectionStart = endSectionStart;
        }

        private string GetSourceFileName()
        {
            // WORDSNET-11245 if SourceFullName parameter contains fields we should not normalize text in field argument
            FieldArgument argument = mField.FieldCodeCache.GetArgument(mFieldCode.SourceFullNameArgumentIndex);
            if (argument == null || !argument.ContainsChildFields())
                return mFieldCode.SourceFullName;

            // Mimic MS Word: escape \, if argument contains nested fields
            // This is a HACK. Ideally, we should escape nested fields result, but it is way too complex.
            return argument.Text.Trim('"').Replace(@"\\", @"\");
        }

        private void LockChildFieldsInResult()
        {
            foreach (Node node in mField.GetFieldResultRange())
            {
                if (node.NodeType == NodeType.FieldStart)
                    ((FieldStart)node).IsLocked = true;
            }
        }

        private void MergeFirstParagraph()
        {
            Paragraph separatorParagraph = mField.Separator.ParentParagraph;
            Paragraph firstContentParagraph = separatorParagraph.NextNonAnnotationSibling as Paragraph;

            if (firstContentParagraph != null)
            {
                Node referenceNode = firstContentParagraph.FirstChild;
                Node nextNode;

                for (Node node = separatorParagraph.FirstChild; node != null; node = nextNode)
                {
                    nextNode = node.NextSibling;
                    firstContentParagraph.InsertBefore(node, referenceNode);
                }

                // Move block-level annotations.
                firstContentParagraph.InsertBefore(separatorParagraph.NextSibling, firstContentParagraph, referenceNode);

                separatorParagraph.Remove();
            }
        }

        private readonly Field mField;
        private readonly IFieldIncludeTextCode mFieldCode;

        private class UnpairedBookmarkNodeRemover : INodeCloningListener
        {
            internal UnpairedBookmarkNodeRemover()
            {
                mStarts = new StringToObjDictionary<Node>();
                mEnds = new StringToObjDictionary<Node>();
            }

            public void NotifyNodeCloned(Node source, Node clone)
            {
                if (clone.NodeType == NodeType.BookmarkStart)
                    mStarts[((BookmarkStart)clone).Name] = clone;
                else if (clone.NodeType == NodeType.BookmarkEnd)
                    mEnds[((BookmarkEnd)clone).Name] = clone;
            }

            internal void RemoveUnpairedBookmarkNodes()
            {
                foreach (string key in mStarts.Keys)
                {
                    if (mEnds.ContainsKey(key))
                        mEnds.Remove(key);
                    else
                        mStarts[key].Remove();
                }

                foreach (Node node in mEnds.Values)
                    node.Remove();
            }

            private readonly StringToObjDictionary<Node> mStarts;
            private readonly StringToObjDictionary<Node> mEnds;
        }
    }
}
