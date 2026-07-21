// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2024 by Ilya Navrotskiy

using System;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Allows to specify the elements to be exported to Markdown as raw HTML.
    /// </summary>
    [Flags]
    [CppEnumEnableMetadata]
    public enum MarkdownExportAsHtml
    {
        /// <summary>
        /// Export all elements using Markdown syntax without any raw HTML.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Export tables as raw HTML.
        /// </summary>
        /// <remarks>
        /// <para>When this option is enabled, every table will be exported as raw HTML.
        /// Aspose.Words will try to preserve all formatting of the tables in this case.</para>
        /// <para>If this flag is set, then related <see cref="NonCompatibleTables"/> flag will be ignored.</para>
        /// </remarks>
        Tables = 0x0001,

        /// <summary>
        /// Export tables that cannot be correctly represented in pure Markdown as raw HTML.
        /// </summary>
        /// <remarks>
        /// <para> When this option is enabled, Aspose.Words will only export tables that have merged cells
        /// or nested tables as raw HTML. And all other tables will be exported in Markdown format.
        /// Also note, this option will not preserve all formatting of the table, but only preserves
        /// corresponding spans of the cells.</para>
        /// <para>If related <see cref="Tables"/> flag is set, then this flag will be ignored.</para>
        /// </remarks>
        NonCompatibleTables = 0x0002
    }
}
