// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Represents an add-in task pane object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-office-add-ins/">Work with Office Add-ins</a> documentation article.</para>
    /// </summary>
    /// <dev>2.2.8 CT_OsfTaskpane. This essence has public constructor.</dev>
    public class TaskPane
    {
        /// <summary>
        /// Specifies the index, enumerating from the outside to the inside, of this task pane among other persisted
        /// task panes docked in the same default location.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Specifies the default width value for this task pane instance.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Specifies whether the task pane is locked to the document in the UI and cannot be closed by the user.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Specifies whether the task pane shows as visible by default when the document opens.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Specifies the last-docked location of this task pane object.
        /// </summary>
        /// <dev>In the spec this value represents as string.</dev>
        public TaskPaneDockState DockState { get; set; }

        /// <summary>
        /// Represents an web extension object.
        /// </summary>
        public WebExtension WebExtension
        {
            get { return mExtension; }
        }

        private readonly WebExtension mExtension = new WebExtension();
    }
}
