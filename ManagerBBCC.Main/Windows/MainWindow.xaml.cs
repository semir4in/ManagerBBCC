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

        private async void BaseWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            MessageDialogResult result = await this.ShowMessageAsync("종료", "BBCC 매니저를 종료할까요?",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings()
                {
                    AffirmativeButtonText = "종료",
                    NegativeButtonText = "취소",
                });

            if (result == MessageDialogResult.Affirmative)
            {
                Core.Exit();
            }
        }

        #region Layout

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = true;
        }

        #endregion

        private void LoadBBCCRootButton_Click(object sender, RoutedEventArgs e) => this.LoadBBCCRoot();
        private void OpenBBCCRootButton_Click(object sender, RoutedEventArgs e) => this.OpenBBCCRoot();

        private void RefreshEntryButton_Click(object sender, RoutedEventArgs e)
        {
            Core.CheckTagPoolFromEntry();
            Core.DisplayTags();
            Core.DisplayEntries();
        }
        private void ClearEntryButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Meta.Entries.Clear();
            Core.Meta.Filtered.Clear();
            Core.Meta.TagPool.Clear();

            Core.DisplayEntries();
            Core.DisplayTags();
        }

        private void InstructionButton_Click(object sender, RoutedEventArgs e)
        {

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
                + Environment.NewLine + "Icons made by Freepik (flaticon.com/authors/freepik) from Flaticon (flaticon.com)";

            var result = await this.ShowMessageAsync("ManagerBBCC 정보", detail, MessageDialogStyle.Affirmative, new MetroDialogSettings()
            {
                AffirmativeButtonText = "확인",
            });
            if (result == MessageDialogResult.Affirmative)
            {
                
            }
        }

        #region Advanced

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

        #endregion





        private void TagListButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void DeleteSelectedEntryButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            //if (this.EntryDataGrid.SelectedItems.Count == 0)
            //{
            //    return;
            //}
            //else
            //{
            //    string joinedName = string.Join(", ", this.EntryDataGrid.SelectedItems.Cast<Entry>().ToList().ConvertAll(x => x.Name));

            //    MessageDialogResult result = await this.ShowMessageAsync("엔트리 삭제", $"선택하신 엔트리는\n'{joinedName}'\n입니다. 정말 삭제할까요?",
            //        MessageDialogStyle.AffirmativeAndNegative,
            //        new MetroDialogSettings()
            //        {
            //            AffirmativeButtonText = "삭제",
            //            NegativeButtonText = "취소",
            //        });

            //    if (result != MessageDialogResult.Affirmative)
            //    {
            //        return;
            //    }
            //}

            //foreach (Entry deadmanEntry in this.EntryDataGrid.SelectedItems)
            //{
            //    this.DeleteEntry(deadmanEntry);
            //}

            //Core.Meta.Active.Clear();
            //Core.DisplayEntries();
            //Core.DisplayTags();
        }

        private void ClearEntrySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            //this.EntryDataGrid.SelectedIndex = -1;
        }

        private void TagListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                this.TagScrollViewer.LineDown();
            }
            else
            {
                this.TagScrollViewer.LineUp();
            }
        }

        private void TagListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                foreach (TagFilter added in e.AddedItems)
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
            }

            if (e.RemovedItems.Count > 0)
            {
                foreach (TagFilter removed in e.RemovedItems)
                {
                    if (removed.Tag == Config.NoTag)
                    {
                        Core.Meta.Entries.FindAll(x => x.Tags.Count == 0)
                            .ForEach(x => Core.Meta.Filtered.Remove(x));
                    }
                    else
                    {
                        Core.Meta.Entries.FindAll(x => x.Tags.Contains(removed.Tag))
                            .ForEach(x => Core.Meta.Filtered.Remove(x));
                    }
                }
            }

            var items = (sender as ListView).SelectedItems;
            if (items.Count > 0)
            {
                if (items.Count == 1)
                {
                    TagFilter filter = items[0] as TagFilter;
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
            Core.Meta.CheckPoolCount();

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

        private void DeleteEntry(Entry entry)
        {
            Core.Meta.Entries.Remove(entry);
            //File.Delete(entry.LocalPath);
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

        private async void DeleteTagFromEntryButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            #region Hint for ListViewDialog
            //if (this.EntryDataGrid.SelectedIndex == -1)
            //{
            //    Core.BeepSound();
            //    return;
            //}


            //// Input: string[] 'tag intersection of the selected entries'
            //// Dialog: Affirmative and Nagative, ListBox (Input Container)
            //// Output: string[] 'tags to delete'
            ////await this.ShowMetroDialogAsync(Core.BlackListDialog);

            //// Get intersections
            //List<string> intersection = this.EntryDataGrid.SelectedItems.Cast<Entry>().ToList()
            //    .ConvertAll(entry => entry.Tags)
            //    .Aggregate((tagsA, tagsB) => (tagsB.Intersect(tagsA).ToList()));

            //// 대충 여기에 값을 대입하고
            ////Core.BlackListDialog.SetBlackList(intersection);

            //IDialogManager dm;

            //var vm = new BlackListDialogViewModel();
            //vm.Title = "으아아아아아ㅏ";
            //vm.Message = "게가틍거어어어어ㅓㅓㅓㅓㅓ";

            //var result = 

            //// 여기서 보여주고
            ////List<string> blacklist = await this.ShowMetroDialogAsync(Core.BlackListDialog);

            //// 이 담부터 삭제할 태그들을 조지는걸로


            //// ~편안
            #endregion

            //if (this.EntryDataGrid.SelectedIndex == -1)
            //{
            //    Core.BeepSound();
            //    return;
            //}

            // Input: string[] 'tag intersection of the selected entries'
            // Dialog: Affirmative and Nagative, ListBox (Input Container)
            // Output: string[] 'tags to delete'
            //await this.ShowMetroDialogAsync(Core.BlackListDialog);

            // Get intersections
            //List<string> intersection = this.EntryDataGrid.SelectedItems.Cast<Entry>().ToList()
            //    .ConvertAll(entry => entry.Tags)
            //    .Aggregate((tagsA, tagsB) => (tagsB.Intersect(tagsA).ToList()));

            //if (intersection.Count == 0)
            //{
            //    var result = await this.ShowMessageAsync("태그없음", "선택한 엔트리에 (공통된) 태그가 없습니다. 다시 선택해주세요.", MessageDialogStyle.Affirmative, new MetroDialogSettings()
            //    {
            //        AffirmativeButtonText = "확인",
            //    });

            //    if (result == MessageDialogResult.Affirmative)
            //    {
            //        Core.BeepSound();
            //        return;
            //    }
            //}

            //this.IntersectionListView.ItemsSource = null;
            //this.IntersectionListView.ItemsSource = intersection;

            //this.BlackListViewDialogGrid.Visibility = Visibility.Visible;

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

            Core.CheckTagPoolFromEntry();

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

        #region Entry

        private void EntryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.DeleteSelectedEntryButton.IsEnabled = (this.EntryDataGrid.SelectedItems.Count > 0);

            //if (this.EntryDataGrid.SelectedItems.Count == 0)
            //{
            //    this.IntersectionListView.ItemsSource = null;
            //    return;
            //}

            //this.EntryNameEditBox.IsEnabled 
            //    = this.EntryKeywordEditBox.IsEnabled 
            //    = this.EntryTagEditBox.IsEnabled 
            //    = (this.EntryDataGrid.SelectedItems.Count == 1);

            //List<Entry> selectedEntries = this.EntryDataGrid.SelectedItems.Cast<Entry>().ToList();

            //if (selectedEntries.Count == 1)
            //{
            //    Entry selectedEntry = selectedEntries.Single();

            //    this.EntryNameEditBox.Text = $"{Path.GetFileNameWithoutExtension(selectedEntry.Name)}";
            //    this.EntryKeywordEditBox.Text = $"{selectedEntry.JoinedKeyword}";
            //    this.EntryTagEditBox.Text = $"{selectedEntry.JoinedTag}";
            //}
            //else
            //{
            //    this.EntryNameEditBox.Text
            //        = this.EntryKeywordEditBox.Text
            //        = this.EntryTagEditBox.Text = "";
            //}

            

            //List<string> intersection = selectedEntries
            //    .ConvertAll(entry => entry.Tags)
            //    .Aggregate((tagsA, tagsB) => (tagsB.Intersect(tagsA).ToList()));

            //this.IntersectionListView.ItemsSource = null;
            //this.IntersectionListView.ItemsSource = intersection;
        }

        private void EntryDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.F2)
            //{
            //    if (this.EntryDataGrid.SelectedItems.Count == 1)
            //    {
            //        //string a = "";
            //    }
            //}
        }

        private void EntryEditButton_Click(object sender, RoutedEventArgs e)
        {
            //if (this.EntryDataGrid.SelectedItems.Count == 1)
            //{
            //    //int selectedIndex = this.EntryDataGrid.SelectedIndex;

            //    //Entry selectedEntry = this.EntryDataGrid.SelectedItems.Cast<Entry>().Single();

            //    //selectedEntry.Name = $"{this.EntryNameEditBox.Text}{Path.GetExtension(selectedEntry.Name)}";
            //    //selectedEntry.Keywords = this.EntryKeywordEditBox.Text
            //    //    .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList()
            //    //    .ConvertAll(x => x.Trim()).ToList();
            //    //selectedEntry.Tags = this.EntryTagEditBox.Text
            //    //    .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList()
            //    //    .ConvertAll(x => x.Trim()).ToList();

            //    //Core.DisplayEntries();

            //    //this.EntryDataGrid.SelectedIndex = selectedIndex;
            //}
            //else
            //{
            //    Core.BeepSound();
            //}
        }

        #endregion



        

        private void BaseWindow_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                // File or Directory
                string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string tokenPath in paths)
                {
                    if (Config.ImageExtensions.Contains("*"+ Path.GetExtension(tokenPath)))
                    {
                        File.Copy(tokenPath, Path.Combine(Core.ImageFolderPath, Path.GetFileName(tokenPath)));
                    }
                }

                //Core.ImportEntryFromImage();
                //Core.ImportEntryFromProperty();

                Core.CheckTagPoolFromEntry();
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

        private void ColumnImage_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            //string localPath = image.Source.ToString();

            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.CacheOption = BitmapCacheOption.OnDemand;
            //bitmap.UriSource = new Uri(localPath);
            //bitmap.EndInit();
            //bitmap.Freeze();

            ImageSource source = image.Source;
            source.Freeze();
            image.Source = null;
            //image.Source = null;            


            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void ColumnImage_AnimationLoaded(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(1000);

            string a = "";

            Image image = sender as Image;
            var b = WpfAnimatedGif.ImageBehavior.GetAnimatedSource(image);
            b.Freeze();
        }


        private string tempImagePath = "";
        private void AddTestEntryButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog OpenImageDialog = new CommonOpenFileDialog()
            {
                Title = "테스트 이미지 선택",
                IsFolderPicker = false,
            };
            OpenImageDialog.Filters.Add(new CommonFileDialogFilter("지원 이미지 파일", "*.png;*.jpg;*.gif"));

            CommonFileDialogResult result = OpenImageDialog.ShowDialog();

            if (result == CommonFileDialogResult.Cancel || !File.Exists(OpenImageDialog.FileName))
            {
                Core.BeepSound();
                return;
            }

            //this.TestImage.SetBitmap(OpenImageDialog.FileName);
            this.tempImagePath = OpenImageDialog.FileName;
        }

        private void RemoveTestEntryButton_Click(object sender, RoutedEventArgs e)
        {
            //this.TestImage.Source = null;

            File.Delete(this.tempImagePath);
        }
        
        private void BaseWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.EditorWindow.Hide();
        }
    }
}
