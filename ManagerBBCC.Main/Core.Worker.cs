using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        public static BackgroundWorker InitialWorker { get; set; }
        public static BackgroundWorker ScanWorker { get; set; }
        public static BackgroundWorker WriteWorker { get; set; }
        public static BackgroundWorker DownloadWorker { get; set; }

        public static void BeginInitialization()
        {
            Core.InitialWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
            };
            Core.InitialWorker.DoWork += InitialWorker_DoWork;
            Core.InitialWorker.RunWorkerCompleted += InitialWorker_RunWorkerCompleted;
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
            Core.InitializeVariables();

            Core.Invoke(delegate
            {
                Core.MainWindow.InitialStatus = "설정 불러오기...";
            });
            Thread.Sleep(Config.ThreadSleepTimeout);
            Core.CheckSetting();

            Core.Invoke(delegate
            {
                Core.MainWindow.InitialStatus = "엔트리 불러오기...";
            });
            Thread.Sleep(Config.ThreadSleepTimeout);
            if (Core.CheckEntries(false))
            {
                Core.UpdateMeta();
                
                Core.Invoke(delegate
                {
                    Core.MainWindow.InitialStatus = "엔트리 동기화...";
                });
                Thread.Sleep(Config.ThreadSleepTimeout);
                Core.SyncEntries();
            }

            Core.Invoke(delegate
            {
                Core.MainWindow.InitialStatus = "UI 확인...";
            });
            Thread.Sleep(Config.ThreadSleepTimeout);
            Core.Invoke(delegate
            {
                Core.CheckControls();
            });
            Thread.Sleep(Config.ThreadSleepTimeout);

            Core.Invoke(delegate
            {
                Core.MainWindow.InitialStatus = "준비!";
            });
            Thread.Sleep(Config.ThreadLongSleepTimeout);
            Core.Invoke(delegate
            {
                Core.MainWindow.IsInitialProgress = false;
            });
        }

        private static void InitialWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
#if DEBUG
            if (Core.Setting.OpenDebugWhenStartup)
            {
                Core.DebugWindow.Show();
            }
#endif

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
    }
}
