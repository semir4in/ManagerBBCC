using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

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
            if (File.Exists(Config.SettingFilePath))
            {
                if (FileCheck.IsAvailable(Config.SettingFilePath))
                {
                    File.Delete(Config.SettingFilePath);

                    Process.Start(Application.ResourceAssembly.Location);
                    Environment.Exit(0);
                }
                else
                {

                }
            }
            else
            {

            }
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
