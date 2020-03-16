using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace ManagerBBCC.Main.ExtendedMethods
{
    public static partial class ExtendedMethods
    {
        public static List<T> RemoveRange<T>(this List<T> old, List<T> list)
        {
            List<T> @new = new List<T>(old);

            foreach (T token in list)
            {
                @new.Remove(token);
            }

            return @new;
        }
    }
}
