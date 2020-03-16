using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using ManagerBBCC.Main.Classes;
using ManagerBBCC.Main.Data;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        private static MetaData _meta;
        public static MetaData Meta => Core._meta ?? (Core._meta = new MetaData());

        private static SettingData _setting;
        public static SettingData Setting => Core._setting ?? (Core._setting = SettingData.Load());

        public static bool IsBBCCFolderValid => Directory.Exists(Core.Setting.BbccPath);
        public static bool IsImageFolderValid => Directory.Exists(Core.ImageFolderPath);
        public static bool IsDataFolderValid => Directory.Exists(Core.DataFolderPath);

        public static string ImageFolderPath => Path.Combine(Core.Setting.BbccPath, Config.ImageFolderSubPath);
        public static string DataFolderPath => Path.Combine(Core.Setting.BbccPath, Config.DataFolderSubPath);

        
        public static void Invoke(Action callback) => Application.Current.Dispatcher.Invoke(callback, System.Windows.Threading.DispatcherPriority.Background);
    
        public static bool IsShiftKeyDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        public static Entry LastEntry { get; set; }
        public static List<Entry> SelectedEntries => Core.MainWindow.EntryListView.SelectedItems.Cast<Entry>().ToList();
    }
}
