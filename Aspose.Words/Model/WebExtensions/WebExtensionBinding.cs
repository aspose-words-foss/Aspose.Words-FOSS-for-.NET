// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Specifies a binding relationship between a web extension and the data in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-office-add-ins/">Work with Office Add-ins</a> documentation article.</para>
    /// </summary>
    /// <dev>2.2.3 CT_OsfWebExtensionBinding</dev>
    public class WebExtensionBinding
    {
        /// <summary>
        /// Hide default ctr.
        /// </summary>
        internal WebExtensionBinding() { }

        /// <summary>
        /// Creates web extension binding with specified parameters.
        /// </summary>
        /// <param name="id">Binding identifier.</param>
        /// <param name="bindingType">Binding type.</param>
        /// <param name="appRef">Binding key used to map the binding entry in this list with the bound data in the document.</param>
        public WebExtensionBinding(string id, WebExtensionBindingType bindingType, string appRef)
        {
            Id = id;
            AppRef = appRef;
            BindingType = bindingType;
        }

        /// <summary>
        /// Specifies the binding identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Specifies the binding type.
        /// </summary>
        public WebExtensionBindingType BindingType { get; set; }

        /// <summary>
        /// Specifies the binding key used to map the binding entry in this list with the bound data in the document.
        /// </summary>
        /// <dev>It may be, for example, identifier of the SDT.</dev>
        public string AppRef { get; set; }
    }
}
