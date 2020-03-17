using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ManagerBBCC.Main.Classes;
using ManagerBBCC.Main.ExtendedMethods;

namespace ManagerBBCC.Main.Windows
{
    public partial class EditorWindow : BaseWindow
    {
        public EditorWindow()
        {
            this.InitializeComponent();
        }

        public Entry Current;
        public List<Entry> Overall;
        public List<string> TagIntersection;

        public void Popup()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;

            this.Left = Math.Max(point.X - 10, 0);
            this.Top = Math.Max(point.Y - 10, 0);

            this.Show();
        }

        public void FromEntry(Entry entry, List<Entry> all)
        {
            this.EditorNameBox.IsEnabled = (all.Count == 1);
            this.EditorNameBox.Text = $"{System.IO.Path.GetFileNameWithoutExtension(entry.Name)}";

            if (all.Count > 1)
            {
                this.EditorHeaderBlock.Text = $"편집 ({entry.Name} 외 {all.Count - 1}개)";
            }
            else if (all.Count == 1)
            {
                this.EditorHeaderBlock.Text = $"편집 ({entry.Name})";
            }
            else
            {
                Core.BeepSound();
                return;
            }

            this.EditorKeywordBox.IsEnabled = (all.Count == 1);
            this.EditorKeywordBox.Text = entry.JoinedKeyword;

            List<string> tagUnique = new List<string>();
            all.ConvertAll(x => x.Tags).ForEach(x => tagUnique.AddRange(x));
            tagUnique = tagUnique.Distinct().ToList();

            List<string> tagIntersection = all
                .ConvertAll(x => x.Tags)
                .Aggregate((a, b) => a.Intersect(b).ToList());

            List<string> tagRemain = tagUnique.Except(tagIntersection).ToList();
            if (tagRemain.Count > 0) tagIntersection.Insert(0, "*");
            
            this.EditorTagBox.Text = string.Join(", ", tagIntersection);

            this.Current = entry;
            this.Overall = new List<Entry>(all);
            this.TagIntersection = new List<string>(tagIntersection);
        }

        private void EditorRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.FromEntry(Core.LastEntry, Core.SelectedEntries);
        }

        private void EditorApplyButton_Click(object sender, RoutedEventArgs e) => this.ApplyEditor();

        private void ApplyEditor()
        {
            if (this.Overall.Count == 1)
            {
                string newName = $"{this.EditorNameBox.Text}{System.IO.Path.GetExtension(this.Overall[0].Name)}";
                if (this.Overall[0].Name != newName)
                {
                    this.Overall[0].Name = newName;

                    string newLocalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Overall[0].LocalPath), newName);
                    File.Move(this.Overall[0].LocalPath, newLocalPath);
                    this.Overall[0].LocalPath = newLocalPath;
                }

                this.Overall[0].Keywords = CommaSplit(this.EditorKeywordBox.Text);
                this.Overall[0].Tags = CommaSplit(this.EditorTagBox.Text);
            }
            else
            {
                List<string> newTags = CommaSplit(this.EditorTagBox.Text);
                if (newTags.Contains("*"))
                {
                    newTags.Remove("*");

                    foreach (Entry tokenEntry in this.Overall)
                    {
                        tokenEntry.Tags.RemoveRange(this.TagIntersection);
                        tokenEntry.Tags.AddRange(newTags);
                    }
                }
                else
                {
                    foreach (Entry tokenEntry in this.Overall)
                    {
                        tokenEntry.Tags = newTags;
                    }
                }  
            }

            Core.Meta.Update(this.Overall);
            Core.UpdateMeta();
            Core.SyncEntries();

            this.Hide();

            Core.CheckControls();

            List<string> CommaSplit(string input)
            {
                return input.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ConvertAll(x => x.Trim());
            }
        }

        private void BaseWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ApplyEditor();
            }
        }
    }
}
