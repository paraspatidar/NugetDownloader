﻿using NuGet.Common;
using NuGet.Packaging;
using NuGet.ProjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace NugetWorker
{
    public class ProjectContext : INuGetProjectContext
    {
        public void Log(MessageLevel level, string message, params object[] args)
        {
            // Do your logging here...
            Console.WriteLine(message);
        }

        public FileConflictAction ResolveFileConflict(string message) => FileConflictAction.Ignore;

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider => null;

        public ExecutionContext ExecutionContext => null;

        public void ReportError(string message)
        {
            Console.WriteLine(message);
        }

        void INuGetProjectContext.Log(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        void INuGetProjectContext.ReportError(ILogMessage message)
        {
            throw new NotImplementedException();
        }

        public NuGetActionType ActionType { get; set; }
        public Guid OperationId { get; set; }
    }
}