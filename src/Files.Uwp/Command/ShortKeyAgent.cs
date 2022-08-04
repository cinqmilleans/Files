using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Files.Uwp.Command
{
    public class ShortKeyAgent
    {
        private VirtualKeyModifiers modifier = VirtualKeyModifiers.None;

        public void OnKeyDown(VirtualKey key)
        {
            if (IsExcludedKey(key))
            {
                return;
            }

            var shortKey = new ShortKey(key, modifier = GetCurrentModifier());

            if (shortKey == new ShortKey(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift))
            {

            }
        }

        private static bool IsExcludedKey(VirtualKey key)
            => key is VirtualKey.None or VirtualKey.LeftWindows or VirtualKey.RightWindows
            or VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu
            or VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl
            or VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift;

        private static VirtualKeyModifiers ToModifier(VirtualKey key) => key switch
        {
            VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu => VirtualKeyModifiers.Menu,
            VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl => VirtualKeyModifiers.Control,
            VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift => VirtualKeyModifiers.Shift,
            _ => VirtualKeyModifiers.None;
        };

        private static VirtualKeyModifiers GetCurrentModifier()
        {
            return GetActiveModifier(VirtualKey.Menu, VirtualKeyModifiers.Menu)
                | GetActiveModifier(VirtualKey.Control, VirtualKeyModifiers.Control)
                | GetActiveModifier(VirtualKey.Shift, VirtualKeyModifiers.Shift);

            static VirtualKeyModifiers GetActiveModifier(VirtualKey key, VirtualKeyModifiers modifier)
                => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down) ? modifier : VirtualKeyModifiers.None;
        }
    }
}
