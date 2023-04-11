using Files.App.Commands;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace Files.App.UserControls
{
	public sealed partial class HotKeyPicker : UserControl
	{
		private static DependencyProperty HotKeyProperty { get; } = DependencyProperty
			.Register(nameof(HotKey), typeof(HotKey), typeof(HotKeyPicker), new(HotKey.None));

		private HotKey HotKey
		{
			get => (HotKey)GetValue(HotKeyProperty);
			set
			{
				SetValue(HotKeyProperty, value);
				Box.Text = value.Label;
			}
		}

		public HotKeyPicker() => InitializeComponent();

		protected override void OnPreviewKeyDown(KeyRoutedEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			e.Handled = true;

			if (HotKey.Key is not Keys.None)
			{
				HotKey = HotKey.None;
			}

			if (Enum.IsDefined((Keys)e.Key))
			{
				HotKey = HotKey with { Key = (Keys)e.Key };
			}
			else
			{
				HotKey = HotKey with { Modifier = HotKeyHelpers.GetCurrentKeyModifiers() };
			}
		}

		protected override void OnPreviewKeyUp(KeyRoutedEventArgs e)
		{
			base.OnPreviewKeyUp(e);
			e.Handled = true;

			if (HotKey.Key is Keys.None)
			{
				HotKey = HotKey with { Modifier = HotKeyHelpers.GetCurrentKeyModifiers() };
			}
		}

		private void Box_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
		{
			Box.Select(Box.Text.Length - 1, 0);
		}
	}
}
