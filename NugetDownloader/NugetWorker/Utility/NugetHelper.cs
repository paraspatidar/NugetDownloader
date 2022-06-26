using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NugetWorker.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NugetWorker
{
    public sealed class NugetHelper
    {
        private static readonly Lazy<NugetHelper> lazy =
            new Lazy<NugetHelper>(() => new NugetHelper());

        public static NugetHelper Instance { get { return lazy.Value; } }

        private static  ILogger _logger = new Logger("Initial.log");
        public ILogger logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        private NugetSettings _nugetSettings;


        static NugetHelper()
        {
        }
        private NugetHelper()
        {
        }

        public  void writeToFile(string msg)
        {

            using (StreamWriter sw = new StreamWriter("main.log",true))
            {
                sw.AutoFlush = true;
                sw.WriteLine($"{DateTime.Now} : {msg}");

            }

        }
        public IEnumerable<SourceRepository> GetSourceRepos()
        {
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API s

            IList<SourceRepository> sourcerepos = new List<SourceRepository>();

          
            foreach(NugetRepository nugetRepository in NugetHelper.Instance.GetNugetSettings().NugetRepositories.OrderBy(x=>x.Order))
            {
                PackageSource packageSource = new PackageSource(nugetRepository.Source, nugetRepository.Name);
                if(nugetRepository.IsPrivate)
                {
                    packageSource.Credentials = new PackageSourceCredential(nugetRepository.Name,
                        nugetRepository.Username, nugetRepository.Password, nugetRepository.IsPasswordClearText, "");
                }
                SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
                sourcerepos.Add(sourceRepository);
            }
            if(sourcerepos.Count<1)
            {
                throw new Exception("No Source Repository found!!");
            }
            return sourcerepos;
        }

        public void WriteLogToFile(string msg)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(NugetHelper.Instance.GetNugetSettings().NugetFolder, "log.txt"), true))
            {
                sw.AutoFlush = true;
                sw.WriteLine($"{DateTime.Now} : {msg} | ");

            }
        }
        public string GetTargetFramwork()
        {
            string frameworkName = Assembly.GetExecutingAssembly().GetCustomAttributes(true)
              .OfType<System.Runtime.Versioning.TargetFrameworkAttribute>()
              .Select(x => x.FrameworkName)
              .FirstOrDefault();
            NuGetFramework currentFramework = frameworkName == null
                ? NuGetFramework.AnyFramework
                : NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());

            return frameworkName;
        }

        public FrameworkSpecificGroup GetMostCompatibleGroup(NuGetFramework projectTargetFramework, IEnumerable<NuGet.Packaging.FrameworkSpecificGroup> itemGroups)
        {
            var reducer = new FrameworkReducer();
            var mostCompatibleFramework = reducer.GetNearest(projectTargetFramework, itemGroups.Select(i => i.TargetFramework));

            if (mostCompatibleFramework != null)
            {
                var mostCompatibleGroup = itemGroups.FirstOrDefault(i => i.TargetFramework.Equals(mostCompatibleFramework));

                if (IsValid(mostCompatibleGroup))
                {
                    return mostCompatibleGroup;
                }
            }

            return null;
        }

        public NuGetFramework GetMostCompatibleFramework(NuGetFramework projectTargetFramework, IEnumerable<PackageDependencyGroup> itemGroups)
        {
            var reducer = new FrameworkReducer();
            var mostCompatibleFramework = reducer.GetNearest(projectTargetFramework, itemGroups.Select(i => i.TargetFramework));
            return mostCompatibleFramework;
        }

        private bool IsValid(NuGet.Packaging.FrameworkSpecificGroup frameworkSpecificGroup)
        {
            if (frameworkSpecificGroup != null)
            {
                return (frameworkSpecificGroup.HasEmptyFolder
                        || frameworkSpecificGroup.Items.Any()
                        || !frameworkSpecificGroup.TargetFramework.Equals(NuGetFramework.AnyFramework));
            }

            return false;
        }

        public List<DllInfo> GetInstallPackagesDllPath(PackageWrapper packageWrapper, ref FolderNuGetProject project)
        {
            List<DllInfo> dllinfo = new List<DllInfo>();

            var packageIdentity = packageWrapper.rootPackageIdentity;
            var packageFilePath = project.GetInstalledPackageFilePath(packageIdentity);
            if (!string.IsNullOrWhiteSpace(packageFilePath))
            {
                _logger.LogInformation(packageFilePath);


                var archiveReader = new NuGet.Packaging.PackageArchiveReader(packageFilePath, null, null);
                var nugetFramwork = NuGetFramework.ParseFrameworkName(NugetHelper.Instance.GetTargetFramwork(), new DefaultFrameworkNameProvider());
                var referenceGroup = NugetHelper.Instance.GetMostCompatibleGroup(nugetFramwork, archiveReader.GetReferenceItems());
                if (referenceGroup != null)
                {
                    foreach (var group in referenceGroup.Items)
                    {
                        var installedPackagedFolder = project.GetInstalledPath(packageIdentity);
                        string installedDllPath = string.Empty;
                        if(NugetHelper.Instance.GetNugetSettings().RunningOnwindows)
                        {
                            installedDllPath = Path.Combine(installedPackagedFolder, group.Replace("/", "\\"));
                        }
                        else
                        {
                            installedDllPath = Path.Combine(installedPackagedFolder, group);
                        }
                        

                        var installedDllFolder = Path.GetDirectoryName(installedDllPath);
                        var dllName = Path.GetFileName(installedDllPath);
                        var extension = Path.GetExtension(installedDllPath).ToLower();
                        var processor = group.GetProcessor();

                        _logger.LogInformation($"dll Path: {installedDllPath}");

                        //check if file path exist , then only add
                        if (File.Exists(installedDllPath) && extension==".dll")
                        {
                            dllinfo.Add(
                                new DllInfo()
                                {
                                    name = dllName,
                                    path = installedDllPath,
                                    framework = referenceGroup.TargetFramework.DotNetFrameworkName,
                                    processor = processor,
                                    rootPackage= packageIdentity.Id
                                }
                                );
                        }

                        //also , try to cross refer this with expected folder name to avoid version mismatch
                    }
                }
            }


            return dllinfo;

        }

        public List<PackageIdentity> GetChildPackageIdentities(IPackageSearchMetadata rootPackage)
        {
            List<PackageIdentity> childPackageIdentities = new List<PackageIdentity>();

            //
            //load child package identity , if there is any dependency set
            if (rootPackage.DependencySets != null && rootPackage.DependencySets.Count() > 0)
            {
                //check specific dependency set for this version only  //note our _targetFramwork is .NETCoreApp,Version=v2.0
                //while the deps may be for .NETStandard,Version=v2.0 or .NETCoreApp,Version = v2.0 as well.

                var nugetFramwork = NuGetFramework.ParseFrameworkName(NugetHelper.Instance.GetTargetFramwork(), new DefaultFrameworkNameProvider());
                var mostCompatibleFramework = NugetHelper.Instance.GetMostCompatibleFramework(nugetFramwork, rootPackage.DependencySets);


                if (rootPackage.DependencySets.Any(x => x.TargetFramework == mostCompatibleFramework))
                {
                    var depsForTargetFramwork = rootPackage.DependencySets.Where(x => x.TargetFramework == mostCompatibleFramework).FirstOrDefault();
                    if (depsForTargetFramwork.Packages.Count() > 0)
                    {

                        foreach (var package in depsForTargetFramwork.Packages)
                        {


                            PackageIdentity _identity = new PackageIdentity(package.Id, package.VersionRange.MinVersion);
                            if (_identity != null)
                            {

                                childPackageIdentities.Add(_identity);
                            }

                        }
                    }
                }
            }

            //
            return childPackageIdentities;
        }

        public NugetSettings GetNugetSettings()
        {
            if (_nugetSettings == null)
            {
                string watcherSettingsjson = GetNugetSettingsJson();
                _nugetSettings = ObjectConverter.Instance.GetWatcherSettingsFromJson(watcherSettingsjson);
            }
            return _nugetSettings;
        }

        private string GetNugetSettingsJson()
        {
            var baselocation = AppDomain.CurrentDomain.BaseDirectory;
            var FileLocation = baselocation + "nugetSettings.json";

            return File.ReadAllText(FileLocation);
        }

        public List<DllInfo> GetDllInfoFromDirectory(string directorypath)
        {
            List<DllInfo> dllInfos = new List<DllInfo>();

            Console.WriteLine($"finding dll in folder {directorypath}");
            DirectoryInfo d = new DirectoryInfo(directorypath);//Assuming packagepath is your Folder
           var files = d.GetFiles("*.dll"); //Getting dll files
            foreach(var file in files)
            {
                Console.WriteLine($"found dll  {file.Name.ToLower()} at {Path.Combine(directorypath, file.Name)}");
                dllInfos.Add(new DllInfo() {
                    name = file.Name.ToLower(),
                    path = Path.Combine(directorypath, file.Name)
                });
            }

            return dllInfos;
        }
    }
}
