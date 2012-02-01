using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dlink_prtg
{
    class ResultDTO
    {
        public int TotalDiskSpace { get; set; }
        public int UsedDiskSpace { get; set; }
        public int UnUsedDiskSpace { get; set; }
        public int PercentUsedDiskSpace { get; set; }
        public int Temp { get; set; }
        public string Error { get; set; }
    }
}
