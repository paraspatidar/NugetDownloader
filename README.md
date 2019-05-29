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

## Usage Guide 
**Core Method** 

	string packageName="Newtonsoft.json";
	string version="10.2.1.0"; \\optional
	
	\\initilize NugetEngine from NugetDownloader
	
    NugetEngine nugetEngine = new NugetEngine();
    nugetEngine.GetPackage(packageName, version).Wait();
Thats It , you are done , it will download mentioned package as well as their recursive dependencies in folder which is mentioned in ***nugetSettings.json*** file of consumer/client app.
To know about nugetSettings.json have a look at sample console client for Nuget Downloader at [https://github.com/paraspatidar/NugetDownloader/tree/master/NugetDownloaderTestConsole](https://github.com/paraspatidar/NugetDownloader/tree/master/NugetDownloaderTestConsole) 

---

# Advance Scenarios

 
## NugetSettings.json

Any client application which want to leverage this nuget need this **nugetSettings.json** file  , lets go through its section one by one 

      "NugetFolder": "Nugetdownload",
      "DisableCache": false,
      "CSVDirectory": "Nugetdownload/logs",
as clear by name  , these are some folder settings on where to save downloaded nuget packages and logs , and *DisableCache* is used for direct download to folder instead of first saving to temp location.

---
`   "RunningOnwindows": true,`

Above flag is taking care of file path based on if this nuget is being used on Linux or windows based platform. 
 
---
     "NugetRepositories": [
        {
          "Order": 1,
          "IsPrivate": false,
          "Name": "local",
          "Source": "Nugetdownload", //in case of local , relative path to folder
          "IsPasswordClearText": false,
          "Username": "",
          "Password": ""
        }
    ]

This json section contains list of Repositories which we want to use for searching and downloading the packages.
Its good idea to keep the order and also maintains local folder as well as default nuget endpoint repo for all public nugets.
For Private nugets , you can add/append  details of private repo by giving corresponding values of  attributes like *IsPrivate , Source , Username etc*

---
    "NugetPackageRegEx": "\\s*(?<package>[^:\\n]*)(?:\\:)?(?<version>.*)?",
     
Above is the regex which can be helpful used to Identify Nuget Package name and its version from bulk download text file or multi text box.
for example user wants to download some bulk nugets then he can create an input file where nuget package should be matching with mentioned RegEx for example , Lets assume that you want to download any one nuget or bunch of nuget then sample file name is *nugetToDownload.txt* , we will see its usage in a minute , but before that have a look at *exclue RegEx* as well.

---
    "ExcludeDllRegEx": "\\:?\\s*(?<package>[^:\\n]*)(?:\\:)?(?<dll>.*)?"

As we mentioned that any nuget which we download also downloads the recursive dependencies. Once all dependencies as well as desired parent nuget is downloaded , then we add all framework matching Dlls into our project assembly for runtime compilation.

However these dependencies some time may contain some dlls or dependent dlls which are not compatible with our dotnet version and can cause compilation issue while we are trying to add them in assembly.

In such cases , you can create list of dlls which you want to exclude from *final dll list to refer*   kind of object , for example lets say  we want to download package *Newtonsoft.json* and it also downloads some dependent nuget package say *xyzPackage* which has non compatible dll *abc.dll*  thus we can mentioned that package in a file say *excludedll.txt*which should match above regex by mentioning one package:dll combination per line.

---
lets have a look on  sample example on how to use all these together 

 **nugetToDownload.txt**--> this file contains list of nuget packages required by your function , in this file put one line per nuget with nugetpackage name:version(optional) format, for example :

```
RestSharp
CsvHelper
Newtonsoft.json:10.2.1.0
```
**excludedll.txt**  -> out of all downloaded dlls we want to skip few specific dlls from specific package , then we can mention like :

    xyzPackage:abc.dll
    opqPackage:mnop.dll

Now , here is a sample code using these features :

    //newtonsoft.json:123
    Regex _pkgName = new Regex(NugetHelper.Instance.GetAppSettings()
    					     .NugetPackageRegEx,RegexOptions.Compiled);
    
    //xyzPackage:abc.dll
    Regex _excludeDlls = new Regex(NugetHelper.Instance.GetAppSettings()
    						  .ExcludeDllRegEx,RegexOptions.Compiled);
    						
    var _pkgMatchCollection = _pkgName.Matches(textfilecontent);
    var _excludeDllMatchCollection = _excludeDlls.Matches(textfilecontent);

    foreach (Match match in _pkgMatchCollection)
    {
    	string package = match.Groups["package"]?.Value?.Trim();
	    string version = match.Groups["version"]?.Value?.Trim();
	    System.Console.WriteLine($"going to nugetEngin with  {package}");
	    try
	     {
		   NugetEngine nugetEngine = new NugetEngine();
		   nugetEngine.GetPackage(package, version).Wait();
		    dllInfos.AddRange(nugetEngine.dllInfos);
	     }
	     catch(Exception ex)
		{
		
		  //System.Console.WriteLine($"Error :{ex.Message} , Trace : {ex.StackTrace}");
		}
    }
    
    
    //now remove the dlls which we dont want to add
    foreach(Match excludedll in _excludeDllMatchCollection)
    {
    	var package = excludedll.Groups["package"]?.Value?.Trim().ToLower();
	    var dll = excludedll.Groups["dll"]?.Value?.Trim().ToLower();
	    Console.WriteLine($"trying to remove , if available : {dll} from package  {package}");
	    dllInfos.RemoveAll(x => x.rootPackage.ToLower() == package.ToLower() 
							&& x.name.ToLower() == dll.ToLower());
    }
