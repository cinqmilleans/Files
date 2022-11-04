﻿using System;
using System.Linq;
using System.Text;
using Windows.System;

namespace Files.App.DataModels
{
	public struct ShortKey : IEquatable<ShortKey>
	{
		public static ShortKey None { get; } = new ShortKey(VirtualKey.None, VirtualKeyModifiers.None);

		public VirtualKey Key { get; }
		public VirtualKeyModifiers Modifiers { get; }

		public ShortKey(VirtualKey key) : this(key, VirtualKeyModifiers.None) {}
		public ShortKey(VirtualKey key, VirtualKeyModifiers modifiers)
		{
			if (IsModifier(key))
				throw new ArgumentException("The key cannot be a modifier.", nameof(key));

			Key = key;
			Modifiers = modifiers;

			static bool IsModifier(VirtualKey key)
				=> key is VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu
				or VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl
				or VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift
				or VirtualKey.LeftWindows or VirtualKey.RightWindows;
		}

		public void Deconstruct(out VirtualKey key, out VirtualKeyModifiers modifiers)
			=> (key, modifiers) = (Key, Modifiers);

		public static implicit operator ShortKey(string shortKey) => Parse(shortKey);
		public static implicit operator string(ShortKey shortKey) => shortKey.ToString();

		public static bool operator ==(ShortKey a, ShortKey b) => a.Equals(b);
		public static bool operator !=(ShortKey a, ShortKey b) => !a.Equals(b);

		public static ShortKey Parse(string shortKey)
		{
			var key = VirtualKey.None;
			var modifiers = VirtualKeyModifiers.None;

			var parts = shortKey.Split('+').Select(item => item.Trim().ToLower());
			foreach (string part in parts)
			{
				var m = ToModifiers(part);
				if (m is not VirtualKeyModifiers.None)
					modifiers |= m;

				var k = ToKey(part);
				if (k is not VirtualKey.None)
					key = k;
			}

			return new ShortKey(key, modifiers);

			static VirtualKeyModifiers ToModifiers(string modifiers) => modifiers switch
			{
				"alt" or "menu " => VirtualKeyModifiers.Menu,
				"ctrl" or "control" => VirtualKeyModifiers.Control,
				"shift" => VirtualKeyModifiers.Shift,
				_ => VirtualKeyModifiers.None,
			};

			static VirtualKey ToKey(string part) => part switch
			{
				"alt" or "menu" => VirtualKey.None,
				"ctrl" or "control" => VirtualKey.None,
				"shift" => VirtualKey.None,
				"windows" => VirtualKey.None,
				"0" => VirtualKey.Number0,
				"1" => VirtualKey.Number1,
				"2" => VirtualKey.Number2,
				"3" => VirtualKey.Number3,
				"4" => VirtualKey.Number4,
				"5" => VirtualKey.Number5,
				"6" => VirtualKey.Number6,
				"7" => VirtualKey.Number7,
				"8" => VirtualKey.Number8,
				"9" => VirtualKey.Number9,
				"Pad0" => VirtualKey.NumberPad0,
				"Pad1" => VirtualKey.NumberPad1,
				"Pad2" => VirtualKey.NumberPad2,
				"Pad3" => VirtualKey.NumberPad3,
				"Pad4" => VirtualKey.NumberPad4,
				"Pad5" => VirtualKey.NumberPad5,
				"Pad6" => VirtualKey.NumberPad6,
				"Pad7" => VirtualKey.NumberPad7,
				"Pad8" => VirtualKey.NumberPad8,
				"Pad9" => VirtualKey.NumberPad9,
				_ => Enum.TryParse(part, true, out VirtualKey key) ? key : VirtualKey.None,
			};
		}

		public override string ToString()
		{
			StringBuilder builder = new();
			if (Modifiers.HasFlag(VirtualKeyModifiers.Menu))
				builder.Append("Alt+");
			if (Modifiers.HasFlag(VirtualKeyModifiers.Control))
				builder.Append("Ctrl+");
			if (Modifiers.HasFlag(VirtualKeyModifiers.Shift))
				builder.Append("Shift+");
			if (Modifiers.HasFlag(VirtualKeyModifiers.Windows))
				builder.Append("Win+");
			builder.Append(ToString(Key));
			return builder.ToString();

			static string ToString(VirtualKey key) => key switch
			{
				VirtualKey.Number0 => "0",
				VirtualKey.Number1 => "1",
				VirtualKey.Number2 => "2",
				VirtualKey.Number3 => "3",
				VirtualKey.Number4 => "4",
				VirtualKey.Number5 => "5",
				VirtualKey.Number6 => "6",
				VirtualKey.Number7 => "7",
				VirtualKey.Number8 => "8",
				VirtualKey.Number9 => "9",
				VirtualKey.NumberPad0 => "Pad0",
				VirtualKey.NumberPad1 => "Pad1",
				VirtualKey.NumberPad2 => "Pad2",
				VirtualKey.NumberPad3 => "Pad3",
				VirtualKey.NumberPad4 => "Pad4",
				VirtualKey.NumberPad5 => "Pad5",
				VirtualKey.NumberPad6 => "Pad6",
				VirtualKey.NumberPad7 => "Pad7",
				VirtualKey.NumberPad8 => "Pad8",
				VirtualKey.NumberPad9 => "Pad9",
				_ => key.ToString(),
			};
		}

		public override int GetHashCode() => (Key, Modifiers).GetHashCode();
		public override bool Equals(object? other) => other is ShortKey shortKey && Equals(shortKey);
		public bool Equals(ShortKey other) => (other.Key, other.Modifiers).Equals((Key, Modifiers));
	}
}
