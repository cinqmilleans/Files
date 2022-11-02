using Files.App.ViewModels.Key;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Files.App.UserControls
{
	public sealed partial class ShortKeyPicker : UserControl
	{
		public static readonly DependencyProperty ShortKeyProperty =
			DependencyProperty.Register(nameof(ShortKey), typeof(ShortKey), typeof(ShortKeyPicker), new PropertyMetadata(ShortKey.None));

		public ShortKey ShortKey
		{
			get => (ShortKey)GetValue(ShortKeyProperty);
			set => SetValue(ShortKeyProperty, value);
		}

		public ShortKeyPicker() => InitializeComponent();

		private void Picker_PreviewKeyDown(object _, KeyRoutedEventArgs e)
		{
			e.Handled = true;
		}
	}
}
