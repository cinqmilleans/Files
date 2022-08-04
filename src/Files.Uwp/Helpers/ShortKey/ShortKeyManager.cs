using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.System;

namespace Files.Uwp.Helpers
{
    /*public class ShortKeyManager //: IShortKeyManager
    {
        private class ShortKeyAction : IShortKeyAction
        {
            public string Label => throw new System.NotImplementedException();

            public IReadOnlyCollection<ShortKey> ActiveShortKey
            {
                get => throw new System.NotImplementedException();
                set => throw new System.NotImplementedException();
            }

            public ShortKey DefaultShortKey => throw new System.NotImplementedException();

            public ICommand Command => throw new System.NotImplementedException();
        }


        /*public readonly IDictionary<ShortKeyCodes, ShortKey> ShortKeys = new Dictionary<ShortKeyCodes, ShortKey>();

        public ShortKey GetShortKey(ShortKeyCodes code)
        {
            if (UserShortKeys.ContainsKey(code))
            {
                return UserShortKeys[code];
            }
            return GetDefaultShortKey(code);
        }

        public void SetShortKey(ShortKeyCodes code, ShortKey key)
        {
            if (this[code] != key)
            {
                var defaultShortKey = GetDefaultShortKey(code);
                if (defaultShortKey == key)
                {
                    UserShortKeys.Remove(code);
                }
                else
                {
                    UserShortKeys[code] = key;
                }
            }
        }

        public ShortKey GetDefaultShortKey(ShortKeyCodes code) => code switch
        {
            ShortKeyCodes.ViewDetail => new(VirtualKey.Number1, VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control),
            ShortKeyCodes.ViewTiles => new(VirtualKey.Number2, VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control),
            ShortKeyCodes.ViewGridSmall => new(VirtualKey.Number3, VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control),
            ShortKeyCodes.ViewGridMedium => new(VirtualKey.Number4, VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control),
            ShortKeyCodes.ViewGridLarge => new(VirtualKey.Number5, VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control),
            ShortKeyCodes.ViewColumn => new(VirtualKey.Number6, VirtualKeyModifiers.Menu | VirtualKeyModifiers.Control),
            _ => ShortKey.None,
        };
    }*/
}
