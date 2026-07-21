// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Represents a web extension object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-office-add-ins/">Work with Office Add-ins</a> documentation article.</para>
    /// </summary>
    /// <dev>2.2.7 CT_OsfWebExtension</dev>
    public class WebExtension
    {
        // Hide ctr.
        internal WebExtension() { }

        /// <summary>
        /// Uniquely identifies the web extension instance in the current document.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Specifies whether the user can interact with the web extension or not.
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        /// Specifies the primary reference to an web extension.
        /// </summary>
        public WebExtensionReference Reference
        {
            get { return mReference; }
        }

        /// <summary>
        /// Specifies a list of web extension bindings.
        /// </summary>
        public WebExtensionBindingCollection Bindings
        {
            get { return mBindings; }
        }

        /// <summary>
        /// Specifies alternate references to a web extension.
        /// </summary>
        public WebExtensionReferenceCollection AlternateReferences
        {
            get { return mAlternateReferences; }
        }

        /// <summary>
        /// Represents a set of web extension custom properties.
        /// </summary>
        public WebExtensionPropertyCollection Properties
        {
            get { return mProperties; }
        }

        /// <summary>
        /// Specifies a static image used to render the contents of the application for Office when it is not active.
        /// </summary>
        internal DmlBlip Snapshot
        {
            get { return mSnapshot;  }
            set { mSnapshot = value; }
        }

        private DmlBlip mSnapshot;
        private readonly WebExtensionReference mReference = new WebExtensionReference();
        private readonly WebExtensionBindingCollection mBindings = new WebExtensionBindingCollection();
        private readonly WebExtensionPropertyCollection mProperties = new WebExtensionPropertyCollection();
        private readonly WebExtensionReferenceCollection mAlternateReferences = new WebExtensionReferenceCollection();
    }
}
