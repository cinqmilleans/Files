using Files.Models;
using System.ComponentModel;

namespace Files.Services
{
    public interface IPaneSettingsService : IBaseSettingsService, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the active pane.
        /// </summary>
        Panes Pane { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the height of the pane in a horizontal layout.
        /// </summary>
        double SizeHorizontalPx { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the width of the pane in a vertical layout.
        /// </summary>
        double SizeVerticalPx { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default volume on media.
        /// </summary>
        double MediaVolume { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the preview pane should only show the item preview without the details section
        /// </summary>
        bool ShowPreviewOnly { get; set; }
    }
}
