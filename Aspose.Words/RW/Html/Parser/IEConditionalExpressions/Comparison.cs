// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

using System;
using System.Text;

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a simple comparison, a part of a conditional expression. For example, 'ie', 'vml 1', 'lt ie 9.2', etc.
    /// </summary>
    internal class Comparison : ConditionalExpression
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="operation">A comparison operation.</param>
        /// <param name="feature">A feature name. Cannot be empty or <c>null</c>.</param>
        /// <param name="version">
        /// A version of the feature. Can be <c>null</c> to indicate that the version of the feature is unimportant.
        /// </param>
        internal Comparison(ComparisonOperation operation, string feature, VersionVector version)
        {
            Debug.Assert(StringUtil.HasChars(feature));
            Debug.Assert((mOperation == ComparisonOperation.Equal) || (version != null));

            mOperation = operation;
            mFeature = feature;
            mVersion = version;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            string operation;
            switch (mOperation)
            {
                case ComparisonOperation.Greater:
                    operation = "gt";
                    break;
                case ComparisonOperation.GreaterOrEqual:
                    operation = "gte";
                    break;
                case ComparisonOperation.Less:
                    operation = "lt";
                    break;
                case ComparisonOperation.LessOrEqual:
                    operation = "lte";
                    break;
                default:
                    operation = String.Empty;
                    break;
            }
            result.Append(operation);

            if (operation.Length > 0)
            {
                result.Append(' ');
            }
            result.Append(mFeature);

            if (mVersion != null)
            {
                result.Append(' ');
                result.Append(mVersion);
            }

            return result.ToString();
        }

        internal override bool Matches(Features features)
        {
            Feature feature = features.Get(mFeature);
            if (feature == null)
            {
                // Feature is unavailable.
                return false;
            }
            if (mVersion == null)
            {
                // If no version is specified, we are not interested in a specific version of the feature.
                // We only check if the feature is available at all.
                return true;
            }
            if (feature.Version == null)
            {
                // The feature is available but has no specific version. Since we are interested in a specific version of
                // the feature, the check fails.
                return false;
            }
            // Compare actual and requested versions of the feature.
            return feature.Version.CompareTo(mVersion, mOperation);
        }

        protected override bool IsSubexpression
        {
            get { return false; }
        }

        private readonly ComparisonOperation mOperation;
        private readonly string mFeature;
        private readonly VersionVector mVersion;
    }
}