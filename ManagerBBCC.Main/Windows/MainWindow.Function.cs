﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using ManagerBBCC.Main.Classes;
using ManagerBBCC.Main.ExtendedMethods;
using ManagerBBCC.Main.Functions;

namespace ManagerBBCC.Main.Windows
{
    public partial class MainWindow
    {
        private async void LoadBBCCRoot()
        {
            // Do open warning
            if (Core.Setting.DoLoadBbccWarning)
            {
                string detail = "BBCC를 불러오기 전에 꼭 백업을 해주세요! 프로그램이 오동작하여 BBCC 편집이 제대로 되지 않을 수 있습니다. 손실된 데이터에 대해서는 개발자는 책임지지 않습니다.";

                var warningResult = await this.ShowMessageAsync("데이터 손실 주의", detail, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings()
                {
                    DialogMessageFontSize = 12,

                    AffirmativeButtonText = "돌아가기",
                    NegativeButtonText = "계속하기",
                    FirstAuxiliaryButtonText = " 계속, 이제 그만 볼래요 ",
                });

                if (warningResult == MessageDialogResult.Affirmative)
                {
                    return;
                }
                else if (warningResult == MessageDialogResult.Negative)
                {
                    Core.Setting.DoLoadBbccWarning = true;
                }
                else
                {
                    Core.Setting.DoLoadBbccWarning = false;
                }
            }

            // Open BBCC root
            CommonOpenFileDialog OpenBBCCRootDialog = new CommonOpenFileDialog()
            {
                Title = "BBCC 폴더 선택",
                IsFolderPicker = true,
            };

            CommonFileDialogResult rootResult = OpenBBCCRootDialog.ShowDialog();

            if (rootResult == CommonFileDialogResult.Cancel || !Directory.Exists(OpenBBCCRootDialog.FileName))
            {
                Core.SetStatus("[폴더 선택 - 취소]");
                Core.BeepSound();
                return;
            }

            string bbccRootPath = OpenBBCCRootDialog.FileName;
            Core.Setting.BbccPath = bbccRootPath;

            Core.CheckEntries(true);

            Core.UpdateMeta();
            Core.DisplayTags();
            Core.DisplayEntries();

            Core.CheckControls();
        }
    
        private void OpenBBCCRoot()
        {
            if (Core.IsBBCCFolderValid)
            {
                Process.Start(Core.Setting.BbccPath);
            }
            else
            {
                throw new InvalidOperationException("ERROR:: Core.OpenBBCCRoot() is broken");
            }
        }

        private async void AskCloseAsync(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            MessageDialogResult result = await this.ShowMessageAsync("종료", "정말 BBCC 매니저를 종료할까요?",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings()
                {
                    DialogMessageFontSize = 12,

                    AffirmativeButtonText = "종료",
                    NegativeButtonText = "취소",
                });

            if (result == MessageDialogResult.Affirmative)
            {
                Core.Exit();
            }
        }
    }
}
