using System.Windows;

namespace ManagerBBCC.Main.Windows
{
    public partial class MainWindow
    {
        private int _tagPoolCount = 0;
        public int TagPoolCount
        {
            get => this._tagPoolCount;
            set
            {
                if (this._tagPoolCount == value) return;

                Core.MainWindow.TagHeaderBlock.Text = $"태그 목록 ({value})";

                this._tagPoolCount = value;
            }
        }

        private int _entryCount = 0;
        public int EntryCount
        {
            get => this._entryCount;
            set
            {
                if (this._entryCount == value) return;

                Core.MainWindow.EntryHeaderBlock.Text = $"엔트리 목록 ({value})";

                this._entryCount = value;
            }
        }
        
    }
}
