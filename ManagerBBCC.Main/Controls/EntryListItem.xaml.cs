using ManagerBBCC.Main.Classes;
using ManagerBBCC.Main.ExtendedMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManagerBBCC.Main.Controls
{
    public partial class EntryListItem : Grid
    {
        public EntryListItem()
        {
            this.InitializeComponent();
        }

        #region [ EntryString Binding ]
        public static readonly DependencyProperty EntryStringProperty = DependencyProperty.Register(
            "EntryString",
            typeof(string),
            typeof(EntryListItem));

        public string EntryString
        {
            get => this.GetValue(EntryStringProperty) as string;
            set => this.SetValue(EntryStringProperty, value);
        }
        private void EntryStringBindingBox_TextChanged(object sender, TextChangedEventArgs e) => this.CheckBinding();
        #endregion

        public Entry Entry { get; set; }

        public void CheckBinding()
        {
            this.Entry = Entry.Parse<Entry>(this.EntryStringBindingBox.Text);

            // Col #0
            if (this.Entry.Keywords.Count > 0)
            {
                this.KeywordBlock.Text = string.Join(", ", this.Entry.Keywords);
            }
            else
            {
                this.KeywordBlock.Text = $"키워드없음{this.Entry.ID}";
            }

            // Col #1
            if (File.Exists(this.Entry.LocalPath))
            {
                this.ItemImage.SetBitmap(this.Entry.LocalPath);

                this.ItemNoImageIcon.Visibility = Visibility.Hidden;
                this.TooltipNoImageIcon.Visibility = Visibility.Hidden;
            }
            else
            {
                this.ItemNoImageIcon.Visibility = Visibility.Visible;
                this.TooltipNoImageIcon.Visibility = Visibility.Visible;
            }

            // Col #2
            if (this.Entry.Tags.Count > 0)
            {
                this.TagBlock.Text = string.Join(", ", this.Entry.Tags);
            }
            else
            {
                this.TagBlock.Text = "태그없음";
            }

            // Context menu
            //this.ContextMenuHeaderBlock.Text = $"{this.Entry.Name}";
        }

        private void EditEntryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Core.LastEntry = this.Entry;
            Core.EditorWindow.FromEntry(Core.LastEntry, Core.SelectedEntries);
            Core.EditorWindow.Popup();
        }

        private void DeleteEntryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Core.IsShiftKeyDown)
            {
                foreach (var tokenEntry in Core.SelectedEntries)
                {
                    Core.Meta.Entries.RemoveAll(x => x.ID == tokenEntry.ID);
                    Core.Meta.Filtered.RemoveAll(x => x.ID == tokenEntry.ID);

                    if (File.Exists(tokenEntry.LocalPath))
                    {
                        File.Delete(tokenEntry.LocalPath);
                    }
                }

                Core.UpdateMeta();
                
                Core.DisplayTags();
                Core.DisplayEntries();
            }
            else
            {
                Core.SetStatus($"[제거 - 실패] 제거메뉴를 '시프트+클릭'해 주세요");
                Core.BeepSound();
            }
        }

        private void EntryContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            List<Entry> selected = Core.SelectedEntries;

            if (selected.Count > 1)
            {
                this.ContextMenuHeaderBlock.Text = $"{this.Entry.Name} 외 {selected.Count-1}개";
            }
            else
            {
                this.ContextMenuHeaderBlock.Text = $"{this.Entry.Name}";
            }
        }
    }
}
