using NugetWorker;
using System;

namespace NugetDownloaderTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello !! Please enter Package Name to download.");
            string packageName = Console.ReadLine();
            Console.WriteLine(" Please enter Version Number to download. (if not specific version , then just press enter)");
            string version = Console.ReadLine();
            try
            {
                NugetEngine nugetEngine = new NugetEngine();
                nugetEngine.GetPackage(packageName, version).Wait();
                Console.WriteLine("####################### FINAL DLLS #######################");
                foreach(var dll in nugetEngine.dllInfos)
                {
                    Console.WriteLine($"Relavant dll : {dll.rootPackage} | {dll.framework} | {dll.path} | ");
                }
            }


          
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occured : {ex.Message} | {ex.StackTrace}");
            }
            Console.WriteLine("press any key to exit");
            Console.ReadLine();

        }
    }
}
