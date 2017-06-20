using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer
{
    public class Dir
    {
        public string path;
        public int count;

        public Dir(string p, int c)
        {
            path = p;
            count = c;
        }

        public override string ToString()
        {
            return path + "\t" + count;
        }
    }
}
