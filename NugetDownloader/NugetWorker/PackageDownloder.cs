using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NugetWorker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace NugetWorker
{
    public class PackageDownloder
    {

        public List<DllInfo> downloadedDllPaths = new List<DllInfo>();
        private string _targetFramwork { get; set; }

        private ILogger _logger { get; set; }

        private IList<SourceRepository> _sourceRepos { get; set; }

        public PackageDownloder()
        {

            _targetFramwork = NugetHelper.Instance.GetTargetFramwork();
            _logger = NugetHelper.Instance.logger;
            //_sourceRepos = NugetHelper.Instance.GetSourceRepos();
        }

        public async Task DownloadPackage(PackageWrapper packageWrapper)
        {
            //this will prevent install to look in all repos
            _sourceRepos = new List<SourceRepository>();
            _sourceRepos.Add(packageWrapper.sourceRepository);

            PackageIdentity packageIdentity = packageWrapper.rootPackageIdentity;
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API s

            var rootPath = NugetHelper.Instance.GetNugetSettings().NugetFolder;
            var settings = Settings.LoadDefaultSettings(rootPath, null, new MachineWideSettings());
            var packageSourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);
            var project = new NuGet.ProjectManagement.FolderNuGetProject(rootPath);
            var packageManager = new NuGet.PackageManagement.NuGetPackageManager(sourceRepositoryProvider, settings, rootPath)
            {
                PackagesFolderNuGetProject = project
            };

            var allowPrereleaseVersions = true;
            var allowUnlisted = false;
            NuGet.ProjectManagement.INuGetProjectContext projectContext = new ProjectContext();

            NuGet.PackageManagement.ResolutionContext resolutionContext =
                new NuGet.PackageManagement.ResolutionContext(
                NuGet.Resolver.DependencyBehavior.Lowest,
                allowPrereleaseVersions,
                allowUnlisted,
                NuGet.PackageManagement.VersionConstraints.None);


            if (NugetHelper.Instance.GetNugetSettings().DisableCache)
            {
                resolutionContext.SourceCacheContext.NoCache = true;
                resolutionContext.SourceCacheContext.DirectDownload = true;
            }

            var downloadContext = new PackageDownloadContext(resolutionContext.SourceCacheContext,
                rootPath, resolutionContext.SourceCacheContext.DirectDownload);

            bool packageAlreadyExists = packageManager.PackageExistsInPackagesFolder(packageIdentity,
                NuGet.Packaging.PackageSaveMode.None);
            if (!packageAlreadyExists)
            {
                await packageManager.InstallPackageAsync(
                   project,
                   packageIdentity,
                   resolutionContext,
                   projectContext,
                   downloadContext,
                  _sourceRepos, 
                   new List<SourceRepository>(),
                   CancellationToken.None);

              var packageDeps=  packageManager.GetInstalledPackagesDependencyInfo(project, CancellationToken.None, true);
              _logger.LogInformation($"Package {packageIdentity.Id} is got Installed at  | {project.GetInstalledPath(packageIdentity)} ");
            }
            else
            {
                _logger.LogInformation($"Package {packageIdentity.Id} is Already Installed at  | {project.GetInstalledPath(packageIdentity)} " +
                    $" | skipping instalation !!");
            }

           

            #region GetDll paths
            
             
            var dllstoAdd = NugetHelper.Instance.GetInstallPackagesDllPath(packageWrapper, ref project);
            if (dllstoAdd.Count > 0)
            {
                downloadedDllPaths.AddRange(dllstoAdd);
            }


            ////now iterate for child identities , but as we have alreayd written login for recursive install , check if this
            //is now really required or not ?

            //if (packageWrapper.childPackageIdentities != null && packageWrapper.childPackageIdentities.Count > 0)
            //{
            //    foreach (var childPackageIdentity in packageWrapper.childPackageIdentities)
            //    {

            //        var _dllstoAdd = NugetHelper.Instance.GetInstallPackagesDllPath(packageWrapper., ref project);
            //        if (_dllstoAdd.Count > 0)
            //        {
            //            downloadedDllPaths.AddRange(_dllstoAdd);
            //        }

            //    }
            //}

             

            #endregion


            _logger.LogInformation($"done for package {packageIdentity.Id} , with total Dlls {downloadedDllPaths.Count}");


        }

    }
}
