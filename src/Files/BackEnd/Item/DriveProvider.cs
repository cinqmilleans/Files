using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.BackEnd
{
    internal interface IDriveProvider
    {
        bool LoadSpaces { get; set; }
        bool LoadIcons { get; set; }
    }

    internal class DriveProvider : IDriveProvider
    {
        public bool LoadSpaces { get; set; } = false;
        public bool LoadIcons { get; set; } = false;


    }
}
