using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using MahApps.Metro.Controls;

namespace ManagerBBCC.Main.Windows
{
    /// <summary>
    /// Base window for project bayoen from '<see cref="MetroWindow"/>'
    /// </summary>
    public class BaseWindow : MetroWindow
    {
        public BaseWindow()
        {
            this.TitleCharacterCasing = CharacterCasing.Normal;
            this.ShowIconOnTitleBar = false;
            this.SetResourceReference(Control.BackgroundProperty, "WindowBackgroundBrush");
            this.BorderThickness = new Thickness(1);
            this.WindowTitleBrush = Brushes.Transparent;

            this.MouseLeftButtonDown += BaseWindow_MouseLeftButtonDown;
            this.Closing += BaseWindow_Closing;

            this.CloseToMinimize = false;
            this.CloseToKill = false;
            this.IsDraggable = true;
        }

        /// <summary>
        /// When closed, if it is <see cref="true"/>, <see cref="BaseWindow"/> is minimized
        /// </summary>
        public bool CloseToMinimize
        {
            get => (bool)GetValue(CloseToMinimizeProperty);
            set => SetValue(CloseToMinimizeProperty, value);
        }
        public static readonly DependencyProperty CloseToMinimizeProperty = DependencyProperty.Register(
            "CloseToMinimize",
            typeof(bool),
            typeof(BaseWindow));

        /// <summary>
        /// When closed, if it is <see cref="true"/>, <see cref="BaseWindow"/> is terminated
        /// </summary>
        public bool CloseToKill
        {
            get => (bool)GetValue(CloseToKillProperty);
            set => SetValue(CloseToKillProperty, value);
        }
        public static readonly DependencyProperty CloseToKillProperty = DependencyProperty.Register(
            "CloseToKill",
            typeof(bool),
            typeof(BaseWindow));

        /// <summary>
        /// If <see cref="BaseWindow"/> is draggable, <see cref="Window.DragMove"/> is enabled
        /// </summary>
        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }
        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register(
            "IsDraggable",
            typeof(bool),
            typeof(BaseWindow));

        private void BaseWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.CloseToKill)
            {
                // Do nothing
            }
            else
            {
                e.Cancel = true;
                if (this.CloseToMinimize)
                {
                    this.WindowState = WindowState.Minimized;
                }
                else
                {
                    this.Hide();
                }
            }
        }

        private void BaseWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsDraggable)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Show and activate '<see cref="BaseWindow"/>'
        /// </summary>
        public new void Show()
        {
            base.Show();
            this.Activate();
        }
    }
}
