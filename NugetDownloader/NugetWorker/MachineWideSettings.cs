﻿using NuGet.Common;
using NuGet.Configuration;
using System;
using System.Collections.Generic;

namespace NugetWorker
{
    public class MachineWideSettings : IMachineWideSettings
    {
        private readonly Lazy<IEnumerable<Settings>> _settings;

        public MachineWideSettings()
        {
            var baseDirectory = NuGetEnvironment.GetFolderPath(NuGetFolderPath.MachineWideConfigDirectory);
            _settings = new Lazy<IEnumerable<Settings>>(
                () => (System.Collections.Generic.IEnumerable<NuGet.Configuration.Settings>)global::NuGet.Configuration.Settings.LoadMachineWideSettings(baseDirectory));
        }

        public IEnumerable<Settings> Settings => _settings.Value;

        ISettings IMachineWideSettings.Settings => throw new NotImplementedException();
    }
}