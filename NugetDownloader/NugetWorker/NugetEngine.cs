using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NugetWorker
{
    public class NugetEngine
    {

        IList<PackageWrapper> packageWrappers = new List<PackageWrapper>();
        public List<DllInfo> dllInfos = new List<DllInfo>();
        List<Task> packageDowloadTasks = new List<Task>();
        public async Task GetPackage(string packageName, string version = "")
        {
            try
            {
                PackageFinder packageFinder = new PackageFinder();
                var packageWrappers = packageFinder.GetListOfPackageIdentities(packageName, version);
                if (packageWrappers == null || packageWrappers.Count < 1)
                {
                    throw new Exception("enable to locate package!!!");
                }
                dllInfos.AddRange(packageFinder.dllInfos);
                Console.WriteLine($"Total Dlls {dllInfos.Count} for rootpackage { packageName}-{version}");
                Console.WriteLine($"done with nuget engine!!!! ");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR : {ex.Message} | {ex.StackTrace}");
            }
        }
    }
}
