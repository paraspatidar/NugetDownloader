
			//Also , we can do the same on runtime by passing a file matching the RegEx pattern as per setting
            /*
             * Here is sample for text:
             * //#Nuget:Newtonsoft.json:11.0.2.0
			 * //#Nuget:RestSharp
			
			 * //#Exclude:xyzPackage:abc.dll
             * 
             * 
             * 
             * 
             */


			  //#Nuget:newtonsoft.json:123
            Regex _pkgName = new Regex(NugetHelper.Instance.GetAppSettings().NugetPackageRegEx, 
                                        RegexOptions.Compiled);

            //#Exclude:xyzPackage:abc.dll
            Regex _excludeDlls = new Regex(NugetHelper.Instance.GetAppSettings().ExcludeDllRegEx,
                                        RegexOptions.Compiled);

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
            //to-Be-written
            foreach(Match excludedll in _excludeDllMatchCollection)
            {
                
                var package = excludedll.Groups["package"]?.Value?.Trim().ToLower();
                var dll = excludedll.Groups["dll"]?.Value?.Trim().ToLower();
                Console.WriteLine($"trying to remove , if available : {dll} from package  {package}");
                dllInfos.RemoveAll(x => x.rootPackage.ToLower() == package.ToLower() && x.name.ToLower() == dll.ToLower());
            }