using Microsoft.WindowsAPICodePack.Dialogs;
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
    public partial class MainWindow : BaseWindow
    {
        public MainWindow() => this.Initialize();
        private void BaseWindow_Closing(object sender, CancelEventArgs e) => this.AskCloseAsync(sender, e);

        private void MenuButton_Click(object sender, RoutedEventArgs e) => (sender as Button).ContextMenu.IsOpen = true;
        private void LoadBBCCRootButton_Click(object sender, RoutedEventArgs e) => this.LoadBBCCRoot();
        private void OpenBBCCRootButton_Click(object sender, RoutedEventArgs e) => this.OpenBBCCRoot();

        //// Menu event
        private void InstructionButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.InstructionURL);
        }
        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.DiscordUrl);
            Core.SetStatus($"[디스코드] lab.r4iny 서버로 이동합니다. 집요정: @SemiR4in#0001");
        }


        private async void InfoButton_ClickAsync(object sender, RoutedEventArgs e)
        {
                        string detail = "- 정보"
                + Environment.NewLine + "이 프로그램은 'BridgeBBCC'의 세팅을 돕기 위해 개발된 보조프로그램입니다."
                + Environment.NewLine
                + Environment.NewLine + "- Acknowledgement"
                + Environment.NewLine + "Icons made by Freepik (flaticon.com/authors/freepik) from Flaticon (flaticon.com)"
                + Environment.NewLine
                + Environment.NewLine + "- License"
                + Environment.NewLine + "MIT";

            var result = await this.ShowMessageAsync("ManagerBBCC 정보", detail, MessageDialogStyle.Affirmative, new MetroDialogSettings()
            {
                DialogMessageFontSize = 12,

                AffirmativeButtonText = "확인",
            });
            if (result == MessageDialogResult.Affirmative)
            {
                
            }
        }

        private void ResetSettingButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.IsShiftKeyDown)
            {
                Core.Reset();
            }
            else
            {
                Core.SetStatus($"[초기화 - 실패] 초기화 메뉴를 '시프트+클릭'해 주세요");
                Core.BeepSound();
            }
        }

        private void OpenAppDataLocalFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Config.LocalAppDataPath))
            {
                Process.Start(Config.LocalAppDataPath);
            }
            else
            {

            }
        }

        private void VisitProjectGithubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Config.GithubProjectUrl);
        }



        private void ClearEntrySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            this.EntryListView.SelectedIndex = -1;
        }

        private void TagListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) this.TagScrollViewer.LineDown();
            else this.TagScrollViewer.LineUp();
        }

        private void TagListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<TagFilter> selected = this.TagListView.SelectedItems.Cast<TagFilter>().ToList();
            
            Core.Meta.Filtered.Clear();
            if (selected.Count > 0)
            {
                foreach (TagFilter added in selected)
                {
                    if (added.Tag == Config.NoTag)
                    {
                        Core.Meta.Filtered.AddRange(Core.Meta.Entries.FindAll(x => x.Tags.Count == 0));
                    }
                    else
                    {
                        Core.Meta.Filtered.AddRange(Core.Meta.Entries.FindAll(x => x.Tags.Contains(added.Tag)));
                    }
                }

                if (selected.Count == 1)
                {
                    TagFilter filter = selected[0] as TagFilter;
                    this.EditSelectedTagButton.IsEnabled = (filter.Tag != Config.NoTag);
                    this.DeleteSelectedTagButton.IsEnabled = (filter.Tag != Config.NoTag);
                }
                else
                {
                    this.EditSelectedTagButton.IsEnabled = false;
                    this.DeleteSelectedTagButton.IsEnabled = false;
                }
                this.ClearTagSelectionButton.IsEnabled = true;
            }
            else
            {
                this.EditSelectedTagButton.IsEnabled = false;
                this.DeleteSelectedTagButton.IsEnabled = false;
                this.ClearTagSelectionButton.IsEnabled = false;
            }

            Core.DisplayEntries();
        }

        private void TagListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.TagListView.SelectedIndex = -1;
            }
        }

        private void EditTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TagFilter oldTagFilter = (sender as MenuItem).DataContext as TagFilter;
            this.EditTagAsync(oldTagFilter.Tag);
        }

        private void DeleteTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TagFilter deadmanFilter = (sender as MenuItem).DataContext as TagFilter;
            this.DeleteTag(deadmanFilter.Tag);

            Core.Meta.Filtered.Clear();
            Core.DisplayEntries();
            Core.DisplayTags();
        }

        private async void EditTagAsync(string tag)
        {
            if (tag == Config.NoTag)
            {
                Core.BeepSound();
                return;
            }

            string newTag = await this.ShowInputAsync($"'{tag}' 태그 수정", "새로운 태그를 입력하세요", new MetroDialogSettings()
            {
                DefaultText = tag,
                AffirmativeButtonText = "수정",
                NegativeButtonText = "취소",
            });

            if (newTag == null || newTag.Length == 0)
            {
                Core.BeepSound();
                return;
            }

            if (newTag == Config.NoTag || newTag == tag)
            {
                Core.BeepSound();
                return;
            }

            int overlappedIndex = Core.Meta.TagPool.FindIndex(x => x.Tag == newTag);
            if (overlappedIndex > -1)
            {
                Core.BeepSound();
                MessageDialogResult result = await this.ShowMessageAsync(
                    "태그 중복",
                    $"입력하신 '{newTag}' 태그는 기존에 있는 태그입니다.\n'{tag}' 태그를 '{newTag}' 태그와 병합할까요?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings()
                    {
                        DialogMessageFontSize = 12,

                        AffirmativeButtonText = "병합",
                        NegativeButtonText = "취소",
                    });

                if (result == MessageDialogResult.Affirmative)
                {
                    foreach (Entry entry in Core.Meta.Entries)
                    {
                        if (entry.Tags.Remove(tag))
                        {
                            if (!entry.Tags.Contains(newTag))
                            {
                                entry.Tags.Add(newTag);
                                entry.Tags.Sort();
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                foreach (Entry entry in Core.Meta.Entries)
                {
                    if (entry.Tags.Remove(tag))
                    {
                        entry.Tags.Add(newTag);
                        entry.Tags.Sort();
                    }
                }
            }

            Core.Meta.Filtered.Clear();
            Core.Meta.CheckTagPool();

            Core.DisplayEntries();
            Core.DisplayTags();
        }

        private void DeleteTag(string tag)
        {
            if (tag == Config.NoTag)
            {
                Core.BeepSound();
                return;
            }

            foreach (Entry entry in Core.Meta.Entries)
            {
                entry.Tags.Remove(tag);
            }

            Core.Meta.TagPool.RemoveAll(x => x.Tag == tag);
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Core.Exit();
        }

        private async void AddTagToEntryButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            //if (this.EntryDataGrid.SelectedIndex == -1)
            //{
            //    Core.BeepSound();
            //    return;
            //}

            string rawNewTag = await this.ShowInputAsync($"엔트리에 태그추가", "새로운 태그를 입력하세요 (쉼표로 구분)", new MetroDialogSettings()
            {
                AffirmativeButtonText = "추가",
                NegativeButtonText = "취소",
            });

            if (rawNewTag == null || rawNewTag.Length == 0)
            {
                Core.BeepSound();
                return;
            }

            string[] NewTags = rawNewTag.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tokenTag in NewTags)
            {
                string newTag = tokenTag.Trim();

                //foreach (Entry entry in this.EntryDataGrid.SelectedItems)
                //{
                //    if (!entry.Tags.Contains(newTag))
                //    {
                //        entry.Tags.Add(newTag);
                //        entry.Tags.Sort();
                //    }
                //}

                //if (!Core.Meta.Pool.Contains(newTag))
                //{
                //    Core.Meta.Pool.Add(newTag);
                //    Core.Meta.Pool.Sort();
                //}
            }

            Core.DisplayEntries();
            Core.DisplayTags();
        }

        

        private void DialogAffirmativeButton_Click(object sender, RoutedEventArgs e)
        {
            //List<string> blackList = this.IntersectionListView.SelectedItems.Cast<string>().ToList();

            //if (blackList.Count == 0)
            //{
            //    Core.BeepSound();
            //    return;
            //}

            ////this.BlackListViewDialogGrid.Visibility = Visibility.Collapsed;

            //foreach (Entry tokenEntry in this.EntryDataGrid.SelectedItems)
            //{
            //    blackList.ForEach(x => tokenEntry.Tags.Remove(x));
            //}

            Core.UpdateMeta();

            Core.DisplayEntries();
            Core.DisplayTags();
        }

        private void DialogNegativeButton_Click(object sender, RoutedEventArgs e)
        {
            //this.BlackListViewDialogGrid.Visibility = Visibility.Collapsed;
        }


        private void EditSelectedTagButton_Click(object sender, RoutedEventArgs e)
        {
            TagFilter oldTagFilter = this.TagListView.SelectedItem as TagFilter;
            this.EditTagAsync(oldTagFilter.Tag);
        }

        private async void DeleteSelectedTagButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (this.TagListView.SelectedItems.Count == 0)
            {
                return;
            }
            else if (this.TagListView.SelectedItems.Count > 1)
            {
                string joinedTag = string.Join(", ", this.TagListView.SelectedItems.Cast<TagFilter>().ToList().ConvertAll(x => x.Tag));

                MessageDialogResult result = await this.ShowMessageAsync("태그 삭제", $"선택하신 태그는\n'{joinedTag}'\n태그입니다. 정말 삭제할까요?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings()
                    {
                        DialogMessageFontSize = 12,

                        AffirmativeButtonText = "삭제",
                        NegativeButtonText = "취소",
                    });

                if (result != MessageDialogResult.Affirmative)
                {
                    return;
                }
            }
            
            foreach (TagFilter deadmanFilter in this.TagListView.SelectedItems)
            {
                this.DeleteTag(deadmanFilter.Tag);
            }

            Core.Meta.Filtered.Clear();
            Core.DisplayEntries();
            Core.DisplayTags();
        }

        private void ClearTagSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            this.TagListView.SelectedIndex = -1;
        }



        private void BaseWindow_PreviewDrop(object sender, DragEventArgs e)
        {
            if (!Core.IsBBCCFolderValid)
            {
                Core.BeepSound();
                return;
            }

            if (!Core.IsImageFolderValid)
            {
                Directory.CreateDirectory(Core.ImageFolderPath);
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                // File or Directory
                string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string oldPath in paths)
                {
                    if (Config.ImageExtensions.Contains("*"+ Path.GetExtension(oldPath)))
                    {
                        string newPath = Path.Combine(Core.ImageFolderPath, Path.GetFileName(oldPath));

                        if (oldPath == newPath) continue;
                        File.Copy(oldPath, newPath);

                        Core.ImportD(newPath);
                    }
                }

                Core.UpdateMeta();
                Core.DisplayTags();
                Core.DisplayEntries();
            }
            else
            {
                if (e.Data.GetData(DataFormats.UnicodeText, true) is string data)
                {
                    // String
                    
                }
                else
                {
                    // Not String
                    
                }
            }
        }


        private void BaseWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.EditorWindow.Hide();
        }

        private void DeleteSelectedEntryButton_ClickAsync(object sender, RoutedEventArgs e)
        {

        }

        private void BaseWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                if (Core.LastEntry != null)
                {
                    Core.EditorWindow.FromEntry(Core.LastEntry, Core.SelectedEntries);
                    Core.EditorWindow.Popup();
                }
            }
        }

        private void EntryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = (sender as ListView).SelectedItems.Cast<Entry>().ToList();
            if (items.Count > 0)
            {
                Core.LastEntry = items.First();
            }
            else
            {
                Core.LastEntry = null;
            }
        }

    }
}
