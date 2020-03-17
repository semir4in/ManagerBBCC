using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerBBCC.Main.Functions
{
    public class FileCheck
    {
        public static bool IsAvailable(string path)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return false;
            }

            if (fs != null) fs.Close();

            return true;
        }
    }
}
