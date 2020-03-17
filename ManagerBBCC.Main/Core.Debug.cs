#if DEBUG
using ManagerBBCC.Main.Windows;

namespace ManagerBBCC.Main
{
    public static partial class Core
    {
        private static DebugWindow _debugWindow;
        public static DebugWindow DebugWindow => _debugWindow ?? (_debugWindow = new DebugWindow());
    }
}
#endif
