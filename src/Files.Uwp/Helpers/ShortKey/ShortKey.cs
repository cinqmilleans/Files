﻿using System;
using System.Text;
using Windows.System;
using Windows.UI.Xaml.Input;

namespace Files.Uwp.Helpers
{
    public struct ShortKey : IEquatable<ShortKey>
    {
        public static ShortKey None { get; } = new ShortKey(VirtualKey.None);

        public bool IsEmpty => Key is VirtualKey.None && Modifier is VirtualKeyModifiers.None;
        public bool IsValid => Key is not VirtualKey.None;

        public VirtualKey Key { get; }
        public VirtualKeyModifiers Modifier { get; }

        public ShortKey(VirtualKey key) : this(key, VirtualKeyModifiers.None)
        {
        }
        public ShortKey(VirtualKey key, VirtualKeyModifiers modifier)
        {
            Key = VirtualKey.None;
            Modifier = VirtualKeyModifiers.None;

            if (key is VirtualKey.None)
            {
                throw new ArgumentException(nameof(key), "The key cannot be None.");
            }
            if (isModifier(key))
            {
                throw new ArgumentException(nameof(key), "The key cannot be a modifier.");
            }
            if (modifier.HasFlag(VirtualKeyModifiers.Windows))
            {
                throw new ArgumentException(nameof(modifier), "Windows is not a valid modifier.");
            }

            Key = key;
            Modifier = modifier;

            static bool isModifier(VirtualKey key)
                => key is VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu
                or VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl
                or VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift
                or VirtualKey.LeftWindows or VirtualKey.RightWindows;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            if (Modifier.HasFlag(VirtualKeyModifiers.Menu))
            {
                builder.Append("Menu+");
            }
            if (Modifier.HasFlag(VirtualKeyModifiers.Control))
            {
                builder.Append("Ctrl+");
            }
            if (Modifier.HasFlag(VirtualKeyModifiers.Shift))
            {
                builder.Append("Shift+");
            }
            builder.Append(ToString(Key));
            return builder.ToString();
        }

        public override int GetHashCode() => (Key, Modifier).GetHashCode();
        public override bool Equals(object other) => other is ShortKey shortKey && Equals(shortKey);
        public bool Equals(ShortKey other) => (other.Key, other.Modifier).Equals((Key, Modifier));

        public KeyboardAccelerator ToKeyboardAccelerator(bool isEnabled = true)
            => new() { Key = Key, Modifiers = Modifier, IsEnabled = isEnabled };

        private static string ToString(VirtualKey key) => key switch
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
}
