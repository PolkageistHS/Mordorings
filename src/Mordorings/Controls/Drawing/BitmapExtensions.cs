using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Mordorings.Controls;

public static class BitmapExtensions
{
    public static BitmapSource ToBitmapSource(this Bitmap bitmap)
    {
        using var memory = new MemoryStream();
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = memory;
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }
}
