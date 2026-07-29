// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/12/2015 by Victor Chebotok

using System.Text;
using Aspose.Collections;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents a set of features that are considered supported by HTML import when working with IE conditional expressions.
    /// </summary>
    internal class Features
    {
        /// <summary>
        /// Creates a new set of features.
        /// </summary>
        /// <remarks>
        /// If there are features with same names, only the last one of them is added to the set.
        /// </remarks>
        internal Features(params Feature[] features)
        {
            foreach (Feature feature in features)
            {
                Debug.Assert(feature != null);
                mFeatures[feature.Name] = feature;
            }
        }

        /// <summary>
        /// Adds feature to set.
        /// </summary>
        /// <param name="feature">Feature.</param>
        internal void Add(Feature feature)
        {
            mFeatures[feature.Name] = feature;
        }

        /// <summary>
        /// Gets a feature from the set by name.
        /// </summary>
        /// <param name="featureName">A feature name. Names are case-insensitive.</param>
        /// <returns>A known feature that has the specified name or <c>null</c> if no feature found.</returns>
        internal Feature Get(string featureName)
        {
            return mFeatures[featureName];
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (Feature feature in mFeatures.Values)
            {
                if (result.Length > 0)
                {
                    result.Append("; ");
                }
                result.Append(feature);
            }
            return result.ToString();
        }

        private readonly StringToObjDictionary<Feature> mFeatures = new StringToObjDictionary<Feature>();
    }
}
