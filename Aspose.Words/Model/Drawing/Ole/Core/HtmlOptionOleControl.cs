// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2019 by Alexey Morozov

using Aspose.Ss;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.Drawing.Ole.Core
{
    /// <summary>
    /// Implements OLE HTML radio button control.
    /// </summary>
    internal class HtmlOptionOleControl : HtmlOleControl
    {
        /// <summary>
        /// Creates instance of the <see cref="HtmlOptionOleControl"/> class with a specified name.
        /// </summary>
        internal HtmlOptionOleControl(string name) : base(name)
        {
        }

        /// <summary>
        /// Reads control from a storage.
        /// </summary>
        internal override void Read(MemoryStorage storage)
        {
            base.Read(storage);

            HtmlDocument htmlDocument = HtmlDocument.Load(Html);
            HtmlElementNode input = htmlDocument.Root.FindSingleElementByName("input");
            foreach (HtmlAttribute attribute in input.Attributes)
            {
                if (attribute.Name == "checked")
                {
                    Checked = true;
                    break;
                }
            }
        }

        internal bool Checked { get; private set; }
    }
}
