using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer
{
    class Check
    {
        public static bool CheckIfEmpty(string str)
        {
            if (str.Length > 0)
                return true;
            else
                return false;
        }

        public static bool CheckIfDirExists(string path)
        {
            if (Directory.Exists(path))
                return true;
            else
                return false;
        }
    }
}
