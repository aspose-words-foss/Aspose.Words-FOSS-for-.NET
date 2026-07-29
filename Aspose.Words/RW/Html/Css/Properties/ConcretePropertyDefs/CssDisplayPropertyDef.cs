// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/04/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'display' CSS property.
    /// </summary>
    internal class CssDisplayPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssDisplayPropertyDef()
            : base(
                "display",
                false,
                CssValue.Inline,
                // inline | block | list-item | run-in | inline-block | table | inline-table | table-row-group |
                // table-header-group | table-footer-group | table-row | table-column-group | table-column | table-cell |
                // table-caption | none
                CssValueFilter.Values(
                    CssValue.Inline,
                    CssValue.Block,
                    CssValue.ListItem,
                    CssValue.RunIn,
                    CssValue.InlineBlock,
                    CssValue.InlineTable,
                    CssValue.Table,
                    CssValue.TableRowGroup,
                    CssValue.TableHeaderGroup,
                    CssValue.TableFooterGroup,
                    CssValue.TableRow,
                    CssValue.TableColumnGroup,
                    CssValue.TableColumn,
                    CssValue.TableCell,
                    CssValue.TableCaption,
                    CssValue.None))
        {
            // Empty constructor.
        }
    }
}
