using Files.Backend.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Backend.Services
{
    public interface IIconService
    {
        Task<BitmapImage> GetImage(Icons icon);
    }
}
