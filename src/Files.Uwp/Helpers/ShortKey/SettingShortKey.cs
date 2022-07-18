using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace Files.Uwp.Helpers
{
    [MarkupExtensionReturnType(ReturnType = KeyboardAccelerator)]
    public sealed class SettingShortKey : MarkupExtension
    {
        private static ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        public string Name
        {
            get; set;
        }

        public bool IsEnabled { get; }

        protected override object ProvideValue()
        {
            return resourceLoader.GetString(this.Name);
        }
    }

}
