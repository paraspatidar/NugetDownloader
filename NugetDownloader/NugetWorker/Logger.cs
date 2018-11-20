using System.Threading.Tasks;
using NuGet.Common;
using System;
using log4net;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Linq;

namespace NugetWorker
{
    public class Logger : ILogger
    {
        private  ILog _ILog { get; set; }
        public Logger(string logpath)
        {
            
            //read the log4net config and create logger instance form log4net
            XmlDocument ConfigLoader = new XmlDocument();
            ConfigLoader.Load(File.OpenRead("log4net.config"));
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, ConfigLoader["log4net"]);

            var appender = ((log4net.Appender.FileAppender)repo.GetAppenders().Where(x => x.Name == "RollingLogFileAppender").FirstOrDefault());
            appender.File = logpath;// $"NugetWorker/log/{DateTime.Now.ToString("yyyy_MM_dd")}_{Guid.NewGuid().ToString()}_.log";

            appender.ActivateOptions();
            _ILog = LogManager.GetLogger(typeof(Logger));
        }
        public void Log(LogLevel level, string data)
        {
            Console.WriteLine(data);
            _ILog.Info(data);
        }

        public void Log(ILogMessage message)
        {
            Console.WriteLine(message);
            _ILog.Info(message.Message);
        }

        public Task LogAsync(LogLevel level, string data)
        {
            Console.WriteLine(data);
            _ILog.Info(data);
            return null;
        }

        public Task LogAsync(ILogMessage message)
        {
            Console.WriteLine(message);
            _ILog.Info(message.Message);
            return null;
        }

        public void LogDebug(string data)
        {
            Console.WriteLine(data);
            _ILog.Debug(data);
                
        }

        public void LogError(string data)
        {
            Console.WriteLine(data);
            _ILog.Error(data);
        }

        public void LogInformation(string data)
        {
            Console.WriteLine(data);
            _ILog.Info(data);
        }

        public void LogInformationSummary(string data)
        {
            Console.WriteLine(data);
            _ILog.Info(data);
        }

        public void LogMinimal(string data)
        {
            Console.WriteLine(data);
            _ILog.Info(data);
        }

        public void LogVerbose(string data)
        {
            Console.WriteLine(data);
            _ILog.Debug(data);
        }

        public void LogWarning(string data)
        {
            Console.WriteLine(data);
            _ILog.Warn(data);
        }
    }
}