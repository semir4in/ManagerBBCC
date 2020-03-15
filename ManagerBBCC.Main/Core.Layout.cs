using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManagerBBCC.Main.Controls;
using ManagerBBCC.Main.Windows;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        private static MainWindow _mainWindow;
        public static MainWindow MainWindow => Core._mainWindow ?? (Core._mainWindow = new MainWindow());

        private static EditorWindow _editorWindow;
        public static EditorWindow EditorWindow => Core._editorWindow ?? (Core._editorWindow = new EditorWindow());
    }
}
