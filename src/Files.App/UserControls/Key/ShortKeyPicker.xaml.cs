using Files.Backend.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace Files.App.UserControls
{
	public sealed partial class ShortKeyPicker : UserControl
	{
		//public static readonly DependencyProperty ShortKeyProperty =
		//	DependencyProperty.Register(nameof(ShortKey), typeof(ShortKey), typeof(ShortKeyPicker), null);

		public string Text => "Coucou";

		/*public ShortKey ShortKey
		{
			get => (ShortKey)GetValue(ShortKeyProperty);
			set => SetValue(ShortKeyProperty, value);
		}*/

		public ShortKeyPicker()
		{
			InitializeComponent();
			//Picker.Text = ShortKey.ToString();
		}

		private void Picker_PreviewKeyDown(object _, KeyRoutedEventArgs e)
		{
			//if (e.Key is VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift)
			//	ShortKey = new ShortKey(ShortKey.Key, ShortKey.Modifier | VirtualKeyModifiers.Shift);

			//Picker.Text = ShortKey.ToString();
			e.Handled = true;
		}
	}
}
