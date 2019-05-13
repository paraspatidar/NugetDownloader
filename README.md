# NugetDownloader
.Net Core 2.1 based Library which can be used to dynamically/programmatically search and download Othet Nuget packages.

[![NuGet Badge](https://buildstats.info/nuget/nugetdownloader)](https://www.nuget.org/packages/nugetdownloader/)


## ReadWorld Scenarios and working 

Nuget downloader source code in this repo is the source code for same name Nuget Package [https://www.nuget.org/packages/NugetDownloader/]([https://www.nuget.org/packages/NugetDownloader/](https://www.nuget.org/packages/NugetDownloader/))

Which can be used to **download nuget programmatically** from any *public or private nuget feed*.

Referring this Master Nuget Package(NugetDownloader) in your code, you can programmatically download any other nuget , its dependent nuget packages (dependency chain)  and its associated dlls on runtime or on demand.

The use case for such scenario is specially useful if you are giving your users an ability to write some C# based script which are dependent on some nuget packages and you are going to do some UI based or In-memory complication (for example using **Roslyn** ) then this can be quite useful to successfully compile such script.

Some of the example of similar implementation are mostly Function as a Service (**FaaS**) ,online **WebHooks** or online **C# editors** like :

 - ⚡ Azure functions ( FaaS for Azure) 
 - [Fission](https://fission.io/) (FaaS for Kubernetes)
 - [https://subroute.io](https://subroute.io/) (Online WebHook editor)


And Many more..

## Usage Guide [WIP - Draft meanwhile you can see
https://github.com/paraspatidar/NugetDownloader/blob/master/NugetDownloader/readme.txt ]
