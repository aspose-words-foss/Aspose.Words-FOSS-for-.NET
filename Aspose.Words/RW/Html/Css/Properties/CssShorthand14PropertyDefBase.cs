// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements base class for shorthands properties related to edges of a box, 
    /// like border-style, margin or padding, always use a consistent 1-to-4-value syntax representing edges.
    /// </summary>
    /// <remarks>
    /// The top, right, bottom, and left values can be changed independently using separate properties. 
    /// Syntax:
    ///   name: width{1,4} | inherit
    /// </remarks>
    internal abstract class CssShorthand14PropertyDefBase : CssShorthandPropertyDef
    {
        protected CssShorthand14PropertyDefBase(
            string name,
            string topPropertyName,
            string rightPropertyName,
            string bottomPropertyName,
            string leftPropertyName)
            : base(name, false)
        {
            Debug.Assert(StringUtil.HasChars(topPropertyName));
            Debug.Assert(StringUtil.HasChars(rightPropertyName));
            Debug.Assert(StringUtil.HasChars(bottomPropertyName));
            Debug.Assert(StringUtil.HasChars(leftPropertyName));

            Debug.Assert(StringUtil.IsAsciiLowerCase(topPropertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(rightPropertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(bottomPropertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(leftPropertyName));

            TopPropertyName = topPropertyName;
            RightPropertyName = rightPropertyName;
            BottomPropertyName = bottomPropertyName;
            LeftPropertyName = leftPropertyName;
        }

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations)
        {
            Debug.Assert(individualDeclarations.Count == 4);

            return new CssShorthand14PropertyValue(individualDeclarations[TopPropertyName].Value,
                                                   individualDeclarations[RightPropertyName].Value,
                                                   individualDeclarations[BottomPropertyName].Value,
                                                   individualDeclarations[LeftPropertyName].Value);
        }

        protected override CssDeclarationCollection CreateIndividualDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            Debug.Assert(cssValues != null);
            if ((cssValues.Count == 0) || (cssValues.Count > 4))
                return null;

            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();

            ShorthandPropertyPart[] individualProperties = GetIndividualProperties();
            Debug.Assert(individualProperties.Length == 4);

            CssValueList values = new CssValueList(cssValues);
            if (values.Count == 1)
                values.Add(values[0]);
            if (values.Count == 2)
                values.Add(values[0]);
            if (values.Count == 3)
                values.Add(values[1]);

            for (int i = 0; i < 4; i++)
            {
                int affectedValues;
                CssDeclarationCollection individualDeclarations = individualProperties[i].CreateIndividualDeclarations(
                    values,
                    i,
                    important,
                    isInQuirksMode,
                    out affectedValues);
                if (individualDeclarations == null)
                    return null;

                result.AddOrReplace(individualDeclarations);
            }

            return result.GetDeclarations();
        }

        protected override ShorthandPropertyPart[] GetIndividualProperties()
        {
            if (mIndividualProperties != null)
                return mIndividualProperties;

            mIndividualProperties = new ShorthandPropertyPart[]
            {
                new ShorthandPropertyPart(TopPropertyName, true),
                new ShorthandPropertyPart(RightPropertyName, true),
                new ShorthandPropertyPart(BottomPropertyName, true),
                new ShorthandPropertyPart(LeftPropertyName, true)
            };
            return mIndividualProperties;
        }

        protected string TopPropertyName { get; }

        protected string RightPropertyName { get; }

        protected string BottomPropertyName { get; }

        protected string LeftPropertyName { get; }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
