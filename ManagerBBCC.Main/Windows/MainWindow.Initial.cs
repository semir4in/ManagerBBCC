using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using ManagerBBCC.Main.ExtendedMethods;

namespace ManagerBBCC.Main.Windows
{
    public partial class MainWindow
    {
        private void Initialize()
        {
            this.InitializeComponent();

            this.InitialLogoImage.SetBitmap(ManagerBBCC.Main.Properties.Resources.GearsImage);

            if (Core.Setting.GithubSaved)
            {
                this.GithubStatusBlock.Text = $"깃허브: {Core.Setting.GithubUserName}/{Core.Setting.GithubRepositoryName}";
            }

#if DEBUG
            this.VersionBlock.Text = $"[DEBUG] ManagerBBCC v{Config.VersionString}";

            MenuItem ShowDebugWindowMenuItem = new MenuItem()
            {
                Header = "[DEBUG] Show DebugWindow",
            };
            ShowDebugWindowMenuItem.Click += (sender, e) =>
            {
                Core.DebugWindow.Show();
            };
            this.MenuButton.ContextMenu.Items.Add(ShowDebugWindowMenuItem);
#else
            this.VersionBlock.Text = $"ManagerBBCC v{Config.VersionShortString}";
#endif
            //this.EntryDataGrid.ItemsSource = Core.Meta.Entries;
            this.EntryListView.ItemsSource = Core.Meta.Filtered;
        }

        private bool _isInitialProgress = true;
        public bool IsInitialProgress
        {
            get => this._isInitialProgress;
            set
            {
                if (this._isInitialProgress == value) return;

                this.InitialGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.RightWindowCommands.Visibility = value ? Visibility.Collapsed : Visibility.Visible;

                this._isInitialProgress = value;
            }
        }

        private string _initialStatus = "";
        public string InitialStatus
        {
            get => this._initialStatus;
            set
            {
                if (this._initialStatus == value) return;

                this.InitialStatusBlock.Text = value;

                this._initialStatus = value;
            }
        }
    }
}
