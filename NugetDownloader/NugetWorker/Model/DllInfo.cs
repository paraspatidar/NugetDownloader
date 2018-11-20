using System;
using System.Collections.Generic;
using System.Text;

namespace NugetWorker
{
    public class DllInfo
    {
        public string name { get; set; }
        public string rootPackage { get; set; }
        public string framework { get; set; }

        public string processor { get; set; }
        public string path { get; set; }
    }
}
