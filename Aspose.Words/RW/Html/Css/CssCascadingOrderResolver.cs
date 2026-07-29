// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2013 by Alexey Butalov

using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Resolves CSS declaration order using CSS cascade principle.
    /// </summary>
    /// <remarks>
    /// Implements 2,3 and 4 parts of the cascade principle mechanism:
    /// 2. Sort declarations according to their importance (normal or important) and origin (author, user, or user agent). 
    ///    From highest to lowest precedence:
    ///    1.  user !important declarations
    ///    2.  author !important declarations
    ///    3.  author normal declarations
    ///    4.  user normal declarations
    ///    5.  user agent declarations
    /// 3. If declarations have the same importance and source, sort them by selector specificity.
    /// 4. Finally, if declarations have the same importance, source, and specificity, sort them by the order 
    ///   they are specified in the CSS. The last declaration wins.
    /// http://www.w3.org/TR/CSS21/cascade.html#cascade
    /// </remarks>
    internal class CssCascadingOrderResolver
    {
        internal CssCascadingOrderResolver()
        {
            mDeclarationInfo = new StringToObjDictionary<DeclarationInfo>();
        }

        /// <summary>
        /// Adds CSS declarations to the result considering a CSS specificity and declaration origin.
        /// </summary>
        /// <param name="declarations">CSS declarations.</param>
        /// <param name="origin">CSS declarations origin.</param>
        /// <param name="specificity">CSS specificity of the declarations selector.</param>
        internal void Add(CssDeclarationCollection declarations, CssDeclarationOrigin origin, CssSelectorSpecificity specificity)
        {
            foreach (CssDeclaration declaration in declarations)
            {
                DeclarationInfo declarationInfo = mDeclarationInfo[declaration.Property];

                bool updateDeclarationInfo;
                if (declarationInfo == null)
                {
                    declarationInfo = new DeclarationInfo();
                    mDeclarationInfo.Add(declaration.Property, declarationInfo);
                    updateDeclarationInfo = true;
                }
                else
                {
                    int foundDeclarationPriority = GetDeclarationPriority(declarationInfo.Origin, declarationInfo.Declaration.Important);
                    int newDeclarationPriority = GetDeclarationPriority(origin, declaration.Important);
                    if (newDeclarationPriority > foundDeclarationPriority)
                        updateDeclarationInfo = true;
                    else if (newDeclarationPriority == foundDeclarationPriority)
                        updateDeclarationInfo = specificity.CompareTo(declarationInfo.Specificity) >= 0;
                    else
                        updateDeclarationInfo = false;
                }

                if (updateDeclarationInfo)
                {
                    declarationInfo.Declaration = declaration;
                    declarationInfo.Origin = origin;
                    declarationInfo.Specificity = specificity;
                    declarationInfo.Flags = declarations.GetFlags(declaration.Property);
                }
            }
        }

        /// <summary>
        /// Resets the resolver to initial state.
        /// </summary>
        internal void Clear()
        {
            mDeclarationInfo.Clear();
        }

        /// <summary>
        /// Adds resolved CSS declarations to the specified collection replacing existing declarations.
        /// </summary>
        internal void Resolve(CssDeclarationCollectionBuilder destinationDeclarations)
        {
            foreach (DeclarationInfo item in mDeclarationInfo.Values)
            {
                destinationDeclarations.AddOrReplace(item.Declaration);
                string property = item.Declaration.Property;
                destinationDeclarations.SetFlags(property, item.Flags);
                if (item.Origin == CssDeclarationOrigin.UserAgent)
                {
                    destinationDeclarations.MarkUserAgent(property);
                }
            }
        }

        /// <summary>
        /// Get declaration proprity by origin and !important flag. 
        /// </summary>
        /// <param name="origin">CSS declaration origin.</param>
        /// <param name="important">CSS declaration !important flag.</param>
        /// <returns>Integer value that indicates declaration proprity. Highest value corresponds to highest priority.</returns>
        private static int GetDeclarationPriority(CssDeclarationOrigin origin, bool important)
        {
            const int importantUserAgentPriority = 60;
            const int importantUserPriority = 50;
            const int importantAuthorPriority = 40;
            const int normalAuthorPriority = 30;
            const int normalUserPriority = 20;
            const int normalUserAgentPriority = 10;

            int prority;
            switch (origin)
            {
                case CssDeclarationOrigin.Author:
                    prority = (important) ? importantAuthorPriority : normalAuthorPriority;
                    break;
                case CssDeclarationOrigin.User:
                    prority = (important) ? importantUserPriority : normalUserPriority;
                    break;
                case CssDeclarationOrigin.UserAgent:
                    prority = (important) ? importantUserAgentPriority : normalUserAgentPriority;
                    break;
                default:
                    Debug.Assert(false);
                    return 0;
            }

            return prority;
        }

        private readonly StringToObjDictionary<DeclarationInfo> mDeclarationInfo;
    }

    /// <summary>
    /// Helps to store intermediate results.
    /// </summary>
    internal class DeclarationInfo
    {
        internal CssDeclaration Declaration;
        internal CssDeclarationOrigin Origin;
        internal CssSelectorSpecificity Specificity;
        internal CssDeclarationFlags Flags;
    }
}
