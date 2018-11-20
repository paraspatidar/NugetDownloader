using System;
using System.Collections.Generic;
using System.Text;

namespace NugetWorker.Utility
{
    public class NugetSettings
    {
        public string NugetFolder { get; set; }
        public bool DisableCache { get; set; }
        public string CSVDirectory { get; set; }
        public string NugetPackageRegEx { get; set; }
        public string ExcludeDllRegEx { get; set; }
        public bool RunningOnwindows { get; set; }
        public List<NugetRepository> NugetRepositories { get; set; }
    }
}
