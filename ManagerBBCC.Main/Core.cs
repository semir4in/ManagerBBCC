using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

using ManagerBBCC.Main.Data;
using ManagerBBCC.Main.Functions;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        public static void Initialize()
        {
            Core.BeginInitialization();
        }

        public static void Reset()
        {
            (Core._setting = new SettingData()).Save();

            Process.Start(Application.ResourceAssembly.Location);
            Environment.Exit(0);
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
