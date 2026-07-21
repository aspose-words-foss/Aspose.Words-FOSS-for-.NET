// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2023 by Edward Voronov

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// Represents a corporate (an organization) bibliography source contributor.
    /// </summary>
    public sealed class Corporate : Contributor
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Corporate"/> class.
        /// </summary>
        /// <param name="name">The name of an organization.</param>
        public Corporate(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of an organization.
        /// </summary>
        public string Name { get; set; }

        internal override Contributor Clone()
        {
            return new Corporate(Name);
        }
    }
}
