using System;
using System.Collections.Generic;
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

namespace ManagerBBCC.Main.Windows
{
    public partial class EditorWindow : BaseWindow
    {
        public EditorWindow()
        {
            this.InitializeComponent();
        }

        public void Popup()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;

            this.Left = Math.Max(point.X - 10, 0);
            this.Top = Math.Max(point.Y - 10, 0);

            this.Show();
        }

        public void FromEntry(Entry entry, List<Entry> all)
        {
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
        }

        private void EditorApplyButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
