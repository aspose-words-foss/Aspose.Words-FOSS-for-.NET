// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2020 by Alexander Zhiltsov

using Aspose.Common;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Implements reading 2.1.5 commentsExtensible [MS-DOCX] document parts.
    /// </summary>
    internal static class DocxCommentsExtensibleReader
    {
        /// <summary>
        /// Reads a commentsExtensible document part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.CommentsExtensible);
            if (xmlReader == null)
                return;

            reader.ComplianceInfo.MarkAsHasDocxExtensionsOf(MsWordVersionCore.Word2019);

            while (xmlReader.ReadChild("commentsExtensible"))
            {
                switch (xmlReader.LocalName)
                {
                    case "commentExtensible":
                        ReadCommentExtensible(reader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Reads a commentExtensible element.
        /// </summary>
        private static void ReadCommentExtensible(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            int durableId = NrxXmlUtil.TryHexToInt(xmlReader.ReadAttribute("durableId", "0"));

            CommentExtensible extensible = reader.FindCommentExtensible(durableId);
            if (extensible == null)
            {
                xmlReader.IgnoreElement();
                return;
            }

            extensible.UtcDateTime = FormatterPal.XmlToDateTime(xmlReader.ReadAttribute("dateUtc", ""));
            extensible.IsIntelligentPlaceholder = xmlReader.ReadBoolAttribute("intelligentPlaceholder", false);

            while (xmlReader.ReadChild("commentExtensible"))
            {
                switch (xmlReader.LocalName)
                {
                    case "extLst":
                        ReadCommentExtensibleExtensionList(reader, extensible);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads an 'extLst' element located in a 'commentExtensible' element.
        /// </summary>
        private static void ReadCommentExtensibleExtensionList(DocxDocumentReaderBase reader, CommentExtensible extensible)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("extLst"))
            {
                switch (xmlReader.LocalName)
                {
                    case "ext":
                        ReadCommentExtensibleExtension(reader, extensible);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads an 'ext' element that represents an extension of a 'commentExtensible' element.
        /// Only the 'reactions' extension is currently supported.
        /// </summary>
        private static void ReadCommentExtensibleExtension(DocxDocumentReaderBase reader, CommentExtensible extensible)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string uri = xmlReader.ReadAttribute("uri", null);

            // Only one extension is known at the moment.
            if (uri != ReactionsExtentionUri)
            {
                xmlReader.IgnoreElement();
                return;
            }

            while (xmlReader.ReadChild("ext"))
            {
                switch (xmlReader.LocalName)
                {
                    case "reactions":
                        extensible.Reactions = ReadCommentReactions(reader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the 'reactions' element of the CT_CommentReactions complex type defined in §2.1.1.1 of [MS-OREACTXML]
        /// that specifies information for the reactions to a comment.
        /// </summary>
        private static CommentReactionCollection ReadCommentReactions(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            CommentReactionCollection reactions = new CommentReactionCollection();

            while (xmlReader.ReadChild("reactions"))
            {
                switch (xmlReader.LocalName)
                {
                    case "reaction":
                        reactions.Add(ReadCommentReaction(reader));
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            return reactions;
        }

        /// <summary>
        /// Reads the 'reaction' element of the CT_CommentReaction complex type defined in §2.1.3.1 of [MS-OREACTXML]
        /// that specifies information for a single reaction type.
        /// </summary>
        private static CommentReaction ReadCommentReaction(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            CommentReaction reaction = new CommentReaction();
            reaction.ReactionType = xmlReader.ReadIntAttribute("reactionType", 0);

            while (xmlReader.ReadChild("reaction"))
            {
                switch (xmlReader.LocalName)
                {
                    case "reactionInfo":
                        reaction.ReactionInfos.Add(ReadCommentReactionInfo(reader));
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            return reaction;
        }

        /// <summary>
        /// Reads the 'reactionInfo' element of the CT_CommentReactionInfo complex type defined in §2.1.3.2 of
        /// [MS-OREACTXML] that specifies the user who added the reaction and the time when the reaction was added.
        /// </summary>
        private static CommentReactionInfo ReadCommentReactionInfo(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            CommentReactionInfo reactionInfo = new CommentReactionInfo();
            reactionInfo.DateTimeUtc = FormatterPal.XmlToDateTime(xmlReader.ReadAttribute("dateUtc", ""));

            while (xmlReader.ReadChild("reactionInfo"))
            {
                switch (xmlReader.LocalName)
                {
                    case "user":
                        ReadCommentReactionUser(reader, reactionInfo);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            return reactionInfo;
        }

        /// <summary>
        /// Reads the 'user' element of the CT_User complex type defined in §2.1.3.4 of [MS-OREACTXML] that specifies
        /// identity details of a user into the instance of <see cref="CommentReactionInfo"/>.
        /// </summary>
        private static void ReadCommentReactionUser(DocxDocumentReaderBase reader, CommentReactionInfo reactionInfo)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            reactionInfo.UserId = xmlReader.ReadAttribute("userId", null);
            reactionInfo.UserName = xmlReader.ReadAttribute("userName", null);
            reactionInfo.UserProvider = xmlReader.ReadAttribute("userProvider", null);

            while (xmlReader.ReadChild("user"))
                xmlReader.IgnoreElement();
        }

        internal const string ReactionsExtentionUri = "{CE6994B0-6A32-4C9F-8C6B-6E91EDA988CE}";
    }
}
