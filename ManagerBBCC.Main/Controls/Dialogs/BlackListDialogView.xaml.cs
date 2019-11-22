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
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls.Dialogs;

namespace ManagerBBCC.Main.Controls.Dialogs
{
    public partial class BlackListDialogView : CustomDialog
    {
        public BlackListDialogView()
        {
            this.InitializeComponent();
        }

        public void SetBlackList(List<string> list)
        {
            //this.BlackListView.ItemsSource = null;
            //this.BlackListView.ItemsSource = list;
        }
    }
}
