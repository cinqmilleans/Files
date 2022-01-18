using Files.Models;
using System.ComponentModel;

namespace Files.Services
{
    public interface IPaneSettingsService : IBaseSettingsService, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the pane content.
        /// </summary>
        PaneContents Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the height of the pane in a horizontal layout.
        /// </summary>
        double HorizontalSizePx { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the width of the pane in a vertical layout.
        /// </summary>
        double VerticalSizePx { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default volume on media.
        /// </summary>
        double MediaVolume { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the preview pane should only show the item preview without the details section
        /// </summary>
        bool ShowPreviewOnly { get; set; }

        /// <summary>
        /// Gets if a content is selected
        /// </summary>
        bool HasSelectedContent { get; }

        /// <summary>
        /// Gets or sets if the preview content is selected
        /// </summary>
        bool IsPreviewContentSelected { get; set; }
    }
}
