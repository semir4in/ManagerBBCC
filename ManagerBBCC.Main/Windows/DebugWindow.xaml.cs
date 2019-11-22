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
using System.Windows.Threading;

using MahApps.Metro.Controls;

namespace ManagerBBCC.Main.Windows
{
    public partial class DebugWindow : MetroWindow
    {
        public DebugWindow()
        {
            this.InitializeComponent();
            this.Title += $": {Config.ProjectName}";

            this.Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            this.Timer.Tick += Timer_Tick;
            this.Timer.Start();
        }

        private DispatcherTimer _timer;
        public DispatcherTimer Timer => _timer ?? (_timer = new DispatcherTimer());

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.SettingBlock.Text = $"{Core.Setting.Serialize()}";
            this.MetaBlock.Text = $"{Core.Meta.Serialize()}";

            this.TextOut.Text = $"Core.IsImageFolderValid: {Core.IsImageFolderValid}"
                + Environment.NewLine + $"Core.IsDataFolderValid: {Core.IsDataFolderValid}"
                + Environment.NewLine + $"Core.ActiveEntries.{{Name}}: {string.Join(Environment.NewLine, Core.Meta.Active.ConvertAll(x => x.Name))}";
        }

        private void MetroWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public new void Show()
        {
            base.Show();
            this.Activate();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            (sender as Window).Hide();
        }
    }
}
