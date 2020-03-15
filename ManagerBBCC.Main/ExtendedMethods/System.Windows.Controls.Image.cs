using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace ManagerBBCC.Main.ExtendedMethods
{
    public static partial class ExtendedMethods
    {
        public static bool SetBitmap(this Image image, System.Drawing.Bitmap bitmap)
        {
            try
            {
                using (MemoryStream streamToken = new MemoryStream())
                {
                    bitmap.Save(streamToken, bitmap.RawFormat);
                    streamToken.Position = 0;

                    BitmapImage bitmapImageToken = new BitmapImage();
                    bitmapImageToken.BeginInit();
                    bitmapImageToken.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImageToken.StreamSource = streamToken;
                    bitmapImageToken.EndInit();

                    //ImageBehavior.SetAnimatedSource(image, bitmapImageToken); // for WpfAnimatedGif package
                    image.Source = bitmapImageToken; // Generally
                    bitmapImageToken.Freeze();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool SetBitmap(this Image image, string path)
        {
            try
            {
                BitmapImage bitmapImageToken = new BitmapImage();
                bitmapImageToken.BeginInit();
                bitmapImageToken.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImageToken.UriSource = new Uri(path);
                bitmapImageToken.EndInit();

                ImageBehavior.SetAnimatedSource(image, bitmapImageToken); // for WpfAnimatedGif package
                //image.Source = bitmapImageToken; // Generally
                bitmapImageToken.Freeze();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
