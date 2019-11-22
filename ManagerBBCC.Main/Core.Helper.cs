using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

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

        public static BackgroundWorker InitialWorker { get; set; }
        public static BackgroundWorker ScanWorker { get; set; }
        public static BackgroundWorker WriteWorker { get; set; }
        public static BackgroundWorker DownloadWorker { get; set; }

        public static bool IsBBCCFolderValid => Directory.Exists(Core.Setting.BBCCPath);
        public static bool IsImageFolderValid => Directory.Exists(Core.ImageFolderPath);
        public static bool IsDataFolderValid => Directory.Exists(Core.DataFolderPath);

        public static string ImageFolderPath => Path.Combine(Core.Setting.BBCCPath, Config.ImageFolderSubPath);
        public static string DataFolderPath => Path.Combine(Core.Setting.BBCCPath, Config.DataFolderSubPath);

        public static void BeginInitialization()
        {
            Core.InitialWorker = new BackgroundWorker();
            Core.InitialWorker.DoWork += InitialWorker_DoWork;
            Core.InitialWorker.RunWorkerAsync();
        }

        private static void InitialWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Core.Invoke(delegate
            {
                Core.MainWindow.Show();
                Core.MainWindow.InitialStatus = "초기화...";
            });
            Thread.Sleep(Config.ThreadLongSleepTimeout);

            // Check
            Core.CheckSetting();


            if (Core.IsBBCCFolderValid)
            {
                Core.Invoke(delegate
                {
                    Core.MainWindow.InitialStatus = "BBCC 데이터 불러오기...";
                });
                Thread.Sleep(Config.ThreadSleepTimeout);

                Core.Invoke(delegate
                {
                    Core.ImportEntryFromImage();
                    Core.ImportEntryFromProperty();
                    Core.CheckTagPoolFromEntry();

                    Core.DisplayEntries();
                    Core.DisplayTags();
                });

            }

            Core.Invoke(delegate
            {
                Core.MainWindow.Show();
                Core.MainWindow.InitialStatus = "UI 확인...";
            });
            Thread.Sleep(Config.ThreadSleepTimeout);

            Core.Invoke(delegate
            {
                Core.CheckControls();
            });



            Core.Invoke(delegate
            {
                Core.MainWindow.InitialStatus = "준비!";
            });
            Thread.Sleep(Config.ThreadLongSleepTimeout);

            Core.Invoke(delegate
            {
                Core.MainWindow.IsInitialProgress = false;

                
#if DEBUG
                if (Core.Setting.OpenDebug)
                {
                    Core.DebugWindow.Show();
                }
#endif
            });
            

            Core.InitialWorker.Dispose();
        }



        public static void BeginScanning()
        {
            Core.ScanWorker = new BackgroundWorker();
            Core.ScanWorker.DoWork += ScanWorker_DoWork;
            Core.ScanWorker.RunWorkerAsync();
        }

        private static void ScanWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            Core.ScanWorker.Dispose();
        }



        public static void BeginWriting()
        {
            Core.WriteWorker = new BackgroundWorker();
            Core.WriteWorker.DoWork += WriteWorker_DoWork;
            Core.WriteWorker.RunWorkerAsync();
        }

        private static void WriteWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            Core.WriteWorker.Dispose();
        }

        public static void Invoke(Action callback) => Application.Current.Dispatcher.Invoke(callback);
    }
}
