using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        public static void Initialize()
        {
            Core.BeginInitialization();
        }

        public static void Save()
        {
            Core.Setting.Save();
        }

        public static void Exit()
        {
            Core.Save();
            Application.Current.Shutdown();
        }
    }
}
